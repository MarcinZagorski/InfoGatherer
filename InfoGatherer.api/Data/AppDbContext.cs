using InfoGatherer.api.Data.Entities;
using InfoGatherer.api.Data.Entities.Scrapper;
using InfoGatherer.api.DTOs.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Security.Claims;

namespace InfoGatherer.api.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, Role, Guid>
    {
        private readonly UserInfo _user;
        public UserInfo User { get { return _user; } }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Wibor> Wibors { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor accessor) : base(options)
        {
            var user = accessor?.HttpContext?.User;
            if (user != null)
            {
                if (Guid.TryParse(user.FindFirst("Id")?.Value, out var uid))
                {
                    _user = new UserInfo { Id = uid, Email = user.FindFirstValue(ClaimTypes.NameIdentifier) };
                }
                else if (user.FindFirstValue(ClaimTypes.Upn) != default)
                {
                    var userData = Users.FirstOrDefault(x => x.UserName == user.FindFirstValue(ClaimTypes.Upn));
                    _user = new UserInfo
                    {
                        Id = userData != default ? userData.Id : Guid.Empty,
                        Email = user.FindFirstValue(ClaimTypes.Upn)
                    };
                }
               
            }
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
    
}
