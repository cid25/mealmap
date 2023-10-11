﻿// <auto-generated />
using System;
using Mealmap.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Mealmap.Api.Migrations
{
    [DbContext(typeof(MealmapDbContext))]
    [Migration("20231011123416_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Mealmap.Domain.DishAggregate.Dish", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasMaxLength(80)
                        .HasColumnType("nvarchar(80)");

                    b.Property<string>("Instructions")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("Servings")
                        .HasColumnType("int");

                    b.Property<byte[]>("Version")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.HasKey("Id");

                    b.ToTable("dish", "mealmap");
                });

            modelBuilder.Entity("Mealmap.Domain.MealAggregate.Meal", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DiningDate")
                        .HasColumnType("date");

                    b.Property<byte[]>("Version")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.HasKey("Id");

                    b.ToTable("meal", "mealmap");
                });

            modelBuilder.Entity("Mealmap.Domain.DishAggregate.Dish", b =>
                {
                    b.OwnsOne("Mealmap.Domain.DishAggregate.DishImage", "Image", b1 =>
                        {
                            b1.Property<Guid>("DishId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<byte[]>("Content")
                                .IsRequired()
                                .HasColumnType("varbinary(max)");

                            b1.Property<string>("ContentType")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("DishId");

                            b1.ToTable("dish", "mealmap");

                            b1.WithOwner()
                                .HasForeignKey("DishId");
                        });

                    b.OwnsMany("Mealmap.Domain.DishAggregate.Ingredient", "Ingredients", b1 =>
                        {
                            b1.Property<Guid>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Description")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("nvarchar(100)");

                            b1.Property<Guid>("DishId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<decimal>("Quantity")
                                .HasPrecision(8, 2)
                                .HasColumnType("decimal(8,2)");

                            b1.Property<string>("UnitOfMeasurement")
                                .IsRequired()
                                .HasMaxLength(30)
                                .HasColumnType("nvarchar(30)");

                            b1.HasKey("Id");

                            b1.HasIndex("DishId");

                            b1.ToTable("ingredient", "mealmap");

                            b1.WithOwner()
                                .HasForeignKey("DishId");
                        });

                    b.Navigation("Image");

                    b.Navigation("Ingredients");
                });

            modelBuilder.Entity("Mealmap.Domain.MealAggregate.Meal", b =>
                {
                    b.OwnsMany("Mealmap.Domain.MealAggregate.Course", "Courses", b1 =>
                        {
                            b1.Property<Guid>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("uniqueidentifier");

                            b1.Property<Guid>("DishId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Index")
                                .HasColumnType("int");

                            b1.Property<bool>("MainCourse")
                                .HasColumnType("bit");

                            b1.Property<Guid>("MealId")
                                .HasColumnType("uniqueidentifier");

                            b1.HasKey("Id");

                            b1.HasIndex("DishId");

                            b1.HasIndex("MealId");

                            b1.ToTable("course", "mealmap");

                            b1.HasOne("Mealmap.Domain.DishAggregate.Dish", null)
                                .WithMany()
                                .HasForeignKey("DishId")
                                .OnDelete(DeleteBehavior.Cascade)
                                .IsRequired();

                            b1.WithOwner()
                                .HasForeignKey("MealId");
                        });

                    b.Navigation("Courses");
                });
#pragma warning restore 612, 618
        }
    }
}
