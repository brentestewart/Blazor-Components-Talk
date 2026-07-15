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
