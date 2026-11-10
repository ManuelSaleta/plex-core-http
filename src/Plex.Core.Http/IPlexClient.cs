using Plex.Core.Http.Common;
using Plex.Core.Http.Models;

namespace Plex.Core.Http;

/// <summary>
/// Modern, high-performance HTTP client for communicating with Plex Media Server.
/// </summary>
public interface IPlexClient
{
    /// <summary>
    /// Gets the client configuration options.
    /// </summary>
    PlexClientOptions Options { get; }

    /// <summary>
    /// Gets the server identity information (machine identifier, version, claimed state).
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task<PlexServerIdentity?> GetIdentityAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the PMS server details and capabilities from root "/".
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task<PlexServerCapabilities?> GetServerCapabilitiesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all library sections (directories).
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task<IReadOnlyList<PlexLibrarySection>> GetSectionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a specific library section by its integer ID.
    /// </summary>
    /// <param name="sectionId">The section ID.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task<PlexLibrarySection?> GetSectionAsync(int sectionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Triggers a library scan/refresh on a section.
    /// </summary>
    /// <param name="sectionId">The section ID to refresh.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task<bool> RefreshSectionAsync(int sectionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Empties trash for a given library section.
    /// </summary>
    /// <param name="sectionId">The section ID.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task<bool> EmptySectionTrashAsync(int sectionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all items in a library section, optionally filtered by metadata type.
    /// </summary>
    /// <param name="sectionId">The section ID.</param>
    /// <param name="type">Optional metadata type filter (e.g. Movie, Show, Album).</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task<IReadOnlyList<PlexMetadataItem>> GetSectionItemsAsync(
        int sectionId,
        PlexMetadataType? type = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all albums in a music library section (/library/sections/{sectionId}/albums).
    /// </summary>
    /// <param name="sectionId">The music section ID.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task<IReadOnlyList<PlexMetadataItem>> GetAlbumsAsync(int sectionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all collections in a section, optionally filtered by subtype.
    /// </summary>
    /// <param name="sectionId">The section ID.</param>
    /// <param name="subtype">Optional subtype filter (e.g. "album", "artist", "movie", "show").</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task<IReadOnlyList<PlexMetadataItem>> GetCollectionsAsync(
        int sectionId,
        string? subtype = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves album collections specifically in a music library section.
    /// </summary>
    /// <param name="sectionId">The music section ID.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task<IReadOnlyList<PlexMetadataItem>> GetAlbumCollectionsAsync(int sectionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the items (e.g. albums, movies, tracks) contained within a collection.
    /// </summary>
    /// <param name="collectionId">The collection ratingKey / ID.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task<IReadOnlyList<PlexMetadataItem>> GetCollectionItemsAsync(int collectionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new collection in a library section.
    /// </summary>
    /// <param name="sectionId">The section ID.</param>
    /// <param name="title">Title for the new collection.</param>
    /// <param name="type">Optional metadata type (e.g. PlexMetadataType.Album = 9).</param>
    /// <param name="smart">True if creating a smart collection.</param>
    /// <param name="uri">Optional query URI required when smart=true.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task<PlexMetadataItem?> CreateCollectionAsync(
        int sectionId,
        string title,
        PlexMetadataType? type = null,
        bool smart = false,
        string? uri = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a collection from a library section.
    /// </summary>
    /// <param name="sectionId">The section ID.</param>
    /// <param name="collectionId">The collection ID to delete.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task<bool> DeleteCollectionAsync(int sectionId, int collectionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds an item to a collection by item URI.
    /// </summary>
    /// <param name="collectionId">The collection ID.</param>
    /// <param name="itemUri">The URI of the item to add (e.g. "server://.../com.plexapp.plugins.library/library/metadata/1234").</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task<bool> AddItemToCollectionAsync(int collectionId, string itemUri, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an item from a collection.
    /// </summary>
    /// <param name="collectionId">The collection ID.</param>
    /// <param name="itemId">The item ID to remove.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task<bool> RemoveItemFromCollectionAsync(int collectionId, int itemId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves metadata for a specific item by its ratingKey.
    /// </summary>
    /// <param name="ratingKey">The ratingKey of the metadata item.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task<PlexMetadataItem?> GetMetadataAsync(string ratingKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves children of a metadata item (e.g. tracks for an album, seasons for a show).
    /// </summary>
    /// <param name="ratingKey">The parent ratingKey.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task<IReadOnlyList<PlexMetadataItem>> GetMetadataChildrenAsync(string ratingKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for items within a specific section.
    /// </summary>
    /// <param name="sectionId">The section ID.</param>
    /// <param name="query">The search term.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task<IReadOnlyList<PlexMetadataItem>> SearchSectionAsync(int sectionId, string query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves current active playback sessions.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task<IReadOnlyList<PlexMetadataItem>> GetActiveSessionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a GET request and deserializes the wrapped response container.
    /// </summary>
    /// <typeparam name="T">The container type.</typeparam>
    /// <param name="relativeUri">The relative endpoint path and query.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task<PlexResponse<T>?> GetAsync<T>(string relativeUri, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a raw HTTP request with Plex headers applied.
    /// </summary>
    /// <param name="request">The HTTP request message.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
}
