using DeliveryAPIClient.Models;

namespace DeliveryAPIClient.Services;

public interface IContentService
{
    Task<PagedResponse<ApiContentResponseModel>> GetContentAsync(
        ContentQueryParameters parameters,
        CancellationToken cancellationToken = default);

    Task<ApiContentResponseModel?> GetContentByPathAsync(
        string path,
        ContentQueryParameters? parameters = null,
        CancellationToken cancellationToken = default);

    Task<ApiContentResponseModel?> GetContentByIdAsync(
        Guid id,
        ContentQueryParameters? parameters = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ApiContentResponseModel>> GetContentItemsAsync(
        IEnumerable<Guid> ids,
        ContentQueryParameters? parameters = null,
        CancellationToken cancellationToken = default);

    Task<PagedResponse<T>> GetContentAsync<T>(
        ContentQueryParameters parameters,
        CancellationToken cancellationToken = default)
        where T : ContentItemBase, new();

    Task<T?> GetContentByPathAsync<T>(
        string path,
        ContentQueryParameters? parameters = null,
        CancellationToken cancellationToken = default)
        where T : ContentItemBase, new();

    Task<T?> GetContentByIdAsync<T>(
        Guid id,
        ContentQueryParameters? parameters = null,
        CancellationToken cancellationToken = default)
        where T : ContentItemBase, new();

    Task<IReadOnlyList<T>> GetContentItemsAsync<T>(
        IEnumerable<Guid> ids,
        ContentQueryParameters? parameters = null,
        CancellationToken cancellationToken = default)
        where T : ContentItemBase, new();
}
