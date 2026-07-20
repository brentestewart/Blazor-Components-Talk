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
    // keywords inside them aren't separately coloured. Comments cover both // (C#/Razor) and
    // single-line /* */ (CSS) — tokenising is per line, so block comments don't span lines here.
    private const string Pattern =
        $@"(//[^\n]*|/\*.*?\*/)|(""[^""]*""|'[^']*')|(@(?:{Directives})\b)|(@)|((?<!\w)</?[A-Za-z][\w.-]*)|\b(?:{Keywords})\b|\b[A-Z][A-Za-z0-9_]*\b|\b\d[\w.]*\b";

    [GeneratedRegex(Pattern)]
    private static partial Regex Token();

    /// <summary>Tokenise each line of <paramref name="code"/> into coloured tokens. CSS has its
    /// own grammar (selectors/properties/values), so it takes a separate path; everything else
    /// uses the C#/Razor tokenizer.</summary>
    public static IReadOnlyList<IReadOnlyList<CodeToken>> Tokenize(string code, string language = "razor")
    {
        if (string.Equals(language, "css", StringComparison.OrdinalIgnoreCase))
        {
            return TokenizeCss(code);
        }

        var lines = new List<IReadOnlyList<CodeToken>>();
        foreach (var line in code.ReplaceLineEndings("\n").Split('\n'))   // normalise CRLF/CR first
        {
            lines.Add(TokenizeLine(line));
        }
        return lines;
    }

    // A small CSS tokenizer. Context matters — the same identifier is a selector, a property, or a
    // value depending on where it sits — so this tracks brace depth and the ':' inside a rule
    // rather than using one stateless regex. Tuned for the snippets on screen, not a full grammar.
    private static IReadOnlyList<IReadOnlyList<CodeToken>> TokenizeCss(string code)
    {
        var result = new List<IReadOnlyList<CodeToken>>();
        var depth = 0;          // 0 = selector context; >0 = inside a { } declaration block
        var inValue = false;    // inside a declaration, past the ':' (so: a value, not a property)
        var inComment = false;  // a /* */ comment left open on a previous line

        foreach (var line in code.ReplaceLineEndings("\n").Split('\n'))
        {
            var toks = new List<CodeToken>();
            var i = 0;
            while (i < line.Length)
            {
                var c = line[i];

                if (inComment || (c == '/' && i + 1 < line.Length && line[i + 1] == '*'))
                {
                    var from = inComment ? i : i + 2;
                    var end = line.IndexOf("*/", from, StringComparison.Ordinal);
                    var stop = end < 0 ? line.Length : end + 2;
                    toks.Add(new CodeToken(line[i..stop], "comment"));
                    inComment = end < 0;
                    i = stop;
                    continue;
                }

                if (c is '"' or '\'')
                {
                    var j = i + 1;
                    while (j < line.Length && line[j] != c) j++;
                    if (j < line.Length) j++;   // include the closing quote
                    toks.Add(new CodeToken(line[i..j], "string"));
                    i = j;
                    continue;
                }

                if (char.IsWhiteSpace(c))
                {
                    var j = i;
                    while (j < line.Length && char.IsWhiteSpace(line[j])) j++;
                    toks.Add(new CodeToken(line[i..j], null));
                    i = j;
                    continue;
                }

                switch (c)
                {
                    case '{': depth++; inValue = false; toks.Add(new CodeToken("{", null)); i++; continue;
                    case '}': if (depth > 0) depth--; inValue = false; toks.Add(new CodeToken("}", null)); i++; continue;
                    case ';': inValue = false; toks.Add(new CodeToken(";", null)); i++; continue;
                }

                if (c == ':')
                {
                    if (depth > 0)   // property : value separator
                    {
                        inValue = true;
                        toks.Add(new CodeToken(":", null));
                        i++;
                        continue;
                    }
                    var j = i;       // a pseudo in selector context — ::deep, :hover
                    while (j < line.Length && line[j] == ':') j++;
                    while (j < line.Length && (char.IsLetterOrDigit(line[j]) || line[j] is '-' or '_')) j++;
                    toks.Add(new CodeToken(line[i..j], "directive"));
                    i = j;
                    continue;
                }

                if (c == '@')   // at-rule: @media, @keyframes
                {
                    var j = i + 1;
                    while (j < line.Length && (char.IsLetterOrDigit(line[j]) || line[j] == '-')) j++;
                    toks.Add(new CodeToken(line[i..j], "directive"));
                    i = j;
                    continue;
                }

                if (c == '#')   // #id selector, or a #hex colour value
                {
                    var j = i + 1;
                    while (j < line.Length && Uri.IsHexDigit(line[j])) j++;
                    toks.Add(new CodeToken(line[i..j], depth == 0 ? "tag" : "number"));
                    i = j;
                    continue;
                }

                if (char.IsDigit(c) || (c == '.' && i + 1 < line.Length && char.IsDigit(line[i + 1])))
                {
                    var j = i;      // number with an optional unit (1rem, 100%, 0.9)
                    while (j < line.Length && (char.IsLetterOrDigit(line[j]) || line[j] is '.' or '%')) j++;
                    toks.Add(new CodeToken(line[i..j], "number"));
                    i = j;
                    continue;
                }

                if (c == '.' && depth == 0)   // .class selector
                {
                    var j = i + 1;
                    while (j < line.Length && (char.IsLetterOrDigit(line[j]) || line[j] is '-' or '_')) j++;
                    toks.Add(new CodeToken(line[i..j], "tag"));
                    i = j;
                    continue;
                }

                if (char.IsLetter(c) || c is '-' or '_')
                {
                    var j = i;
                    while (j < line.Length && (char.IsLetterOrDigit(line[j]) || line[j] is '-' or '_')) j++;
                    // Same word, different job by position: selector element / property / value.
                    var kind = depth == 0 ? "tag" : inValue ? "type" : "keyword";
                    toks.Add(new CodeToken(line[i..j], kind));
                    i = j;
                    continue;
                }

                toks.Add(new CodeToken(c.ToString(), null));   // punctuation: > + ~ , ( ) *
                i++;
            }
            result.Add(toks);
        }
        return result;
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
