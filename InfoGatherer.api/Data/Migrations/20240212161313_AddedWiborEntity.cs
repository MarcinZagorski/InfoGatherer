using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfoGatherer.api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedWiborEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Scrapper");

            migrationBuilder.CreateTable(
                name: "Wibor",
                schema: "Scrapper",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newsequentialId()"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    Source = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Overnight = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: false),
                    TomorrowNext = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: false),
                    SpotWeek = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: false),
                    OneMonth = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: false),
                    ThreeMonths = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: false),
                    SixMonths = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: false),
                    OneYear = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wibor", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Wibor",
                schema: "Scrapper");
        }
    }
}
