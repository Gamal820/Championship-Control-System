using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Championship_Control_System.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketPriceToMatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TicketPrice",
                table: "Match",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TicketPrice",
                table: "Match");
        }
    }
}
