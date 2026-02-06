using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Championship_Control_System.Migrations
{
    /// <inheritdoc />
    public partial class AddModelsandFixUserModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Championship",
                columns: table => new
                {
                    ChampionshipID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChampionshipName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Season = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Logo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Champion__0947429778D8F492", x => x.ChampionshipID);
                });

            migrationBuilder.CreateTable(
                name: "Stadium",
                columns: table => new
                {
                    StadiumID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StadiumName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Capacity = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Stadium__ED83303868D5F43E", x => x.StadiumID);
                });

            migrationBuilder.CreateTable(
                name: "Team",
                columns: table => new
                {
                    TeamID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeamName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FoundationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Logo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StadiumID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Team__123AE7B9560400F4", x => x.TeamID);
                    table.ForeignKey(
                        name: "FK_Team_Stadium",
                        column: x => x.StadiumID,
                        principalTable: "Stadium",
                        principalColumn: "StadiumID");
                });

            migrationBuilder.CreateTable(
                name: "Coach",
                columns: table => new
                {
                    CoachID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BirthData = table.Column<DateOnly>(type: "date", nullable: true),
                    Img = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TeamID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Coach__F411D9A124C3AD99", x => x.CoachID);
                    table.ForeignKey(
                        name: "FK_Coach_Team",
                        column: x => x.TeamID,
                        principalTable: "Team",
                        principalColumn: "TeamID");
                });

            migrationBuilder.CreateTable(
                name: "Match",
                columns: table => new
                {
                    MatchID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    HomeGoals = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    AwayGoals = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    AvailableTicket = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    StadiumID = table.Column<int>(type: "int", nullable: true),
                    ChampionshipID = table.Column<int>(type: "int", nullable: true),
                    HomeTeamID = table.Column<int>(type: "int", nullable: true),
                    AwayTeamID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Match__4218C837F1D022D7", x => x.MatchID);
                    table.ForeignKey(
                        name: "FK_Match_AwayTeam",
                        column: x => x.AwayTeamID,
                        principalTable: "Team",
                        principalColumn: "TeamID");
                    table.ForeignKey(
                        name: "FK_Match_Championship",
                        column: x => x.ChampionshipID,
                        principalTable: "Championship",
                        principalColumn: "ChampionshipID");
                    table.ForeignKey(
                        name: "FK_Match_HomeTeam",
                        column: x => x.HomeTeamID,
                        principalTable: "Team",
                        principalColumn: "TeamID");
                    table.ForeignKey(
                        name: "FK_Match_Stadium",
                        column: x => x.StadiumID,
                        principalTable: "Stadium",
                        principalColumn: "StadiumID");
                });

            migrationBuilder.CreateTable(
                name: "Player",
                columns: table => new
                {
                    PlayerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Img = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Position = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ShirtNumber = table.Column<int>(type: "int", nullable: true),
                    TeamID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Player__4A4E74A806D6C4C2", x => x.PlayerID);
                    table.ForeignKey(
                        name: "FK_Player_Team",
                        column: x => x.TeamID,
                        principalTable: "Team",
                        principalColumn: "TeamID");
                });

            migrationBuilder.CreateTable(
                name: "Team_Championship",
                columns: table => new
                {
                    TeamId = table.Column<int>(type: "int", nullable: false),
                    ChampionshipId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Team_Cha__72AE9390DE28B07D", x => new { x.TeamId, x.ChampionshipId });
                    table.ForeignKey(
                        name: "FK_TC_Championship",
                        column: x => x.ChampionshipId,
                        principalTable: "Championship",
                        principalColumn: "ChampionshipID");
                    table.ForeignKey(
                        name: "FK_TC_Team",
                        column: x => x.TeamId,
                        principalTable: "Team",
                        principalColumn: "TeamID");
                });

            migrationBuilder.CreateTable(
                name: "TeamStanding",
                columns: table => new
                {
                    StandingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Played = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    Won = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    Lost = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    GoalDifference = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    TeamID = table.Column<int>(type: "int", nullable: true),
                    ChampionshipID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TeamStan__FC2758E130E70C30", x => x.StandingID);
                    table.ForeignKey(
                        name: "FK_Standing_Championship",
                        column: x => x.ChampionshipID,
                        principalTable: "Championship",
                        principalColumn: "ChampionshipID");
                    table.ForeignKey(
                        name: "FK_Standing_Team",
                        column: x => x.TeamID,
                        principalTable: "Team",
                        principalColumn: "TeamID");
                });

            migrationBuilder.CreateTable(
                name: "MatchEvent",
                columns: table => new
                {
                    EventID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Minute = table.Column<int>(type: "int", nullable: true),
                    EventType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MatchID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MatchEve__7944C87044D9C5EC", x => x.EventID);
                    table.ForeignKey(
                        name: "FK_Event_Match",
                        column: x => x.MatchID,
                        principalTable: "Match",
                        principalColumn: "MatchID");
                });

            migrationBuilder.CreateTable(
                name: "Ticket",
                columns: table => new
                {
                    TicketID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SeatNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BookingDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    MatchId = table.Column<int>(type: "int", nullable: true),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Ticket__712CC62700A93A21", x => x.TicketID);
                    table.ForeignKey(
                        name: "FK_Ticket_Match",
                        column: x => x.MatchId,
                        principalTable: "Match",
                        principalColumn: "MatchID");
                    table.ForeignKey(
                        name: "FK_Ticket_User",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "User_See_Event",
                columns: table => new
                {
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EventID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User_See__001C802B7224D085", x => new { x.UserID, x.EventID });
                    table.ForeignKey(
                        name: "FK_See_Event",
                        column: x => x.EventID,
                        principalTable: "MatchEvent",
                        principalColumn: "EventID");
                    table.ForeignKey(
                        name: "FK_See_User",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Coach_TeamID",
                table: "Coach",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_Match_AwayTeamID",
                table: "Match",
                column: "AwayTeamID");

            migrationBuilder.CreateIndex(
                name: "IX_Match_ChampionshipID",
                table: "Match",
                column: "ChampionshipID");

            migrationBuilder.CreateIndex(
                name: "IX_Match_HomeTeamID",
                table: "Match",
                column: "HomeTeamID");

            migrationBuilder.CreateIndex(
                name: "IX_Match_StadiumID",
                table: "Match",
                column: "StadiumID");

            migrationBuilder.CreateIndex(
                name: "IX_MatchEvent_MatchID",
                table: "MatchEvent",
                column: "MatchID");

            migrationBuilder.CreateIndex(
                name: "IX_Player_TeamID",
                table: "Player",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_Team_StadiumID",
                table: "Team",
                column: "StadiumID");

            migrationBuilder.CreateIndex(
                name: "IX_Team_Championship_ChampionshipId",
                table: "Team_Championship",
                column: "ChampionshipId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamStanding_ChampionshipID",
                table: "TeamStanding",
                column: "ChampionshipID");

            migrationBuilder.CreateIndex(
                name: "IX_TeamStanding_TeamID",
                table: "TeamStanding",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_MatchId",
                table: "Ticket",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_UserID",
                table: "Ticket",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_User_See_Event_EventID",
                table: "User_See_Event",
                column: "EventID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coach");

            migrationBuilder.DropTable(
                name: "Player");

            migrationBuilder.DropTable(
                name: "Team_Championship");

            migrationBuilder.DropTable(
                name: "TeamStanding");

            migrationBuilder.DropTable(
                name: "Ticket");

            migrationBuilder.DropTable(
                name: "User_See_Event");

            migrationBuilder.DropTable(
                name: "MatchEvent");

            migrationBuilder.DropTable(
                name: "Match");

            migrationBuilder.DropTable(
                name: "Team");

            migrationBuilder.DropTable(
                name: "Championship");

            migrationBuilder.DropTable(
                name: "Stadium");
        }
    }
}
