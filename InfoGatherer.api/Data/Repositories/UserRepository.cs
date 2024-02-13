using DocumentFormat.OpenXml.Office2010.Excel;
using InfoGatherer.api.Configuration;
using InfoGatherer.api.Data.Entities;
using InfoGatherer.api.Data.Repositories.Interfaces;
using InfoGatherer.api.DTOs.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NLog;
using ILogger = NLog.ILogger;

namespace InfoGatherer.api.Data.Repositories
{
    public class UserRepository(AppDbContext context, UserManager<AppUser> userManager, ILogger logger) : IUserRepository
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly AppDbContext _context = context;
        private readonly ILogger _logger = logger;

        public async Task<bool> RegisterUserAsync(UserRegisterDto userRegisterDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = new AppUser
                {
                    UserName = userRegisterDto.Email,
                    Email = userRegisterDto.Email,
                    FirstName = userRegisterDto.FirstName,
                    LastName = userRegisterDto.LastName
                };

                var result = await _userManager.CreateAsync(user, userRegisterDto.Password);

                if (result.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, AppRoles.User.ToString());

                    if (!roleResult.Succeeded)
                    {
                        _logger.Error($"Failed to add role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                        await transaction.RollbackAsync();
                        return false;
                    }

                    await transaction.CommitAsync();
                    return true;
                }
                else
                {
                    _logger.Error($"User registration failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    await transaction.RollbackAsync();
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during user registration.");
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
        public async Task<AppUser> LoginAsync(string email, string password)
        {
            //TODO: Protection of multiple incorrect password entry
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.CheckPasswordAsync(user, password);
                if (result)
                {
                    return user;
                }
            }
            return null;
        }
        public async Task<bool> ChangePasswordAsync(string oldPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(_context.User.Id.ToString());
            if (user == null) return false;
            var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            return result.Succeeded;
        }
        public async Task<AppUser> CheckApiKey(string apiKey)
        {
            var user = await _context.Users.Where(x=> x.ApiKey == apiKey).FirstOrDefaultAsync();
            if(user == null)
            {
                user = _context.User == null ? null : await _context.Users.FindAsync(_context.User.Id);
            }
            return user;
        }
    }
}
