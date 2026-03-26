using System.Text.Json.Serialization;

namespace DeliveryAPIClient.Models;

public class ApiContentStartItemModel
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;
}
