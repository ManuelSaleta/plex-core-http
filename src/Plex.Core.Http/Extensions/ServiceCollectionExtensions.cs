using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Plex.Core.Http.Handlers;

namespace Plex.Core.Http.Extensions;

/// <summary>
/// Dependency injection extension methods for registering <see cref="IPlexClient"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds and configures the <see cref="IPlexClient"/> and <see cref="PlexClient"/> with the specified options delegate.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">The action to configure <see cref="PlexClientOptions"/>.</param>
    /// <returns>The <see cref="IHttpClientBuilder"/> for further HTTP pipeline configuration.</returns>
    public static IHttpClientBuilder AddPlexClient(
        this IServiceCollection services,
        Action<PlexClientOptions> configureOptions)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

        services.Configure(configureOptions);
        services.TryAddTransient<PlexAuthHandler>();

        return services.AddHttpClient<IPlexClient, PlexClient>((serviceProvider, httpClient) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<PlexClientOptions>>().Value;
            if (options.ServerUri != null)
            {
                httpClient.BaseAddress = options.ServerUri;
            }
            if (options.Timeout > TimeSpan.Zero)
            {
                httpClient.Timeout = options.Timeout;
            }
        }).AddHttpMessageHandler<PlexAuthHandler>();
    }

    /// <summary>
    /// Adds and configures the <see cref="IPlexClient"/> with pre-instantiated options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="options">The options instance.</param>
    /// <returns>The <see cref="IHttpClientBuilder"/> for further HTTP pipeline configuration.</returns>
    public static IHttpClientBuilder AddPlexClient(
        this IServiceCollection services,
        PlexClientOptions options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));
        return services.AddPlexClient(opt =>
        {
            opt.ServerUri = options.ServerUri;
            opt.Token = options.Token;
            opt.ClientIdentifier = options.ClientIdentifier;
            opt.Product = options.Product;
            opt.Version = options.Version;
            opt.Device = options.Device;
            opt.Platform = options.Platform;
            opt.Timeout = options.Timeout;
        });
    }
}
