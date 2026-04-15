namespace DeliveryAPIClient.Models;

/// <summary>Query parameters for media list and collection endpoints.</summary>
public class MediaQueryParameters
{
    /// <summary>
    /// Scope of items to fetch, e.g. <c>"children:/"</c>.
    /// </summary>
    public string? Fetch { get; set; }

    /// <summary>
    /// Filter expressions applied server-side, e.g. <c>"mediaType:Image"</c>.
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
    /// Properties to expand inline. Default: <c>"properties[$all]"</c>.
    /// </summary>
    public string? Expand { get; set; } = "properties[$all]";

    /// <summary>
    /// Limit which fields are returned, e.g. <c>"properties[title,url]"</c>.
    /// </summary>
    public string? Fields { get; set; }
}
