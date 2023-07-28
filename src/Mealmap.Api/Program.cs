﻿using System.Reflection;
using Mealmap.Api;
using Mealmap.Api.InputMappers;
using Mealmap.Api.OutputMappers;
using Mealmap.Api.RequestFormatters;
using Mealmap.Api.Swagger;
using Mealmap.DataAccess;
using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.MealAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);


// Add configuration
builder.Services.Configure<HostingOptions>(
    builder.Configuration.GetSection(HostingOptions.SectionName));

// Add domain services
builder.Services.AddScoped<MealService>();

// Add data access
builder.Services.AddDbContext<MealmapDbContext>(options
    => options.UseSqlServer(
        builder.Configuration.GetConnectionString("MealmapDb")));
builder.Services.AddScoped<IMealRepository, SqlMealRepository>();
builder.Services.AddScoped<IDishRepository, SqlDishRepository>();

// Add data transfer
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IRequestContext, RequestContext>();
builder.Services.AddAutoMapper(typeof(MapperProfile));
builder.Services.AddScoped<DishInputMapper>();
builder.Services.AddScoped<DishOutputMapper>();
builder.Services.AddScoped<MealInputMapper>();
builder.Services.AddScoped<MealOutputMapper>();

builder.Services.AddControllers(options =>
    options.InputFormatters.Insert(0, new ImageInputFormatter())
    )
    .ConfigureApiBehaviorOptions(options =>
        options.SuppressMapClientErrors = true
    );

// Add Swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerOperationExamples();
builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "Mealmap API",
            Description = "An API for managing dishes and meals."
        });

        options.DocumentFilter<ServersDocumentFilter>();
        options.OperationFilter<IfMatchHeaderFilter>();
        options.CustomSchemaIds(type => type.Name.Replace("DTO", string.Empty));
        options.ExampleFilters();

        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    }
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c => c.RouteTemplate = "api/swagger/{documentname}/swagger.json");
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "Mealmap API");
        c.RoutePrefix = "api/swagger";
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();



public partial class Program { }
