namespace MvvmSample.Example;

/// <summary>
/// A tiny view model showing the pattern: it takes a dependency (constructor-injected, like any
/// service), exposes state the component renders (<see cref="People"/>), loads on init, and offers
/// an action (<see cref="Refresh"/>). Both go through <c>Process(...)</c>, so busy-state, error
/// handling, and the re-render come for free. Notice there is no Blazor type in here — that is what
/// makes it plain-unit-testable.
/// </summary>
public class SampleViewModel(IGreetingService greetings) : ViewModelBase
{
    public IReadOnlyList<string> People { get; private set; } = [];

    public override Task OnInitializedAsync() => Load();

    public Task Refresh() => Load();

    private Task Load() => Process(
        async () => People = await greetings.GetPeopleAsync(),
        busyMessage: "Loading people…");
}
