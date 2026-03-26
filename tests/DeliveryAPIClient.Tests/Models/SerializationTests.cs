using System.Text.Json;
using DeliveryAPIClient.Models;
using Xunit;

namespace DeliveryAPIClient.Tests.Models;

public class SerializationTests
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    [Fact]
    public void ApiContentResponseModel_RoundTrip_PreservesAllFields()
    {
        var id = Guid.NewGuid();
        var json = $$"""
            {
              "id": "{{id}}",
              "contentType": "blogPost",
              "name": "Hello World",
              "createDate": "2024-01-15T10:00:00",
              "updateDate": "2024-02-20T14:30:00",
              "route": { "path": "/hello-world", "startItem": { "id": "{{Guid.NewGuid()}}", "path": "root" } },
              "cultures": {},
              "properties": {
                "title": "My Title",
                "count": 42
              }
            }
            """;

        var model = JsonSerializer.Deserialize<ApiContentResponseModel>(json, Options);

        Assert.NotNull(model);
        Assert.Equal(id, model.Id);
        Assert.Equal("blogPost", model.ContentType);
        Assert.Equal("Hello World", model.Name);
        Assert.Equal(new DateTime(2024, 1, 15, 10, 0, 0), model.CreateDate);
        Assert.Equal(new DateTime(2024, 2, 20, 14, 30, 0), model.UpdateDate);
        Assert.Equal("/hello-world", model.Route.Path);
        Assert.True(model.Properties.ContainsKey("title"));
        Assert.True(model.Properties.ContainsKey("count"));
    }

    [Fact]
    public void ApiContentResponseModel_PropertiesDictionary_RoundTrips()
    {
        var json = """
            {
              "id": "00000000-0000-0000-0000-000000000001",
              "contentType": "test",
              "name": "Test",
              "createDate": "2024-01-01T00:00:00",
              "updateDate": "2024-01-01T00:00:00",
              "route": { "path": "/test" },
              "cultures": {},
              "properties": {
                "stringProp": "hello",
                "numberProp": 123,
                "boolProp": true,
                "nullProp": null
              }
            }
            """;

        var model = JsonSerializer.Deserialize<ApiContentResponseModel>(json, Options);

        Assert.NotNull(model);
        Assert.Equal(4, model.Properties.Count);
        Assert.Equal("hello", model.Properties["stringProp"]!.Value.GetString());
        Assert.Equal(123, model.Properties["numberProp"]!.Value.GetInt32());
        Assert.True(model.Properties["boolProp"]!.Value.GetBoolean());
        Assert.Null(model.Properties["nullProp"]);
    }

    [Fact]
    public void ApiMediaWithCropsResponseModel_RoundTrip_PreservesAllFields()
    {
        var id = Guid.NewGuid();
        var json = $$"""
            {
              "id": "{{id}}",
              "name": "hero-image.jpg",
              "mediaType": "Image",
              "url": "/media/hero-image.jpg",
              "extension": "jpg",
              "width": 1920,
              "height": 1080,
              "bytes": 204800,
              "path": "/media/hero-image.jpg",
              "createDate": "2024-01-01T00:00:00",
              "updateDate": "2024-01-01T00:00:00",
              "focalPoint": { "left": 0.5, "top": 0.3 },
              "crops": [
                { "alias": "thumbnail", "width": 150, "height": 150 }
              ],
              "properties": {}
            }
            """;

        var model = JsonSerializer.Deserialize<ApiMediaWithCropsResponseModel>(json, Options);

        Assert.NotNull(model);
        Assert.Equal(id, model.Id);
        Assert.Equal("hero-image.jpg", model.Name);
        Assert.Equal("Image", model.MediaType);
        Assert.Equal("/media/hero-image.jpg", model.Url);
        Assert.Equal("jpg", model.Extension);
        Assert.Equal(1920, model.Width);
        Assert.Equal(1080, model.Height);
        Assert.Equal(204800, model.Bytes);
        Assert.NotNull(model.FocalPoint);
        Assert.Equal(0.5, model.FocalPoint.Left);
        Assert.Equal(0.3, model.FocalPoint.Top);
        Assert.NotNull(model.Crops);
        Assert.Single(model.Crops);
        Assert.Equal("thumbnail", model.Crops[0].Alias);
    }

    [Fact]
    public void PagedResponse_ContentResponseModel_RoundTrip_PreservesTotalAndItems()
    {
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var json = $$"""
            {
              "total": 42,
              "items": [
                {
                  "id": "{{id1}}",
                  "contentType": "page",
                  "name": "Page One",
                  "createDate": "2024-01-01T00:00:00",
                  "updateDate": "2024-01-01T00:00:00",
                  "route": { "path": "/page-one" },
                  "cultures": {},
                  "properties": {}
                },
                {
                  "id": "{{id2}}",
                  "contentType": "page",
                  "name": "Page Two",
                  "createDate": "2024-01-01T00:00:00",
                  "updateDate": "2024-01-01T00:00:00",
                  "route": { "path": "/page-two" },
                  "cultures": {},
                  "properties": {}
                }
              ]
            }
            """;

        var paged = JsonSerializer.Deserialize<PagedResponse<ApiContentResponseModel>>(json, Options);

        Assert.NotNull(paged);
        Assert.Equal(42L, paged.Total);
        Assert.Equal(2, paged.Items.Count);
        Assert.Equal(id1, paged.Items[0].Id);
        Assert.Equal(id2, paged.Items[1].Id);
        Assert.Equal("Page One", paged.Items[0].Name);
    }

    [Fact]
    public void ApiContentResponseModel_NullOptionalFields_DeserializesWithoutError()
    {
        var json = """
            {
              "id": "00000000-0000-0000-0000-000000000001",
              "contentType": "minimal",
              "name": "Minimal",
              "createDate": "2024-01-01T00:00:00",
              "updateDate": "2024-01-01T00:00:00",
              "route": { "path": "/minimal" },
              "cultures": {},
              "properties": {}
            }
            """;

        var model = JsonSerializer.Deserialize<ApiContentResponseModel>(json, Options);

        Assert.NotNull(model);
        Assert.Empty(model.Properties);
        Assert.Empty(model.Cultures);
    }

    [Fact]
    public void ApiMediaWithCropsResponseModel_NullOptionalFields_DeserializesWithoutError()
    {
        var json = """
            {
              "id": "00000000-0000-0000-0000-000000000001",
              "name": "file.pdf",
              "mediaType": "File",
              "url": "/media/file.pdf",
              "path": "/media/file.pdf",
              "createDate": "2024-01-01T00:00:00",
              "updateDate": "2024-01-01T00:00:00",
              "properties": {}
            }
            """;

        var model = JsonSerializer.Deserialize<ApiMediaWithCropsResponseModel>(json, Options);

        Assert.NotNull(model);
        Assert.Null(model.Extension);
        Assert.Null(model.Width);
        Assert.Null(model.Height);
        Assert.Null(model.Bytes);
        Assert.Null(model.FocalPoint);
        Assert.Null(model.Crops);
    }
}
