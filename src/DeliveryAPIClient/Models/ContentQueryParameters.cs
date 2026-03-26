namespace DeliveryAPIClient.Models;

public class ContentQueryParameters
{
    public string? Fetch { get; set; }
    public List<string>? Filter { get; set; }
    public List<string>? Sort { get; set; }
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 10;
    public string? Expand { get; set; } = "properties[$all]";
    public string? Fields { get; set; }

    // Headers
    public string? Language { get; set; }
    public string? Segment { get; set; }
    public bool? Preview { get; set; }
    public string? StartItem { get; set; }
}
