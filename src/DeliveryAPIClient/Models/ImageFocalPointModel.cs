using System.Text.Json.Serialization;

namespace DeliveryAPIClient.Models;

public class ImageFocalPointModel
{
    [JsonPropertyName("left")]
    public double Left { get; set; }

    [JsonPropertyName("top")]
    public double Top { get; set; }
}
