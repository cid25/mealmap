using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mealmap.Migrations.Migrations
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
                name: "dish",
                schema: "mealmap",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Servings = table.Column<int>(type: "int", nullable: false),
                    Image_Content = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Image_ContentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Instructions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dish", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "meal",
                schema: "mealmap",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiningDate = table.Column<DateTime>(type: "date", nullable: false),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_meal", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ingredient",
                schema: "mealmap",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    UnitOfMeasurement = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DishId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ingredient", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ingredient_dish_DishId",
                        column: x => x.DishId,
                        principalSchema: "mealmap",
                        principalTable: "dish",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "course",
                schema: "mealmap",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false),
                    MainCourse = table.Column<bool>(type: "bit", nullable: false),
                    DishId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MealId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course", x => x.Id);
                    table.ForeignKey(
                        name: "FK_course_dish_DishId",
                        column: x => x.DishId,
                        principalSchema: "mealmap",
                        principalTable: "dish",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_course_meal_MealId",
                        column: x => x.MealId,
                        principalSchema: "mealmap",
                        principalTable: "meal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_course_DishId",
                schema: "mealmap",
                table: "course",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_course_MealId",
                schema: "mealmap",
                table: "course",
                column: "MealId");

            migrationBuilder.CreateIndex(
                name: "IX_ingredient_DishId",
                schema: "mealmap",
                table: "ingredient",
                column: "DishId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "course",
                schema: "mealmap");

            migrationBuilder.DropTable(
                name: "ingredient",
                schema: "mealmap");

            migrationBuilder.DropTable(
                name: "meal",
                schema: "mealmap");

            migrationBuilder.DropTable(
                name: "dish",
                schema: "mealmap");
        }
    }
}
