using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfoGatherer.api.Data.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string JwtId { get; set; }
        public Guid UserId { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Token { get; set; }
    }
    public class RefreshTokenConfig : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> b)
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).HasDefaultValueSql("newsequentialId()"); 
            b.Property(x => x.Token).HasMaxLength(64);
            b.Property(x => x.JwtId).HasMaxLength(36);
            b.HasOne<AppUser>().WithMany().HasForeignKey(x => x.UserId);
            b.ToTable("RefreshTokens");
        }
    }
}
