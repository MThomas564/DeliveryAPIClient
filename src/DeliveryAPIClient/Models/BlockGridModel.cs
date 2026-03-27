using System.Text.Json.Serialization;

namespace DeliveryAPIClient.Models;

/// <summary>
/// The top-level wrapper for a Block Grid property.
/// </summary>
public class BlockGridModel
{
    /// <summary>Total number of columns in the grid (e.g. 12).</summary>
    [JsonPropertyName("gridColumns")]
    public int GridColumns { get; set; }

    [JsonPropertyName("items")]
    public List<BlockGridItem> Items { get; set; } = [];
}

/// <summary>
/// A single item placed in a Block Grid, including its position, areas, and content.
/// </summary>
public class BlockGridItem
{
    [JsonPropertyName("rowSpan")]
    public int RowSpan { get; set; } = 1;

    [JsonPropertyName("columnSpan")]
    public int ColumnSpan { get; set; } = 12;

    /// <summary>Number of sub-columns available to areas within this item.</summary>
    [JsonPropertyName("areaGridColumns")]
    public int AreaGridColumns { get; set; }

    /// <summary>Named areas that can contain nested block items.</summary>
    [JsonPropertyName("areas")]
    public List<BlockGridArea> Areas { get; set; } = [];

    [JsonPropertyName("content")]
    public ApiContentResponseModel Content { get; set; } = new();

    [JsonPropertyName("settings")]
    public ApiContentResponseModel? Settings { get; set; }
}

/// <summary>
/// A named area within a Block Grid item that can contain nested items.
/// </summary>
public class BlockGridArea
{
    [JsonPropertyName("alias")]
    public string Alias { get; set; } = string.Empty;

    [JsonPropertyName("rowSpan")]
    public int RowSpan { get; set; } = 1;

    [JsonPropertyName("columnSpan")]
    public int ColumnSpan { get; set; } = 12;

    /// <summary>Nested block items within this area.</summary>
    [JsonPropertyName("items")]
    public List<BlockGridItem> Items { get; set; } = [];
}
