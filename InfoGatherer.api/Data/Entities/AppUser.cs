using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace InfoGatherer.api.Data.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ApiKey { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class Role : IdentityRole<Guid>
    {

    }
    public class AppUserConfig : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasDefaultValueSql("newsequentialId()");
            builder.Property(x => x.FirstName).HasMaxLength(50);
            builder.Property(x => x.LastName).HasMaxLength(50);
            builder.Property(x => x.ApiKey).HasMaxLength(32).ValueGeneratedOnAdd().HasDefaultValueSql("replace(newid(),'-','')");
            builder.Property(x => x.CreatedAt).ValueGeneratedOnAdd().HasDefaultValueSql("getdate()");

            builder.ToTable("AspNetUsers", b => b.IsTemporal(temp =>
            {
                temp.UseHistoryTable("AspNetUsersHistory");
                temp.HasPeriodStart("StartTime");
                temp.HasPeriodEnd("EndTime");
            }));
        }
    }
}
