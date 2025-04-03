using API.DTOs;
using API.Helpers;
using API.Models;

namespace API.Interfaces;

public interface IUserRepository
{
    void Update(AppUsers user);
    Task<IEnumerable<AppUsers>> GetUsersAsync();
    Task<AppUsers?> GetUserByIdAsync(int id);
    Task<AppUsers?> GetUserByUsernameAsync(string username);
    Task<AppUsers?> GetUserByPhotoId(int photoId); 
    Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
    Task<MemberDto?> GetMemberAsync(string username, bool isCurrentUser);
}
