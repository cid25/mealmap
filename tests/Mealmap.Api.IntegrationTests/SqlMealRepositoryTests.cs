﻿using Mealmap.Model;
using Mealmap.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Mealmap.Api.IntegrationTests
{
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
            dbContext.Meals.Add(new Meal("Krabby Patty") { Id = new Guid("00000000-0000-0000-0000-000000000001") } );
            dbContext.Meals.Add(new Meal("Sailors Surprise") { Id = new Guid("00000000-0000-0000-0000-000000000010") });
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
        public void GetById_WhenGivenNonExistingId_ReturnsNull()
        {
            const string nonExistingGuid = "99999999-9999-9999-9999-999999999999";
            var result = _repository.GetById(new Guid(nonExistingGuid));

            result.Should().BeNull();
        }

        [Fact]
        public void GetById_WhenGivenExistingId_ReturnsMeal()
        {
            const string existingGuid = "00000000-0000-0000-0000-000000000001";
            var result = _repository.GetById(new Guid(existingGuid));

            result.Should().NotBeNull();
            result!.Name.Should().Be("Krabby Patty");
        }

        [Fact]
        public void Create_WhenGivenMealWithoutId_ThrowsArgumentNullException()
        {
            const string someMealName = "Salty Sea Dog";
            Meal meal = new(someMealName);
            
            Action act = () => _repository.Create(meal);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Create_WhenGivenMealWithId_CreatesEntry()
        {
            const string someMealName = "Salty Sea Dog";
            Meal meal = new(someMealName) { Id = Guid.NewGuid() };

            _repository.Create(meal);

            _dbContext.Meals.First(x => x.Name == someMealName).Should().NotBeNull();
        }
    }
}