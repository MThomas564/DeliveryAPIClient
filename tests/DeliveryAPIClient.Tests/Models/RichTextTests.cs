using System.Text.Json;
using DeliveryAPIClient.Models;
using Xunit;

namespace DeliveryAPIClient.Tests.Models;

public class RichTextTests
{
    private static readonly JsonSerializerOptions Options = new() { PropertyNameCaseInsensitive = true };

    private const string SimpleJson = """
        {
          "tag": "#root",
          "attributes": {},
          "elements": [
            {
              "tag": "p",
              "attributes": {},
              "elements": [
                { "tag": "#text", "text": "Hello ", "attributes": {} },
                {
                  "tag": "strong",
                  "attributes": {},
                  "elements": [
                    { "tag": "#text", "text": "world", "attributes": {} }
                  ]
                }
              ]
            }
          ],
          "blocks": []
        }
        """;

    private const string BlockJson = """
        {
          "tag": "#root",
          "attributes": {},
          "elements": [
            {
              "tag": "p",
              "attributes": {},
              "elements": [{ "tag": "#text", "text": "Before block.", "attributes": {} }]
            },
            {
              "tag": "umb-rte-block",
              "attributes": { "content-id": "705a6633-3e20-4f3a-be76-4b450a397608" },
              "elements": []
            },
            {
              "tag": "p",
              "attributes": {},
              "elements": [{ "tag": "#text", "text": "After block.", "attributes": {} }]
            }
          ],
          "blocks": [
            {
              "content": {
                "id": "705a6633-3e20-4f3a-be76-4b450a397608",
                "contentType": "callout",
                "name": "My Callout",
                "properties": {},
                "createDate": "2024-01-01T00:00:00Z",
                "updateDate": "2024-01-01T00:00:00Z",
                "route": { "path": "/", "startItem": { "id": "00000000-0000-0000-0000-000000000000", "path": "/" } },
                "cultures": {}
              },
              "settings": null
            }
          ]
        }
        """;

    [Fact]
    public void Deserialize_SimpleRichText_ParsesCorrectly()
    {
        var model = JsonSerializer.Deserialize<RichTextModel>(SimpleJson, Options);

        Assert.NotNull(model);
        Assert.Equal("#root", model.Tag);
        Assert.Single(model.Elements!);
        Assert.False(model.HasBlocks);
    }

    [Fact]
    public void ToHtml_SimpleText_RendersCorrectHtml()
    {
        var model = JsonSerializer.Deserialize<RichTextModel>(SimpleJson, Options)!;

        var html = model.ToHtml();

        Assert.Equal("<p>Hello <strong>world</strong></p>", html);
    }

    [Fact]
    public void ToHtml_WithBlock_RendersPlaceholderDiv()
    {
        var model = JsonSerializer.Deserialize<RichTextModel>(BlockJson, Options)!;

        var html = model.ToHtml();

        Assert.Contains("data-umb-block=\"705a6633-3e20-4f3a-be76-4b450a397608\"", html);
        Assert.Contains("<p>Before block.</p>", html);
        Assert.Contains("<p>After block.</p>", html);
    }

    [Fact]
    public void HasBlocks_WhenBlocksPresent_ReturnsTrue()
    {
        var model = JsonSerializer.Deserialize<RichTextModel>(BlockJson, Options)!;

        Assert.True(model.HasBlocks);
        Assert.Single(model.Blocks!);
    }

    [Fact]
    public void GetBlock_ByContentId_ReturnsCorrectBlock()
    {
        var model = JsonSerializer.Deserialize<RichTextModel>(BlockJson, Options)!;
        var id = Guid.Parse("705a6633-3e20-4f3a-be76-4b450a397608");

        var block = model.GetBlock(id);

        Assert.NotNull(block);
        Assert.Equal("callout", block.Content.ContentType);
    }

    [Fact]
    public void GetBlock_UnknownId_ReturnsNull()
    {
        var model = JsonSerializer.Deserialize<RichTextModel>(BlockJson, Options)!;

        Assert.Null(model.GetBlock(Guid.NewGuid()));
    }

    [Fact]
    public void RichTextElement_IsBlockNode_ReturnsTrue()
    {
        var el = new RichTextElement
        {
            Tag = "umb-rte-block",
            Attributes = new Dictionary<string, string>
            {
                ["content-id"] = "705a6633-3e20-4f3a-be76-4b450a397608"
            }
        };

        Assert.True(el.IsBlock);
        Assert.Equal(Guid.Parse("705a6633-3e20-4f3a-be76-4b450a397608"), el.BlockContentId);
    }

    [Fact]
    public void ToHtml_VoidElement_RendersAsSelfClosing()
    {
        var json = "{\"tag\":\"#root\",\"attributes\":{},\"blocks\":[],\"elements\":[{\"tag\":\"br\",\"attributes\":{},\"elements\":[]}]}";
        var model = JsonSerializer.Deserialize<RichTextModel>(json, Options)!;

        Assert.Equal("<br />", model.ToHtml());
    }

    [Fact]
    public void ToHtml_TextNodeWithSpecialChars_IsHtmlEncoded()
    {
        var json = "{\"tag\":\"#root\",\"attributes\":{},\"blocks\":[],\"elements\":[{\"tag\":\"#text\",\"text\":\"<script>alert('xss')</script>\",\"attributes\":{}}]}";
        var model = JsonSerializer.Deserialize<RichTextModel>(json, Options)!;
        var html = model.ToHtml();

        Assert.DoesNotContain("<script>", html);
        Assert.Contains("&lt;script&gt;", html);
    }

    [Fact]
    public void GetRichTextProperty_OnContentItemBase_DeserializesCorrectly()
    {
        var item = new ApiContentResponseModel
        {
            Properties = new Dictionary<string, System.Text.Json.JsonElement?>
            {
                ["bodyText"] = JsonSerializer.Deserialize<System.Text.Json.JsonElement>(SimpleJson)
            }
        };

        // Access via subclass to call the protected helper
        var post = new TestPost(item);

        Assert.NotNull(post.BodyText);
        Assert.Equal("<p>Hello <strong>world</strong></p>", post.BodyText.ToHtml());
    }

    // Helper subclass to expose protected method
    private class TestPost : ContentItemBase
    {
        public TestPost(ApiContentResponseModel source)
        {
            Id = source.Id;
            Properties = source.Properties;
        }

        public RichTextModel? BodyText => GetRichTextProperty("bodyText");
    }
}
