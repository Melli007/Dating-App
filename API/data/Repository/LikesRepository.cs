using System;
using API.DTOs;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using API.Models;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class LikesRepository(DataContext context) : ILikesRepository
{
    public void AddLike(UserLike like)
    {
        context.Likes.Add(like);
    }

    public void DeleteLike(UserLike like)
    {
        context.Likes.Remove(like);
    }

    public async Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId)
    {
        return await context.Likes
        .Where(x => x.SourceUserId == currentUserId)
        .Select(x => x.TargetUserId)
        .ToListAsync();
    }

    public async Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId)
    {
        return await context.Likes.FindAsync(sourceUserId, targetUserId);
    }

    public async Task<PagedList<MemberDto>> GetUserLikes(LikesParams likesParams)
    {
        var likes = context.Likes.AsQueryable();
        IQueryable<MemberDto> query;

         switch (likesParams.Predicate)
    {
        case "liked":
            query = likes
                .Where(x => x.SourceUserId == likesParams.UserId)
                .Select(x => x.TargetUser)
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
                        IsMain = p.IsMain
                    }).ToList()
                });
              break;
                case "likedBy":
                            query = likes
                .Where(x => x.TargetUserId == likesParams.UserId)
                .Select(x => x.SourceUser)
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
                        IsMain = p.IsMain
                    }).ToList()
                });
            break;
        default:
           var likeIds = await GetCurrentUserLikeIds(likesParams.UserId);

          query = likes
           .Where(x => x.TargetUserId == likesParams.UserId && likeIds.Contains(x.SourceUserId))
           .Select(x => x.SourceUser)
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
                        IsMain = p.IsMain
                    }).ToList()
                });
            break;
        }
        return await PagedList<MemberDto>.CreateAsync(query, likesParams.PageNumber, likesParams.PageSize);
    }
}
