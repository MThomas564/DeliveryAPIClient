using System.Text.Json.Serialization;

namespace DeliveryAPIClient.Models;

/// <summary>
/// Represents a Color Picker property value.
/// </summary>
public class ColorPickerModel
{
    /// <summary>Human-readable label configured on the data type.</summary>
    [JsonPropertyName("label")]
    public string? Label { get; set; }

    /// <summary>Hex color string, e.g. "#FF0000".</summary>
    [JsonPropertyName("color")]
    public string Color { get; set; } = string.Empty;

    public override string ToString() => Color;
}
