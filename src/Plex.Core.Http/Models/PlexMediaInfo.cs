using System.Text.Json.Serialization;

namespace Plex.Core.Http.Models;

/// <summary>
/// Represents media stream specifications (resolution, bitrate, codecs, parts).
/// </summary>
public class PlexMediaInfo
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("duration")]
    public long? Duration { get; set; }

    [JsonPropertyName("bitrate")]
    public int? Bitrate { get; set; }

    [JsonPropertyName("width")]
    public int? Width { get; set; }

    [JsonPropertyName("height")]
    public int? Height { get; set; }

    [JsonPropertyName("aspectRatio")]
    public double? AspectRatio { get; set; }

    [JsonPropertyName("audioChannels")]
    public int? AudioChannels { get; set; }

    [JsonPropertyName("audioCodec")]
    public string? AudioCodec { get; set; }

    [JsonPropertyName("videoCodec")]
    public string? VideoCodec { get; set; }

    [JsonPropertyName("videoResolution")]
    public string? VideoResolution { get; set; }

    [JsonPropertyName("container")]
    public string? Container { get; set; }

    [JsonPropertyName("videoFrameRate")]
    public string? VideoFrameRate { get; set; }

    [JsonPropertyName("Part")]
    public List<PlexPart>? Part { get; set; }
}
