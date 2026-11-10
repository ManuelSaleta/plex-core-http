using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Plex.Core.Http.Common;

namespace Plex.Core.Http.Handlers;

/// <summary>
/// DelegatingHandler that injects required Plex Media Server headers into HTTP requests.
/// </summary>
public class PlexAuthHandler : DelegatingHandler
{
    private readonly PlexClientOptions _options;

    /// <summary>
    /// Initializes a new instance of <see cref="PlexAuthHandler"/> with options.
    /// </summary>
    public PlexAuthHandler(IOptions<PlexClientOptions> options)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Initializes a new instance of <see cref="PlexAuthHandler"/> with explicit options.
    /// </summary>
    public PlexAuthHandler(PlexClientOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ApplyPlexHeaders(request, _options);
        return base.SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// Applies the Plex standard headers and JSON accept header to an HttpRequestMessage.
    /// </summary>
    public static void ApplyPlexHeaders(HttpRequestMessage request, PlexClientOptions options)
    {
        if (request == null || options == null) return;

        // Force JSON response
        if (!request.Headers.Accept.Any(h => h.MediaType == "application/json"))
        {
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        if (!string.IsNullOrEmpty(options.Token) && !request.Headers.Contains(PlexConstants.HeaderToken))
        {
            request.Headers.TryAddWithoutValidation(PlexConstants.HeaderToken, options.Token);
        }

        if (!string.IsNullOrEmpty(options.ClientIdentifier) && !request.Headers.Contains(PlexConstants.HeaderClientIdentifier))
        {
            request.Headers.TryAddWithoutValidation(PlexConstants.HeaderClientIdentifier, options.ClientIdentifier);
        }

        if (!string.IsNullOrEmpty(options.Product) && !request.Headers.Contains(PlexConstants.HeaderProduct))
        {
            request.Headers.TryAddWithoutValidation(PlexConstants.HeaderProduct, options.Product);
        }

        if (!string.IsNullOrEmpty(options.Version) && !request.Headers.Contains(PlexConstants.HeaderVersion))
        {
            request.Headers.TryAddWithoutValidation(PlexConstants.HeaderVersion, options.Version);
        }

        if (!string.IsNullOrEmpty(options.Device) && !request.Headers.Contains(PlexConstants.HeaderDevice))
        {
            request.Headers.TryAddWithoutValidation(PlexConstants.HeaderDevice, options.Device);
        }

        if (!string.IsNullOrEmpty(options.Platform) && !request.Headers.Contains(PlexConstants.HeaderPlatform))
        {
            request.Headers.TryAddWithoutValidation(PlexConstants.HeaderPlatform, options.Platform);
        }
    }
}
