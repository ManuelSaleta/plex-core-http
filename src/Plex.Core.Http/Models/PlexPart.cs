using System.Text.Json.Serialization;

namespace Plex.Core.Http.Models;

/// <summary>
/// Represents a media file part within a media item.
/// </summary>
public class PlexPart
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("key")]
    public string? Key { get; set; }

    [JsonPropertyName("duration")]
    public long? Duration { get; set; }

    [JsonPropertyName("file")]
    public string? File { get; set; }

    [JsonPropertyName("size")]
    public long? Size { get; set; }

    [JsonPropertyName("container")]
    public string? Container { get; set; }

    [JsonPropertyName("videoProfile")]
    public string? VideoProfile { get; set; }

    [JsonPropertyName("audioProfile")]
    public string? AudioProfile { get; set; }

    [JsonPropertyName("has64bitOffsets")]
    public bool? Has64bitOffsets { get; set; }
}
