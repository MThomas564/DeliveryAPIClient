namespace DeliveryAPIClient.Models;

public class MediaQueryParameters
{
    public string? Fetch { get; set; }
    public List<string>? Filter { get; set; }
    public List<string>? Sort { get; set; }
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 10;
    public string? Expand { get; set; } = "properties[$all]";
    public string? Fields { get; set; }
}
