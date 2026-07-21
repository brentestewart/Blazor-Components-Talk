using System.Runtime.CompilerServices;

namespace MvvmSample;

/// <summary>
/// Base class for a view model — the testable logic and state behind a component. The component
/// (<see cref="ViewModelComponentBase{TViewModel}"/>) injects the VM, forwards Blazor's lifecycle
/// to it, and wires <see cref="StateHasChanged"/> / <see cref="InvokeAsync"/> so the VM can request
/// a re-render safely, even from a background thread.
/// </summary>
public abstract class ViewModelBase
{
    /// <summary>Ask the owning component to re-render. Wired by the component and marshaled to the
    /// renderer's thread, so it is safe to call from any thread. No-op until wired.</summary>
    public Action StateHasChanged { get; set; } = () => { };

    /// <summary>Run a callback on the renderer's synchronization context. Wired by the component.</summary>
    public Func<Func<Task>, Task> InvokeAsync { get; set; } = f => f();

    /// <summary>Raised when <see cref="IsProcessing"/> flips, so the component can re-render its busy UI.</summary>
    public Action<bool> IsProcessingChanged { get; set; } = _ => { };

    /// <summary>True while a <see cref="Process"/> call is running.</summary>
    public bool IsProcessing { get; private set; }

    /// <summary>Message to show while busy.</summary>
    public string ProcessingMessage { get; private set; } = "Loading...";

    /// <summary>Set when a <see cref="Process"/> call throws; null when clear.</summary>
    public string? ErrorMessage { get; protected set; }

    /// <summary>Clear the current error.</summary>
    public void ClearError() => ErrorMessage = null;

    // Lifecycle hooks the component forwards to. Override the ones you need — the VM never
    // touches Blazor's ComponentBase directly, which is what keeps it unit-testable.
    public virtual Task OnInitializedAsync() => Task.CompletedTask;
    public virtual Task OnParametersSetAsync() => Task.CompletedTask;
    public virtual Task OnAfterFirstRenderAsync() => Task.CompletedTask;

    /// <summary>Called by the component when it is disposed. Override to clean up anything set up in
    /// the hooks above — unsubscribe from service events, dispose timers, cancel work — so the VM
    /// (and whatever it holds onto) can be collected. The symmetric "out" to OnInitializedAsync.</summary>
    public virtual void OnDisposed() { }

    /// <summary>
    /// Wrap an async action with busy-state, error handling, and a re-render. Route every VM
    /// method that does work through here: the UI gets a spinner while it runs, exceptions are
    /// captured into <see cref="ErrorMessage"/>, and the component re-renders when it finishes —
    /// without repeating that boilerplate in every method. <paramref name="caller"/> is filled in
    /// automatically so a failure names the method it came from.
    /// </summary>
    protected async Task Process(
        Func<Task> action,
        string busyMessage = "Loading...",
        [CallerMemberName] string caller = "")
    {
        try
        {
            ErrorMessage = null;
            ProcessingMessage = busyMessage;
            IsProcessing = true;
            IsProcessingChanged(true);
            await action();
        }
        catch (OperationCanceledException)
        {
            // The work was cancelled — e.g. the component is going away, or a newer request
            // superseded this one. That isn't a failure to show the user, so leave it be.
        }
        catch (Exception ex)
        {
            OnError(ex, caller);
        }
        finally
        {
            IsProcessing = false;
            IsProcessingChanged(false);
            StateHasChanged();
        }
    }

    /// <summary>
    /// Called when <see cref="Process"/> catches a non-cancellation exception. The default puts a
    /// short message in <see cref="ErrorMessage"/>; override it to also log the exception (call
    /// <c>base.OnError(...)</c> to keep the message) so real failures aren't invisible.
    /// </summary>
    protected virtual void OnError(Exception ex, string caller) =>
        ErrorMessage = $"{caller} failed: {ex.Message}";
}
