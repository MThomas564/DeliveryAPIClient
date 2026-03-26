using DeliveryAPIClient.Client;
using DeliveryAPIClient.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DeliveryAPIClient.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUmbracoDeliveryApiClient(
        this IServiceCollection services,
        Action<DeliveryApiOptions> configureOptions)
    {
        services.AddOptions<DeliveryApiOptions>().Configure(configureOptions);

        services.AddHttpClient<IDeliveryApiClient, Client.DeliveryApiClient>(
            "UmbracoDeliveryApi",
            (sp, client) =>
            {
                var opts = sp.GetRequiredService<IOptions<DeliveryApiOptions>>().Value;
                client.BaseAddress = new Uri(opts.BaseUrl.TrimEnd('/') + "/");
            });

        services.AddScoped<IContentService, ContentService>();
        services.AddScoped<IMediaService, MediaService>();

        return services;
    }
}
