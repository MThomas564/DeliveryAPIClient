using DeliveryAPIClient.Extensions;
using DeliveryAPIClient.Models;

namespace DeliveryAPIClient.Examples.Models;

/// <summary>
/// Example typed content model for a "blogPost" content type.
/// Inherit from ContentItemBase and use GetProperty&lt;T&gt; / GetImageProperty
/// to expose strongly-typed access to Umbraco content properties.
/// </summary>
public class BlogPost : ContentItemBase
{
    public string? Title       => GetProperty<string>("title");
    public string? Summary     => GetProperty<string>("summary");
    public DateTime? Published => GetProperty<DateTime>("publishDate");
    public string[]? Tags      => GetProperty<string[]>("tags");

    /// <summary>
    /// Hero image — populated when the content is fetched with expand=properties[$all].
    /// Includes Url, Width, Height, Extension, FocalPoint, and named Crops.
    /// </summary>
    public ApiMediaWithCropsResponseModel? HeroImage => GetImageProperty("heroImage");

    /// <summary>
    /// Rich text body. Call .ToHtml() for a rendered HTML string.
    /// Use .HasBlocks / .GetBlock(id) to access embedded block content.
    /// </summary>
    public RichTextModel? BodyText => GetRichTextProperty("bodyText");

    /// <summary>
    /// Author linked via a Content Picker (not Member Picker).
    /// The Delivery API intentionally blocks Member Picker properties — use a
    /// dedicated Author content type and a Content Picker instead.
    /// Requires expand=properties[$all] to populate.
    /// </summary>
    public AuthorContent? Author
    {
        get
        {
            var raw = GetProperty<ApiContentResponseModel>("author");
            return raw?.As<AuthorContent>();
        }
    }
}
