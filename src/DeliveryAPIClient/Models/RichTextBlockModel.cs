using System.Text.Json.Serialization;

namespace DeliveryAPIClient.Models;

/// <summary>
/// An embedded block inside a rich text property.
/// The content-id in the element tree matches Content.Id.
/// </summary>
public class RichTextBlockModel
{
    /// <summary>
    /// The block's content item. Includes full properties when the rich text property
    /// was fetched with expand=properties[$all].
    /// </summary>
    [JsonPropertyName("content")]
    public ApiContentResponseModel Content { get; set; } = new();

    /// <summary>Optional settings content item for the block.</summary>
    [JsonPropertyName("settings")]
    public ApiContentResponseModel? Settings { get; set; }
}
