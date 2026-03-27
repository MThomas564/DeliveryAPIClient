using DeliveryAPIClient.Models;

namespace DeliveryAPIClient.Examples.Models;

/// <summary>
/// Typed model for an "author" content type linked via a Content Picker property.
///
/// IMPORTANT — Member Picker vs Content Picker:
/// The Umbraco Delivery API intentionally does NOT expose Member Picker properties.
/// This is a deliberate security decision to prevent leaking member data.
/// A member picker property will always deserialize as null.
///
/// Recommended approach for "author" data:
///   1. Create an "Author" document type in Umbraco (name, bio, photo, etc.)
///   2. Create author content nodes under a /authors root
///   3. On your blog post, use a Content Picker (not Member Picker) pointing at /authors
///   4. Fetch with expand=properties[$all] to inline the author data
///
/// The author node then comes back as a full ApiContentResponseModel
/// and can be mapped to this typed class via .As&lt;AuthorContent&gt;() or GetProperty&lt;T&gt;.
/// </summary>
public class AuthorContent : ContentItemBase
{
    public string? DisplayName => GetProperty<string>("displayName");
    public string? Bio         => GetProperty<string>("bio");
    public string? JobTitle    => GetProperty<string>("jobTitle");

    /// <summary>Author profile photo.</summary>
    public ApiMediaWithCropsResponseModel? Photo => GetImageProperty("photo");
}
