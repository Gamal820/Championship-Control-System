using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Championship_Control_System.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerToMatchEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlayerId",
                table: "MatchEvent",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MatchEvent_PlayerId",
                table: "MatchEvent",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchEvent_Player_PlayerId",
                table: "MatchEvent",
                column: "PlayerId",
                principalTable: "Player",
                principalColumn: "PlayerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchEvent_Player_PlayerId",
                table: "MatchEvent");

            migrationBuilder.DropIndex(
                name: "IX_MatchEvent_PlayerId",
                table: "MatchEvent");

            migrationBuilder.DropColumn(
                name: "PlayerId",
                table: "MatchEvent");
        }
    }
}
