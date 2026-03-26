using DeliveryAPIClient.Models;

namespace DeliveryAPIClient.Examples.Models;

/// <summary>
/// Example typed content model for a "homePage" content type.
/// Demonstrates accessing a nested content picker property (expanded inline).
/// </summary>
public class HomePage : ContentItemBase
{
    public string? Heading        => GetProperty<string>("heading");
    public string? IntroText      => GetProperty<string>("introText");
    public ApiMediaWithCropsResponseModel? HeroImage => GetImageProperty("heroImage");

    /// <summary>
    /// A content picker property — only populated when fetched with expand=properties[$all].
    /// The returned value is the full ApiContentResponseModel for the linked item,
    /// which can itself be mapped to a typed model using .As&lt;T&gt;().
    /// </summary>
    public ApiContentResponseModel? FeaturedArticle => GetProperty<ApiContentResponseModel>("featuredArticle");
}
