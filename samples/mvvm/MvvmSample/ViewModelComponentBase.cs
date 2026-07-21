using Microsoft.AspNetCore.Components;

namespace MvvmSample;

/// <summary>
/// Base component that binds to a view model of type <typeparamref name="TViewModel"/>. It injects
/// the VM, forwards Blazor's lifecycle to it, gives it a thread-safe way to re-render, and unwires
/// everything on dispose so nothing leaks. A page or component uses it with
/// <c>@inherits ViewModelComponentBase&lt;MyViewModel&gt;</c> and then reads state off <c>Vm</c>.
/// </summary>
public abstract class ViewModelComponentBase<TViewModel> : ComponentBase, IDisposable
    where TViewModel : ViewModelBase
{
    /// <summary>The view model, resolved from DI. Register it (usually transient) in Program.cs.</summary>
    [Inject] public TViewModel ViewModel { get; set; } = default!;

    /// <summary>Shorthand for the injected view model, handy in markup: <c>@Vm.Something</c>.</summary>
    protected TViewModel Vm => ViewModel;

    protected override async Task OnInitializedAsync()
    {
        WireViewModel();
        await ViewModel.OnInitializedAsync();
    }

    protected override Task OnParametersSetAsync() => ViewModel.OnParametersSetAsync();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await ViewModel.OnAfterFirstRenderAsync();
    }

    // Hand the VM thread-safe delegates. StateHasChanged is routed THROUGH InvokeAsync, so a VM
    // that re-renders from a background continuation (after an await, a timer, a service event) is
    // automatically marshaled back onto the renderer's thread — no "current thread is not
    // associated with the Dispatcher" exceptions.
    private void WireViewModel()
    {
        ViewModel.StateHasChanged = () => InvokeAsync(StateHasChanged);
        ViewModel.InvokeAsync = InvokeAsync;
        ViewModel.IsProcessingChanged = _ => InvokeAsync(StateHasChanged);
    }

    // Break the component <-> VM link on dispose. The delegates above capture `this`, so a
    // longer-lived VM (or one referenced elsewhere) would otherwise keep this component alive.
    // Reset them to the harmless no-ops the VM started with.
    public virtual void Dispose()
    {
        ViewModel.OnDisposed();   // let the VM clean up what it set up in its lifecycle hooks
        ViewModel.StateHasChanged = () => { };
        ViewModel.InvokeAsync = f => f();
        ViewModel.IsProcessingChanged = _ => { };
        GC.SuppressFinalize(this);
    }
}
