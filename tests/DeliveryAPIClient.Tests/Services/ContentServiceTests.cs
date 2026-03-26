using DeliveryAPIClient.Client;
using DeliveryAPIClient.Models;
using DeliveryAPIClient.Services;
using Moq;
using Xunit;

namespace DeliveryAPIClient.Tests.Services;

public class ContentServiceTests
{
    private readonly Mock<IDeliveryApiClient> _clientMock;
    private readonly ContentService _service;

    public ContentServiceTests()
    {
        _clientMock = new Mock<IDeliveryApiClient>();
        _service = new ContentService(_clientMock.Object);
    }

    private static ApiContentResponseModel MakeContent(string name = "Test")
    {
        return new ApiContentResponseModel
        {
            Id = Guid.NewGuid(),
            ContentType = "page",
            Name = name,
            Route = new ApiContentRouteModel { Path = "/test" }
        };
    }

    // -------------------------------------------------------------------------
    // Non-generic delegation
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetContentAsync_DelegatesToClient()
    {
        var expected = new PagedResponse<ApiContentResponseModel>
        {
            Total = 1,
            Items = new List<ApiContentResponseModel> { MakeContent() }
        };
        var parameters = new ContentQueryParameters();

        _clientMock
            .Setup(c => c.GetContentAsync(parameters, default))
            .ReturnsAsync(expected);

        var result = await _service.GetContentAsync(parameters);

        Assert.Equal(expected, result);
        _clientMock.Verify(c => c.GetContentAsync(parameters, default), Times.Once);
    }

    [Fact]
    public async Task GetContentByPathAsync_DelegatesToClientWithCorrectArgs()
    {
        var content = MakeContent();
        var parameters = new ContentQueryParameters
        {
            Expand = "properties[$all]",
            Fields = null,
            Language = "en-US",
            Segment = null,
            Preview = false,
            StartItem = null
        };

        _clientMock
            .Setup(c => c.GetContentByPathAsync(
                "/test",
                "properties[$all]",
                null,
                "en-US",
                null,
                false,
                null,
                default))
            .ReturnsAsync(content);

        var result = await _service.GetContentByPathAsync("/test", parameters);

        Assert.Equal(content, result);
    }

    [Fact]
    public async Task GetContentByPathAsync_NullParameters_UsesDefaultExpand()
    {
        var content = MakeContent();

        _clientMock
            .Setup(c => c.GetContentByPathAsync(
                "/test",
                "properties[$all]",
                null,
                null,
                null,
                null,
                null,
                default))
            .ReturnsAsync(content);

        var result = await _service.GetContentByPathAsync("/test", null);

        Assert.Equal(content, result);
    }

    [Fact]
    public async Task GetContentByIdAsync_DelegatesToClientWithCorrectArgs()
    {
        var id = Guid.NewGuid();
        var content = MakeContent();

        _clientMock
            .Setup(c => c.GetContentByIdAsync(
                id,
                "properties[$all]",
                null,
                null,
                null,
                null,
                null,
                default))
            .ReturnsAsync(content);

        var result = await _service.GetContentByIdAsync(id);

        Assert.Equal(content, result);
    }

    [Fact]
    public async Task GetContentItemsAsync_DelegatesToClientWithCorrectArgs()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var items = new List<ApiContentResponseModel> { MakeContent("A"), MakeContent("B") };

        _clientMock
            .Setup(c => c.GetContentItemsAsync(
                ids,
                "properties[$all]",
                null,
                null,
                null,
                null,
                null,
                default))
            .ReturnsAsync(items.AsReadOnly());

        var result = await _service.GetContentItemsAsync(ids);

        Assert.Equal(2, result.Count);
    }

    // -------------------------------------------------------------------------
    // Generic mapping
    // -------------------------------------------------------------------------

    public class BlogPost : ContentItemBase
    {
        public string? Title { get; set; }
    }

    [Fact]
    public async Task GetContentByPathAsync_Generic_MapsResultToTypedSubclass()
    {
        var id = Guid.NewGuid();
        var content = new ApiContentResponseModel
        {
            Id = id,
            ContentType = "blogPost",
            Name = "My Blog",
            Route = new ApiContentRouteModel { Path = "/blog/my-blog" }
        };

        _clientMock
            .Setup(c => c.GetContentByPathAsync(
                "/blog/my-blog",
                "properties[$all]",
                null,
                null,
                null,
                null,
                null,
                default))
            .ReturnsAsync(content);

        var result = await _service.GetContentByPathAsync<BlogPost>("/blog/my-blog");

        Assert.NotNull(result);
        Assert.IsType<BlogPost>(result);
        Assert.Equal(id, result.Id);
        Assert.Equal("My Blog", result.Name);
        Assert.Equal("blogPost", result.ContentType);
    }

    [Fact]
    public async Task GetContentByPathAsync_Generic_WhenClientReturnsNull_ReturnsNull()
    {
        _clientMock
            .Setup(c => c.GetContentByPathAsync(
                "/nonexistent",
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<bool?>(),
                It.IsAny<string?>(),
                default))
            .ReturnsAsync((ApiContentResponseModel?)null);

        var result = await _service.GetContentByPathAsync<BlogPost>("/nonexistent");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetContentAsync_Generic_MapsPagedResultToTypedSubclass()
    {
        var id = Guid.NewGuid();
        var paged = new PagedResponse<ApiContentResponseModel>
        {
            Total = 1,
            Items = new List<ApiContentResponseModel>
            {
                new() { Id = id, ContentType = "blogPost", Name = "Blog 1", Route = new() }
            }
        };
        var parameters = new ContentQueryParameters();

        _clientMock
            .Setup(c => c.GetContentAsync(parameters, default))
            .ReturnsAsync(paged);

        var result = await _service.GetContentAsync<BlogPost>(parameters);

        Assert.Equal(1L, result.Total);
        Assert.Single(result.Items);
        Assert.IsType<BlogPost>(result.Items[0]);
        Assert.Equal(id, result.Items[0].Id);
    }

    [Fact]
    public async Task GetContentByIdAsync_Generic_MapsResultToTypedSubclass()
    {
        var id = Guid.NewGuid();
        var content = new ApiContentResponseModel
        {
            Id = id,
            ContentType = "blogPost",
            Name = "Blog",
            Route = new ApiContentRouteModel()
        };

        _clientMock
            .Setup(c => c.GetContentByIdAsync(
                id,
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<bool?>(),
                It.IsAny<string?>(),
                default))
            .ReturnsAsync(content);

        var result = await _service.GetContentByIdAsync<BlogPost>(id);

        Assert.NotNull(result);
        Assert.IsType<BlogPost>(result);
        Assert.Equal(id, result.Id);
    }

    [Fact]
    public async Task GetContentItemsAsync_Generic_MapsAllItemsToTypedSubclass()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var items = ids.Select(id => new ApiContentResponseModel
        {
            Id = id,
            ContentType = "blogPost",
            Name = $"Blog {id}",
            Route = new ApiContentRouteModel()
        }).ToList();

        _clientMock
            .Setup(c => c.GetContentItemsAsync(
                ids,
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<bool?>(),
                It.IsAny<string?>(),
                default))
            .ReturnsAsync(items.AsReadOnly());

        var result = await _service.GetContentItemsAsync<BlogPost>(ids);

        Assert.Equal(2, result.Count);
        Assert.All(result, item => Assert.IsType<BlogPost>(item));
    }
}
