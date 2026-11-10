using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plex.Core.Http.Common;
using Plex.Core.Http.Handlers;
using Plex.Core.Http.Models;

namespace Plex.Core.Http;

/// <summary>
/// Implementation of <see cref="IPlexClient"/> for Plex Media Server.
/// </summary>
public class PlexClient : IPlexClient, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly bool _disposeHttpClient;

    /// <summary>
    /// Default JSON serializer options configured for Plex API responses.
    /// </summary>
    public static readonly JsonSerializerOptions DefaultJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <inheritdoc />
    public PlexClientOptions Options { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="PlexClient"/> with an injected <see cref="HttpClient"/> and options.
    /// </summary>
    [ActivatorUtilitiesConstructor]
    public PlexClient(HttpClient httpClient, IOptions<PlexClientOptions> options)
        : this(httpClient, options?.Value ?? throw new ArgumentNullException(nameof(options)), disposeClient: false)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="PlexClient"/> with an injected <see cref="HttpClient"/> and options.
    /// </summary>
    public PlexClient(HttpClient httpClient, PlexClientOptions options, bool disposeClient = false)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        Options = options ?? throw new ArgumentNullException(nameof(options));
        _disposeHttpClient = disposeClient;

        if (_httpClient.BaseAddress == null && Options.ServerUri != null)
        {
            _httpClient.BaseAddress = Options.ServerUri;
        }

        if (Options.Timeout > TimeSpan.Zero)
        {
            _httpClient.Timeout = Options.Timeout;
        }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="PlexClient"/> with options, managing its own HttpClient.
    /// </summary>
    public PlexClient(PlexClientOptions options)
        : this(new HttpClient(), options, disposeClient: true)
    {
    }

    /// <inheritdoc />
    public async Task<PlexServerIdentity?> GetIdentityAsync(CancellationToken cancellationToken = default)
    {
        var response = await GetAsync<PlexServerIdentity>("identity", cancellationToken).ConfigureAwait(false);
        return response?.MediaContainer;
    }

    /// <inheritdoc />
    public async Task<PlexServerCapabilities?> GetServerCapabilitiesAsync(CancellationToken cancellationToken = default)
    {
        var response = await GetAsync<PlexServerCapabilities>("/", cancellationToken).ConfigureAwait(false);
        return response?.MediaContainer;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PlexLibrarySection>> GetSectionsAsync(CancellationToken cancellationToken = default)
    {
        var response = await GetAsync<PlexMediaContainer<PlexMetadataItem>>("library/sections/all", cancellationToken).ConfigureAwait(false);
        return response?.MediaContainer?.Directory ?? (IReadOnlyList<PlexLibrarySection>)Array.Empty<PlexLibrarySection>();
    }

    /// <inheritdoc />
    public async Task<PlexLibrarySection?> GetSectionAsync(int sectionId, CancellationToken cancellationToken = default)
    {
        var response = await GetAsync<PlexMediaContainer<PlexMetadataItem>>($"library/sections/{sectionId}", cancellationToken).ConfigureAwait(false);
        return response?.MediaContainer?.Directory?.FirstOrDefault();
    }

    /// <inheritdoc />
    public async Task<bool> RefreshSectionAsync(int sectionId, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"library/sections/{sectionId}/refresh");
        using var response = await SendAsync(request, cancellationToken).ConfigureAwait(false);
        return response.IsSuccessStatusCode;
    }

    /// <inheritdoc />
    public async Task<bool> EmptySectionTrashAsync(int sectionId, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, $"library/sections/{sectionId}/emptyTrash");
        using var response = await SendAsync(request, cancellationToken).ConfigureAwait(false);
        return response.IsSuccessStatusCode;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PlexMetadataItem>> GetSectionItemsAsync(
        int sectionId,
        PlexMetadataType? type = null,
        CancellationToken cancellationToken = default)
    {
        var path = $"library/sections/{sectionId}/all";
        if (type.HasValue)
        {
            path += $"?type={(int)type.Value}";
        }

        var response = await GetAsync<PlexMediaContainer<PlexMetadataItem>>(path, cancellationToken).ConfigureAwait(false);
        return response?.MediaContainer?.Metadata ?? (IReadOnlyList<PlexMetadataItem>)Array.Empty<PlexMetadataItem>();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PlexMetadataItem>> GetAlbumsAsync(int sectionId, CancellationToken cancellationToken = default)
    {
        var response = await GetAsync<PlexMediaContainer<PlexMetadataItem>>($"library/sections/{sectionId}/albums", cancellationToken).ConfigureAwait(false);
        return response?.MediaContainer?.Metadata ?? (IReadOnlyList<PlexMetadataItem>)Array.Empty<PlexMetadataItem>();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PlexMetadataItem>> GetCollectionsAsync(
        int sectionId,
        string? subtype = null,
        CancellationToken cancellationToken = default)
    {
        var path = $"library/sections/{sectionId}/collections";
        if (!string.IsNullOrWhiteSpace(subtype))
        {
            path += $"?subtype={Uri.EscapeDataString(subtype)}";
        }

        var response = await GetAsync<PlexMediaContainer<PlexMetadataItem>>(path, cancellationToken).ConfigureAwait(false);
        return response?.MediaContainer?.Metadata ?? (IReadOnlyList<PlexMetadataItem>)Array.Empty<PlexMetadataItem>();
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<PlexMetadataItem>> GetAlbumCollectionsAsync(int sectionId, CancellationToken cancellationToken = default)
    {
        return GetCollectionsAsync(sectionId, "album", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PlexMetadataItem>> GetCollectionItemsAsync(int collectionId, CancellationToken cancellationToken = default)
    {
        var response = await GetAsync<PlexMediaContainer<PlexMetadataItem>>($"library/collections/{collectionId}/items", cancellationToken).ConfigureAwait(false);
        return response?.MediaContainer?.Metadata ?? (IReadOnlyList<PlexMetadataItem>)Array.Empty<PlexMetadataItem>();
    }

    /// <inheritdoc />
    public async Task<PlexMetadataItem?> CreateCollectionAsync(
        int sectionId,
        string title,
        PlexMetadataType? type = null,
        bool smart = false,
        string? uri = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Collection title cannot be empty.", nameof(title));

        var query = new List<string>
        {
            $"sectionId={sectionId}",
            $"title={Uri.EscapeDataString(title)}"
        };

        if (type.HasValue)
        {
            query.Add($"type={(int)type.Value}");
        }

        if (smart)
        {
            query.Add("smart=true");
            if (!string.IsNullOrWhiteSpace(uri))
            {
                query.Add($"uri={Uri.EscapeDataString(uri)}");
            }
        }

        var endpoint = $"library/collections?{string.Join("&", query)}";
        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
        using var response = await SendAsync(request, cancellationToken).ConfigureAwait(false);

        await EnsureSuccessResponseAsync(response, request.RequestUri, cancellationToken).ConfigureAwait(false);

        var result = await response.Content.ReadFromJsonAsync<PlexResponse<PlexMediaContainer<PlexMetadataItem>>>(
            DefaultJsonOptions,
            cancellationToken).ConfigureAwait(false);

        return result?.MediaContainer?.Metadata?.FirstOrDefault();
    }

    /// <inheritdoc />
    public async Task<bool> DeleteCollectionAsync(int sectionId, int collectionId, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"library/sections/{sectionId}/collection/{collectionId}");
        using var response = await SendAsync(request, cancellationToken).ConfigureAwait(false);
        return response.IsSuccessStatusCode;
    }

    /// <inheritdoc />
    public async Task<bool> AddItemToCollectionAsync(int collectionId, string itemUri, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(itemUri))
            throw new ArgumentException("Item URI cannot be empty.", nameof(itemUri));

        using var request = new HttpRequestMessage(HttpMethod.Put, $"library/collections/{collectionId}/items?uri={Uri.EscapeDataString(itemUri)}");
        using var response = await SendAsync(request, cancellationToken).ConfigureAwait(false);
        return response.IsSuccessStatusCode;
    }

    /// <inheritdoc />
    public async Task<bool> RemoveItemFromCollectionAsync(int collectionId, int itemId, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, $"library/collections/{collectionId}/items/{itemId}");
        using var response = await SendAsync(request, cancellationToken).ConfigureAwait(false);
        return response.IsSuccessStatusCode;
    }

    /// <inheritdoc />
    public async Task<PlexMetadataItem?> GetMetadataAsync(string ratingKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(ratingKey))
            throw new ArgumentException("Rating key cannot be empty.", nameof(ratingKey));

        var response = await GetAsync<PlexMediaContainer<PlexMetadataItem>>($"library/metadata/{ratingKey}", cancellationToken).ConfigureAwait(false);
        return response?.MediaContainer?.Metadata?.FirstOrDefault();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PlexMetadataItem>> GetMetadataChildrenAsync(string ratingKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(ratingKey))
            throw new ArgumentException("Rating key cannot be empty.", nameof(ratingKey));

        var response = await GetAsync<PlexMediaContainer<PlexMetadataItem>>($"library/metadata/{ratingKey}/children", cancellationToken).ConfigureAwait(false);
        return response?.MediaContainer?.Metadata ?? (IReadOnlyList<PlexMetadataItem>)Array.Empty<PlexMetadataItem>();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PlexMetadataItem>> SearchSectionAsync(int sectionId, string query, CancellationToken cancellationToken = default)
    {
        var path = $"library/sections/{sectionId}/all?title={Uri.EscapeDataString(query)}";
        var response = await GetAsync<PlexMediaContainer<PlexMetadataItem>>(path, cancellationToken).ConfigureAwait(false);
        return response?.MediaContainer?.Metadata ?? (IReadOnlyList<PlexMetadataItem>)Array.Empty<PlexMetadataItem>();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PlexMetadataItem>> GetActiveSessionsAsync(CancellationToken cancellationToken = default)
    {
        var response = await GetAsync<PlexMediaContainer<PlexMetadataItem>>("status/sessions", cancellationToken).ConfigureAwait(false);
        return response?.MediaContainer?.Metadata ?? (IReadOnlyList<PlexMetadataItem>)Array.Empty<PlexMetadataItem>();
    }

    /// <inheritdoc />
    public async Task<PlexResponse<T>?> GetAsync<T>(string relativeUri, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, relativeUri);
        using var response = await SendAsync(request, cancellationToken).ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureSuccessResponseAsync(response, request.RequestUri, cancellationToken).ConfigureAwait(false);

        return await response.Content.ReadFromJsonAsync<PlexResponse<T>>(DefaultJsonOptions, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        PlexAuthHandler.ApplyPlexHeaders(request, Options);
        return await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    private static async Task EnsureSuccessResponseAsync(HttpResponseMessage response, Uri? requestUri, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            string? body = null;
            try
            {
                body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                // Ignore body read failures during error handling
            }

            throw new PlexException(
                $"Plex request to '{requestUri}' failed with status {(int)response.StatusCode} ({response.StatusCode}).",
                response.StatusCode,
                requestUri,
                body);
        }
    }

    /// <summary>
    /// Disposes the underlying HttpClient if owned.
    /// </summary>
    public void Dispose()
    {
        if (_disposeHttpClient)
        {
            _httpClient.Dispose();
        }
        GC.SuppressFinalize(this);
    }
}
