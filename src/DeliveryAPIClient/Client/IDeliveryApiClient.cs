using DeliveryAPIClient.Models;

namespace DeliveryAPIClient.Client;

public interface IDeliveryApiClient
{
    // Content endpoints
    Task<PagedResponse<ApiContentResponseModel>> GetContentAsync(
        ContentQueryParameters parameters,
        CancellationToken cancellationToken = default);

    Task<ApiContentResponseModel?> GetContentByPathAsync(
        string path,
        string? expand = "properties[$all]",
        string? fields = null,
        string? language = null,
        string? segment = null,
        bool? preview = null,
        string? startItem = null,
        CancellationToken cancellationToken = default);

    Task<ApiContentResponseModel?> GetContentByIdAsync(
        Guid id,
        string? expand = "properties[$all]",
        string? fields = null,
        string? language = null,
        string? segment = null,
        bool? preview = null,
        string? startItem = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ApiContentResponseModel>> GetContentItemsAsync(
        IEnumerable<Guid> ids,
        string? expand = "properties[$all]",
        string? fields = null,
        string? language = null,
        string? segment = null,
        bool? preview = null,
        string? startItem = null,
        CancellationToken cancellationToken = default);

    // Media endpoints
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
