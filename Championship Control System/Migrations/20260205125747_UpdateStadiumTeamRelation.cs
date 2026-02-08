using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Championship_Control_System.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStadiumTeamRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Team_Stadium",
                table: "Team");

            migrationBuilder.DropIndex(
                name: "IX_Team_StadiumID",
                table: "Team");

            migrationBuilder.CreateIndex(
                name: "IX_Team_StadiumID",
                table: "Team",
                column: "StadiumID",
                unique: true,
                filter: "[StadiumID] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Team_Stadium_StadiumID",
                table: "Team",
                column: "StadiumID",
                principalTable: "Stadium",
                principalColumn: "StadiumID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Team_Stadium_StadiumID",
                table: "Team");

            migrationBuilder.DropIndex(
                name: "IX_Team_StadiumID",
                table: "Team");

            migrationBuilder.CreateIndex(
                name: "IX_Team_StadiumID",
                table: "Team",
                column: "StadiumID");

            migrationBuilder.AddForeignKey(
                name: "FK_Team_Stadium",
                table: "Team",
                column: "StadiumID",
                principalTable: "Stadium",
                principalColumn: "StadiumID");
        }
    }
}
