using System.Text.Json;
using System.Text.Json.Serialization;

namespace DeliveryAPIClient.Models;

public class ContentItemBase
{
    private static readonly JsonSerializerOptions _deserializeOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

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
            return element.Value.Deserialize<T>(_deserializeOptions);
        }
        catch (JsonException)
        {
            return default;
        }
    }

    protected ApiMediaWithCropsResponseModel? GetImageProperty(string alias)
        => GetProperty<ApiMediaWithCropsResponseModel>(alias);
}
