using FluentAssertions;
using Mealmap.Api.DataAccess;
using Mealmap.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Mealmap.Api.IntegrationTests
{
    [Collection("InSequence")]
    public class SqlDishRepositoryTests
    {
        private readonly MealmapDbContext _dbContext;
        private readonly SqlDishRepository _repository;

        public SqlDishRepositoryTests()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            var dbOptions = new DbContextOptionsBuilder<MealmapDbContext>()
                .UseSqlServer(configuration.GetConnectionString("MealmapDb"))
                .Options;
            _dbContext = new MealmapDbContext(dbOptions);

            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();
            seedData(_dbContext);

            _repository = new SqlDishRepository(_dbContext);
        }

        private void seedData(MealmapDbContext dbContext)
        {
            dbContext.Dishes.Add(new Dish("Krabby Patty") { Id = new Guid("00000000-0000-0000-0000-000000000001") } );
            dbContext.Dishes.Add(new Dish("Sailors Surprise") { Id = new Guid("00000000-0000-0000-0000-000000000010") });
            dbContext.SaveChanges();
        }


        [Fact]
        public void GetAll_ReturnsAllDishes()
        {
            var expectedCount = _dbContext.Dishes.Count();
            
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
        public void GetById_WhenIdExists_ReturnsDish()
        {
            const string existingGuid = "00000000-0000-0000-0000-000000000001";
            var result = _repository.GetById(new Guid(existingGuid));

            result.Should().NotBeNull();
            result!.Name.Should().Be("Krabby Patty");
        }

        [Fact]
        public void Create_WhenDishValid_CreatesEntry()
        {
            const string someDishName = "Salty Sea Dog";
            Guid someGuid = Guid.NewGuid();
            Dish dish = new(someDishName) { Id = someGuid };

            _repository.Create(dish);

            _dbContext.Dishes.First(x => x.Id == someGuid).Should().NotBeNull();
        }
    }
}
