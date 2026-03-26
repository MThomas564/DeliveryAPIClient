using DeliveryAPIClient.Client;
using DeliveryAPIClient.Extensions;
using DeliveryAPIClient.Models;

namespace DeliveryAPIClient.Services;

public class ContentService : IContentService
{
    private readonly IDeliveryApiClient _client;

    public ContentService(IDeliveryApiClient client)
    {
        _client = client;
    }

    public Task<PagedResponse<ApiContentResponseModel>> GetContentAsync(
        ContentQueryParameters parameters,
        CancellationToken cancellationToken = default)
        => _client.GetContentAsync(parameters, cancellationToken);

    public Task<ApiContentResponseModel?> GetContentByPathAsync(
        string path,
        ContentQueryParameters? parameters = null,
        CancellationToken cancellationToken = default)
        => _client.GetContentByPathAsync(
            path,
            parameters?.Expand ?? "properties[$all]",
            parameters?.Fields,
            parameters?.Language,
            parameters?.Segment,
            parameters?.Preview,
            parameters?.StartItem,
            cancellationToken);

    public Task<ApiContentResponseModel?> GetContentByIdAsync(
        Guid id,
        ContentQueryParameters? parameters = null,
        CancellationToken cancellationToken = default)
        => _client.GetContentByIdAsync(
            id,
            parameters?.Expand ?? "properties[$all]",
            parameters?.Fields,
            parameters?.Language,
            parameters?.Segment,
            parameters?.Preview,
            parameters?.StartItem,
            cancellationToken);

    public async Task<IReadOnlyList<ApiContentResponseModel>> GetContentItemsAsync(
        IEnumerable<Guid> ids,
        ContentQueryParameters? parameters = null,
        CancellationToken cancellationToken = default)
        => await _client.GetContentItemsAsync(
            ids,
            parameters?.Expand ?? "properties[$all]",
            parameters?.Fields,
            parameters?.Language,
            parameters?.Segment,
            parameters?.Preview,
            parameters?.StartItem,
            cancellationToken);

    public async Task<PagedResponse<T>> GetContentAsync<T>(
        ContentQueryParameters parameters,
        CancellationToken cancellationToken = default)
        where T : ContentItemBase, new()
    {
        var raw = await GetContentAsync(parameters, cancellationToken);
        return new PagedResponse<T>
        {
            Total = raw.Total,
            Items = raw.Items.Select(i => i.As<T>()).OfType<T>().ToList()
        };
    }

    public async Task<T?> GetContentByPathAsync<T>(
        string path,
        ContentQueryParameters? parameters = null,
        CancellationToken cancellationToken = default)
        where T : ContentItemBase, new()
    {
        var raw = await GetContentByPathAsync(path, parameters, cancellationToken);
        return raw.As<T>();
    }

    public async Task<T?> GetContentByIdAsync<T>(
        Guid id,
        ContentQueryParameters? parameters = null,
        CancellationToken cancellationToken = default)
        where T : ContentItemBase, new()
    {
        var raw = await GetContentByIdAsync(id, parameters, cancellationToken);
        return raw.As<T>();
    }

    public async Task<IReadOnlyList<T>> GetContentItemsAsync<T>(
        IEnumerable<Guid> ids,
        ContentQueryParameters? parameters = null,
        CancellationToken cancellationToken = default)
        where T : ContentItemBase, new()
    {
        var raw = await GetContentItemsAsync(ids, parameters, cancellationToken);
        return raw.Select(i => i.As<T>()).OfType<T>().ToList().AsReadOnly();
    }
}
