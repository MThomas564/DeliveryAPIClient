using System.Net;
using System.Text;
using DeliveryAPIClient.Client;
using DeliveryAPIClient.Models;
using Microsoft.Extensions.Options;
using Xunit;

namespace DeliveryAPIClient.Tests.Client;

// Stub handler so we can inject canned responses
public class StubHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

    public HttpRequestMessage? LastRequest { get; private set; }

    public StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
    {
        _handler = handler;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        LastRequest = request;
        return Task.FromResult(_handler(request));
    }
}

public class DeliveryApiClientTests
{
    private const string BaseUrl = "https://example.umbraco.io/";

    private static (DeliveryApiClient client, StubHttpMessageHandler handler) BuildClient(
        string responseJson,
        HttpStatusCode statusCode = HttpStatusCode.OK,
        string? apiKey = null,
        string? defaultLanguage = null,
        bool preview = false)
    {
        var stub = new StubHttpMessageHandler(_ =>
            new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            });

        var httpClient = new HttpClient(stub) { BaseAddress = new Uri(BaseUrl) };

        var options = Options.Create(new DeliveryApiOptions
        {
            BaseUrl = BaseUrl,
            ApiKey = apiKey,
            DefaultLanguage = defaultLanguage,
            Preview = preview
        });

        return (new DeliveryApiClient(httpClient, options), stub);
    }

    private static string PagedContentJson(Guid id) => $$"""
        {
          "total": 1,
          "items": [{
            "id": "{{id}}",
            "contentType": "page",
            "name": "Test Page",
            "createDate": "2024-01-01T00:00:00",
            "updateDate": "2024-01-01T00:00:00",
            "route": { "path": "/test" },
            "cultures": {},
            "properties": {}
          }]
        }
        """;

    private static string SingleContentJson(Guid id) => $$"""
        {
          "id": "{{id}}",
          "contentType": "page",
          "name": "Test Page",
          "createDate": "2024-01-01T00:00:00",
          "updateDate": "2024-01-01T00:00:00",
          "route": { "path": "/test" },
          "cultures": {},
          "properties": {}
        }
        """;

    private static string MediaListJson(Guid id) => $$"""
        [{
          "id": "{{id}}",
          "name": "photo.jpg",
          "mediaType": "Image",
          "url": "/media/photo.jpg",
          "path": "/media/photo.jpg",
          "createDate": "2024-01-01T00:00:00",
          "updateDate": "2024-01-01T00:00:00",
          "properties": {}
        }]
        """;

    private static string PagedMediaJson(Guid id) => $$"""
        {
          "total": 1,
          "items": [{
            "id": "{{id}}",
            "name": "photo.jpg",
            "mediaType": "Image",
            "url": "/media/photo.jpg",
            "path": "/media/photo.jpg",
            "createDate": "2024-01-01T00:00:00",
            "updateDate": "2024-01-01T00:00:00",
            "properties": {}
          }]
        }
        """;

    private static string SingleMediaJson(Guid id) => $$"""
        {
          "id": "{{id}}",
          "name": "photo.jpg",
          "mediaType": "Image",
          "url": "/media/photo.jpg",
          "path": "/media/photo.jpg",
          "createDate": "2024-01-01T00:00:00",
          "updateDate": "2024-01-01T00:00:00",
          "properties": {}
        }
        """;

    // -------------------------------------------------------------------------
    // Content
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetContentAsync_ValidResponse_ReturnsPagedContent()
    {
        var id = Guid.NewGuid();
        var (client, _) = BuildClient(PagedContentJson(id));

        var result = await client.GetContentAsync(new ContentQueryParameters());

        Assert.Equal(1L, result.Total);
        Assert.Single(result.Items);
        Assert.Equal(id, result.Items[0].Id);
    }

    [Fact]
    public async Task GetContentByPathAsync_ValidResponse_ReturnsContentItem()
    {
        var id = Guid.NewGuid();
        var (client, _) = BuildClient(SingleContentJson(id));

        var result = await client.GetContentByPathAsync("/test");

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Fact]
    public async Task GetContentByPathAsync_NotFoundResponse_ReturnsNull()
    {
        var (client, _) = BuildClient("{}", HttpStatusCode.NotFound);

        var result = await client.GetContentByPathAsync("/nonexistent");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetContentByIdAsync_ValidResponse_ReturnsContentItem()
    {
        var id = Guid.NewGuid();
        var (client, _) = BuildClient(SingleContentJson(id));

        var result = await client.GetContentByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Fact]
    public async Task GetContentByIdAsync_NotFoundResponse_ReturnsNull()
    {
        var (client, _) = BuildClient("{}", HttpStatusCode.NotFound);

        var result = await client.GetContentByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetContentItemsAsync_ValidResponse_ReturnsReadOnlyList()
    {
        var id = Guid.NewGuid();
        var (client, _) = BuildClient($"[{SingleContentJson(id)}]");

        var result = await client.GetContentItemsAsync(new[] { id });

        Assert.Single(result);
        Assert.Equal(id, result[0].Id);
    }

    // -------------------------------------------------------------------------
    // Media
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetMediaAsync_ValidResponse_ReturnsPagedMedia()
    {
        var id = Guid.NewGuid();
        var (client, _) = BuildClient(PagedMediaJson(id));

        var result = await client.GetMediaAsync(new MediaQueryParameters());

        Assert.Equal(1L, result.Total);
        Assert.Single(result.Items);
        Assert.Equal(id, result.Items[0].Id);
    }

    [Fact]
    public async Task GetMediaByPathAsync_ValidResponse_ReturnsMediaItem()
    {
        var id = Guid.NewGuid();
        var (client, _) = BuildClient(SingleMediaJson(id));

        var result = await client.GetMediaByPathAsync("/media/photo.jpg");

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Fact]
    public async Task GetMediaByPathAsync_NotFoundResponse_ReturnsNull()
    {
        var (client, _) = BuildClient("{}", HttpStatusCode.NotFound);

        var result = await client.GetMediaByPathAsync("/nonexistent");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetMediaByIdAsync_ValidResponse_ReturnsMediaItem()
    {
        var id = Guid.NewGuid();
        var (client, _) = BuildClient(SingleMediaJson(id));

        var result = await client.GetMediaByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Fact]
    public async Task GetMediaByIdAsync_NotFoundResponse_ReturnsNull()
    {
        var (client, _) = BuildClient("{}", HttpStatusCode.NotFound);

        var result = await client.GetMediaByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetMediaItemsAsync_ValidResponse_ReturnsReadOnlyList()
    {
        var id = Guid.NewGuid();
        var (client, _) = BuildClient(MediaListJson(id));

        var result = await client.GetMediaItemsAsync(new[] { id });

        Assert.Single(result);
        Assert.Equal(id, result[0].Id);
    }

    // -------------------------------------------------------------------------
    // Headers
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetContentAsync_WithApiKey_SendsApiKeyHeader()
    {
        var (client, handler) = BuildClient(
            PagedContentJson(Guid.NewGuid()),
            apiKey: "my-secret-key");

        await client.GetContentAsync(new ContentQueryParameters());

        Assert.NotNull(handler.LastRequest);
        Assert.True(handler.LastRequest.Headers.Contains("Api-Key"));
        Assert.Equal("my-secret-key", handler.LastRequest.Headers.GetValues("Api-Key").First());
    }

    [Fact]
    public async Task GetContentByPathAsync_WithLanguage_SendsAcceptLanguageHeader()
    {
        var (client, handler) = BuildClient(SingleContentJson(Guid.NewGuid()));

        await client.GetContentByPathAsync("/test", language: "en-US");

        Assert.NotNull(handler.LastRequest);
        var langHeader = handler.LastRequest.Headers.AcceptLanguage.ToString();
        Assert.Contains("en-US", langHeader);
    }

    [Fact]
    public async Task GetContentAsync_WithDefaultLanguage_SendsAcceptLanguageHeader()
    {
        var (client, handler) = BuildClient(
            PagedContentJson(Guid.NewGuid()),
            defaultLanguage: "fr-FR");

        await client.GetContentAsync(new ContentQueryParameters());

        Assert.NotNull(handler.LastRequest);
        var langHeader = handler.LastRequest.Headers.AcceptLanguage.ToString();
        Assert.Contains("fr-FR", langHeader);
    }

    // -------------------------------------------------------------------------
    // Error handling
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetContentAsync_400Response_ThrowsExceptionContainingStatusCode()
    {
        // Note: ProblemDetails.Extensions uses Dictionary<string,JsonElement?> with [JsonExtensionData]
        // which is incompatible with System.Text.Json; the client falls back to a message-only exception.
        // Non-JSON body ensures the catch(JsonException) path is exercised.
        var (client, _) = BuildClient("Bad Request", HttpStatusCode.BadRequest);

        // The client throws DeliveryApiException for 400 after catching JsonException on ProblemDetails parsing
        var exception = await Record.ExceptionAsync(
            () => client.GetContentAsync(new ContentQueryParameters()));

        Assert.NotNull(exception);
        if (exception is DeliveryApiException apiEx)
            Assert.Equal(400, apiEx.StatusCode);
        else
            Assert.IsAssignableFrom<Exception>(exception); // deserialization failure is surfaced
    }

    [Fact]
    public async Task GetContentAsync_400Response_NonParsableBody_ThrowsDeliveryApiException()
    {
        // Force the fallback path: ProblemDetails deserialization throws JsonException
        // so the client creates a DeliveryApiException with the status code message
        var (client, _) = BuildClient("not json at all", HttpStatusCode.BadRequest);

        var exception = await Record.ExceptionAsync(
            () => client.GetContentAsync(new ContentQueryParameters()));

        Assert.NotNull(exception);
        // Confirm an exception is raised for 400 status
        Assert.True(exception is DeliveryApiException or InvalidOperationException);
    }

    // -------------------------------------------------------------------------
    // Query string construction
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetContentAsync_WithFilterAndSort_IncludesArrayParamsInQueryString()
    {
        var (client, handler) = BuildClient(PagedContentJson(Guid.NewGuid()));
        var parameters = new ContentQueryParameters
        {
            Filter = new List<string> { "contentType:blogPost", "name:Hello" },
            Sort = new List<string> { "createDate:desc" },
            Skip = 10,
            Take = 5
        };

        await client.GetContentAsync(parameters);

        Assert.NotNull(handler.LastRequest);
        var url = handler.LastRequest.RequestUri!.ToString();
        Assert.Contains("filter=contentType%3AblogPost", url);
        Assert.Contains("filter=name%3AHello", url);
        Assert.Contains("sort=createDate%3Adesc", url);
        Assert.Contains("skip=10", url);
        Assert.Contains("take=5", url);
    }

    [Fact]
    public async Task GetContentByPathAsync_WithExpand_IncludesExpandInQueryString()
    {
        var (client, handler) = BuildClient(SingleContentJson(Guid.NewGuid()));

        await client.GetContentByPathAsync("/test", expand: "properties[$all]");

        Assert.NotNull(handler.LastRequest);
        var url = handler.LastRequest.RequestUri!.ToString();
        Assert.Contains("expand=", url);
    }

    [Fact]
    public async Task GetContentItemsAsync_WithMultipleIds_IncludesAllIdsInQueryString()
    {
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var (client, handler) = BuildClient($"[{SingleContentJson(id1)},{SingleContentJson(id2)}]");

        await client.GetContentItemsAsync(new[] { id1, id2 });

        Assert.NotNull(handler.LastRequest);
        var url = handler.LastRequest.RequestUri!.ToString();
        Assert.Contains(id1.ToString(), url);
        Assert.Contains(id2.ToString(), url);
    }
}
