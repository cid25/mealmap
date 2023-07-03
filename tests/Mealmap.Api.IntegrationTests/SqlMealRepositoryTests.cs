using Mealmap.Model;
using Mealmap.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using FluentAssertions;

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
            dbContext.Meals.Add(new Meal("Krabby Patty") { Id = Guid.NewGuid() } );
            dbContext.Meals.Add(new Meal("Sailors Surprise") { Id = Guid.NewGuid() });
            dbContext.SaveChanges();
        }


        [Fact]
        public void GetAll_ReturnsCompleteSet()
        {
            var result = _repository.GetAll();

            result.Should().NotBeEmpty().And.HaveCount(2);
        }
    }
}
