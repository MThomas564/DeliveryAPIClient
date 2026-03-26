using DeliveryAPIClient.Models;

namespace DeliveryAPIClient.Client;

public class DeliveryApiException : Exception
{
    public int StatusCode { get; }
    public ProblemDetails? ProblemDetails { get; }

    public DeliveryApiException(int statusCode, string message)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public DeliveryApiException(int statusCode, ProblemDetails details)
        : base(details.Title ?? details.Detail ?? $"API error with status code {statusCode}")
    {
        StatusCode = statusCode;
        ProblemDetails = details;
    }
}
