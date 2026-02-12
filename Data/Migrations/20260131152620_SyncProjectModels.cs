using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityTournamentPro.Data.Migrations
{
    /// <inheritdoc />
    public partial class SyncProjectModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "TournamentSelections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Faculty",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TournamentSelections");

            migrationBuilder.DropColumn(
                name: "Faculty",
                table: "AspNetUsers");
        }
    }
}
