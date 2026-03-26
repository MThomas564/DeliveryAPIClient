using DeliveryAPIClient.Models;

namespace DeliveryAPIClient.Services;

public interface IMediaService
{
    Task<PagedResponse<ApiMediaWithCropsResponseModel>> GetMediaAsync(
        MediaQueryParameters parameters,
        CancellationToken cancellationToken = default);

    Task<ApiMediaWithCropsResponseModel?> GetMediaByPathAsync(
        string path,
        string? expand = null,
        string? fields = null,
        CancellationToken cancellationToken = default);

    Task<ApiMediaWithCropsResponseModel?> GetMediaByIdAsync(
        Guid id,
        string? expand = null,
        string? fields = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ApiMediaWithCropsResponseModel>> GetMediaItemsAsync(
        IEnumerable<Guid> ids,
        string? expand = null,
        string? fields = null,
        CancellationToken cancellationToken = default);
}
