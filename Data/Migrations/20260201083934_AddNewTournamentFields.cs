using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityTournamentPro.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNewTournamentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "TournamentSelections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Faculty",
                table: "TournamentSelections",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ManagerStudentNumber",
                table: "TournamentSelections",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "TournamentSelections",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TeamManager",
                table: "TournamentSelections",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TeamPlayers",
                table: "TournamentSelections",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "TournamentSelections");

            migrationBuilder.DropColumn(
                name: "Faculty",
                table: "TournamentSelections");

            migrationBuilder.DropColumn(
                name: "ManagerStudentNumber",
                table: "TournamentSelections");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "TournamentSelections");

            migrationBuilder.DropColumn(
                name: "TeamManager",
                table: "TournamentSelections");

            migrationBuilder.DropColumn(
                name: "TeamPlayers",
                table: "TournamentSelections");
        }
    }
}
