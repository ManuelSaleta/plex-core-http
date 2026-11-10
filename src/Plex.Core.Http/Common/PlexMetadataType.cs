namespace Plex.Core.Http.Common;

/// <summary>
/// Metadata item types supported by Plex Media Server.
/// </summary>
public enum PlexMetadataType
{
    /// <summary>
    /// Movie (1)
    /// </summary>
    Movie = 1,

    /// <summary>
    /// TV Show (2)
    /// </summary>
    Show = 2,

    /// <summary>
    /// Season (3)
    /// </summary>
    Season = 3,

    /// <summary>
    /// Episode (4)
    /// </summary>
    Episode = 4,

    /// <summary>
    /// Trailer (5)
    /// </summary>
    Trailer = 5,

    /// <summary>
    /// Person / Cast / Crew (7)
    /// </summary>
    Person = 7,

    /// <summary>
    /// Music Artist (8)
    /// </summary>
    Artist = 8,

    /// <summary>
    /// Music Album (9)
    /// </summary>
    Album = 9,

    /// <summary>
    /// Music Track (10)
    /// </summary>
    Track = 10,

    /// <summary>
    /// Video Clip (12)
    /// </summary>
    Clip = 12,

    /// <summary>
    /// Photo (13)
    /// </summary>
    Photo = 13,

    /// <summary>
    /// Photo Album (14)
    /// </summary>
    PhotoAlbum = 14,

    /// <summary>
    /// Playlist (15)
    /// </summary>
    Playlist = 15,

    /// <summary>
    /// Playlist folder (16)
    /// </summary>
    PlaylistFolder = 16,

    /// <summary>
    /// Collection (18)
    /// </summary>
    Collection = 18
}
