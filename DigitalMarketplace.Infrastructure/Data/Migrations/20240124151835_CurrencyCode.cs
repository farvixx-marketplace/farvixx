using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalMarketplace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CurrencyCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency_Code",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Currency",
                type: "character varying(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency_Code",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Currency");
        }
    }
}
