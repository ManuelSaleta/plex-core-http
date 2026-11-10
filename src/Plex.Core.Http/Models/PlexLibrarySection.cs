using System.Text.Json.Serialization;

namespace Plex.Core.Http.Models;

/// <summary>
/// Represents a Plex library section (Directory).
/// </summary>
public class PlexLibrarySection
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("agent")]
    public string? Agent { get; set; }

    [JsonPropertyName("scanner")]
    public string? Scanner { get; set; }

    [JsonPropertyName("language")]
    public string? Language { get; set; }

    [JsonPropertyName("uuid")]
    public string? Uuid { get; set; }

    [JsonPropertyName("updatedAt")]
    public long? UpdatedAt { get; set; }

    [JsonPropertyName("createdAt")]
    public long? CreatedAt { get; set; }

    [JsonPropertyName("scannedAt")]
    public long? ScannedAt { get; set; }

    [JsonPropertyName("refreshing")]
    public bool? Refreshing { get; set; }

    [JsonPropertyName("thumb")]
    public string? Thumb { get; set; }

    [JsonPropertyName("art")]
    public string? Art { get; set; }

    [JsonPropertyName("composite")]
    public string? Composite { get; set; }

    [JsonPropertyName("filters")]
    public bool? Filters { get; set; }

    [JsonPropertyName("allowSync")]
    public bool? AllowSync { get; set; }

    /// <summary>
    /// Gets the section Key parsed as an integer ID.
    /// </summary>
    [JsonIgnore]
    public int Id => int.TryParse(Key, out var id) ? id : 0;
}
