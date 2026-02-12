using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityTournamentPro.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGenderCategoryToTournament : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GenderCategory",
                table: "Tournaments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Karma");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GenderCategory",
                table: "Tournaments");
        }

    }
}
