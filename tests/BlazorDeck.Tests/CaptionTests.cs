using Bunit;
using BlazorDeck.Components;

namespace BlazorDeck.Tests;

// A bUnit render test: spin the real component up in memory, then assert on the DOM it produced.
public class CaptionTests : BunitContext
{
    [Fact]
    public void Renders_its_child_content_in_a_caption_paragraph()
    {
        var cut = Render<Caption>(p =>
            p.AddChildContent("It's all components."));

        var caption = cut.Find("p.caption");
        Assert.Equal("It's all components.", caption.TextContent);
    }
}
