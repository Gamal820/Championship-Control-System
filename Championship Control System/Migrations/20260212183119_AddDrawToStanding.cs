using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Championship_Control_System.Migrations
{
    /// <inheritdoc />
    public partial class AddDrawToStanding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Draw",
                table: "TeamStanding",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Draw",
                table: "TeamStanding");
        }
    }
}
