using System.Text.Json;
using System.Text.Json.Serialization;

namespace DeliveryAPIClient.Models;

public class ApiMediaWithCropsResponseModel
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("mediaType")]
    public string MediaType { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("extension")]
    public string? Extension { get; set; }

    [JsonPropertyName("width")]
    public int? Width { get; set; }

    [JsonPropertyName("height")]
    public int? Height { get; set; }

    [JsonPropertyName("bytes")]
    public int? Bytes { get; set; }

    [JsonPropertyName("properties")]
    public Dictionary<string, JsonElement?> Properties { get; set; } = new();

    [JsonPropertyName("focalPoint")]
    public ImageFocalPointModel? FocalPoint { get; set; }

    [JsonPropertyName("crops")]
    public List<ImageCropModel>? Crops { get; set; }

    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    [JsonPropertyName("createDate")]
    public DateTime CreateDate { get; set; }

    [JsonPropertyName("updateDate")]
    public DateTime UpdateDate { get; set; }
}
