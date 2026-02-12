using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityTournamentPro.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixRoutesAndTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Announcements_TournamentId",
                table: "Announcements",
                column: "TournamentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_Tournaments_TournamentId",
                table: "Announcements",
                column: "TournamentId",
                principalTable: "Tournaments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_Tournaments_TournamentId",
                table: "Announcements");

            migrationBuilder.DropIndex(
                name: "IX_Announcements_TournamentId",
                table: "Announcements");
        }
    }
}
