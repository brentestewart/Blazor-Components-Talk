using System.Text.RegularExpressions;

namespace BlazorDeck.Components;

/// <summary>One token of code: its text and a colour kind (null = plain, default text colour).</summary>
public readonly record struct CodeToken(string Text, string? Kind);

/// <summary>
/// A compact Razor/C# syntax highlighter that runs in C#, so slides render coloured code
/// directly in their markup — correct from the first paint, including prerender, with no JS and
/// no flash. It is a pure function (code in → per-line tokens out), which keeps it testable.
/// Not a full grammar; a stand-in tuned for the snippets on screen. A production app would use a
/// real grammar (and Razor's mix of HTML + C# + @ is exactly the part off-the-shelf ones miss).
/// </summary>
public static partial class CodeHighlighter
{
    private const string Keywords =
        "public|private|protected|internal|class|record|struct|interface|void|async|await|new|" +
        "return|if|else|for|foreach|while|in|var|string|int|bool|double|null|true|false|namespace|" +
        "using|get|set|static|readonly|event|partial|override|virtual|abstract|this|typeof|nameof";

    // Structural Razor directives whose whole @word is coloured (unlike @expressions such as
    // @Title, where only the "@" is coloured and the identifier stays a plain variable name).
    private const string Directives =
        "code|functions|foreach|if|else|for|while|do|switch|using|namespace|inject|page|layout|" +
        "implements|inherits|typeparam|attributes|rendermode|bind|ref|key|onclick|oninput|onchange";

    // Token types, tried left-to-right at each position. The (?<!\w) before a tag's "<" keeps
    // generics (IEnumerable<TItem>) from being read as tags. Comment/string come first so
    // keywords inside them aren't separately coloured.
    private const string Pattern =
        $@"(//[^\n]*)|(""[^""]*""|'[^']*')|(@(?:{Directives})\b)|(@)|((?<!\w)</?[A-Za-z][\w.-]*)|\b(?:{Keywords})\b|\b[A-Z][A-Za-z0-9_]*\b|\b\d[\w.]*\b";

    [GeneratedRegex(Pattern)]
    private static partial Regex Token();

    /// <summary>Tokenise each line of <paramref name="code"/> into coloured tokens.</summary>
    public static IReadOnlyList<IReadOnlyList<CodeToken>> Tokenize(string code)
    {
        var lines = new List<IReadOnlyList<CodeToken>>();
        foreach (var line in code.ReplaceLineEndings("\n").Split('\n'))   // normalise CRLF/CR first
        {
            lines.Add(TokenizeLine(line));
        }
        return lines;
    }

    private static List<CodeToken> TokenizeLine(string src)
    {
        var tokens = new List<CodeToken>();
        var last = 0;
        foreach (Match m in Token().Matches(src))
        {
            if (m.Index > last)
            {
                tokens.Add(new CodeToken(src[last..m.Index], null));   // gap = plain text
            }
            tokens.Add(new CodeToken(m.Value, Classify(m, src)));
            last = m.Index + m.Length;
        }
        if (last < src.Length)
        {
            tokens.Add(new CodeToken(src[last..], null));
        }
        return tokens;
    }

    private static string? Classify(Match m, string src)
    {
        if (m.Groups[1].Success) return "comment";
        if (m.Groups[2].Success) return "string";
        if (m.Groups[3].Success || m.Groups[4].Success) return "directive";
        if (m.Groups[5].Success) return "tag";

        var text = m.Value;
        if (char.IsDigit(text[0])) return "number";
        if (char.IsUpper(text[0])) return IsTypePosition(src, m.Index, m.Index + m.Length) ? "type" : null;
        return "keyword";
    }

    // A Capitalized identifier is a *type* only in obvious type position — followed by ?, [, or
    // <T> (not </close); inside generic args (preceded by < or ,); or an attribute name right
    // after "[" ([Parameter]). Otherwise it's a plain member/variable name (Title, ChildContent).
    private static bool IsTypePosition(string src, int start, int end)
    {
        var before = start > 0 ? src[start - 1] : '\0';
        var after = end < src.Length ? src[end] : '\0';
        return after == '?' || after == '['
            || (after == '<' && (end + 1 >= src.Length || src[end + 1] != '/'))
            || before == '<' || before == ',' || before == '[';
    }
}
