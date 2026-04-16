using System.Text.Json;

namespace DeliveryAPIClient.Internal;

internal static class DeliveryApiJsonOptions
{
    internal static readonly JsonSerializerOptions Default = new()
    {
        PropertyNameCaseInsensitive = true
    };
}
