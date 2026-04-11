using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HabitTrackerAspNetMVCWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddKanbanStatusToHabit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KanbanStatus",
                table: "Habits",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KanbanStatus",
                table: "Habits");
        }
    }
}
