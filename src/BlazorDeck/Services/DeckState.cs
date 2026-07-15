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
    public int ThemeIndex => _themeIndex;
    public bool ShowOverview { get; private set; }
    public bool ShowSettings { get; private set; }

    /// <summary>Fallback transition for slides that don't set their own. Adjustable in settings.</summary>
    public SlideTransition DefaultTransition { get; private set; } = SlideTransition.Fade;

    /// <summary>Whether the last move advanced (true) or went back (false) — drives directional transitions.</summary>
    public bool LastForward { get; private set; } = true;

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
        if (Index < Count - 1) { LastForward = true; Index++; Notify(); }
    }

    public void Prev()
    {
        if (Index > 0) { LastForward = false; Index--; Notify(); }
    }

    public void Goto(int index)
    {
        if (index >= 0 && index < Count && index != Index)
        {
            LastForward = index > Index;
            Index = index;
            Notify();
        }
    }

    public void CycleTheme()
    {
        _themeIndex = (_themeIndex + 1) % DeckThemes.All.Count;
        Notify();
    }

    public void SetTheme(int index)
    {
        if (index >= 0 && index < DeckThemes.All.Count && index != _themeIndex)
        {
            _themeIndex = index;
            Notify();
        }
    }

    public void SetDefaultTransition(SlideTransition transition)
    {
        if (DefaultTransition != transition)
        {
            DefaultTransition = transition;
            Notify();
        }
    }

    public void ToggleSettings()
    {
        ShowSettings = !ShowSettings;
        Notify();
    }

    public void CloseSettings()
    {
        if (ShowSettings) { ShowSettings = false; Notify(); }
    }

    private void Notify() => OnChange?.Invoke();
}
