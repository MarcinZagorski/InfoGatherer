using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace InfoGatherer.api.Data.Entities.Scrapper
{
    public class Wibor
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Source { get; set; }

        public decimal Overnight { get; set; } // "ON"
        public decimal TomorrowNext { get; set; } // "TN"
        public decimal SpotWeek { get; set; } // "SW"
        public decimal OneMonth { get; set; } // "1M"
        public decimal ThreeMonths { get; set; } // "3M"
        public decimal SixMonths { get; set; } // "6M"
        public decimal OneYear { get; set; } // "1R"
    }
    public class WiborConfig : IEntityTypeConfiguration<Wibor>
    {
        public void Configure(EntityTypeBuilder<Wibor> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasDefaultValueSql("newsequentialId()");
            builder.Property(x => x.CreatedAt).ValueGeneratedOnAdd().HasDefaultValueSql("getdate()");
            builder.Property(x => x.Source).HasMaxLength(100);

            builder.Property(x => x.Overnight).HasPrecision(5,4);
            builder.Property(x => x.TomorrowNext).HasPrecision(5, 4);
            builder.Property(x => x.SpotWeek).HasPrecision(5, 4);
            builder.Property(x => x.OneMonth).HasPrecision(5,4);
            builder.Property(x => x.ThreeMonths).HasPrecision(5,4);
            builder.Property(x => x.SixMonths).HasPrecision(5,4);
            builder.Property(x => x.OneYear).HasPrecision(5,4);

            builder.ToTable("Wibor","Scrapper");
        }
    }
}
