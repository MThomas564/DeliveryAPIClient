using System.Text.Json;
using DeliveryAPIClient.Extensions;
using DeliveryAPIClient.Models;
using Xunit;

namespace DeliveryAPIClient.Tests.Extensions;

public class ContentItemExtensionsTests
{
    private static JsonElement CreateJsonElement(object value)
    {
        var json = JsonSerializer.Serialize(value);
        return JsonDocument.Parse(json).RootElement;
    }

    private static ApiContentResponseModel BuildSource()
    {
        var id = Guid.NewGuid();
        return new ApiContentResponseModel
        {
            Id = id,
            ContentType = "blogPost",
            Name = "Test Article",
            CreateDate = new DateTime(2024, 3, 1, 9, 0, 0),
            UpdateDate = new DateTime(2024, 4, 15, 12, 0, 0),
            Route = new ApiContentRouteModel { Path = "/articles/test" },
            Cultures = new Dictionary<string, JsonElement?> { ["en"] = null },
            Properties = new Dictionary<string, JsonElement?>
            {
                ["title"] = CreateJsonElement("Hello World")
            }
        };
    }

    // -------------------------------------------------------------------------
    // As<T> — non-null source
    // -------------------------------------------------------------------------

    [Fact]
    public void As_CopiesId()
    {
        var source = BuildSource();
        var result = source.As<ContentItemBase>()!;
        Assert.Equal(source.Id, result.Id);
    }

    [Fact]
    public void As_CopiesContentType()
    {
        var source = BuildSource();
        var result = source.As<ContentItemBase>()!;
        Assert.Equal(source.ContentType, result.ContentType);
    }

    [Fact]
    public void As_CopiesName()
    {
        var source = BuildSource();
        var result = source.As<ContentItemBase>()!;
        Assert.Equal(source.Name, result.Name);
    }

    [Fact]
    public void As_CopiesCreateDate()
    {
        var source = BuildSource();
        var result = source.As<ContentItemBase>()!;
        Assert.Equal(source.CreateDate, result.CreateDate);
    }

    [Fact]
    public void As_CopiesUpdateDate()
    {
        var source = BuildSource();
        var result = source.As<ContentItemBase>()!;
        Assert.Equal(source.UpdateDate, result.UpdateDate);
    }

    [Fact]
    public void As_CopiesRoute()
    {
        var source = BuildSource();
        var result = source.As<ContentItemBase>()!;
        Assert.Same(source.Route, result.Route);
    }

    [Fact]
    public void As_SharesCulturesReference()
    {
        var source = BuildSource();
        var result = source.As<ContentItemBase>()!;
        Assert.Same(source.Cultures, result.Cultures);
    }

    [Fact]
    public void As_SharesPropertiesDictionaryReference()
    {
        var source = BuildSource();
        var result = source.As<ContentItemBase>()!;
        Assert.Same(source.Properties, result.Properties);
    }

    [Fact]
    public void As_ReturnsCorrectConcreteType()
    {
        var source = BuildSource();

        var result = source.As<TestTypedItem>();

        Assert.IsType<TestTypedItem>(result);
    }

    [Fact]
    public void As_AllFieldsCopied_EndToEnd()
    {
        var source = BuildSource();
        var result = source.As<TestTypedItem>()!;

        Assert.Equal(source.Id, result.Id);
        Assert.Equal(source.ContentType, result.ContentType);
        Assert.Equal(source.Name, result.Name);
        Assert.Equal(source.CreateDate, result.CreateDate);
        Assert.Equal(source.UpdateDate, result.UpdateDate);
        Assert.Same(source.Route, result.Route);
        Assert.Same(source.Cultures, result.Cultures);
        Assert.Same(source.Properties, result.Properties);
    }

    // -------------------------------------------------------------------------
    // As<T> — nullable overload
    // -------------------------------------------------------------------------

    [Fact]
    public void As_NullableOverload_WhenSourceIsNull_ReturnsNull()
    {
        ApiContentResponseModel? source = null;
        var result = source.As<ContentItemBase>();
        Assert.Null(result);
    }

