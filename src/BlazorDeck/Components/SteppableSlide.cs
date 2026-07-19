using BlazorDeck.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorDeck.Components;

/// <summary>
/// Base for a slide with build-steps. Exposes the deck's current <see cref="Step"/> and
/// re-renders when it changes; a subclass just declares <see cref="StepCount"/> and reads
/// <see cref="Step"/> to decide what to reveal / focus.
/// </summary>
public abstract class SteppableSlide : ComponentBase, ISteppable, IDisposable
{
    [Inject] protected DeckState State { get; set; } = default!;

    /// <summary>Total steps, including the initial state (1 == no fragments).</summary>
    public abstract int StepCount { get; }

    /// <summary>The deck's current step within this slide (0-based).</summary>
    protected int Step => State.Step;

    protected override void OnInitialized() => State.OnChange += StateHasChanged;

    public void Dispose() => State.OnChange -= StateHasChanged;
}
