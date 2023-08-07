using System.Reflection;
using Mealmap.Api;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Api.OutputMappers;
using Mealmap.Api.RequestFormatters;
using Mealmap.Api.Swagger;
using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.MealAggregate;
using Mealmap.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);


// Add Configuration
builder.Services.Configure<HostingOptions>(
    builder.Configuration.GetSection(HostingOptions.SectionName));

// Add Domain Services
builder.Services.AddScoped<IMealService, MealService>();

// Add Infrastructure Services
builder.Services.AddDbContext<MealmapDbContext>(options
    => options.UseSqlServer(
        builder.Configuration.GetConnectionString("MealmapDb")));
builder.Services.AddScoped<IMealRepository, SqlMealRepository>();
builder.Services.AddScoped<IDishRepository, SqlDishRepository>();

// Add Application Services
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IRequestContext, RequestContext>();
builder.Services.AddAutoMapper(typeof(AutomapperProfile));
builder.Services.AddScoped<IOutputMapper<DishDTO, Dish>, DishOutputMapper>();
builder.Services.AddScoped<IOutputMapper<MealDTO, Meal>, MealOutputMapper>();

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
