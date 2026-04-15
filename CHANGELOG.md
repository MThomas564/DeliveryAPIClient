# Changelog

All notable changes to this project will be documented in this file.

## [1.0.0] - 2024-04-15

### Added

- `IDeliveryApiClient` with 8 endpoints covering content and media (list, by-path, by-id, batch by IDs)
- `IContentService` and `IMediaService` for convenient higher-level access
- `AddUmbracoDeliveryApiClient()` DI extension for both ASP.NET Core and Blazor WASM
- `ContentItemBase` base class with typed property helpers for all Umbraco property editors:
  - `GetProperty<T>` for scalars (string, int, bool, DateTime, etc.)
  - `GetImageProperty` / `GetMultipleImageProperty` (Media Picker)
  - `GetImageCropperProperty` (standalone Image Cropper)
  - `GetContentProperty` / `GetMultipleContentProperty` (Content Picker / MNTP)
  - `GetRichTextProperty` (TinyMCE / block-based rich text)
  - `GetBlockListProperty` / `GetBlockGridProperty`
  - `GetLinkProperty` / `GetLinksProperty` (Link Picker / Multi-URL Picker)
  - `GetColorPickerProperty`
- `ContentItemExtensions.As<T>()` for mapping raw `ApiContentResponseModel` to a typed subclass
- `ContentItemExtensions.GetAllImageProperties()` for enumerating all media properties on any item
- `DeliveryApiException` with `StatusCode` and structured `ProblemDetails` support
- Full response models: `ApiContentResponseModel`, `ApiMediaWithCropsResponseModel`, `PagedResponse<T>`, `RichTextModel`, `BlockGridModel`, `BlockItemModel`, `LinkModel`, `ColorPickerModel`, `ImageCropperModel`
- Targets `net8.0`, `net9.0`, and `net10.0`
