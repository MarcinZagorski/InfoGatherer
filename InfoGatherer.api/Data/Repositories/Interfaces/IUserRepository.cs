using InfoGatherer.api.Data.Entities;
using InfoGatherer.api.DTOs.Users;

namespace InfoGatherer.api.Data.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> RegisterUserAsync(UserRegisterDto userRegisterDto);
        Task<bool> UserExistsAsync(string email);
        Task<AppUser> LoginAsync(string email, string password);
        Task<bool> ChangePasswordAsync(string oldPassword, string newPassword);
        Task<AppUser> CheckApiKey(string apiKey);
    }
}
