namespace Plex.Core.Http.Common;

/// <summary>
/// Well-known constants used by Plex Media Server HTTP endpoints and headers.
/// </summary>
public static class PlexConstants
{
    /// <summary>
    /// Authentication token header.
    /// </summary>
    public const string HeaderToken = "X-Plex-Token";

    /// <summary>
    /// Unique client identifier header.
    /// </summary>
    public const string HeaderClientIdentifier = "X-Plex-Client-Identifier";

    /// <summary>
    /// Client application product name header.
    /// </summary>
    public const string HeaderProduct = "X-Plex-Product";

    /// <summary>
    /// Client application version header.
    /// </summary>
    public const string HeaderVersion = "X-Plex-Version";

    /// <summary>
    /// Device friendly or model name header.
    /// </summary>
    public const string HeaderDevice = "X-Plex-Device";

    /// <summary>
    /// Client platform name header.
    /// </summary>
    public const string HeaderPlatform = "X-Plex-Platform";

    /// <summary>
    /// Container start header for pagination offset.
    /// </summary>
    public const string HeaderContainerStart = "X-Plex-Container-Start";

    /// <summary>
    /// Container size header for pagination page size.
    /// </summary>
    public const string HeaderContainerSize = "X-Plex-Container-Size";

    /// <summary>
    /// Container total size returned by PMS.
    /// </summary>
    public const string HeaderContainerTotalSize = "X-Plex-Container-Total-Size";

    /// <summary>
    /// Default port for Plex Media Server.
    /// </summary>
    public const int DefaultPort = 32400;
}
