using System;
using API.DTOs;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using API.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repository;

public class UserRepository(DataContext context) : IUserRepository
{
    public async Task<MemberDto?> GetMemberAsync(string username, bool isCurrentUser)
    {
         var query = context.Users
            .Where(u => u.UserName == username)
            .Select(user => new MemberDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Age = user.DateOfBirth.CalculateAge(), // Using the provided method
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain)!.Url,
                KnownAs = user.KnownAs,
                Created = user.Created,
                LastActive = user.LastActive,
                Gender = user.Gender,
                Introduction = user.Introduction,
                Interests = user.Interests,
                LookingFor = user.LookingFor,
                City = user.City,
                Country = user.Country,
                Photos = user.Photos.Select(p => new PhotoDto
                {
                    Id = p.Id,
                    Url = p.Url,
                    IsMain = p.IsMain,
                    IsApproved = p.IsApproved
                }).ToList()
            })
            .AsQueryable();

            if (isCurrentUser) query = query.IgnoreQueryFilters();

            return await query.FirstOrDefaultAsync();
    }

    public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
    {
        var query = context.Users.AsQueryable();

        query = query.Where(x => x.UserName != userParams.CurrentUsername);

        if (userParams.Gender != null)
        {
            query = query.Where(x => x.Gender == userParams.Gender);
        }

        var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge-1));
        var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

        query = query.Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);

        query = userParams.OrderBy switch
        {
            "created" => query.OrderByDescending(x => x.Created),
            _ => query.OrderByDescending(x => x.LastActive)
        };
        
        return await PagedList<MemberDto>.CreateAsync( query.Select(users => new MemberDto
        {
            Id = users.Id,
            UserName = users.UserName,
            Age = users.DateOfBirth.CalculateAge(),
            PhotoUrl = users.Photos.FirstOrDefault(p => p.IsMain)!.Url,
            KnownAs = users.KnownAs,
            Created = users.Created,
            LastActive = users.LastActive,
            Gender = users.Gender,
            Introduction = users.Introduction,
            Interests = users.Interests,
            LookingFor = users.LookingFor,
            City = users.City,
            Country = users.Country,
            Photos = users.Photos.Select(photo => new PhotoDto
            {
                Id = photo.Id,
                Url = photo.Url,
                IsMain = photo.IsMain,
                IsApproved = photo.IsApproved
            }).ToList()
        }), userParams.PageNumber, userParams.PageSize);
    }

    public async Task<AppUsers?> GetUserByIdAsync(int id)
    {
        return await context.Users.FindAsync(id);
    }

    public async Task<AppUsers?> GetUserByPhotoId(int photoId)
    {
       return await context.Users
       .Include(p => p.Photos)
       .IgnoreQueryFilters()
       .Where(p => p.Photos.Any(p => p.Id == photoId))
       .FirstOrDefaultAsync();
    }

    public async Task<AppUsers?> GetUserByUsernameAsync(string username)
    {
        return await context.Users
        .Include(x=> x.Photos)
        .SingleOrDefaultAsync(x => x.UserName == username);
    }

    public async Task<IEnumerable<AppUsers>> GetUsersAsync()
    {
        return await context.Users
        .Include(x => x.Photos)
        .ToListAsync();
    }

    public  void Update(AppUsers user)
    {
        context.Entry(user).State = EntityState.Modified;
    }
}

