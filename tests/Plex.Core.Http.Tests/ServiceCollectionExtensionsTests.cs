using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plex.Core.Http.Extensions;

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
        client.Should().NotBeNull();

        var options = provider.GetRequiredService<IOptions<PlexClientOptions>>().Value;
        options.ServerUri?.ToString().Should().Be("http://192.168.1.50:32400/");
        options.Token.Should().Be("my-test-token");
        options.ClientIdentifier.Should().Be("my-unique-client");
        options.Product.Should().Be("UnitTests");
    }
}
