using System.Text.Json.Serialization;

namespace Plex.Core.Http.Models;

/// <summary>
/// Server details and capabilities returned by root endpoint "/".
/// </summary>
public class PlexServerCapabilities
{
    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("friendlyName")]
    public string FriendlyName { get; set; } = string.Empty;

    [JsonPropertyName("machineIdentifier")]
    public string MachineIdentifier { get; set; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("platform")]
    public string? Platform { get; set; }

    [JsonPropertyName("platformVersion")]
    public string? PlatformVersion { get; set; }

    [JsonPropertyName("myPlex")]
    public bool? MyPlex { get; set; }

    [JsonPropertyName("myPlexMappingState")]
    public string? MyPlexMappingState { get; set; }

    [JsonPropertyName("myPlexSigninState")]
    public string? MyPlexSigninState { get; set; }

    [JsonPropertyName("myPlexSubscription")]
    public bool? MyPlexSubscription { get; set; }

    [JsonPropertyName("myPlexUsername")]
    public string? MyPlexUsername { get; set; }

    [JsonPropertyName("allowSync")]
    public bool? AllowSync { get; set; }

    [JsonPropertyName("allowSharing")]
    public bool? AllowSharing { get; set; }

    [JsonPropertyName("multiuser")]
    public bool? Multiuser { get; set; }
}
