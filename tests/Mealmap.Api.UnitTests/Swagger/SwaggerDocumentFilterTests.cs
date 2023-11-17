using Mealmap.Api.Shared;
using Mealmap.Api.Swagger;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Mealmap.Api.UnitTests.Swagger;

public class SwaggerDocumentFilterTests
{
    public SwaggerDocumentFilterTests()
    {
    }

    [Fact]
    public void Apply_WhenSingleAddressAndHost_AddsProperServer()
    {
        var filter = setupFilter(
            addresses: ["https://localhost:5001"],
            hosts: ["test.com"]
        );
        var (servers, swaggerDoc) = mockDocument();

        filter.Apply(swaggerDoc, mockDocumentFilterContext());

        servers.ElementAt(0).Url.Should().Be("https://test.com:5001");
    }

    [Fact]
    public void Apply_WhenTwoAddressesAndHosts_AddsFourServers()
    {
        var filter = setupFilter(
            addresses: ["https://localhost:5001", "http://localhost:5002"],
            hosts: ["test.com", "example.com"]
        );
        var (servers, swaggerDoc) = mockDocument();

        filter.Apply(swaggerDoc, mockDocumentFilterContext());

        servers.Should().HaveCount(4);
    }

    [Theory]
    [InlineData("http")]
    [InlineData("https")]
    public void Apply_WhenAddressHasStandardPort_ServerHasNone(string schema)
    {
        var filter = setupFilter(
            addresses: [schema + "://localhost"],
            hosts: ["test.com"]
        );
        var (servers, swaggerDoc) = mockDocument();

        filter.Apply(swaggerDoc, mockDocumentFilterContext());

        servers.ElementAt(0).Url.Should().EndWith("test.com");
    }

    private static ServersDocumentFilter setupFilter(string[] addresses, string[] hosts)
    {
        var adress = new List<string>();
        adress.AddRange(addresses.ToList());
        var addressFeature = Mock.Of<IServerAddressesFeature>(
            m => m.Addresses == adress);

        return new ServersDocumentFilter(
            Options.Create(new HostingOptions() { Hosts = hosts }),
            Mock.Of<IServer>(m => m.Features.Get<IServerAddressesFeature>() == addressFeature)
        );
    }

    private static DocumentFilterContext mockDocumentFilterContext()
    {
        return new DocumentFilterContext(
            Mock.Of<IEnumerable<ApiDescription>>(),
            Mock.Of<ISchemaGenerator>(),
            new SchemaRepository(string.Empty)
        );
    }

    private static (List<OpenApiServer>, OpenApiDocument) mockDocument()
    {
        var servers = new List<OpenApiServer>();
        var swaggerDoc = Mock.Of<OpenApiDocument>(m => m.Servers == servers);

        return (servers, swaggerDoc);
    }
}
