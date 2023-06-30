using Mealmap.Model;
using Mealmap.Repositories;
using AutoMapper;
using Mealmap.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IMealRepository, SqlMealRepository>();
builder.Services.AddAutoMapper(typeof(MealMapperProfile));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
