using InfoGatherer.api.DTOs.Users;

namespace InfoGatherer.api.Data.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> RegisterUserAsync(UserRegisterDto userRegisterDto);
        Task<bool> UserExistsAsync(string email);
    }
}
