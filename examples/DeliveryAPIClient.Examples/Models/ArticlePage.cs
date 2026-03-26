using DeliveryAPIClient.Models;

namespace DeliveryAPIClient.Examples.Models;

/// <summary>
/// Example typed content model for an "article" content type with multiple media properties.
/// </summary>
public class ArticlePage : ContentItemBase
{
    public string? Headline    => GetProperty<string>("headline");
    public string? BodyText    => GetProperty<string>("bodyText");
    public string? Author      => GetProperty<string>("author");

    /// <summary>Full-width banner image with crops.</summary>
    public ApiMediaWithCropsResponseModel? Banner => GetImageProperty("banner");

    /// <summary>Thumbnail used in listing pages.</summary>
    public ApiMediaWithCropsResponseModel? Thumbnail => GetImageProperty("thumbnail");
}
