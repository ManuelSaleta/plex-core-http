namespace Plex.Core.Http;

/// <summary>
/// Configuration options for the Plex HTTP client.
/// </summary>
public class PlexClientOptions
{
    /// <summary>
    /// The name used for binding the configuration options json obj for DI.
    /// </summary>
    public const string SectionName = "PlexClientOptions";
    /// <summary>
    /// Base URL for the Plex Media Server instance (e.g. "http://192.168.1.100:32400" or direct plex URL).
    /// </summary>
    public Uri? ServerUri { get; set; }

    /// <summary>
    /// Convenience string setter/getter for <see cref="ServerUri"/>.
    /// </summary>
    public string? ServerUrl
    {
        get => ServerUri?.ToString();
        set => ServerUri = string.IsNullOrWhiteSpace(value) ? null : new Uri(value);
    }

    /// <summary>
    /// Authentication token for Plex Media Server (X-Plex-Token).
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Unique identifier for this client instance (X-Plex-Client-Identifier).
    /// </summary>
    public string ClientIdentifier { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// Name of client application (X-Plex-Product). Defaults to "Plex.Core.Http".
    /// </summary>
    public string Product { get; set; } = "Plex.Core.Http";

    /// <summary>
    /// Version of client application (X-Plex-Version). Defaults to "1.0.0".
    /// </summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// Friendly name or model of client device (X-Plex-Device).
    /// </summary>
    public string? Device { get; set; }

    /// <summary>
    /// Platform name of client device (X-Plex-Platform).
    /// </summary>
    public string? Platform { get; set; }

    /// <summary>
    /// Request timeout. Defaults to 30 seconds.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
}
