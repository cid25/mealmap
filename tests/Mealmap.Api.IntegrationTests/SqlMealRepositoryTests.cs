﻿using FluentAssertions;
using Mealmap.Api.Repositories;
using Mealmap.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Mealmap.Api.IntegrationTests
{
    [Collection("InSequence")]
    public class SqlMealRepositoryTests
    {
        private readonly MealmapDbContext _dbContext;
        private readonly SqlMealRepository _repository;

        public SqlMealRepositoryTests()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            var dbOptions = new DbContextOptionsBuilder<MealmapDbContext>()
                .UseSqlServer(configuration.GetConnectionString("MealmapDb"))
                .Options;
            _dbContext = new MealmapDbContext(dbOptions);

            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();
            seedData(_dbContext);

            _repository = new SqlMealRepository(_dbContext);
        }

        private void seedData(MealmapDbContext dbContext)
        {
            var dish = new Dish("Sailors Surprise") { Id = new Guid("00000000-0000-0000-0000-000000000001") };
            dbContext.Meals.Add(new Meal() { Id = new Guid("10000000-0000-0000-0000-000000000001"), Date = new DateOnly(2020,1,1), Dish = dish } );
            dbContext.Meals.Add(new Meal() { Id = new Guid("10000000-0000-0000-0000-000000000010"), Date = new DateOnly(2020, 1, 2), Dish = dish });
            dbContext.SaveChanges();
        }


        [Fact]
        public void GetAll_ReturnsAllMeals()
        {
            var expectedCount = _dbContext.Meals.Count();
            
            var result = _repository.GetAll();

            result.Should().NotBeEmpty().And.HaveCount(expectedCount);
        }

        [Fact]
        public void GetById_WhenMealWithIdNonExisting_ReturnsNull()
        {
            const string nonExistingGuid = "99999999-9999-9999-9999-999999999999";
            var result = _repository.GetById(new Guid(nonExistingGuid));

            result.Should().BeNull();
        }

        [Fact]
        public void GetById_WhenMealWithIdExists_ReturnsMeal()
        {
            const string existingGuid = "10000000-0000-0000-0000-000000000001";
            var result = _repository.GetById(new Guid(existingGuid));

            result.Should().NotBeNull();
        }

        [Fact]
        public void Create_WhenMealValid_CreatesEntry()
        {
            var dish = _dbContext.Dishes.First();
            var guid = Guid.NewGuid();
            Meal meal = new() { Id = guid, Date = new DateOnly(2020,12,31), Dish = dish};

            _repository.Create(meal);

            _dbContext.Meals.First(x => x.Id == guid).Should().NotBeNull();
        }
    }
}
