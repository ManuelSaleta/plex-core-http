using System.Text.Json;
using System.Text.Json.Serialization;

namespace Plex.Core.Http.Serialization;

/// <summary>
/// Configured JSON serializer options and convenience methods for Plex Media Server payloads.
/// </summary>
public static class PlexJsonSerializer
{
    /// <summary>
    /// Default JSON serializer options configured for case-insensitivity, string-to-number reading, and omitting nulls.
    /// </summary>
    public static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// Deserializes a JSON string into the specified Plex model.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    /// <param name="json">The JSON string.</param>
    /// <returns>The deserialized object or null.</returns>
    public static T? Deserialize<T>(string json) =>
        JsonSerializer.Deserialize<T>(json, DefaultOptions);

    /// <summary>
    /// Serializes a Plex model into a JSON string.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    /// <param name="value">The object to serialize.</param>
    /// <returns>A formatted JSON string.</returns>
    public static string Serialize<T>(T value) =>
        JsonSerializer.Serialize(value, DefaultOptions);
}
