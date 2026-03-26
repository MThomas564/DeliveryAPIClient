namespace DeliveryAPIClient.Client;

public class DeliveryApiOptions
{
    public required string BaseUrl { get; init; }
    public string? ApiKey { get; init; }
    public bool Preview { get; init; }
    public string? DefaultLanguage { get; init; }
}
