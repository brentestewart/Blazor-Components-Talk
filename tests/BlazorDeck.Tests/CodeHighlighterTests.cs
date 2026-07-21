using BlazorDeck.Components;

namespace BlazorDeck.Tests;

// The highlighter is a pure function (code in → per-line tokens out), so it tests cleanly with
// plain xunit — no rendering needed. These guard the per-language colouring the deck relies on.
public class CodeHighlighterTests
{
    private static IReadOnlyList<CodeToken> Tokens(string code, string language) =>
        CodeHighlighter.Tokenize(code, language).SelectMany(line => line).ToList();

    [Fact]
    public void Colours_a_csharp_keyword_as_a_keyword()
    {
        var tokens = Tokens("public int Count;", "csharp");
        Assert.Contains(tokens, t => t.Text == "public" && t.Kind == "keyword");
    }

    [Fact]
    public void Colours_a_browser_global_as_a_type_in_javascript()
    {
        var tokens = Tokens("document.title", "javascript");
        Assert.Contains(tokens, t => t.Text == "document" && t.Kind == "type");
    }

    [Fact]
    public void Treats_function_as_a_keyword_in_js_but_not_csharp()
    {
        Assert.Contains(Tokens("function f", "javascript"), t => t.Text == "function" && t.Kind == "keyword");
        Assert.DoesNotContain(Tokens("function f", "csharp"), t => t.Text == "function" && t.Kind == "keyword");
    }
}
