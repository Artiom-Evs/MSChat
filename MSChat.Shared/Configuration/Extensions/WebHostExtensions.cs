using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;

namespace MSChat.Shared.Configuration.Extensions;

public static class WebHostExtensions
{
    public static void ConfigureKestrelWithGrpc(this IWebHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureKestrel((context, options) =>
        {
            // configure endpoints by appsettings configuration
            options.Configure(context.Configuration.GetSection("Kestrel"));

            // configure endpoints from list of the ports defined in ASPNETCORE_HTTP_PORTS
            var httpPorts = context.Configuration["ASPNETCORE_HTTP_PORTS"]?.Split(';') ?? [];
            foreach (var portStr in httpPorts)
            {
                if (int.TryParse(portStr, out var port))
                {
                    options.ListenAnyIP(port);
                }
            }

            // configure endpoints from list of the ports defined in ASPNETCORE_HTTPS_PORTS
            var httpsPorts = context.Configuration["ASPNETCORE_HTTPS_PORTS"]?.Split(';') ?? [];
            foreach (var portStr in httpsPorts)
            {
                if (int.TryParse(portStr, out var port))
                {
                    options.ListenAnyIP(port, options =>
                    {
                        options.UseHttps();
                    });
                }
            }

            // configure endpoints from list of the URLs defined in ASPNETCORE_URLS
            var urls = context.Configuration["ASPNETCORE_URLS"]?.Split(';') ?? [];
            foreach (var url in urls)
            {
                options.ListenByUrl(url);
            }

            // configure gRPC endpoint by appsettings configuration
            var grpcSettings = context.Configuration.GetGrpcSettings();
            options.ListenByUrl(grpcSettings.Url, options =>
            {
                options.Protocols = HttpProtocols.Http2;
            });
        });
    }

    public static void ListenByUrl(this KestrelServerOptions serverOptions, string url, Action<ListenOptions>? configure = null)
    {
        var uri = new Uri(url);
        var ipAddress = uri.Host switch
        {
            "localhost" => IPAddress.Loopback,
            "0.0.0.0" => IPAddress.Any,
            _ => IPAddress.Parse(uri.Host)
        };
        var endpoint = new IPEndPoint(ipAddress, uri.Port);

        serverOptions.Listen(endpoint, options =>
        {
            if (configure != null)
                configure(options);
            if (uri.Scheme == "https")
                options.UseHttps();
        });
    }
}
