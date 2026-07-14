namespace BlazorDeck;

/// <summary>
/// Deck-wide theme, flowed to every slide via a cascading parameter (concept #11)
/// and to the DOM as the <c>--accent</c> custom property.
/// </summary>
public record DeckTheme(string Accent, string Name)
{
    public static DeckTheme Indigo { get; } = new("#7c8cff", "Indigo");
    public static DeckTheme Amber { get; } = new("#f5a623", "Amber");
}
