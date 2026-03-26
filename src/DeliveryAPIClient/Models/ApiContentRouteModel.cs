using System.Text.Json.Serialization;

namespace DeliveryAPIClient.Models;

public class ApiContentRouteModel
{
    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    [JsonPropertyName("queryString")]
    public string? QueryString { get; set; }

    [JsonPropertyName("startItem")]
    public ApiContentStartItemModel StartItem { get; set; } = new();
}
