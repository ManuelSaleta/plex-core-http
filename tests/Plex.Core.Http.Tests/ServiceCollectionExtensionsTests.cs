using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plex.Core.Http.Extensions;
using Xunit;

namespace Plex.Core.Http.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddPlexClient_RegistersServicesCorrectly()
    {
        var services = new ServiceCollection();

        services.AddPlexClient(options =>
        {
            options.ServerUrl = "http://192.168.1.50:32400";
            options.Token = "my-test-token";
            options.ClientIdentifier = "my-unique-client";
            options.Product = "UnitTests";
        });

        var provider = services.BuildServiceProvider();

        var client = provider.GetService<IPlexClient>();
        Assert.NotNull(client);

        var options = provider.GetRequiredService<IOptions<PlexClientOptions>>().Value;
        Assert.Equal("http://192.168.1.50:32400/", options.ServerUri?.ToString());
        Assert.Equal("my-test-token", options.Token);
        Assert.Equal("my-unique-client", options.ClientIdentifier);
        Assert.Equal("UnitTests", options.Product);
    }
}
