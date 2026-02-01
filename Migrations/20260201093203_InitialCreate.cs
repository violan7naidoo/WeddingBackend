using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OurBigDay.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeddingDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DayNumber = table.Column<int>(type: "int", nullable: false),
                    ThemeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeddingDays", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "WeddingDays",
                columns: new[] { "Id", "Date", "DayNumber", "ThemeName" },
                values: new object[,]
                {
                    { 1, new DateOnly(2025, 3, 14), 1, "Sangeet" },
                    { 2, new DateOnly(2025, 3, 15), 2, "Night Before" },
                    { 3, new DateOnly(2025, 3, 16), 3, "Christian Wedding" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeddingDays");
        }
    }
}
