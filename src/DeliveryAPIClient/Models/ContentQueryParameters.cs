namespace DeliveryAPIClient.Models;

/// <summary>Query parameters for content list and collection endpoints.</summary>
public class ContentQueryParameters
{
    /// <summary>
    /// Scope of items to fetch, e.g. <c>"children:/blog"</c> or <c>"descendants:/news"</c>.
    /// </summary>
    public string? Fetch { get; set; }

    /// <summary>
    /// Filter expressions applied server-side, e.g. <c>"contentType:blogPost"</c>.
    /// Multiple values are combined with AND.
    /// </summary>
    public List<string>? Filter { get; set; }

    /// <summary>
    /// Sort expressions, e.g. <c>"createDate:desc"</c>. Multiple values define a sort chain.
    /// </summary>
    public List<string>? Sort { get; set; }

    /// <summary>Number of items to skip (pagination offset). Default: <c>0</c>.</summary>
    public int Skip { get; set; } = 0;

    /// <summary>Maximum number of items to return per page. Default: <c>10</c>.</summary>
    public int Take { get; set; } = 10;

    /// <summary>
    /// Properties to expand inline, e.g. <c>"properties[$all]"</c> or <c>"properties[heroImage]"</c>.
    /// Default: <c>"properties[$all]"</c>.
    /// </summary>
    public string? Expand { get; set; } = "properties[$all]";

    /// <summary>
    /// Limit which fields are returned, e.g. <c>"properties[title,heroImage]"</c>.
    /// </summary>
    public string? Fields { get; set; }

    /// <summary>Culture variant to request, e.g. <c>"en-us"</c>. Overrides <c>DefaultLanguage</c> from options.</summary>
    public string? Language { get; set; }

    /// <summary>Segment variant, e.g. <c>"segment-one"</c>.</summary>
    public string? Segment { get; set; }

    /// <summary>
    /// When <see langword="true"/>, includes draft content. Requires an API key.
    /// Overrides the global <c>Preview</c> setting from options.
    /// </summary>
    public bool? Preview { get; set; }

    /// <summary>
    /// Restricts results to a subtree rooted at this path or GUID,
    /// e.g. <c>"/blog"</c>. Sets the <c>Start-Item</c> request header.
    /// </summary>
    public string? StartItem { get; set; }
}
