using System.Text.Json.Serialization;

namespace DeliveryAPIClient.Models;

/// <summary>
/// Represents a standalone Image Cropper property (an image uploaded directly to the
/// property, not via the Media Library). For Media Picker properties use
/// ApiMediaWithCropsResponseModel / GetImageProperty instead.
/// </summary>
public class ImageCropperModel
{
    /// <summary>Relative URL to the image file.</summary>
    [JsonPropertyName("src")]
    public string Src { get; set; } = string.Empty;

    /// <summary>Focal point as fractions (0–1) from the left/top edges.</summary>
    [JsonPropertyName("focalPoint")]
    public ImageFocalPointModel? FocalPoint { get; set; }

    /// <summary>Named crops defined on the data type.</summary>
    [JsonPropertyName("crops")]
    public List<ImageCropModel>? Crops { get; set; }

    /// <summary>Returns the URL for a named crop, falling back to Src if not found.</summary>
    public string GetCropUrl(string alias) =>
        Crops?.FirstOrDefault(c => string.Equals(c.Alias, alias, StringComparison.OrdinalIgnoreCase)) is not null
            ? $"{Src}?crop={alias}"
            : Src;
}
