using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Championship_Control_System.Migrations
{
    /// <inheritdoc />
    public partial class AddCoachToTeamRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coach_Team",
                table: "Coach");

            migrationBuilder.DropIndex(
                name: "IX_Coach_TeamID",
                table: "Coach");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Team");

            migrationBuilder.AddColumn<int>(
                name: "CoachId",
                table: "Team",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Team_CoachId",
                table: "Team",
                column: "CoachId",
                unique: true,
                filter: "[CoachId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Team_Coach_CoachId",
                table: "Team",
                column: "CoachId",
                principalTable: "Coach",
                principalColumn: "CoachID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Team_Coach_CoachId",
                table: "Team");

            migrationBuilder.DropIndex(
                name: "IX_Team_CoachId",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "CoachId",
                table: "Team");

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Team",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coach_TeamID",
                table: "Coach",
                column: "TeamID");

            migrationBuilder.AddForeignKey(
                name: "FK_Coach_Team",
                table: "Coach",
                column: "TeamID",
                principalTable: "Team",
                principalColumn: "TeamID");
        }
    }
}
