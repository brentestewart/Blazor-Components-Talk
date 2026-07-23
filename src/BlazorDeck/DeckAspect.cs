namespace BlazorDeck;

/// <summary>
/// The shape of the deck's design canvas. The canvas is always 1280 wide — only its height
/// changes — so every horizontal <c>cqw</c> unit (fonts, padding, the left rail) stays identical
/// whichever shape is picked, and a taller canvas simply gives slides more vertical room.
/// </summary>
public enum DeckAspect
{
    /// <summary>Match the screen exactly, so no letterbox bars appear on any display.</summary>
    Auto,
    Wide,       // 16:9  — 1280x720, most projectors and external monitors
    Widescreen, // 16:10 — 1280x800, laptop panels and WXGA projectors
    Classic,    // 4:3   — 1280x960, older projectors
}

/// <summary>Presentation-facing helpers for the canvas shapes.</summary>
public static class DeckAspects
{
    /// <summary>The shapes a presenter can pick in settings, in display order.</summary>
    public static IReadOnlyList<DeckAspect> Selectable { get; } = new[]
    {
        DeckAspect.Auto,
        DeckAspect.Wide,
        DeckAspect.Widescreen,
        DeckAspect.Classic,
    };

    /// <summary>A short, human-friendly label for the settings buttons.</summary>
    public static string Label(this DeckAspect aspect) => aspect switch
    {
        DeckAspect.Wide => "16:9",
        DeckAspect.Widescreen => "16:10",
        DeckAspect.Classic => "4:3",
        _ => "Auto",
    };

    /// <summary>
    /// Width ÷ height for a fixed shape, or <c>null</c> for <see cref="DeckAspect.Auto"/> —
    /// which has no ratio of its own, because JS measures the live display instead.
    /// </summary>
    public static double? Ratio(this DeckAspect aspect) => aspect switch
    {
        DeckAspect.Wide => 16d / 9d,
        DeckAspect.Widescreen => 16d / 10d,
        DeckAspect.Classic => 4d / 3d,
        _ => null,
    };
}
