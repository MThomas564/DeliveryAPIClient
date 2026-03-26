# Umbraco Delivery API Client

A .NET 10 client library for the [Umbraco Content Delivery API v2](https://docs.umbraco.com/umbraco-cms/reference/content-delivery-api). Works with standard .NET applications and Blazor WASM.

## Installation

Add a project reference to `DeliveryAPIClient.csproj`, or reference the package once published to NuGet.

## Quick Start

### 1. Register services

```csharp
services.AddUmbracoDeliveryApiClient(options =>
{
    options.BaseUrl = "https://your-umbraco-site.com";
    options.ApiKey  = "your-api-key"; // optional
});
```

### 2. Register in Blazor WASM (`Program.cs`)

```csharp
// Program.cs
var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddUmbracoDeliveryApiClient(options =>
{
    options.BaseUrl = "https://your-umbraco-site.com";
    options.ApiKey  = "your-api-key"; // optional
});

await builder.Build().RunAsync();
```

### 3. Use in a Blazor component

```razor
@page "/blog/{Slug}"
@inject IContentService ContentService

@if (post is null)
{
    <p>Loading...</p>
}
else
{
    <h1>@post.Title</h1>
    @if (post.HeroImage is { } img)
    {
        <img src="@img.Url" alt="@post.Title" width="@img.Width" height="@img.Height" />
    }
    <div>@post.Summary</div>
}

@code {
    [Parameter] public string Slug { get; set; } = "";

    private BlogPost? post;

    protected override async Task OnParametersSetAsync()
    {
        post = await ContentService.GetContentByPathAsync<BlogPost>($"/blog/{Slug}");
    }
}
```

### Nav menu example

```razor
@inject IContentService ContentService

<nav>
    @foreach (var item in navItems)
    {
        <a href="@item.Route.Path">@item.Name</a>
    }
</nav>

@code {
    private IReadOnlyList<ApiContentResponseModel> navItems = [];

    protected override async Task OnInitializedAsync()
    {
        var result = await ContentService.GetContentAsync(new ContentQueryParameters
        {
            Fetch  = "children:/",
            Filter = ["contentType:navigationItem"],
            Sort   = ["sortOrder:asc"],
            Fields = "properties[]" // only need name + route, skip properties
        });

        navItems = result.Items;
    }
}
```

---

## Typed Content Models

The real power of this library is mapping dynamic Umbraco content to strongly-typed C# classes.

### Create a typed model

Inherit from `ContentItemBase` and expose properties using the protected `GetProperty<T>` and `GetImageProperty` helpers:

```csharp
using DeliveryAPIClient.Models;

public class BlogPost : ContentItemBase
{
    // Simple scalar properties
    public string? Title       => GetProperty<string>("title");
    public string? BodyText    => GetProperty<string>("bodyText");
    public DateTime? Published => GetProperty<DateTime>("publishDate");
    public string[]? Tags      => GetProperty<string[]>("tags");

    // Image/media picker property
    public ApiMediaWithCropsResponseModel? HeroImage => GetImageProperty("heroImage");

    // Nested content (e.g. a content picker expanded with properties[$all])
    public ApiContentResponseModel? FeaturedItem => GetProperty<ApiContentResponseModel>("featuredItem");
}
```

### Fetch as a typed model

```csharp
// Single item
BlogPost? post = await contentService.GetContentByPathAsync<BlogPost>("/blog/my-post");

// Paged collection
PagedResponse<BlogPost> posts = await contentService.GetContentAsync<BlogPost>(new ContentQueryParameters
{
    Fetch  = "children:/blog",
    Sort   = ["createDate:desc"],
    Take   = 10,
    Expand = "properties[$all]"
});

// Multiple items by ID
IReadOnlyList<BlogPost> items = await contentService.GetContentItemsAsync<BlogPost>(
    [id1, id2, id3]
);
```

### Map a raw response manually

```csharp
ApiContentResponseModel? raw = await contentService.GetContentByPathAsync("/blog/my-post");
BlogPost? post = raw.As<BlogPost>(); // ContentItemExtensions
```

---

## Working with Images

Umbraco media items are returned with full crop and focal point data.

### Image from a typed model

```csharp
public class ArticlePage : ContentItemBase
{
    public ApiMediaWithCropsResponseModel? Banner => GetImageProperty("banner");
}

// Usage
var article = await contentService.GetContentByPathAsync<ArticlePage>("/articles/first");

if (article?.Banner is { } banner)
{
    Console.WriteLine(banner.Url);            // /media/abc123/photo.jpg
    Console.WriteLine($"{banner.Width}x{banner.Height}");
    Console.WriteLine(banner.Extension);      // jpg

    // Focal point (for CSS background-position etc.)
    if (banner.FocalPoint is { } fp)
        Console.WriteLine($"Focal point: {fp.Left}, {fp.Top}");

    // Named crops
    foreach (var crop in banner.Crops ?? [])
        Console.WriteLine($"{crop.Alias}: {crop.Width}x{crop.Height}");
}
```

### Find all images on any content item

```csharp
using DeliveryAPIClient.Extensions;

var page = await contentService.GetContentByPathAsync("/home");

foreach (var (alias, media) in page.GetAllImageProperties())
{
    Console.WriteLine($"  {alias}: {media.Url} ({media.Width}x{media.Height})");
}
```

---

## Media API

```csharp
// Paged media query
var result = await mediaService.GetMediaAsync(new MediaQueryParameters
{
    Fetch  = "children:/",
    Filter = ["mediaType:Image"],
    Sort   = ["createDate:desc"],
    Take   = 20
});

// Single item by path or ID
var image = await mediaService.GetMediaByPathAsync("/media/hero.jpg");
var image = await mediaService.GetMediaByIdAsync(Guid.Parse("..."));
```

---

## Raw API Client

For full control, inject `IDeliveryApiClient` directly:

```csharp
public class MyService(IDeliveryApiClient client)
{
    public async Task<PagedResponse<ApiContentResponseModel>> GetBlogPosts()
        => await client.GetContentAsync(new ContentQueryParameters
        {
            Fetch   = "children:/blog",
            Filter  = ["contentType:blogPost"],
            Sort    = ["createDate:desc"],
            Skip    = 0,
            Take    = 10,
            Expand  = "properties[$all]",
            Language = "en-us",
            Preview  = false
        });
}
```

---

## Query Parameters

| Parameter  | Description                                                        | Example                        |
|------------|--------------------------------------------------------------------|--------------------------------|
| `Fetch`    | Scope of items to fetch                                            | `"children:/blog"`             |
| `Filter`   | Filter expressions (repeatable)                                    | `["contentType:blogPost"]`     |
| `Sort`     | Sort expressions (repeatable)                                      | `["createDate:desc"]`          |
| `Skip`     | Pagination offset (default: 0)                                     | `20`                           |
| `Take`     | Pagination page size (default: 10)                                 | `10`                           |
| `Expand`   | Properties to expand (default: `"properties[$all]"`)               | `"properties[heroImage]"`      |
| `Fields`   | Limit returned fields                                              | `"properties[title,heroImage]"` |
| `Language` | Culture variant (content only)                                     | `"en-us"`                      |
| `Segment`  | Segment variant (content only)                                     | `"segment-one"`                |
| `Preview`  | Include draft content (requires Api-Key, content only)             | `true`                         |
| `StartItem`| Root node scope — URL segment or GUID (content only)              | `"/blog"`                      |

---

## Error Handling

Non-success responses throw `DeliveryApiException`. 404s on single-item endpoints return `null`.

```csharp
try
{
    var page = await contentService.GetContentByPathAsync("/missing");
    // page is null for 404
}
catch (DeliveryApiException ex) when (ex.StatusCode == 401)
{
    Console.WriteLine("Check your Api-Key configuration.");
}
catch (DeliveryApiException ex)
{
    Console.WriteLine($"API error {ex.StatusCode}: {ex.Message}");
    if (ex.ProblemDetails is { } pd)
        Console.WriteLine(pd.Detail);
}
```

---

## Blazor WASM

Registration is identical — `IHttpClientFactory` is supported in Blazor WASM:

```csharp
// Program.cs
builder.Services.AddUmbracoDeliveryApiClient(options =>
{
    options.BaseUrl = "https://your-umbraco-site.com";
    options.ApiKey  = "your-api-key";
});
```

---

## Project Structure

```
src/
  DeliveryAPIClient/
    Client/           IDeliveryApiClient, DeliveryApiClient, options, exceptions
    Services/         IContentService, IMediaService and implementations
    Models/           ContentItemBase, response models, query parameter classes
    Extensions/       ServiceCollectionExtensions, ContentItemExtensions
examples/
  DeliveryAPIClient.Examples/
    Models/           Example typed content models
    Program.cs        Runnable console examples
tests/
  DeliveryAPIClient.Tests/   xUnit test suite (80 tests)
```
