using System.Reflection;
using Mealmap.Api.DataTransfer;
using Mealmap.Api.DataAccess;
using Mealmap.Api.Formatters;
using Mealmap.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddScoped(x => {
    var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
    var factory = x.GetRequiredService<IUrlHelperFactory>();
    return factory.GetUrlHelper(actionContext!);
});
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

        options.CustomSchemaIds(type => type.Name.Replace("DTO", string.Empty));

        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    }
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


#pragma warning disable CS1591
public partial class Program { }
#pragma warning restore CS1591
