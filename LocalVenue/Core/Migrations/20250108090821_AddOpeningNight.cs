using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LocalVenue.Migrations
{
    /// <inheritdoc />
    public partial class AddOpeningNight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OpeningNight",
                table: "Shows",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OpeningNight",
                table: "Shows");
        }
    }
}