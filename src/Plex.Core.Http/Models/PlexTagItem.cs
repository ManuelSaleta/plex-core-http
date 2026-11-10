using System.Text.Json.Serialization;

namespace Plex.Core.Http.Models;

/// <summary>
/// Represents a metadata tag item (e.g. Genre, Director, Role, Writer).
/// </summary>
public class PlexTagItem
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("tag")]
    public string Tag { get; set; } = string.Empty;

    [JsonPropertyName("role")]
    public string? Role { get; set; }

    [JsonPropertyName("thumb")]
    public string? Thumb { get; set; }
}
