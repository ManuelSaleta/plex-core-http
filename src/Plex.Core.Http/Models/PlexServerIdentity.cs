using System.Text.Json.Serialization;

namespace Plex.Core.Http.Models;

/// <summary>
/// Server identity information returned by /identity.
/// </summary>
public class PlexServerIdentity
{
    [JsonPropertyName("machineIdentifier")]
    public string MachineIdentifier { get; set; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("claimed")]
    public bool Claimed { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }
}
