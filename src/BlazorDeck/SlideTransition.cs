namespace BlazorDeck;

/// <summary>
/// How a slide enters. Set per-slide on <see cref="SlideInfo"/>; slides left as
/// <see cref="Inherit"/> fall back to the deck's default transition.
/// </summary>
public enum SlideTransition
{
    Inherit,
    None,
    Fade,
    Slide,
    SlideVertical,
    Scale,
}

/// <summary>Presentation-facing helpers for the deck-level transitions.</summary>
public static class SlideTransitions
{
    /// <summary>
    /// The transitions a presenter can pick or cycle through as the deck default,
    /// in cycle order. Excludes <see cref="SlideTransition.Inherit"/>, which is a
    /// per-slide "use the deck default" marker, not a default in its own right.
    /// </summary>
    public static IReadOnlyList<SlideTransition> Selectable { get; } = new[]
    {
        SlideTransition.Fade,
        SlideTransition.Slide,
        SlideTransition.SlideVertical,
        SlideTransition.Scale,
        SlideTransition.None,
    };

    /// <summary>A short, human-friendly label (e.g. for the nav bar and settings buttons).</summary>
    public static string Label(this SlideTransition transition) => transition switch
    {
        SlideTransition.SlideVertical => "Vertical",
        _ => transition.ToString(),
    };
}
