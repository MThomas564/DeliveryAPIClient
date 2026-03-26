using System.Text.Json.Serialization;

namespace DeliveryAPIClient.Models;

public class PagedResponse<T>
{
    [JsonPropertyName("total")]
    public long Total { get; set; }

    [JsonPropertyName("items")]
    public List<T> Items { get; set; } = [];
}
