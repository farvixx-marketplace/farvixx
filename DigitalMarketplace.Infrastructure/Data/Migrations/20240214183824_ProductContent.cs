using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalMarketplace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProductContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "Products");
        }
    }
}
