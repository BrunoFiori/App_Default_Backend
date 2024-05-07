using App_Default_Backend.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace App_Default_Backend.Core.Contract
{
    public interface IAuthManager
    {
        Task<IEnumerable<IdentityError>> Register(InputUser inputUser);
        Task<bool> GetByEmail(string email);
        Task<AuthResponse?> Login(LoginUser inputUser);
        Task<string?> CreateRefreshToken();
        Task<AuthResponse?> VerifyRefeshToken(AuthResponse request);

    }
}
