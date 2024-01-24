using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DigitalMarketplace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PriceCurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Price");

            migrationBuilder.DropColumn(
                name: "Price_Id",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "Price_CurrencySymbol",
                table: "Products",
                newName: "Currency_CurrencySymbol");

            migrationBuilder.RenameColumn(
                name: "Price_CurrencyName",
                table: "Products",
                newName: "Currency_Name");

            migrationBuilder.RenameColumn(
                name: "Price_CurrencyHtmlCode",
                table: "Products",
                newName: "Currency_HtmlCode");

            migrationBuilder.RenameColumn(
                name: "Price_Amount",
                table: "Products",
                newName: "Price");

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "Users",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "CurrencyName",
                table: "Users",
                type: "character varying(25)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CurrencyName",
                table: "Users",
                column: "CurrencyName");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Currency_CurrencyName",
                table: "Users",
                column: "CurrencyName",
                principalTable: "Currency",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Currency_CurrencyName",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_CurrencyName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CurrencyName",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Currency_CurrencySymbol",
                table: "Products",
                newName: "Price_CurrencySymbol");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Products",
                newName: "Price_Amount");

            migrationBuilder.RenameColumn(
                name: "Currency_Name",
                table: "Products",
                newName: "Price_CurrencyName");

            migrationBuilder.RenameColumn(
                name: "Currency_HtmlCode",
                table: "Products",
                newName: "Price_CurrencyHtmlCode");

            migrationBuilder.AddColumn<int>(
                name: "Price_Id",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Price",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    CurrencyHtmlCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CurrencyName = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    CurrencySymbol = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Price", x => x.Id);
                });
        }
    }
}
