﻿// <auto-generated />
using Mealmap.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace Mealmap.Api.Migrations
{
    [DbContext(typeof(MealmapDbContext))]
    partial class MealmapDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Mealmap.Model.Dish", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("dishes", "mealmap");
                });

            modelBuilder.Entity("Mealmap.Model.Meal", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DiningDate")
                        .HasColumnType("date");

                    b.Property<Guid?>("DishId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("DishId");

                    b.ToTable("meals", "mealmap");
                });

            modelBuilder.Entity("Mealmap.Model.Meal", b =>
                {
                    b.HasOne("Mealmap.Model.Dish", "Dish")
                        .WithMany()
                        .HasForeignKey("DishId");

                    b.Navigation("Dish");
                });
#pragma warning restore 612, 618
        }
    }
}
