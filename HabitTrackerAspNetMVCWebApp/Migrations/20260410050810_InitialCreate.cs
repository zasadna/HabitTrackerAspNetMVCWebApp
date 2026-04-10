using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HabitTrackerAspNetMVCWebApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Habits_AspNetUsers_ApplicationUserId",
                table: "Habits");

            migrationBuilder.DropIndex(
                name: "IX_Habits_ApplicationUserId",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Habits");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Habits",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Habits_ApplicationUserId",
                table: "Habits",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Habits_AspNetUsers_ApplicationUserId",
                table: "Habits",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
