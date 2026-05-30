using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OurBigDay.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixWeddingCapitalisation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE ""WeddingDays"" SET ""ThemeName"" = 'Hindu Wedding & Night Before/Sangeet' WHERE ""Id"" = 1");
            migrationBuilder.Sql(@"UPDATE ""WeddingDays"" SET ""ThemeName"" = 'Christian Wedding & Reception' WHERE ""Id"" = 3");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE ""WeddingDays"" SET ""ThemeName"" = 'Hindu wedding and night before/Sangeet' WHERE ""Id"" = 1");
            migrationBuilder.Sql(@"UPDATE ""WeddingDays"" SET ""ThemeName"" = 'Christian wedding and reception' WHERE ""Id"" = 3");
        }
    }
}
