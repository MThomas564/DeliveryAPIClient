# Umbraco Delivery API Client

[![NuGet](https://img.shields.io/nuget/v/Umbraco.DeliveryApiClient)](https://www.nuget.org/packages/Umbraco.DeliveryApiClient)
[![CI](https://github.com/MThomas564/DeliveryAPIClient/actions/workflows/ci.yml/badge.svg)](https://github.com/MThomas564/DeliveryAPIClient/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

A strongly-typed .NET client for the [Umbraco Content Delivery API v2](https://docs.umbraco.com/umbraco-cms/reference/content-delivery-api). Supports typed content models, image crops, block lists, rich text, DI integration, and Blazor WASM. Targets .NET 8, 9, and 10.

## Installation

```bash
dotnet add package Umbraco.DeliveryApiClient
```

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

## Supported Property Types

All Umbraco property values live in the `Properties` dictionary as `JsonElement?`. The helper methods on `ContentItemBase` wrap `GetProperty<T>()` for typed access. Simple types work directly; complex types have dedicated helpers and models.

### Simple types — use `GetProperty<T>()`

| Umbraco editor | C# type | Example |
|---|---|---|
| Textstring, Textarea, Label, Email | `string?` | `GetProperty<string>("title")` |
| Numeric | `decimal?` | `GetProperty<decimal>("price")` |
| True/False | `bool?` | `GetProperty<bool>("isFeatured")` |
| Date Picker | `DateTime?` | `GetProperty<DateTime>("publishDate")` |
| Slider (single value) | `decimal?` | `GetProperty<decimal>("rating")` |
| Tags, Checkboxes | `string[]?` | `GetProperty<string[]>("tags")` |
| Dropdown (single) | `string?` | `GetProperty<string>("status")` |
| Dropdown (multiple) | `string[]?` | `GetProperty<string[]>("categories")` |
| Radio button list | `string?` | `GetProperty<string>("layout")` |
| Repeatable textstrings | `string[]?` | `GetProperty<string[]>("bullets")` |

### Complex types — dedicated helpers and models

| Umbraco editor | Helper method | Returns |
|---|---|---|
| Media Picker (single) | `GetImageProperty("alias")` | `ApiMediaWithCropsResponseModel?` |
| Media Picker (multiple) | `GetMultipleImageProperty("alias")` | `List<ApiMediaWithCropsResponseModel>?` |
| Image Cropper *(standalone)* | `GetImageCropperProperty("alias")` | `ImageCropperModel?` |
| Content Picker (single) | `GetContentProperty("alias")` | `ApiContentResponseModel?` |
| Multi-node Tree Picker | `GetMultipleContentProperty("alias")` | `List<ApiContentResponseModel>?` |
| Rich Text Editor | `GetRichTextProperty("alias")` | `RichTextModel?` |
| Block List | `GetBlockListProperty("alias")` | `List<BlockItemModel>?` |
| Block Grid | `GetBlockGridProperty("alias")` | `BlockGridModel?` |
| Link Picker (single) | `GetLinkProperty("alias")` | `LinkModel?` |
| Multi-URL Picker | `GetLinksProperty("alias")` | `List<LinkModel>?` |
| Color Picker | `GetColorPickerProperty("alias")` | `ColorPickerModel?` |

> **Note:** Content Picker and Media Picker return minimal reference objects by default. Pass `expand=properties[$all]` to get full inline data for linked items.

> **Note:** Nested Content (legacy editor) returns `List<ApiContentResponseModel>` — use `GetMultipleContentProperty`.

### Key models

**`ApiMediaWithCropsResponseModel`** — Media item with image data:
```csharp
media.Url          // "/media/abc/photo.jpg"
media.Width        // 1920
media.Height       // 1080
media.Extension    // "jpg"
media.FocalPoint   // ImageFocalPointModel { Left, Top }
media.Crops        // List<ImageCropModel> { Alias, Width, Height, Coordinates }
```

**`LinkModel`** — Multi-URL Picker / Link Picker:
```csharp
link.Url           // "/about" or "https://example.com"
link.Name          // "About us"
link.Target        // "_blank" or null
link.IsExternal    // false
link.OpensInNewTab // false
```

**`RichTextModel`** — Rich Text Editor:
```csharp
body.ToHtml()              // rendered HTML string
body.HasBlocks             // true if contains embedded blocks
body.GetBlock(contentId)   // RichTextBlockModel?
```

**`BlockItemModel`** — Block List item:
```csharp
block.Content.ContentType  // "callout"
block.Content.Properties   // Dictionary<string, JsonElement?>
block.Settings             // ApiContentResponseModel? (optional settings block)
```

**`BlockGridModel`** — Block Grid:
```csharp
grid.GridColumns           // 12
grid.Items                 // List<BlockGridItem>
item.ColumnSpan            // 6
item.RowSpan               // 1
item.Areas                 // List<BlockGridArea>
item.Content               // ApiContentResponseModel
```

**`ColorPickerModel`** — Color Picker:
```csharp
color.Color  // "#FF0000"
color.Label  // "Red"
```

**`ImageCropperModel`** — standalone Image Cropper:
```csharp
cropper.Src             // "/media/abc/image.jpg"
cropper.FocalPoint      // ImageFocalPointModel?
cropper.Crops           // List<ImageCropModel>?
cropper.GetCropUrl("thumbnail")
```

---

## Known Limitations & Umbraco API Constraints

These are deliberate restrictions in the Umbraco Delivery API itself — not limitations of this SDK.

### ⚠️ Member Picker is blocked by design

The Umbraco Delivery API **intentionally does not expose Member Picker properties**. This is a security decision by the Umbraco team to prevent member data (emails, passwords, personal information) from leaking through the headless API.

> "The Member Picker property editor is not supported in the Delivery API to avoid the risk of leaking member data."
> — Umbraco Documentation

**This applies regardless of:**
- Whether you supply an `Api-Key`
- Whether Preview mode is enabled
- Any server-side configuration

A member picker property will always return `null`. There is no configuration to change this.

#### The recommended pattern for "author" data

Since Author is not a built-in Umbraco content type, **you need to create one**. Use a Content Picker (not Member Picker) to link it:

**Step 1 — Create an Author document type in Umbraco backoffice:**

| Property alias | Type        | Notes                    |
|----------------|-------------|--------------------------|
| `displayName`  | Textstring  |                          |
| `bio`          | Textarea    |                          |
| `jobTitle`     | Textstring  |                          |
| `photo`        | Media Picker | Image only              |

**Step 2 — Create author content nodes** under a `/authors` root in the content tree.

**Step 3 — On your BlogPost document type**, add a property:
- Alias: `author`
- Type: **Content Picker** (not Member Picker)
- Allowed content types: Author

**Step 4 — In your typed model:**

```csharp
public class BlogPost : ContentItemBase
{
    // Author comes back as a nested ApiContentResponseModel when expanded,
    // and can be projected to a typed AuthorContent model.
    public AuthorContent? Author
    {
        get
        {
            var raw = GetProperty<ApiContentResponseModel>("author");
            return raw?.As<AuthorContent>();
        }
    }
}

public class AuthorContent : ContentItemBase
{
    public string? DisplayName => GetProperty<string>("displayName");
    public string? Bio         => GetProperty<string>("bio");
    public string? JobTitle    => GetProperty<string>("jobTitle");
    public ApiMediaWithCropsResponseModel? Photo => GetImageProperty("photo");
}
```

**Step 5 — Fetch with expansion** so the linked author is inlined:

```csharp
var post = await contentService.GetContentByPathAsync<BlogPost>(
    "/blog/my-post",
    parameters: new ContentQueryParameters { Expand = "properties[$all]" }
);

Console.WriteLine(post?.Author?.DisplayName);  // "Jane Smith"
Console.WriteLine(post?.Author?.Photo?.Url);   // "/media/.../jane.jpg"
```

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
