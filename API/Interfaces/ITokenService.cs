using API.Models;

namespace API;
public interface ITokenService{
    Task<string> CreateToken(AppUsers user);
}