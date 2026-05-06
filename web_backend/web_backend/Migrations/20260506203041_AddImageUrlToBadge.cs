using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlToBadge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Badges",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Badges");
        }
    }
}
