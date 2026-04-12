using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HabitTrackerAspNetMVCWebApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHabitLogStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "HabitLogs");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "HabitLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "HabitLogs");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "HabitLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
