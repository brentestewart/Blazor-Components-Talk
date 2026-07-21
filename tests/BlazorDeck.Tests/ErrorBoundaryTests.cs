using Bunit;
using BlazorDeck.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorDeck.Tests;

// The deck wraps its slide host in an ErrorBoundary so a throwing slide degrades to a fallback
// instead of killing the circuit. These render the real pieces and assert that behaviour.
public class ErrorBoundaryTests : BunitContext
{
    // Stands in for a slide with a bug: throws while rendering.
    private sealed class Boom : ComponentBase
    {
        protected override void BuildRenderTree(RenderTreeBuilder builder)
            => throw new InvalidOperationException("boom!");
    }

    [Fact]
    public void ErrorBoundary_catches_a_throwing_child_and_renders_the_error_content()
    {
        var cut = Render<ErrorBoundary>(ps => ps
            .Add<Boom>(p => p.ChildContent)
            .Add(p => p.ErrorContent, ex => $"<p class=\"caught\">{ex.Message}</p>"));

        Assert.Equal("boom!", cut.Find("p.caught").TextContent);
    }

    [Fact]
    public void SlideErrorFallback_shows_the_exception_message_and_no_button_without_recover()
    {
        var cut = Render<SlideErrorFallback>(p => p
            .Add(x => x.Error, new InvalidOperationException("kaboom")));

        Assert.Contains("kaboom", cut.Find(".se-msg").TextContent);
        Assert.Empty(cut.FindAll(".se-btn"));
    }
}
