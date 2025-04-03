using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.DTOs;
using Microsoft.AspNetCore.Identity;
using API.Extensions;

namespace API.Controllers;

public class AccountController(UserManager<AppUsers> userManager, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto) 
    {
        if(await UserExists(registerDto.Username)){
            return BadRequest("Username is taken");
        }
        
        var user = new AppUsers
        {
            UserName = registerDto.Username.ToLower(),
            KnownAs = registerDto.KnownAs!,
            Gender = registerDto.Gender!,
            DateOfBirth = DateOnly.Parse(registerDto.DateOfBirth!),
            City = registerDto.City!,
            Country = registerDto.Country!
        };

        user.UserName = registerDto.Username.ToLower();

        var result = await userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded) return BadRequest(result.Errors);

        return new UserDto
        {
            Username = user.UserName,
            Token = await tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await userManager.Users
        .Include(p => p.Photos)
        .FirstOrDefaultAsync(x =>
         x.NormalizedUserName == loginDto.Username.ToUpper());
        if (user == null || user.UserName == null)
        {
            return Unauthorized("Invalid Username");
        }
        
        var result = await userManager.CheckPasswordAsync(user, loginDto.Password);

        if (!result) return Unauthorized();

          return new UserDto{
            Username = user.UserName,
            KnownAs = user.KnownAs,
            Token = await tokenService.CreateToken(user),
            Gender = user.Gender,
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
        };
    }

    private async Task<bool> UserExists(string username)
    {
        return await userManager.Users.AnyAsync(x=> x.NormalizedUserName == username.ToUpper());
    }

}