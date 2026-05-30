using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814

namespace OurBigDay.Api.Migrations
{
    /// <inheritdoc />
    public partial class RestructureWeddingDays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Move any WeddingItems from Day 2 (Night Before) to Day 1 (Sangeet → merged Day 3)
            migrationBuilder.Sql(@"UPDATE ""WeddingItems"" SET ""DayId"" = 1 WHERE ""DayId"" = 2");

            // Delete Day 2 (cascades its DayCategories)
            migrationBuilder.DeleteData(
                table: "WeddingDays",
                keyColumn: "Id",
                keyValue: 2);

            // Update Day 1: Sangeet → Hindu wedding and night before/Sangeet (DayNumber 3, Dec 18 2026)
            migrationBuilder.UpdateData(
                table: "WeddingDays",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DayNumber", "ThemeName", "Date" },
                values: new object[] { 3, "Hindu wedding and night before/Sangeet", new DateOnly(2026, 12, 18) });

            // Update Day 3: Christian Wedding → Christian wedding and reception (DayNumber 4, Dec 19 2026)
            migrationBuilder.UpdateData(
                table: "WeddingDays",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DayNumber", "ThemeName", "Date" },
                values: new object[] { 4, "Christian wedding and reception", new DateOnly(2026, 12, 19) });

            // Add Night Before's unique categories to the merged Day 1
            migrationBuilder.InsertData(
                table: "DayCategories",
                columns: new[] { "DayId", "CategoryId", "DisplayOrder" },
                values: new object[,]
                {
                    { 1, 9, 9 },
                    { 1, 10, 10 },
                    { 1, 11, 11 },
                    { 1, 12, 12 },
                });

            // Insert new Day 4 (Mendhi) and Day 5 (Haldi)
            migrationBuilder.InsertData(
                table: "WeddingDays",
                columns: new[] { "Id", "DayNumber", "ThemeName", "Date" },
                values: new object[,]
                {
                    { 4, 1, "Mendhi", new DateOnly(2026, 12, 16) },
                    { 5, 2, "Haldi", new DateOnly(2026, 12, 17) },
                });

            // Insert DayCategories for Mendhi (Day 4): Venue, Food, Mendhi Artists, Mendi outfit
            migrationBuilder.InsertData(
                table: "DayCategories",
                columns: new[] { "DayId", "CategoryId", "DisplayOrder" },
                values: new object[,]
                {
                    { 4, 1, 1 },
                    { 4, 2, 2 },
                    { 4, 33, 3 },
                    { 4, 12, 4 },
                });

            // Insert DayCategories for Haldi (Day 5): Venue, Food, Clothes, Decor
            migrationBuilder.InsertData(
                table: "DayCategories",
                columns: new[] { "DayId", "CategoryId", "DisplayOrder" },
                values: new object[,]
                {
                    { 5, 1, 1 },
                    { 5, 2, 2 },
                    { 5, 5, 3 },
                    { 5, 10, 4 },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove new days and their categories
            migrationBuilder.DeleteData(table: "DayCategories", keyColumns: new[] { "DayId", "CategoryId" }, keyValues: new object[] { 4, 1 });
            migrationBuilder.DeleteData(table: "DayCategories", keyColumns: new[] { "DayId", "CategoryId" }, keyValues: new object[] { 4, 2 });
            migrationBuilder.DeleteData(table: "DayCategories", keyColumns: new[] { "DayId", "CategoryId" }, keyValues: new object[] { 4, 33 });
            migrationBuilder.DeleteData(table: "DayCategories", keyColumns: new[] { "DayId", "CategoryId" }, keyValues: new object[] { 4, 12 });
            migrationBuilder.DeleteData(table: "DayCategories", keyColumns: new[] { "DayId", "CategoryId" }, keyValues: new object[] { 5, 1 });
            migrationBuilder.DeleteData(table: "DayCategories", keyColumns: new[] { "DayId", "CategoryId" }, keyValues: new object[] { 5, 2 });
            migrationBuilder.DeleteData(table: "DayCategories", keyColumns: new[] { "DayId", "CategoryId" }, keyValues: new object[] { 5, 5 });
            migrationBuilder.DeleteData(table: "DayCategories", keyColumns: new[] { "DayId", "CategoryId" }, keyValues: new object[] { 5, 10 });
            migrationBuilder.DeleteData(table: "WeddingDays", keyColumn: "Id", keyValue: 4);
            migrationBuilder.DeleteData(table: "WeddingDays", keyColumn: "Id", keyValue: 5);

            // Remove the Night Before categories that were merged into Day 1
            migrationBuilder.DeleteData(table: "DayCategories", keyColumns: new[] { "DayId", "CategoryId" }, keyValues: new object[] { 1, 9 });
            migrationBuilder.DeleteData(table: "DayCategories", keyColumns: new[] { "DayId", "CategoryId" }, keyValues: new object[] { 1, 10 });
            migrationBuilder.DeleteData(table: "DayCategories", keyColumns: new[] { "DayId", "CategoryId" }, keyValues: new object[] { 1, 11 });
            migrationBuilder.DeleteData(table: "DayCategories", keyColumns: new[] { "DayId", "CategoryId" }, keyValues: new object[] { 1, 12 });

            // Restore Day 1 to Sangeet
            migrationBuilder.UpdateData(
                table: "WeddingDays",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DayNumber", "ThemeName", "Date" },
                values: new object[] { 1, "Sangeet", new DateOnly(2025, 3, 14) });

            // Restore Day 3 to Christian Wedding
            migrationBuilder.UpdateData(
                table: "WeddingDays",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DayNumber", "ThemeName", "Date" },
                values: new object[] { 3, "Christian Wedding", new DateOnly(2025, 3, 16) });

            // Restore Day 2 (Night Before)
            migrationBuilder.InsertData(
                table: "WeddingDays",
                columns: new[] { "Id", "DayNumber", "ThemeName", "Date" },
                values: new object[] { 2, 2, "Night Before", new DateOnly(2025, 3, 15) });

            migrationBuilder.InsertData(
                table: "DayCategories",
                columns: new[] { "DayId", "CategoryId", "DisplayOrder" },
                values: new object[,]
                {
                    { 2, 1, 1 },
                    { 2, 2, 2 },
                    { 2, 9, 3 },
                    { 2, 10, 4 },
                    { 2, 11, 5 },
                    { 2, 12, 6 },
                });

            // NOTE: WeddingItems that were moved from Day 2 to Day 1 cannot be automatically reversed.
            // Manual cleanup of WeddingItems may be required after rolling back this migration.
        }
    }
}
