using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mealmap.Api.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "mealmap");

            migrationBuilder.CreateTable(
                name: "dishes",
                schema: "mealmap",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dishes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "meals",
                schema: "mealmap",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiningDate = table.Column<DateTime>(type: "date", nullable: false),
                    DishId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_meals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_meals_dishes_DishId",
                        column: x => x.DishId,
                        principalSchema: "mealmap",
                        principalTable: "dishes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_meals_DishId",
                schema: "mealmap",
                table: "meals",
                column: "DishId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "meals",
                schema: "mealmap");

            migrationBuilder.DropTable(
                name: "dishes",
                schema: "mealmap");
        }
    }
}
