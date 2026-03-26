using System.Text.Json.Serialization;

namespace DeliveryAPIClient.Models;

public class ImageCropModel
{
    [JsonPropertyName("alias")]
    public string? Alias { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("coordinates")]
    public ImageCropCoordinatesModel? Coordinates { get; set; }
}
