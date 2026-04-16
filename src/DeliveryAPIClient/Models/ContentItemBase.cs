using System.Text.Json;
using System.Text.Json.Serialization;
using DeliveryAPIClient.Internal;

namespace DeliveryAPIClient.Models;

public class ContentItemBase
{

    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("contentType")]
    public string ContentType { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("createDate")]
    public DateTime CreateDate { get; set; }

    [JsonPropertyName("updateDate")]
    public DateTime UpdateDate { get; set; }

    [JsonPropertyName("route")]
    public ApiContentRouteModel Route { get; set; } = new();

    [JsonPropertyName("cultures")]
    public Dictionary<string, JsonElement?> Cultures { get; set; } = new();

    [JsonPropertyName("properties")]
    public Dictionary<string, JsonElement?> Properties { get; set; } = new();

    protected T? GetProperty<T>(string alias)
    {
        if (!Properties.TryGetValue(alias, out var element) || element is null)
            return default;

        try
        {
            return element.Value.Deserialize<T>(DeliveryApiJsonOptions.Default);
        }
        catch (JsonException)
        {
            return default;
        }
    }

    // ── Media ────────────────────────────────────────────────────────────────

    /// <summary>Single Media Picker property (image, file, etc.).</summary>
    protected ApiMediaWithCropsResponseModel? GetImageProperty(string alias)
        => GetProperty<ApiMediaWithCropsResponseModel>(alias);

    /// <summary>Multiple Media Picker property.</summary>
    protected List<ApiMediaWithCropsResponseModel>? GetMultipleImageProperty(string alias)
        => GetProperty<List<ApiMediaWithCropsResponseModel>>(alias);

    /// <summary>
    /// Standalone Image Cropper property (image uploaded directly to the property,
    /// not via the Media Library). For Media Picker use GetImageProperty instead.
    /// </summary>
    protected ImageCropperModel? GetImageCropperProperty(string alias)
        => GetProperty<ImageCropperModel>(alias);

    // ── Content ──────────────────────────────────────────────────────────────

    /// <summary>Single Content Picker or related content property.</summary>
    protected ApiContentResponseModel? GetContentProperty(string alias)
        => GetProperty<ApiContentResponseModel>(alias);

    /// <summary>Multi-node Tree Picker or multiple Content Picker property.</summary>
    protected List<ApiContentResponseModel>? GetMultipleContentProperty(string alias)
        => GetProperty<List<ApiContentResponseModel>>(alias);

    // ── Rich Text ────────────────────────────────────────────────────────────

    /// <summary>
    /// Rich text (TinyMCE / block-based) property.
    /// Call .ToHtml() to render. Use .HasBlocks / .GetBlock() for embedded blocks.
    /// </summary>
    protected RichTextModel? GetRichTextProperty(string alias)
        => GetProperty<RichTextModel>(alias);

    // ── Blocks ───────────────────────────────────────────────────────────────

    /// <summary>Block List property — returns the ordered list of block items.</summary>
    protected List<BlockItemModel>? GetBlockListProperty(string alias)
        => GetProperty<List<BlockItemModel>>(alias);

    /// <summary>Block Grid property — returns the grid wrapper including columns and items.</summary>
    protected BlockGridModel? GetBlockGridProperty(string alias)
        => GetProperty<BlockGridModel>(alias);

    // ── Links ────────────────────────────────────────────────────────────────

    /// <summary>Single link from a Multi-URL Picker or Link Picker property.</summary>
    protected LinkModel? GetLinkProperty(string alias)
        => GetProperty<LinkModel>(alias);

    /// <summary>Multiple links from a Multi-URL Picker property.</summary>
    protected List<LinkModel>? GetLinksProperty(string alias)
        => GetProperty<List<LinkModel>>(alias);

    // ── Other ─────────────────────────────────────────────────────────────

    /// <summary>Color Picker property.</summary>
    protected ColorPickerModel? GetColorPickerProperty(string alias)
        => GetProperty<ColorPickerModel>(alias);
}
