using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityTournamentPro.Data.Migrations
{
    /// <inheritdoc />
    public partial class SonDuzenlemeler : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TeamA",
                table: "Matches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TeamB",
                table: "Matches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeamA",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "TeamB",
                table: "Matches");
        }
    }
}
