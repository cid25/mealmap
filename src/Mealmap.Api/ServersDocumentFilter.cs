using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Mealmap.Api
{
    public class ServersDocumentFilter : IDocumentFilter
    {
        private readonly HostingOptions _options;
        private readonly IServer _server;
        
        public ServersDocumentFilter(IOptions<HostingOptions> options, IServer server)
          => (_options, _server) = (options.Value, server);

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var addressFeature = _server.Features.Get<IServerAddressesFeature>();

            if (addressFeature != null)
            {
                foreach (var address in addressFeature.Addresses)
                {
                    var scheme = address.Split("://")[0];
                    var maybePort = address.Split(":")[^1];
                    int port;
                    if (!int.TryParse(maybePort, out port) 
                        || (scheme == "http" && port == 80)
                        || (scheme == "https" && port == 443)
                    )
                        port = -1;

                    foreach (var host in _options.Hosts)
                    {
                        if (host != string.Empty)
                        {
                            UriBuilder addressBuilder = new UriBuilder()
                            {
                                Scheme = scheme,
                                Host = host,
                                Port = port
                            };
                            swaggerDoc.Servers.Add(new OpenApiServer() { Url = addressBuilder.Uri.ToString().TrimEnd('/') });
                        }
                    }
                }
            }
        }
    }
}
