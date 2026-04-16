namespace DeliveryAPIClient.Client;

/// <summary>Configuration options for the Umbraco Delivery API client.</summary>
public class DeliveryApiOptions
{
    /// <summary>
    /// The base URL of your Umbraco site, e.g. <c>https://your-umbraco-site.com</c>.
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Optional API key. Required when <see cref="Preview"/> is <see langword="true"/>
    /// or when the Delivery API is configured to require authentication.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// When <see langword="true"/>, draft/unpublished content is included in responses.
    /// Requires a valid <see cref="ApiKey"/>.
    /// </summary>
    public bool Preview { get; set; }

    /// <summary>
    /// Default culture variant to request, e.g. <c>en-us</c>.
    /// Applied as the <c>Accept-Language</c> request header and can be overridden
    /// per-request via the <c>language</c> argument or <c>ContentQueryParameters.Language</c>.
    ///</summary>
    public string? DefaultLanguage { get; set; }
}
