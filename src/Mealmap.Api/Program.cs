using System.Reflection;
using Mealmap.Api.Dishes;
using Mealmap.Api.Meals;
using Mealmap.Api.Settings;
using Mealmap.Api.Shared;
using Mealmap.Api.Swagger;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.Common.Validation;
using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.MealAggregate;
using Mealmap.Infrastructure.DataAccess;
using Mealmap.Infrastructure.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.Filters;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    // Add Configuration
    builder.Services
        .Configure<HostingOptions>(builder.Configuration.GetSection(HostingOptions.SectionName))
        .Configure<AngularOptions>(builder.Configuration.GetSection(AngularOptions.SectionName));

    // Add Domain services
    builder.Services.RegisterDeferredDomainValidation();

    // Add Infrastructure services
    builder.Services.AddDbContext<MealmapDbContext>(options
        => options.UseSqlServer(
            builder.Configuration.GetConnectionString("MealmapDb"),
            b => b.MigrationsAssembly("Mealmap.Migrations")));
    builder.Services
        .AddScoped<IUnitOfWork, UnitOfWork>()
        .AddScoped<IMealRepository, SqlMealRepository>()
        .AddScoped<IRepository<Dish>, SqlDishRepository>();

    // Add Application services
    builder.Services
        .AddHttpContextAccessor()
        .AddScoped<IRequestContext, RequestContext>()
        .AddScoped<UrlBuilder>()
        .AddAutoMapper(typeof(AutomapperProfile))
        .Scan(scan => scan.FromAssembliesOf(typeof(ICommandProcessor<,>))
            .AddClasses(
                classes => classes.AssignableTo(typeof(ICommandProcessor<,>))).AsImplementedInterfaces()
            .AddClasses(
                classes => classes.AssignableTo(typeof(IQueryResponder<,>))).AsImplementedInterfaces()
        )
        .AddScoped<DishDataTransferObjectValidator>()
        .AddScoped<IOutputMapper<DishDTO, Dish>, DishOutputMapper>()
        .AddScoped<MealDataTransferObjectValidator>()
        .AddScoped<IOutputMapper<MealDTO, Meal>, MealOutputMapper>()
        .Decorate<ICommandProcessor<CreateDishCommand, DishDTO>, CommandLoggerDecorator<CreateDishCommand, DishDTO>>()
        .Decorate<ICommandProcessor<UpdateDishCommand, DishDTO>, CommandLoggerDecorator<UpdateDishCommand, DishDTO>>()
        .Decorate<ICommandProcessor<DeleteDishCommand, DishDTO>, CommandLoggerDecorator<DeleteDishCommand, DishDTO>>()
        .Decorate<ICommandProcessor<UpdateDishImageCommand, DishDTO>, CommandLoggerDecorator<UpdateDishImageCommand, DishDTO>>()
        .Decorate<ICommandProcessor<DeleteDishImageCommand, DishDTO>, CommandLoggerDecorator<DeleteDishImageCommand, DishDTO>>()
        .Decorate<ICommandProcessor<CreateMealCommand, MealDTO>, CommandLoggerDecorator<CreateMealCommand, MealDTO>>()
        .Decorate<ICommandProcessor<UpdateMealCommand, MealDTO>, CommandLoggerDecorator<UpdateMealCommand, MealDTO>>()
        .Decorate<ICommandProcessor<DeleteMealCommand, MealDTO>, CommandLoggerDecorator<DeleteMealCommand, MealDTO>>();

    builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration);
    builder.Services.AddControllers(options =>
        options.InputFormatters.Insert(0, new ImageInputFormatter())
        );
    builder.Services.AddProblemDetails();

    // Add Swashbuckle services
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

    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger(c => c.RouteTemplate = "api/swagger/{documentname}/swagger.json");
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "Mealmap API");
            c.RoutePrefix = "api/swagger";
        });
    }

    app.UseExceptionHandler("/api/error");

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseStatusCodePages();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.MapFallbackToFile("index.html");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}


// required for Boundary Tests
public partial class Program { }
