using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OurBigDay.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentsAndImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagesJson",
                table: "WeddingItems",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WeddingItemPayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WeddingItemId = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PaidDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeddingItemPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeddingItemPayments_WeddingItems_WeddingItemId",
                        column: x => x.WeddingItemId,
                        principalTable: "WeddingItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WeddingItemPayments_WeddingItemId",
                table: "WeddingItemPayments",
                column: "WeddingItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "WeddingItemPayments");

            migrationBuilder.DropColumn(name: "ImagesJson", table: "WeddingItems");
        }
    }
}
