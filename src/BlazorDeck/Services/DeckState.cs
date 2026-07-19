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

    /// <summary>Current build-step within the slide (0-based). Advancing consumes steps before slides.</summary>
    public int Step { get; private set; }

    /// <summary>How many steps the current slide has (1 == no fragments). Set by the deck per slide.</summary>
    public int StepCount { get; private set; } = 1;

    // When stepping backward across a slide boundary, land on the new slide's LAST step —
    // but we only learn its step count once it renders, so remember the intent until then.
    private bool _landOnLastStep;

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
        if (Step < StepCount - 1) { Step++; Notify(); return; }   // advance within the slide first
        if (Index < Count - 1) { LastForward = true; Index++; ResetSteps(); Notify(); }
    }

    public void Prev()
    {
        if (Step > 0) { Step--; Notify(); return; }               // step back within the slide first
        if (Index > 0) { LastForward = false; Index--; ResetSteps(landOnLast: true); Notify(); }
    }

    public void Goto(int index)
    {
        if (index >= 0 && index < Count && index != Index)
        {
            LastForward = index > Index;
            Index = index;
            ResetSteps();
            Notify();
        }
    }

    private void ResetSteps(bool landOnLast = false)
    {
        Step = 0;
        StepCount = 1;                 // until the new slide reports its own count
        _landOnLastStep = landOnLast;
    }

    /// <summary>The deck reports the current slide's step count once it has rendered.</summary>
    public void SetStepCount(int count)
    {
        count = count < 1 ? 1 : count;
        var step = _landOnLastStep ? count - 1 : Math.Min(Step, count - 1);
        if (count == StepCount && step == Step) return;   // no change — avoid a render loop
        StepCount = count;
        Step = step;
        _landOnLastStep = false;
        Notify();
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

    /// <summary>Advance to the next selectable transition, wrapping around (mirrors <see cref="CycleTheme"/>).</summary>
    public void CycleTransition()
    {
        var options = SlideTransitions.Selectable;
        var current = -1;
        for (var i = 0; i < options.Count; i++)
        {
            if (options[i] == DefaultTransition) { current = i; break; }
        }
        SetDefaultTransition(options[(current + 1) % options.Count]);
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
