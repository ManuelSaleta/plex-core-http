using System.Text.Json.Serialization;

namespace Plex.Core.Http.Models;

/// <summary>
/// Represents a Plex metadata item (Movie, Show, Episode, Artist, Album, Track, Collection, etc.).
/// </summary>
public class PlexMetadataItem
{
    [JsonPropertyName("ratingKey")]
    public string RatingKey { get; set; } = string.Empty;

    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("guid")]
    public string? Guid { get; set; }

    [JsonPropertyName("parentGuid")]
    public string? ParentGuid { get; set; }

    [JsonPropertyName("grandparentGuid")]
    public string? GrandparentGuid { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("subtype")]
    public string? Subtype { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("titleSort")]
    public string? TitleSort { get; set; }

    [JsonPropertyName("originalTitle")]
    public string? OriginalTitle { get; set; }

    [JsonPropertyName("parentTitle")]
    public string? ParentTitle { get; set; }

    [JsonPropertyName("grandparentTitle")]
    public string? GrandparentTitle { get; set; }

    [JsonPropertyName("summary")]
    public string? Summary { get; set; }

    [JsonPropertyName("studio")]
    public string? Studio { get; set; }

    [JsonPropertyName("year")]
    public int? Year { get; set; }

    [JsonPropertyName("rating")]
    public double? Rating { get; set; }

    [JsonPropertyName("contentRating")]
    public string? ContentRating { get; set; }

    [JsonPropertyName("duration")]
    public long? Duration { get; set; }

    [JsonPropertyName("index")]
    public int? Index { get; set; }

    [JsonPropertyName("parentIndex")]
    public int? ParentIndex { get; set; }

    [JsonPropertyName("leafCount")]
    public int? LeafCount { get; set; }

    [JsonPropertyName("childCount")]
    public int? ChildCount { get; set; }

    [JsonPropertyName("smart")]
    public bool? Smart { get; set; }

    [JsonPropertyName("thumb")]
    public string? Thumb { get; set; }

    [JsonPropertyName("art")]
    public string? Art { get; set; }

    [JsonPropertyName("parentThumb")]
    public string? ParentThumb { get; set; }

    [JsonPropertyName("grandparentThumb")]
    public string? GrandparentThumb { get; set; }

    [JsonPropertyName("parentKey")]
    public string? ParentKey { get; set; }

    [JsonPropertyName("parentRatingKey")]
    public string? ParentRatingKey { get; set; }

    [JsonPropertyName("grandparentKey")]
    public string? GrandparentKey { get; set; }

    [JsonPropertyName("grandparentRatingKey")]
    public string? GrandparentRatingKey { get; set; }

    [JsonPropertyName("addedAt")]
    public long? AddedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public long? UpdatedAt { get; set; }

    [JsonPropertyName("originallyAvailableAt")]
    public string? OriginallyAvailableAt { get; set; }

    [JsonPropertyName("viewCount")]
    public int? ViewCount { get; set; }

    [JsonPropertyName("lastViewedAt")]
    public long? LastViewedAt { get; set; }

    [JsonPropertyName("Media")]
    public List<PlexMediaInfo>? Media { get; set; }

    [JsonPropertyName("Genre")]
    public List<PlexTagItem>? Genre { get; set; }

    [JsonPropertyName("Role")]
    public List<PlexTagItem>? Role { get; set; }

    [JsonPropertyName("Director")]
    public List<PlexTagItem>? Director { get; set; }

    [JsonPropertyName("Writer")]
    public List<PlexTagItem>? Writer { get; set; }

    /// <summary>
    /// Gets the RatingKey parsed as an integer ID.
    /// </summary>
    [JsonIgnore]
    public int Id => int.TryParse(RatingKey, out var id) ? id : 0;
}
