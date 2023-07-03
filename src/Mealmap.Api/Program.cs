using Mealmap.Model;
using Mealmap.Repositories;
using AutoMapper;
using Mealmap.Api;
using Mealmap.Api.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IMealRepository, SqlMealRepository>();
builder.Services.AddAutoMapper(typeof(MealMapperProfile));
builder.Services.AddDbContext<MealmapDbContext>(options
    => options.UseSqlServer(builder.Configuration.GetConnectionString("MealmapDb")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.CustomSchemaIds(type => type.Name.Replace("DTO", string.Empty))
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();


public partial class Program { }
