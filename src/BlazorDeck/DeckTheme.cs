namespace BlazorDeck;

/// <summary>
/// A deck theme preset: a name, light/dark mode, and the primary/accent colour.
/// These feed MudBlazor's <c>MudTheme</c> (surfaces come from the MudBlazor palette),
/// so the deck and any MudBlazor components share one source of truth.
/// </summary>
public record DeckTheme(string Name, string AccentName, bool IsDark, string Accent);

/// <summary>The built-in presets the deck cycles through (dark first, then light).</summary>
public static class DeckThemes
{
    public static DeckTheme DarkIndigo { get; } = new("Dark · Indigo", "Indigo", true, "#7c8cff");
    public static DeckTheme DarkAmber { get; } = new("Dark · Amber", "Amber", true, "#f5a623");
    public static DeckTheme LightIndigo { get; } = new("Light · Indigo", "Indigo", false, "#4f5bd5");
    public static DeckTheme LightAmber { get; } = new("Light · Amber", "Amber", false, "#b26a00");

    public static IReadOnlyList<DeckTheme> All { get; } =
        new[] { DarkIndigo, DarkAmber, LightIndigo, LightAmber };
}
