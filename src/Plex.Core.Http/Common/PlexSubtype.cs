namespace Plex.Core.Http.Common;

/// <summary>
/// Well-known metadata subtypes supported by Plex Media Server (e.g. for filtering collections).
/// </summary>
public static class PlexSubtype
{
    /// <summary>
    /// Music album subtype ("album").
    /// </summary>
    public const string Album = "album";

    /// <summary>
    /// Music artist subtype ("artist").
    /// </summary>
    public const string Artist = "artist";

    /// <summary>
    /// Movie subtype ("movie").
    /// </summary>
    public const string Movie = "movie";

    /// <summary>
    /// TV show subtype ("show").
    /// </summary>
    public const string Show = "show";

    /// <summary>
    /// Podcast subtype ("podcast").
    /// </summary>
    public const string Podcast = "podcast";

    /// <summary>
    /// News clip subtype ("news").
    /// </summary>
    public const string News = "news";

    /// <summary>
    /// Photo subtype ("photo").
    /// </summary>
    public const string Photo = "photo";
}
