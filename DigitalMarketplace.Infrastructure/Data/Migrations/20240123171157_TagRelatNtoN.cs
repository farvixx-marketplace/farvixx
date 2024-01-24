using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalMarketplace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TagRelatNtoN : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductTag");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Tags",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ProductId",
                table: "Tags",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Products_ProductId",
                table: "Tags",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Products_ProductId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_ProductId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Tags");

            migrationBuilder.CreateTable(
                name: "ProductTag",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    TagsName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTag", x => new { x.ProductId, x.TagsName });
                    table.ForeignKey(
                        name: "FK_ProductTag_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductTag_Tags_TagsName",
                        column: x => x.TagsName,
                        principalTable: "Tags",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductTag_TagsName",
                table: "ProductTag",
                column: "TagsName");
        }
    }
}
