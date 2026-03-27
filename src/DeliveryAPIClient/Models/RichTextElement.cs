using System.Text.Json.Serialization;

namespace DeliveryAPIClient.Models;

/// <summary>
/// A single node in the Umbraco rich text element tree.
/// tag="#root"   → root container
/// tag="#text"   → text node (use Text property)
/// tag="umb-rte-block" → embedded block (use attributes["content-id"])
/// Any other tag → standard HTML element
/// </summary>
public class RichTextElement
{
    [JsonPropertyName("tag")]
    public string Tag { get; set; } = string.Empty;

    /// <summary>Text content — only set when Tag is "#text".</summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("attributes")]
    public Dictionary<string, string>? Attributes { get; set; }

    [JsonPropertyName("elements")]
    public List<RichTextElement>? Elements { get; set; }

    public bool IsTextNode  => Tag == "#text";
    public bool IsRoot      => Tag == "#root";
    public bool IsBlock     => Tag == "umb-rte-block";

    /// <summary>The content-id attribute on an umb-rte-block element.</summary>
    public Guid? BlockContentId =>
        Attributes?.TryGetValue("content-id", out var id) == true && Guid.TryParse(id, out var g)
            ? g : null;
}
