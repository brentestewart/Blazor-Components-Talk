namespace BlazorDeck.Services;

/// <summary>
/// The deck's state container (concept #13): current slide index + theme, shared across
/// the stage, the nav bar, and the position counter. Raises <see cref="OnChange"/> so
/// subscribers can re-render.
/// </summary>
public class DeckState
{
    private int _themeIndex;

    public int Index { get; private set; }
    public int Count { get; private set; }
    public DeckTheme Theme => DeckThemes.All[_themeIndex];
    public bool ShowOverview { get; private set; }

    public event Action? OnChange;

    public void SetCount(int count) => Count = count;

    public void ToggleOverview()
    {
        ShowOverview = !ShowOverview;
        Notify();
    }

    public void CloseOverview()
    {
        if (ShowOverview) { ShowOverview = false; Notify(); }
    }

    public void Next()
    {
        if (Index < Count - 1) { Index++; Notify(); }
    }

    public void Prev()
    {
        if (Index > 0) { Index--; Notify(); }
    }

    public void Goto(int index)
    {
        if (index >= 0 && index < Count && index != Index) { Index = index; Notify(); }
    }

    public void CycleTheme()
    {
        _themeIndex = (_themeIndex + 1) % DeckThemes.All.Count;
        Notify();
    }

    private void Notify() => OnChange?.Invoke();
}
