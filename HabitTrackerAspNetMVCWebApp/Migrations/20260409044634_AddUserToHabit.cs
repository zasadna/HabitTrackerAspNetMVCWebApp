using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HabitTrackerAspNetMVCWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddUserToHabit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Habits",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Habits");
        }
    }
}
