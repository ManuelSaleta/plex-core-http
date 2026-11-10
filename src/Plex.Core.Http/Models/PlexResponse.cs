using System.Text.Json.Serialization;

namespace Plex.Core.Http.Models;

/// <summary>
/// Root container wrapper returned by Plex Media Server JSON endpoints.
/// </summary>
/// <typeparam name="T">The payload container type inside MediaContainer.</typeparam>
public class PlexResponse<T>
{
    /// <summary>
    /// Gets or sets the primary MediaContainer.
    /// </summary>
    [JsonPropertyName("MediaContainer")]
    public T? MediaContainer { get; set; }
}
