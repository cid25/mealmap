using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mealmap.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AttendeesPerCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Attendees",
                schema: "mealmap",
                table: "course",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attendees",
                schema: "mealmap",
                table: "course");
        }
    }
}
