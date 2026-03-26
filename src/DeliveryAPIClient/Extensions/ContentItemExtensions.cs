using System.Text.Json;
using DeliveryAPIClient.Models;

namespace DeliveryAPIClient.Extensions;

public static class ContentItemExtensions
{
    private static readonly JsonSerializerOptions DeserializeOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Maps a raw <see cref="ApiContentResponseModel"/> to a typed subclass <typeparamref name="T"/>.
    /// Copies all base class fields and shares the Properties dictionary reference.
    /// Returns null when source is null.
    /// </summary>
    public static T? As<T>(this ApiContentResponseModel? source) where T : ContentItemBase, new()
    {
        if (source is null)
            return null;

        return new T
        {
            Id = source.Id,
            ContentType = source.ContentType,
            Name = source.Name,
            CreateDate = source.CreateDate,
            UpdateDate = source.UpdateDate,
            Route = source.Route,
            Cultures = source.Cultures,
            Properties = source.Properties
        };
    }

    /// <summary>
    /// Returns all properties on the item that can be deserialized as <see cref="ApiMediaWithCropsResponseModel"/>,
    /// yielding the property alias and deserialized media model for each match.
    /// </summary>
    public static IEnumerable<(string Alias, ApiMediaWithCropsResponseModel Media)> GetAllImageProperties(
        this ContentItemBase item)
    {
        foreach (var kvp in item.Properties)
        {
            if (kvp.Value is null)
                continue;

            ApiMediaWithCropsResponseModel? media = null;
            try
            {
                media = kvp.Value.Value.Deserialize<ApiMediaWithCropsResponseModel>(DeserializeOptions);
            }
            catch (JsonException)
            {
                // Not a media object — skip
            }

            if (media is not null && !string.IsNullOrEmpty(media.Url))
                yield return (kvp.Key, media);
        }
    }
}