    [Fact]
    public void As_NullableOverload_WhenSourceIsNotNull_ReturnsMappedItem()
    {
        ApiContentResponseModel? source = BuildSource();
        var result = source.As<TestTypedItem>();

        Assert.NotNull(result);
        Assert.IsType<TestTypedItem>(result);
        Assert.Equal(source.Id, result.Id);
    }

    // -------------------------------------------------------------------------
    // GetAllImageProperties
    // -------------------------------------------------------------------------

    [Fact]
    public void GetAllImageProperties_WithValidMediaProperty_ReturnsIt()
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

        var item = new ContentItemBase
        {
            Properties = new Dictionary<string, JsonElement?>
            {
                ["heroImage"] = CreateJsonElement(media)
            }
        };

        var images = item.GetAllImageProperties().ToList();

        Assert.Single(images);
        Assert.Equal("heroImage", images[0].Alias);
        Assert.Equal(mediaId, images[0].Media.Id);
        Assert.Equal("/media/hero.jpg", images[0].Media.Url);
    }

    [Fact]
    public void GetAllImageProperties_FiltersOutNonImageProperties()
    {
        var media = new
        {
            id = Guid.NewGuid(),
            name = "photo.jpg",
            mediaType = "Image",
            url = "/media/photo.jpg",
            path = "/media/photo.jpg",
            createDate = "2024-01-01T00:00:00",
            updateDate = "2024-01-01T00:00:00",
            properties = new { }
        };

        var item = new ContentItemBase
        {
            Properties = new Dictionary<string, JsonElement?>
            {
                ["title"] = CreateJsonElement("A string"),
                ["count"] = CreateJsonElement(42),
                ["photo"] = CreateJsonElement(media)
            }
        };

        var images = item.GetAllImageProperties().ToList();

        Assert.Single(images);
        Assert.Equal("photo", images[0].Alias);
    }

    [Fact]
    public void GetAllImageProperties_MediaWithoutUrl_IsExcluded()
    {
        // A JSON object that can be deserialized as ApiMediaWithCropsResponseModel but has no URL
        var media = new
        {
            id = Guid.NewGuid(),
            name = "nourl.jpg",
            mediaType = "Image",
            url = "",
            path = "/media/nourl.jpg",
            createDate = "2024-01-01T00:00:00",
            updateDate = "2024-01-01T00:00:00",
            properties = new { }
        };

        var item = new ContentItemBase
        {
            Properties = new Dictionary<string, JsonElement?>
            {
                ["image"] = CreateJsonElement(media)
            }
        };

        Assert.Empty(item.GetAllImageProperties());
    }

    [Fact]
    public void GetAllImageProperties_NullPropertyValue_IsSkipped()
    {
        var item = new ContentItemBase
        {
            Properties = new Dictionary<string, JsonElement?>
            {
                ["nullProp"] = null
            }
        };

        Assert.Empty(item.GetAllImageProperties());
    }

    [Fact]
    public void GetAllImageProperties_EmptyProperties_ReturnsEmpty()
    {
        var item = new ContentItemBase();
        Assert.Empty(item.GetAllImageProperties());
    }

    [Fact]
    public void GetAllImageProperties_MultipleMediaProperties_ReturnsAll()
    {
        var media1 = new
        {
            id = Guid.NewGuid(),
            name = "img1.jpg",
            mediaType = "Image",
            url = "/media/img1.jpg",
            path = "/media/img1.jpg",
            createDate = "2024-01-01T00:00:00",
            updateDate = "2024-01-01T00:00:00",
            properties = new { }
        };
        var media2 = new
        {
            id = Guid.NewGuid(),
            name = "img2.jpg",
            mediaType = "Image",
            url = "/media/img2.jpg",
            path = "/media/img2.jpg",
            createDate = "2024-01-01T00:00:00",
            updateDate = "2024-01-01T00:00:00",
            properties = new { }
        };

        var item = new ContentItemBase
        {
            Properties = new Dictionary<string, JsonElement?>
            {
                ["image1"] = CreateJsonElement(media1),
                ["image2"] = CreateJsonElement(media2)
            }
        };

        var images = item.GetAllImageProperties().ToList();
        Assert.Equal(2, images.Count);
    }

    // Helper typed class
    public class TestTypedItem : ContentItemBase { }
}
