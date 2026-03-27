using System.Text.Json.Serialization;

namespace DeliveryAPIClient.Models;

/// <summary>
/// A single item in a Block List property.
/// Content holds the block's data; Settings holds optional layout/display configuration.
/// Both are full content items when fetched with expand=properties[$all].
/// </summary>
public class BlockItemModel
{
    [JsonPropertyName("content")]
    public ApiContentResponseModel Content { get; set; } = new();

    [JsonPropertyName("settings")]
    public ApiContentResponseModel? Settings { get; set; }
}
