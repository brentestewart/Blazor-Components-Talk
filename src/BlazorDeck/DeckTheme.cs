namespace BlazorDeck;

/// <summary>
/// A full deck palette, flowed to every slide via a cascading parameter (concept #11)
/// and to the DOM as CSS custom properties on the deck root.
/// </summary>
public record DeckTheme(
    string Name,
    string Accent,
    string Bg,        // page background
    string Fg,        // primary text
    string Surface,   // raised panels / cards
    string Surface2,  // deeper wells: code blocks, nav bar
    string Border,
    string CodeComment);

/// <summary>The built-in themes the deck cycles through (dark first, then light).</summary>
public static class DeckThemes
{
    public static DeckTheme DarkIndigo { get; } = new(
        "Dark · Indigo", "#7c8cff", "#0f1115", "#e8e8ea", "#12141a", "#0b0c10", "#23262f", "#6a9955");

    public static DeckTheme DarkAmber { get; } = new(
        "Dark · Amber", "#f5a623", "#0f1115", "#e8e8ea", "#12141a", "#0b0c10", "#23262f", "#6a9955");

    public static DeckTheme LightIndigo { get; } = new(
        "Light · Indigo", "#4f5bd5", "#f6f7f9", "#1b1d23", "#ffffff", "#eef1f4", "#dce1e8", "#4d7a2e");

    public static DeckTheme LightAmber { get; } = new(
        "Light · Amber", "#b26a00", "#f6f7f9", "#1b1d23", "#ffffff", "#eef1f4", "#dce1e8", "#4d7a2e");

    public static IReadOnlyList<DeckTheme> All { get; } =
        new[] { DarkIndigo, DarkAmber, LightIndigo, LightAmber };
}
