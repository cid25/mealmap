using System.Reflection;
using Mealmap.Api;
using Mealmap.Api.DataAccess;
using Mealmap.Api.DataTransfer;
using Mealmap.Api.Formatters;
using Mealmap.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<HostingOptions>(
    builder.Configuration.GetSection(HostingOptions.SectionName));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IMealRepository, SqlMealRepository>();
builder.Services.AddScoped<IDishRepository, SqlDishRepository>();
builder.Services.AddAutoMapper(typeof(MapperProfile));
builder.Services.AddScoped<MealMapper>();
builder.Services.AddScoped<DishMapper>();
builder.Services.AddDbContext<MealmapDbContext>(options
    => options.UseSqlServer(
        builder.Configuration.GetConnectionString("MealmapDb")));

builder.Services.AddControllers(options =>
    options.InputFormatters.Insert(0, new ImageInputFormatter())
    )
    .ConfigureApiBehaviorOptions(options =>
        options.SuppressMapClientErrors = true
    );
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "Mealmap API",
            Description = "An API for managing dishes and meals."
        });
        
        options.DocumentFilter<ServersDocumentFilter>();
        options.CustomSchemaIds(type => type.Name.Replace("DTO", string.Empty));

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


#pragma warning disable CS1591
public partial class Program { }
#pragma warning restore CS1591
