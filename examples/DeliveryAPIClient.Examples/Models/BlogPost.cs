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
    public string? BodyText    => GetProperty<string>("bodyText");
    public DateTime? Published => GetProperty<DateTime>("publishDate");
    public string[]? Tags      => GetProperty<string[]>("tags");

    /// <summary>
    /// Hero image — populated when the content is fetched with expand=properties[$all].
    /// Includes Url, Width, Height, Extension, FocalPoint, and named Crops.
    /// </summary>
    public ApiMediaWithCropsResponseModel? HeroImage => GetImageProperty("heroImage");
}
