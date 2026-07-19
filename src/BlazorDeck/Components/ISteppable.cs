namespace BlazorDeck.Components;

/// <summary>
/// A slide that has intermediate build-steps (fragments). The deck reads <see cref="StepCount"/>
/// off the rendered slide instance (via DynamicComponent.Instance) to know how many times →
/// advances within the slide before moving on. A plain slide has one step and never opts in.
/// </summary>
public interface ISteppable
{
    /// <summary>Total steps on this slide, including the initial state (so 1 == no fragments).</summary>
    int StepCount { get; }
}
