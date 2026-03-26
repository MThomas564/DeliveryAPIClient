namespace DeliveryAPIClient.Client;

public class DeliveryApiOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string? ApiKey { get; set; }
    public bool Preview { get; set; }
    public string? DefaultLanguage { get; set; }
}
