using System.Text;
using API.Data;
using API.Extensions;
using API.Middleware;
using API.Models;
using API.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);


// Add your application services using your custom extension method.
builder.Services.AddIdentityCore<AppUsers>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
}).AddRoles<AppRole>()
.AddRoleManager<RoleManager<AppRole>>()
.AddEntityFrameworkStores<DataContext>(); 

builder.Services.AddApplicationServices(builder.Configuration);

// Configure Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options=> {
    var tokenKey = builder.Configuration["TokenKey"]?? throw new Exception("TokenKey not found");
    options.TokenValidationParameters = new TokenValidationParameters{
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
        ValidateIssuer = false,
        ValidateAudience = false
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            
            return Task.CompletedTask;
        }    
    };
});

builder.Services.AddAuthorizationBuilder()
.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin","Moderator"));

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(x=> x.AllowAnyHeader().AllowAnyMethod().AllowCredentials()
.WithOrigins("http://localhost:4200","https://localhost:4200")
);

app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");
app.MapFallbackToController("Index", "Fallback");
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUsers>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    await context.Database.MigrateAsync();
    await context.Database.ExecuteSqlRawAsync("DELETE FROM [Connections]");
    await Seed.SeedUsers(userManager, roleManager);
}
catch (Exception ex)
{   
   var logger = services.GetRequiredService<ILogger<Program>>();
   logger.LogError(ex, "An error occurred during migration");
}

app.Run();
