using System.Text.Json.Serialization;

namespace Plex.Core.Http.Models;

/// <summary>
/// General MediaContainer holding lists of metadata items or library directories.
/// </summary>
/// <typeparam name="T">Metadata item type.</typeparam>
public class PlexMediaContainer<T>
{
    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("totalSize")]
    public int? TotalSize { get; set; }

    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    [JsonPropertyName("allowSync")]
    public bool? AllowSync { get; set; }

    [JsonPropertyName("identifier")]
    public string? Identifier { get; set; }

    [JsonPropertyName("mediaTagPrefix")]
    public string? MediaTagPrefix { get; set; }

    [JsonPropertyName("mediaTagVersion")]
    public long? MediaTagVersion { get; set; }

    [JsonPropertyName("title1")]
    public string? Title1 { get; set; }

    [JsonPropertyName("title2")]
    public string? Title2 { get; set; }

    [JsonPropertyName("viewGroup")]
    public string? ViewGroup { get; set; }

    [JsonPropertyName("viewMode")]
    public string? ViewMode { get; set; }

    [JsonPropertyName("librarySectionID")]
    public int? LibrarySectionID { get; set; }

    [JsonPropertyName("librarySectionTitle")]
    public string? LibrarySectionTitle { get; set; }

    [JsonPropertyName("librarySectionUUID")]
    public string? LibrarySectionUUID { get; set; }

    /// <summary>
    /// Metadata items contained in the response (e.g. albums, collections, tracks, movies).
    /// </summary>
    [JsonPropertyName("Metadata")]
    public List<T>? Metadata { get; set; }

    /// <summary>
    /// Directories contained in the response (e.g. library sections).
    /// </summary>
    [JsonPropertyName("Directory")]
    public List<PlexLibrarySection>? Directory { get; set; }
}

/// <summary>
/// MediaContainer alias for standard PlexMetadataItem collections.
/// </summary>
public class PlexMediaContainer : PlexMediaContainer<PlexMetadataItem>
{
}
