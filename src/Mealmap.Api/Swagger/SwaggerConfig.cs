using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Mealmap.Api.Swagger;

public class SwaggerConfig
{
    private readonly string _authUrl;
    private readonly string _tokenUrl;
    private readonly string _clientId;
    private readonly string _scope;

    public SwaggerConfig(IConfiguration configuration)
    {
        var tenantId = configuration["AzureAd:TenantId"];
        _authUrl = $"{configuration["AzureAd:Instance"]}{tenantId}/oauth2/v2.0/authorize";
        _tokenUrl = $"{configuration["AzureAd:Instance"]}{tenantId}/oauth2/v2.0/token";
        _clientId = configuration["AzureAd:ClientId"]!;
        _scope = $"api://{_clientId}/access";
    }

    public void ConfigureSwaggerGen(SwaggerGenOptions options)
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "Mealmap API",
            Description = "An API for managing dishes and meals."
        });

        options.AddSecurityDefinition("OAuth", CreateSecurityScheme());
        options.AddSecurityRequirement(CreateSecurityRequirement());

        options.DocumentFilter<ServersDocumentFilter>();
        options.OperationFilter<IfMatchHeaderFilter>();
        options.CustomSchemaIds(type => type.Name.Replace("DTO", string.Empty));
        options.ExampleFilters();

        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    }

    public void ConfigureSwaggerUI(SwaggerUIOptions options)
    {
        options.SwaggerEndpoint("/api/swagger/v1/swagger.json", "Mealmap API");
        options.RoutePrefix = "api/swagger";
        options.OAuthUsePkce();
        options.OAuthClientId(_clientId);
        options.OAuthScopes(_scope);
        options.EnablePersistAuthorization();
    }

    private OpenApiSecurityScheme CreateSecurityScheme()
    {
        return new()
        {
            Description = "OAuth 2.0",
            Name = "OAuth",
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new System.Uri(_authUrl),
                    TokenUrl = new System.Uri(_tokenUrl),
                    Scopes = new Dictionary<string, string>
                    {
                        {_scope, "Use API"}
                    }
                }
            }
        };
    }

    private OpenApiSecurityRequirement CreateSecurityRequirement()
    {
        return new()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "oauth2"
                    }
                },
                new[] {"access"}
            }
        };
    }
}
