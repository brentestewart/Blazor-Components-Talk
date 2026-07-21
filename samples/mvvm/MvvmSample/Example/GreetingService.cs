namespace MvvmSample.Example;

/// <summary>A fake service — waits a moment, then returns some names. The delay makes the VM's
/// busy state visible so the <c>Process(...)</c> spinner has something to show.</summary>
public sealed class GreetingService : IGreetingService
{
    private static readonly string[] People = ["Ada", "Grace", "Alan", "Edsger", "Barbara"];

    public async Task<IReadOnlyList<string>> GetPeopleAsync()
    {
        await Task.Delay(600);   // simulate a slow call
        return People;
    }
}
