using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DeliveryAPIClient.Models;
using Microsoft.Extensions.Options;

namespace DeliveryAPIClient.Client;

public class DeliveryApiClient : IDeliveryApiClient
{
    private readonly HttpClient _httpClient;
    private readonly DeliveryApiOptions _options;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public DeliveryApiClient(HttpClient httpClient, IOptions<DeliveryApiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    // -------------------------------------------------------------------------
    // Content
    // -------------------------------------------------------------------------

    public async Task<PagedResponse<ApiContentResponseModel>> GetContentAsync(
        ContentQueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var qs = BuildContentQueryString(parameters);
        var request = new HttpRequestMessage(HttpMethod.Get, $"umbraco/delivery/api/v2/content{qs}");
        ApplyContentHeaders(request, parameters.Language, parameters.Segment, parameters.Preview, parameters.StartItem);

        var response = await SendAsync(request, cancellationToken);
        await EnsureSuccessAsync(response, allowNotFound: false);

        return await DeserializeAsync<PagedResponse<ApiContentResponseModel>>(response);
    }

    public async Task<ApiContentResponseModel?> GetContentByPathAsync(
        string path,
        string? expand = "properties[$all]",
        string? fields = null,
        string? language = null,
        string? segment = null,
        bool? preview = null,
        string? startItem = null,
        CancellationToken cancellationToken = default)
    {
        var qs = BuildQueryString(new Dictionary<string, string?>
        {
            ["expand"] = expand,
            ["fields"] = fields
        });

        var encodedPath = Uri.EscapeDataString(path.TrimStart('/'));
        var request = new HttpRequestMessage(HttpMethod.Get, $"umbraco/delivery/api/v2/content/item/{encodedPath}{qs}");
        ApplyContentHeaders(request, language, segment, preview, startItem);

        var response = await SendAsync(request, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        await EnsureSuccessAsync(response, allowNotFound: false);
        return await DeserializeAsync<ApiContentResponseModel>(response);
    }

    public async Task<ApiContentResponseModel?> GetContentByIdAsync(
        Guid id,
        string? expand = "properties[$all]",
        string? fields = null,
        string? language = null,
        string? segment = null,
        bool? preview = null,
        string? startItem = null,
        CancellationToken cancellationToken = default)
    {
        var qs = BuildQueryString(new Dictionary<string, string?>
        {
            ["expand"] = expand,
            ["fields"] = fields
        });

        var request = new HttpRequestMessage(HttpMethod.Get, $"umbraco/delivery/api/v2/content/item/{id}{qs}");
        ApplyContentHeaders(request, language, segment, preview, startItem);

        var response = await SendAsync(request, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        await EnsureSuccessAsync(response, allowNotFound: false);
        return await DeserializeAsync<ApiContentResponseModel>(response);
    }

    public async Task<IReadOnlyList<ApiContentResponseModel>> GetContentItemsAsync(
        IEnumerable<Guid> ids,
        string? expand = "properties[$all]",
        string? fields = null,
        string? language = null,
        string? segment = null,
        bool? preview = null,
        string? startItem = null,
        CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();
        var qs = BuildQueryStringWithArrays(
            new Dictionary<string, string?> { ["expand"] = expand, ["fields"] = fields },
            new Dictionary<string, IEnumerable<string>> { ["id"] = idList.Select(g => g.ToString()) });

        var request = new HttpRequestMessage(HttpMethod.Get, $"umbraco/delivery/api/v2/content/items{qs}");
        ApplyContentHeaders(request, language, segment, preview, startItem);

        var response = await SendAsync(request, cancellationToken);
        await EnsureSuccessAsync(response, allowNotFound: false);

        var result = await DeserializeAsync<List<ApiContentResponseModel>>(response);
        return result.AsReadOnly();
    }

    // -------------------------------------------------------------------------
    // Media
    // -------------------------------------------------------------------------

    public async Task<PagedResponse<ApiMediaWithCropsResponseModel>> GetMediaAsync(
        MediaQueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var qs = BuildMediaQueryString(parameters);
        var request = new HttpRequestMessage(HttpMethod.Get, $"umbraco/delivery/api/v2/media{qs}");
        ApplyDefaultHeaders(request);

        var response = await SendAsync(request, cancellationToken);
        await EnsureSuccessAsync(response, allowNotFound: false);

        return await DeserializeAsync<PagedResponse<ApiMediaWithCropsResponseModel>>(response);
    }

    public async Task<ApiMediaWithCropsResponseModel?> GetMediaByPathAsync(
        string path,
        string? expand = null,
        string? fields = null,
        CancellationToken cancellationToken = default)
    {
        var qs = BuildQueryString(new Dictionary<string, string?>
        {
            ["expand"] = expand,
            ["fields"] = fields
        });

        var encodedPath = Uri.EscapeDataString(path.TrimStart('/'));
        var request = new HttpRequestMessage(HttpMethod.Get, $"umbraco/delivery/api/v2/media/item/{encodedPath}{qs}");
        ApplyDefaultHeaders(request);

        var response = await SendAsync(request, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        await EnsureSuccessAsync(response, allowNotFound: false);
        return await DeserializeAsync<ApiMediaWithCropsResponseModel>(response);
    }

    public async Task<ApiMediaWithCropsResponseModel?> GetMediaByIdAsync(
        Guid id,
        string? expand = null,
        string? fields = null,
        CancellationToken cancellationToken = default)
    {
        var qs = BuildQueryString(new Dictionary<string, string?>
        {
            ["expand"] = expand,
            ["fields"] = fields
        });

        var request = new HttpRequestMessage(HttpMethod.Get, $"umbraco/delivery/api/v2/media/item/{id}{qs}");
        ApplyDefaultHeaders(request);

        var response = await SendAsync(request, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        await EnsureSuccessAsync(response, allowNotFound: false);
        return await DeserializeAsync<ApiMediaWithCropsResponseModel>(response);
    }

    public async Task<IReadOnlyList<ApiMediaWithCropsResponseModel>> GetMediaItemsAsync(
        IEnumerable<Guid> ids,
        string? expand = null,
        string? fields = null,
        CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();
        var qs = BuildQueryStringWithArrays(
            new Dictionary<string, string?> { ["expand"] = expand, ["fields"] = fields },
            new Dictionary<string, IEnumerable<string>> { ["id"] = idList.Select(g => g.ToString()) });

        var request = new HttpRequestMessage(HttpMethod.Get, $"umbraco/delivery/api/v2/media/items{qs}");
        ApplyDefaultHeaders(request);

        var response = await SendAsync(request, cancellationToken);
        await EnsureSuccessAsync(response, allowNotFound: false);

        var result = await DeserializeAsync<List<ApiMediaWithCropsResponseModel>>(response);
        return result.AsReadOnly();
    }

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    private Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_options.ApiKey))
            request.Headers.TryAddWithoutValidation("Api-Key", _options.ApiKey);

        return _httpClient.SendAsync(request, cancellationToken);
    }

    private void ApplyDefaultHeaders(HttpRequestMessage request)
    {
        if (!string.IsNullOrWhiteSpace(_options.DefaultLanguage))
            request.Headers.AcceptLanguage.TryParseAdd(_options.DefaultLanguage);

        if (_options.Preview)
            request.Headers.TryAddWithoutValidation("Preview", "true");
    }

    private void ApplyContentHeaders(
        HttpRequestMessage request,
        string? language,
        string? segment,
        bool? preview,
        string? startItem)
    {
        var effectiveLanguage = language ?? _options.DefaultLanguage;
        if (!string.IsNullOrWhiteSpace(effectiveLanguage))
            request.Headers.AcceptLanguage.TryParseAdd(effectiveLanguage);

        var effectivePreview = preview ?? (_options.Preview ? true : (bool?)null);
        if (effectivePreview == true)
            request.Headers.TryAddWithoutValidation("Preview", "true");

        if (!string.IsNullOrWhiteSpace(segment))
            request.Headers.TryAddWithoutValidation("Segment", segment);

        if (!string.IsNullOrWhiteSpace(startItem))
            request.Headers.TryAddWithoutValidation("Start-Item", startItem);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, bool allowNotFound)
    {
        if (response.IsSuccessStatusCode)
            return;

        var statusCode = (int)response.StatusCode;

        if (allowNotFound && response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return;

        if (statusCode is 400 or 401 or 403)
        {
            ProblemDetails? problem = null;
            try
            {
                var content = await response.Content.ReadAsStringAsync();
                problem = JsonSerializer.Deserialize<ProblemDetails>(content, JsonOptions);
            }
            catch (JsonException) { }

            if (problem is not null)
                throw new DeliveryApiException(statusCode, problem);

            throw new DeliveryApiException(statusCode, $"Request failed with status code {statusCode}.");
        }

        throw new DeliveryApiException(statusCode, $"Request failed with status code {statusCode}.");
    }

    private static async Task<T> DeserializeAsync<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<T>(content, JsonOptions);
        return result ?? throw new InvalidOperationException("Response deserialized to null.");
    }

    private static string BuildQueryString(Dictionary<string, string?> parameters)
    {
        var pairs = parameters
            .Where(kvp => kvp.Value is not null)
            .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value!)}");

        var qs = string.Join("&", pairs);
        return qs.Length > 0 ? "?" + qs : string.Empty;
    }

