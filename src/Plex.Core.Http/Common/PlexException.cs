using System.Net;

namespace Plex.Core.Http.Common;

/// <summary>
/// Exception thrown when a Plex Media Server HTTP request fails.
/// </summary>
public class PlexException : HttpRequestException
{
    /// <summary>
    /// Gets the raw response content body if available.
    /// </summary>
    public string? ResponseBody { get; }

    /// <summary>
    /// Gets the target URI of the request.
    /// </summary>
    public Uri? RequestUri { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="PlexException"/>.
    /// </summary>
    public PlexException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="PlexException"/> with status code and details.
    /// </summary>
    public PlexException(
        string message,
        HttpStatusCode? statusCode,
        Uri? requestUri = null,
        string? responseBody = null,
        Exception? innerException = null)
        : base(message, innerException, statusCode)
    {
        RequestUri = requestUri;
        ResponseBody = responseBody;
    }
}
