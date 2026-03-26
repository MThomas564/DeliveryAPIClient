using DeliveryAPIClient.Client;
using DeliveryAPIClient.Examples.Models;
using DeliveryAPIClient.Extensions;
using DeliveryAPIClient.Models;
using DeliveryAPIClient.Services;
using Microsoft.Extensions.DependencyInjection;

// ──────────────────────────────────────────────────────────────────────────────
// Setup — register the SDK with your Umbraco site URL and optional API key.
// ──────────────────────────────────────────────────────────────────────────────
var services = new ServiceCollection();

services.AddUmbracoDeliveryApiClient(options =>
{
    options.BaseUrl = "https://your-umbraco-site.com"; // <-- replace with real URL
    options.ApiKey  = "your-api-key";                  // <-- optional
});

var provider = services.BuildServiceProvider();
var contentService = provider.GetRequiredService<IContentService>();
var mediaService   = provider.GetRequiredService<IMediaService>();
var rawClient      = provider.GetRequiredService<IDeliveryApiClient>();

// ──────────────────────────────────────────────────────────────────────────────
// Example 1: Fetch a single page as a typed model
// ──────────────────────────────────────────────────────────────────────────────
Console.WriteLine("=== Example 1: Typed content model ===");

BlogPost? post = await contentService.GetContentByPathAsync<BlogPost>("/blog/my-first-post");

if (post is not null)
{
    Console.WriteLine($"Title:   {post.Title}");
    Console.WriteLine($"Summary: {post.Summary}");
    Console.WriteLine($"Tags:    {string.Join(", ", post.Tags ?? [])}");

    if (post.HeroImage is { } hero)
    {
        Console.WriteLine($"Hero image: {hero.Url} ({hero.Width}x{hero.Height})");

        if (hero.FocalPoint is { } fp)
            Console.WriteLine($"  Focal point: left={fp.Left}, top={fp.Top}");

        foreach (var crop in hero.Crops ?? [])
            Console.WriteLine($"  Crop '{crop.Alias}': {crop.Width}x{crop.Height}");
    }
}

// ──────────────────────────────────────────────────────────────────────────────
// Example 2: Paged content query — all blog posts sorted by date
// ──────────────────────────────────────────────────────────────────────────────
Console.WriteLine("\n=== Example 2: Paged query ===");

PagedResponse<BlogPost> blogPosts = await contentService.GetContentAsync<BlogPost>(
    new ContentQueryParameters
    {
        Fetch  = "children:/blog",
        Filter = ["contentType:blogPost"],
        Sort   = ["createDate:desc"],
        Skip   = 0,
        Take   = 5,
        Expand = "properties[$all]"
    });

Console.WriteLine($"Total posts: {blogPosts.Total}");
foreach (var p in blogPosts.Items)
    Console.WriteLine($"  [{p.Published:yyyy-MM-dd}] {p.Title}");

// ──────────────────────────────────────────────────────────────────────────────
// Example 3: Map a raw response to a typed model manually
// ──────────────────────────────────────────────────────────────────────────────
Console.WriteLine("\n=== Example 3: Manual mapping with As<T>() ===");

ApiContentResponseModel? raw = await contentService.GetContentByPathAsync("/home");
HomePage? home = raw.As<HomePage>();

if (home is not null)
{
    Console.WriteLine($"Heading:   {home.Heading}");
    Console.WriteLine($"Intro:     {home.IntroText}");

    // Access a linked content item (returned expanded when using properties[$all])
    if (home.FeaturedArticle is { } featured)
    {
        ArticlePage? article = featured.As<ArticlePage>();
        Console.WriteLine($"Featured:  {article?.Headline}");
    }
}

// ──────────────────────────────────────────────────────────────────────────────
// Example 4: Find all image properties on a content item
// ──────────────────────────────────────────────────────────────────────────────
Console.WriteLine("\n=== Example 4: GetAllImageProperties ===");

ApiContentResponseModel? anyPage = await contentService.GetContentByPathAsync("/articles/first");

if (anyPage is not null)
{
    foreach (var (alias, media) in anyPage.GetAllImageProperties())
        Console.WriteLine($"  {alias}: {media.Url} ({media.Width}x{media.Height})");
}

// ──────────────────────────────────────────────────────────────────────────────
// Example 5: Media API — list images in a folder
// ──────────────────────────────────────────────────────────────────────────────
Console.WriteLine("\n=== Example 5: Media API ===");

var mediaResult = await mediaService.GetMediaAsync(new MediaQueryParameters
{
    Fetch  = "children:/",
    Filter = ["mediaType:Image"],
    Sort   = ["createDate:desc"],
    Take   = 10
});

Console.WriteLine($"Total media items: {mediaResult.Total}");
foreach (var item in mediaResult.Items)
    Console.WriteLine($"  {item.Name}: {item.Url} ({item.Extension}, {item.Bytes} bytes)");

// ──────────────────────────────────────────────────────────────────────────────
// Example 6: Error handling
// ──────────────────────────────────────────────────────────────────────────────
Console.WriteLine("\n=== Example 6: Error handling ===");

try
{
    // 404 returns null, no exception
    var missing = await contentService.GetContentByPathAsync("/does-not-exist");
    Console.WriteLine(missing is null ? "404 → returned null (no exception)" : "Found unexpectedly");
}
catch (DeliveryApiException ex)
{
    Console.WriteLine($"API error {ex.StatusCode}: {ex.Message}");
}
