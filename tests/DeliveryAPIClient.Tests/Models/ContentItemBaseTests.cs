using System.Text.Json;
using DeliveryAPIClient.Extensions;
using DeliveryAPIClient.Models;
using Xunit;

namespace DeliveryAPIClient.Tests.Models;

// Concrete subclass used to test protected methods via public wrappers
public class TestContentItem : ContentItemBase
{
    public string? Title => GetProperty<string>("title");
    public int? Count => GetProperty<int?>("count");
    public ApiMediaWithCropsResponseModel? HeroImage => GetImageProperty("heroImage");
}

public class ContentItemBaseTests
{
    private static JsonElement CreateJsonElement(object value)
    {
        var json = JsonSerializer.Serialize(value);
        return JsonDocument.Parse(json).RootElement;
    }

    [Fact]
    public void GetProperty_ExistingStringKey_ReturnsCorrectValue()
    {
        var item = new TestContentItem
        {
            Properties = new Dictionary<string, JsonElement?>
            {
                ["title"] = CreateJsonElement("Hello World")
            }
        };

        Assert.Equal("Hello World", item.Title);
    }

    [Fact]
    public void GetProperty_ExistingIntKey_ReturnsCorrectValue()
    {
        var item = new TestContentItem
        {
            Properties = new Dictionary<string, JsonElement?>
            {
                ["count"] = CreateJsonElement(42)
            }
        };

        Assert.Equal(42, item.Count);
    }

    [Fact]
    public void GetProperty_MissingKey_ReturnsNull()
    {
        var item = new TestContentItem
        {
            Properties = new Dictionary<string, JsonElement?>()
        };

        Assert.Null(item.Title);
        Assert.Null(item.Count);
    }

    [Fact]
    public void GetProperty_NullValue_ReturnsNull()
    {
        var item = new TestContentItem
        {
            Properties = new Dictionary<string, JsonElement?>
            {
                ["title"] = null
            }
        };

        Assert.Null(item.Title);
    }

    [Fact]
    public void GetImageProperty_ValidMediaJsonElement_DeserializesCorrectly()
    {
        var mediaId = Guid.NewGuid();
        var media = new
        {
            id = mediaId,
            name = "hero.jpg",
            mediaType = "Image",
            url = "/media/hero.jpg",
            path = "/media/hero.jpg",
            createDate = "2024-01-01T00:00:00",
            updateDate = "2024-01-01T00:00:00",
            properties = new { }
        };

        var item = new TestContentItem
        {
            Properties = new Dictionary<string, JsonElement?>
            {
                ["heroImage"] = CreateJsonElement(media)
            }
        };

        var result = item.HeroImage;

        Assert.NotNull(result);
        Assert.Equal(mediaId, result.Id);
        Assert.Equal("hero.jpg", result.Name);
        Assert.Equal("/media/hero.jpg", result.Url);
    }

    [Fact]
    public void GetImageProperty_MissingKey_ReturnsNull()
    {
        var item = new TestContentItem
        {
            Properties = new Dictionary<string, JsonElement?>()
        };

        Assert.Null(item.HeroImage);
    }

    [Fact]
    public void GetAllImageProperties_ReturnsOnlyValidMediaItems()
    {
        var mediaId = Guid.NewGuid();
        var media = new
        {
            id = mediaId,
            name = "photo.jpg",
            mediaType = "Image",
            url = "/media/photo.jpg",
            path = "/media/photo.jpg",
            createDate = "2024-01-01T00:00:00",
            updateDate = "2024-01-01T00:00:00",
            properties = new { }
        };

        var item = new TestContentItem
        {
            Properties = new Dictionary<string, JsonElement?>
            {
                ["title"] = CreateJsonElement("Not an image"),
                ["count"] = CreateJsonElement(99),
                ["photo"] = CreateJsonElement(media)
            }
        };

        var images = item.GetAllImageProperties().ToList();

        Assert.Single(images);
        Assert.Equal("photo", images[0].Alias);
        Assert.Equal(mediaId, images[0].Media.Id);
    }

    [Fact]
    public void GetAllImageProperties_EmptyProperties_ReturnsEmpty()
    {
        var item = new TestContentItem
        {
            Properties = new Dictionary<string, JsonElement?>()
        };

        Assert.Empty(item.GetAllImageProperties());
    }

    [Fact]
    public void GetAllImageProperties_NoImageProperties_ReturnsEmpty()
    {
        var item = new TestContentItem
        {
            Properties = new Dictionary<string, JsonElement?>
            {
                ["title"] = CreateJsonElement("Hello"),
                ["count"] = CreateJsonElement(5)
            }
        };

        Assert.Empty(item.GetAllImageProperties());
    }

    [Fact]
    public void GetAllImageProperties_NullPropertyValues_AreSkipped()
    {
        var item = new TestContentItem
        {
            Properties = new Dictionary<string, JsonElement?>
            {
                ["nullProp"] = null
            }
        };

        Assert.Empty(item.GetAllImageProperties());
    }
}
