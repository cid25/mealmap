using Mealmap.Model;
using Mealmap.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;

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
            dbContext.Meals.Add(new Meal() { Id = new Guid("00000000-0000-0000-0000-000000000001") } );
            dbContext.Meals.Add(new Meal() { Id = new Guid("00000000-0000-0000-0000-000000000010") });
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
        public void GetById_WhenIdNonExisting_ReturnsNull()
        {
            const string nonExistingGuid = "99999999-9999-9999-9999-999999999999";
            var result = _repository.GetById(new Guid(nonExistingGuid));

            result.Should().BeNull();
        }

        [Fact]
        public void GetById_WhenIdExisting_ReturnsMeal()
        {
            const string existingGuid = "00000000-0000-0000-0000-000000000001";
            var result = _repository.GetById(new Guid(existingGuid));

            result.Should().NotBeNull();
        }

        [Fact]
        public void Create_WhenMealValid_CreatesEntry()
        {
            var guid = Guid.NewGuid();
            Meal meal = new() { Id = guid };

            _repository.Create(meal);

            _dbContext.Meals.First(x => x.Id == guid).Should().NotBeNull();
        }
    }
}
