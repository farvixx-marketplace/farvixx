using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalMarketplace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NullableUserCurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Currency_CurrencyCode",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "CurrencyCode",
                table: "Users",
                type: "character varying(5)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(5)");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Currency_CurrencyCode",
                table: "Users",
                column: "CurrencyCode",
                principalTable: "Currency",
                principalColumn: "Code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Currency_CurrencyCode",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "CurrencyCode",
                table: "Users",
                type: "character varying(5)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(5)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Currency_CurrencyCode",
                table: "Users",
                column: "CurrencyCode",
                principalTable: "Currency",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
