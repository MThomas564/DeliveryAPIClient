using System.Text.Json.Serialization;

namespace DeliveryAPIClient.Models;

/// <summary>
/// Represents a single link from a Multi-URL Picker or Link Picker property.
/// Use GetLinkProperty / GetLinksProperty on ContentItemBase to access these.
/// </summary>
public class LinkModel
{
    /// <summary>The resolved URL. Relative for content/media links, absolute for external.</summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>Display name / link text.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>HTML target attribute value, e.g. "_blank". Null means same window.</summary>
    [JsonPropertyName("target")]
    public string? Target { get; set; }

    /// <summary>"content", "media", or "external".</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    public bool IsExternal => Type == "external";
    public bool OpensInNewTab => Target == "_blank";
}
