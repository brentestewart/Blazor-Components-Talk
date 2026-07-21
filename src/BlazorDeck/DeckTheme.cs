using MudBlazor;

namespace BlazorDeck;

/// <summary>
/// A deck theme preset: a display name, its accent's name, the mode (dark/light), and the MudBlazor
/// theme it renders with. Two presets share one <see cref="Mud"/> theme per accent; the mode is
/// chosen via the provider's IsDarkMode, so a theme's light and dark palettes both come from it.
/// </summary>
public record DeckTheme(string Name, string AccentName, bool IsDark, MudTheme Mud);

/// <summary>
/// The built-in themes and the presets that select them. Each theme fully defines its own light and
/// dark palettes — they share colour values today, but each theme is defined and edited on its own.
/// </summary>
public static class DeckThemes
{
    // One full MudTheme per accent. Each carries a complete light and dark palette; the deck picks
    // which one renders via IsDarkMode. Swapping the whole theme is the accent axis; IsDarkMode is
    // the mode axis — no per-render mutation.
    private static readonly MudTheme Indigo = new()
    {
        PaletteDark = new PaletteDark
        {
            Primary = "#7c8cff",
            Background = "#0f1115",
            Surface = "#12141a",
            BackgroundGray = "#0b0c10",
            TextPrimary = "#e8e8ea",
            TextSecondary = "#9aa0ac",
            LinesDefault = "#23262f",
            AppbarBackground = "#0a0b0e",
            DrawerBackground = "#12141a",
        },
        PaletteLight = new PaletteLight
        {
            Primary = "#4f5bd5",
            Background = "#f6f7f9",
            Surface = "#ffffff",
            BackgroundGray = "#eef1f4",
            TextPrimary = "#1b1d23",
            TextSecondary = "#5b616b",
            LinesDefault = "#dce1e8",
            AppbarBackground = "#ffffff",
            DrawerBackground = "#ffffff",
        },
    };

    private static readonly MudTheme Amber = new()
    {
        PaletteDark = new PaletteDark
        {
            Primary = "#f5a623",
            Background = "#0f1115",
            Surface = "#12141a",
            BackgroundGray = "#0b0c10",
            TextPrimary = "#e8e8ea",
            TextSecondary = "#9aa0ac",
            LinesDefault = "#23262f",
            AppbarBackground = "#0a0b0e",
            DrawerBackground = "#12141a",
        },
        PaletteLight = new PaletteLight
        {
            Primary = "#b26a00",
            Background = "#f6f7f9",
            Surface = "#ffffff",
            BackgroundGray = "#eef1f4",
            TextPrimary = "#1b1d23",
            TextSecondary = "#5b616b",
            LinesDefault = "#dce1e8",
            AppbarBackground = "#ffffff",
            DrawerBackground = "#ffffff",
        },
    };

    public static DeckTheme DarkIndigo { get; } = new("Dark · Indigo", "Indigo", true, Indigo);
    public static DeckTheme LightIndigo { get; } = new("Light · Indigo", "Indigo", false, Indigo);
    public static DeckTheme DarkAmber { get; } = new("Dark · Amber", "Amber", true, Amber);
    public static DeckTheme LightAmber { get; } = new("Light · Amber", "Amber", false, Amber);

    /// <summary>The presets the deck cycles through (dark first, then light).</summary>
    public static IReadOnlyList<DeckTheme> All { get; } =
        new[] { DarkIndigo, DarkAmber, LightIndigo, LightAmber };
}
