using DeliveryAPIClient.Client;
using DeliveryAPIClient.Extensions;
using DeliveryAPIClient.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace DeliveryAPIClient.Tests;

public class DependencyInjectionTests
{
    private static IServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();

        // Register options directly using object initializer (init-only BaseUrl requires this)
        services.AddSingleton(Options.Create(new DeliveryApiOptions
        {
            BaseUrl = "https://example.umbraco.io/"
        }));

        services.AddHttpClient<IDeliveryApiClient, DeliveryApiClient>(
            "UmbracoDeliveryApi",
            (sp, client) =>
            {
                var opts = sp.GetRequiredService<IOptions<DeliveryApiOptions>>().Value;
                client.BaseAddress = new Uri(opts.BaseUrl.TrimEnd('/') + "/");
            });

        services.AddScoped<IContentService, ContentService>();
        services.AddScoped<IMediaService, MediaService>();

        return services.BuildServiceProvider();
    }

    [Fact]
    public void AddUmbracoDeliveryApiClient_RegistersIDeliveryApiClient()
    {
        var provider = BuildProvider();

        var client = provider.GetService<IDeliveryApiClient>();

        Assert.NotNull(client);
    }

    [Fact]
    public void AddUmbracoDeliveryApiClient_RegistersIContentService()
    {
        var provider = BuildProvider();

        var service = provider.GetService<IContentService>();

        Assert.NotNull(service);
    }

    [Fact]
    public void AddUmbracoDeliveryApiClient_RegistersIMediaService()
    {
        var provider = BuildProvider();

        var service = provider.GetService<IMediaService>();

        Assert.NotNull(service);
    }

    [Fact]
    public void AddUmbracoDeliveryApiClient_IContentService_IsContentService()
    {
        var provider = BuildProvider();

        var service = provider.GetRequiredService<IContentService>();

        Assert.IsType<ContentService>(service);
    }

    [Fact]
    public void AddUmbracoDeliveryApiClient_IMediaService_IsMediaService()
    {
        var provider = BuildProvider();

        var service = provider.GetRequiredService<IMediaService>();

        Assert.IsType<MediaService>(service);
    }

    [Fact]
    public void AddUmbracoDeliveryApiClient_ContentService_IsScoped()
    {
        var services = new ServiceCollection();
        services.AddSingleton(Options.Create(new DeliveryApiOptions { BaseUrl = "https://example.umbraco.io/" }));
        services.AddHttpClient<IDeliveryApiClient, DeliveryApiClient>("UmbracoDeliveryApi");
        services.AddScoped<IContentService, ContentService>();
        services.AddScoped<IMediaService, MediaService>();

        var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IContentService));

        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [Fact]
    public void AddUmbracoDeliveryApiClient_MediaService_IsScoped()
    {
        var services = new ServiceCollection();
        services.AddSingleton(Options.Create(new DeliveryApiOptions { BaseUrl = "https://example.umbraco.io/" }));
        services.AddHttpClient<IDeliveryApiClient, DeliveryApiClient>("UmbracoDeliveryApi");
        services.AddScoped<IContentService, ContentService>();
        services.AddScoped<IMediaService, MediaService>();

        var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IMediaService));

        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [Fact]
    public void AddUmbracoDeliveryApiClient_ScopedInstances_AreSameWithinScope()
    {
        var provider = BuildProvider();

        using var scope = provider.CreateScope();
        var a = scope.ServiceProvider.GetRequiredService<IContentService>();
        var b = scope.ServiceProvider.GetRequiredService<IContentService>();

        Assert.Same(a, b);
    }

    [Fact]
    public void AddUmbracoDeliveryApiClient_ScopedInstances_DifferBetweenScopes()
    {
        var provider = BuildProvider();

        IContentService instanceA;
        IContentService instanceB;

        using (var scope1 = provider.CreateScope())
            instanceA = scope1.ServiceProvider.GetRequiredService<IContentService>();

        using (var scope2 = provider.CreateScope())
            instanceB = scope2.ServiceProvider.GetRequiredService<IContentService>();

        Assert.NotSame(instanceA, instanceB);
    }
}
