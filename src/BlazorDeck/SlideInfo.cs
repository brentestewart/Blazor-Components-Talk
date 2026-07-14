namespace BlazorDeck;

/// <summary>
/// One entry in the deck: the slide component's type, a title used by the overview
/// (#20) and progress, and how the slide enters. The ordered list of these is the
/// talk's table of contents.
/// </summary>
public record SlideInfo(Type Type, string Title, SlideTransition Transition = SlideTransition.Inherit);
