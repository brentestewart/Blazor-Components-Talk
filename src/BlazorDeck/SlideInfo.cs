namespace BlazorDeck;

/// <summary>
/// One entry in the deck: the slide component's type, a title used by the overview
/// (#20) and progress, and how the slide enters. The ordered list of these is the
/// talk's table of contents.
/// </summary>
public record SlideInfo(Type Type, string Title, SlideTransition Transition = SlideTransition.Inherit)
{
    /// <summary>Label shown in the corner meta-tag (&lt;Tag /&gt;). Defaults to the component
    /// type name when null — override it where the raw type name reads poorly.</summary>
    public string? Tag { get; init; }
}
