using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceWorkerActiveWithStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "Workers");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Workers",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Workers");

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Workers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
