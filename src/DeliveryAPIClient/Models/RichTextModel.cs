using System.Text;
using System.Text.Json.Serialization;
using System.Web;

namespace DeliveryAPIClient.Models;

/// <summary>
/// The structured JSON response for an Umbraco rich text (TinyMCE) property.
/// Use ToHtml() to get a rendered HTML string, or walk Elements directly.
/// </summary>
public class RichTextModel
{
    [JsonPropertyName("tag")]
    public string Tag { get; set; } = "#root";

    [JsonPropertyName("attributes")]
    public Dictionary<string, string>? Attributes { get; set; }

    [JsonPropertyName("elements")]
    public List<RichTextElement>? Elements { get; set; }

    /// <summary>
    /// Embedded blocks. Keyed by Content.Id — matches the content-id attribute
    /// on umb-rte-block elements in the tree.
    /// </summary>
    [JsonPropertyName("blocks")]
    public List<RichTextBlockModel>? Blocks { get; set; }

    public bool HasBlocks => Blocks?.Count > 0;

    /// <summary>
    /// Look up an embedded block by its content ID.
    /// </summary>
    public RichTextBlockModel? GetBlock(Guid contentId) =>
        Blocks?.FirstOrDefault(b => b.Content.Id == contentId);

    /// <summary>
    /// Renders the element tree to an HTML string.
    /// umb-rte-block elements are rendered as div placeholders:
    ///   &lt;div data-umb-block="guid" data-content-type="alias"&gt;&lt;/div&gt;
    /// Use these to mount Blazor components or JS in the consuming app.
    /// </summary>
    public string ToHtml() =>
        RenderElements(Elements);

    private static string RenderElements(IEnumerable<RichTextElement>? elements)
    {
        if (elements is null) return string.Empty;
        var sb = new StringBuilder();
        foreach (var el in elements)
            sb.Append(RenderElement(el));
        return sb.ToString();
    }

    private static string RenderElement(RichTextElement el)
    {
        if (el.IsTextNode)
            return HttpUtility.HtmlEncode(el.Text ?? string.Empty);

        if (el.IsRoot)
            return RenderElements(el.Elements);

        if (el.IsBlock)
        {
            var id = el.Attributes?.GetValueOrDefault("content-id") ?? string.Empty;
            return $"""<div data-umb-block="{id}"></div>""";
        }

        // Self-closing void elements
        if (VoidElements.Contains(el.Tag.ToLowerInvariant()))
        {
            var attrs = BuildAttributes(el.Attributes);
            return $"<{el.Tag}{attrs} />";
        }

        var innerHtml = RenderElements(el.Elements);
        var attrStr   = BuildAttributes(el.Attributes);
        return $"<{el.Tag}{attrStr}>{innerHtml}</{el.Tag}>";
    }

    private static string BuildAttributes(Dictionary<string, string>? attrs)
    {
        if (attrs is null or { Count: 0 }) return string.Empty;
        var sb = new StringBuilder();
        foreach (var (k, v) in attrs)
            sb.Append($" {k}=\"{HttpUtility.HtmlAttributeEncode(v)}\"");
        return sb.ToString();
    }

    private static readonly HashSet<string> VoidElements =
    [
        "area", "base", "br", "col", "embed", "hr", "img",
        "input", "link", "meta", "param", "source", "track", "wbr"
    ];
}
