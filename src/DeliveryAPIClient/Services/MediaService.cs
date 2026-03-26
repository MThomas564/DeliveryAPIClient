using DeliveryAPIClient.Client;
using DeliveryAPIClient.Models;

namespace DeliveryAPIClient.Services;

public class MediaService : IMediaService
{
    private readonly IDeliveryApiClient _client;

    public MediaService(IDeliveryApiClient client)
    {
        _client = client;
    }

    public Task<PagedResponse<ApiMediaWithCropsResponseModel>> GetMediaAsync(
        MediaQueryParameters parameters,
        CancellationToken cancellationToken = default)
        => _client.GetMediaAsync(parameters, cancellationToken);

    public Task<ApiMediaWithCropsResponseModel?> GetMediaByPathAsync(
        string path,
        string? expand = null,
        string? fields = null,
        CancellationToken cancellationToken = default)
        => _client.GetMediaByPathAsync(path, expand, fields, cancellationToken);

    public Task<ApiMediaWithCropsResponseModel?> GetMediaByIdAsync(
        Guid id,
        string? expand = null,
        string? fields = null,
        CancellationToken cancellationToken = default)
        => _client.GetMediaByIdAsync(id, expand, fields, cancellationToken);

    public Task<IReadOnlyList<ApiMediaWithCropsResponseModel>> GetMediaItemsAsync(
        IEnumerable<Guid> ids,
        string? expand = null,
        string? fields = null,
        CancellationToken cancellationToken = default)
        => _client.GetMediaItemsAsync(ids, expand, fields, cancellationToken);
}
