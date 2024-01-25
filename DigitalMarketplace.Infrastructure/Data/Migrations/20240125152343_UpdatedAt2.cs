using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalMarketplace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedAt2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "character varying(55)",
                maxLength: 55,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(55)",
                oldMaxLength: 55,
                oldDefaultValueSql: "[FirstName] + '-' + [LastName] + '-' + [Id]");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 1, 25, 15, 4, 27, 560, DateTimeKind.Utc).AddTicks(2015));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 1, 25, 15, 4, 27, 560, DateTimeKind.Utc).AddTicks(1556));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "character varying(55)",
                maxLength: 55,
                nullable: false,
                defaultValueSql: "[FirstName] + '-' + [LastName] + '-' + [Id]",
                oldClrType: typeof(string),
                oldType: "character varying(55)",
                oldMaxLength: 55);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 1, 25, 15, 4, 27, 560, DateTimeKind.Utc).AddTicks(2015),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 1, 25, 15, 4, 27, 560, DateTimeKind.Utc).AddTicks(1556),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }
    }
}
