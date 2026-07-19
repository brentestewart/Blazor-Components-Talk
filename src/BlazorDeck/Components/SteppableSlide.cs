using Microsoft.AspNetCore.Components;

namespace BlazorDeck.Components;

/// <summary>
/// Base for a slide with build-steps. The deck cascades the current step <em>per transition
/// layer</em> (so an outgoing slide keeps its step while it animates out), and a subclass just
/// declares <see cref="StepCount"/> and reads <see cref="Step"/> to decide what to reveal / focus.
/// Changing the cascaded step re-renders the slide automatically.
/// </summary>
public abstract class SteppableSlide : ComponentBase, ISteppable
{
    /// <summary>The current step within this slide (0-based), cascaded by the deck.</summary>
    [CascadingParameter(Name = "Step")] protected int Step { get; set; }

    /// <summary>Total steps, including the initial state (1 == no fragments).</summary>
    public abstract int StepCount { get; }
}
