using System.Reflection;
using Mealmap.Api;
using Mealmap.Api.CommandHandlers;
using Mealmap.Api.CommandValidators;
using Mealmap.Api.OutputMappers;
using Mealmap.Api.RequestFormatters;
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
        .Configure<AngularSettings>(builder.Configuration.GetSection(AngularSettings.SectionName));

    // Add Domain Services
    builder.Services.RegisterDeferredDomainValidation();

    // Add Infrastructure Services
    builder.Services.AddDbContext<MealmapDbContext>(options
        => options.UseSqlServer(
            builder.Configuration.GetConnectionString("MealmapDb"),
            b => b.MigrationsAssembly("Mealmap.Migrations")));
    builder.Services
        .AddScoped<IUnitOfWork, UnitOfWork>()
        .AddScoped<IMealRepository, SqlMealRepository>()
        .AddScoped<IRepository<Dish>, SqlDishRepository>();

    // Add Application Services
    builder.Services
        .AddHttpContextAccessor()
        .AddScoped<IRequestContext, RequestContext>()
        .AddScoped<UrlBuilder>()
        .AddDataTransferObjectValidators()
        .AddCommandHandlers()
        .AddOutputMappers();

    builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration);
    builder.Services.AddControllers(options =>
        options.InputFormatters.Insert(0, new ImageInputFormatter())
        );
    builder.Services.AddProblemDetails();

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
