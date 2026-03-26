using DeliveryAPIClient.Client;
using DeliveryAPIClient.Models;
using DeliveryAPIClient.Services;
using Moq;
using Xunit;

namespace DeliveryAPIClient.Tests.Services;

public class MediaServiceTests
{
    private readonly Mock<IDeliveryApiClient> _clientMock;
    private readonly MediaService _service;

    public MediaServiceTests()
    {
        _clientMock = new Mock<IDeliveryApiClient>();
        _service = new MediaService(_clientMock.Object);
    }

    private static ApiMediaWithCropsResponseModel MakeMedia(string name = "photo.jpg")
    {
        return new ApiMediaWithCropsResponseModel
        {
            Id = Guid.NewGuid(),
            Name = name,
            MediaType = "Image",
            Url = $"/media/{name}",
            Path = $"/media/{name}"
        };
    }

    [Fact]
    public async Task GetMediaAsync_DelegatesToClient()
    {
        var expected = new PagedResponse<ApiMediaWithCropsResponseModel>
        {
            Total = 1,
            Items = new List<ApiMediaWithCropsResponseModel> { MakeMedia() }
        };
        var parameters = new MediaQueryParameters();

        _clientMock
            .Setup(c => c.GetMediaAsync(parameters, default))
            .ReturnsAsync(expected);

        var result = await _service.GetMediaAsync(parameters);

        Assert.Equal(expected, result);
        _clientMock.Verify(c => c.GetMediaAsync(parameters, default), Times.Once);
    }

    [Fact]
    public async Task GetMediaByPathAsync_DelegatesToClientWithCorrectArgs()
    {
        var media = MakeMedia();

        _clientMock
            .Setup(c => c.GetMediaByPathAsync("/media/photo.jpg", "properties[$all]", null, default))
            .ReturnsAsync(media);

        var result = await _service.GetMediaByPathAsync("/media/photo.jpg", "properties[$all]", null);

        Assert.Equal(media, result);
        _clientMock.Verify(
            c => c.GetMediaByPathAsync("/media/photo.jpg", "properties[$all]", null, default),
            Times.Once);
    }

    [Fact]
    public async Task GetMediaByPathAsync_WhenClientReturnsNull_ReturnsNull()
    {
        _clientMock
            .Setup(c => c.GetMediaByPathAsync("/nonexistent", null, null, default))
            .ReturnsAsync((ApiMediaWithCropsResponseModel?)null);

        var result = await _service.GetMediaByPathAsync("/nonexistent");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetMediaByIdAsync_DelegatesToClientWithCorrectArgs()
    {
        var id = Guid.NewGuid();
        var media = MakeMedia();

        _clientMock
            .Setup(c => c.GetMediaByIdAsync(id, null, null, default))
            .ReturnsAsync(media);

        var result = await _service.GetMediaByIdAsync(id);

        Assert.Equal(media, result);
        _clientMock.Verify(c => c.GetMediaByIdAsync(id, null, null, default), Times.Once);
    }

    [Fact]
    public async Task GetMediaByIdAsync_WhenClientReturnsNull_ReturnsNull()
    {
        var id = Guid.NewGuid();

        _clientMock
            .Setup(c => c.GetMediaByIdAsync(id, null, null, default))
            .ReturnsAsync((ApiMediaWithCropsResponseModel?)null);

        var result = await _service.GetMediaByIdAsync(id);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetMediaItemsAsync_DelegatesToClientWithCorrectArgs()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var items = ids.Select(id => MakeMedia($"{id}.jpg")).ToList();

        _clientMock
            .Setup(c => c.GetMediaItemsAsync(ids, null, null, default))
            .ReturnsAsync(items.AsReadOnly());

        var result = await _service.GetMediaItemsAsync(ids);

        Assert.Equal(2, result.Count);
        _clientMock.Verify(c => c.GetMediaItemsAsync(ids, null, null, default), Times.Once);
    }

    [Fact]
    public async Task GetMediaItemsAsync_WithExpandAndFields_PassesThemToClient()
    {
        var ids = new[] { Guid.NewGuid() };
        var items = new List<ApiMediaWithCropsResponseModel> { MakeMedia() };

        _clientMock
            .Setup(c => c.GetMediaItemsAsync(ids, "properties[$all]", "url,name", default))
            .ReturnsAsync(items.AsReadOnly());

        var result = await _service.GetMediaItemsAsync(ids, "properties[$all]", "url,name");

        Assert.Single(result);
        _clientMock.Verify(
            c => c.GetMediaItemsAsync(ids, "properties[$all]", "url,name", default),
            Times.Once);
    }
}