    private static string BuildQueryStringWithArrays(
        Dictionary<string, string?> scalars,
        Dictionary<string, IEnumerable<string>> arrays)
    {
        var parts = new List<string>();

        foreach (var kvp in scalars.Where(k => k.Value is not null))
            parts.Add($"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value!)}");

        foreach (var kvp in arrays)
            foreach (var value in kvp.Value)
                parts.Add($"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(value)}");

        return parts.Count > 0 ? "?" + string.Join("&", parts) : string.Empty;
    }

    private static string BuildContentQueryString(ContentQueryParameters p)
    {
        var scalars = new Dictionary<string, string?>
        {
            ["fetch"] = p.Fetch,
            ["skip"] = p.Skip.ToString(),
            ["take"] = p.Take.ToString(),
            ["expand"] = p.Expand,
            ["fields"] = p.Fields
        };

        var arrays = new Dictionary<string, IEnumerable<string>>();
        if (p.Filter is { Count: > 0 })
            arrays["filter"] = p.Filter;
        if (p.Sort is { Count: > 0 })
            arrays["sort"] = p.Sort;

        return BuildQueryStringWithArrays(scalars, arrays);
    }

    private static string BuildMediaQueryString(MediaQueryParameters p)
    {
        var scalars = new Dictionary<string, string?>
        {
            ["fetch"] = p.Fetch,
            ["skip"] = p.Skip.ToString(),
            ["take"] = p.Take.ToString(),
            ["expand"] = p.Expand,
            ["fields"] = p.Fields
        };

        var arrays = new Dictionary<string, IEnumerable<string>>();
        if (p.Filter is { Count: > 0 })
            arrays["filter"] = p.Filter;
        if (p.Sort is { Count: > 0 })
            arrays["sort"] = p.Sort;

        return BuildQueryStringWithArrays(scalars, arrays);
    }
}
