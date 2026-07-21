# MVVM starter for Blazor

A small, self-contained MVVM base for Blazor components. Two files are the whole pattern; the
`Example/` folder shows them in use. Copy `ViewModelBase.cs` + `ViewModelComponentBase.cs` into your
project and go — they're framework-only (no third-party UI dependency).

This is stripped down from a production version. In a real app the base grows page-title, auth/role
gating, logging, and app-specific concerns — but the reusable core is just what's here.

## The idea

- **View model** (`ViewModelBase`) holds the state and logic. It has **no Blazor types**, so it's a
  plain class you can unit-test directly.
- **Component** (`ViewModelComponentBase<TViewModel>`) is almost all markup. It injects the VM,
  forwards the lifecycle to it, and lets the VM ask for a re-render.

```razor
@inherits ViewModelComponentBase<MyViewModel>

<h3>@Vm.Title</h3>
<button @onclick="Vm.DoSomething" disabled="@Vm.IsProcessing">Go</button>
```

## Three things worth pointing out

1. **Thread-safe re-render.** The component wires the VM's `StateHasChanged` *through* `InvokeAsync`:

   ```csharp
   ViewModel.StateHasChanged = () => InvokeAsync(StateHasChanged);
   ```

   So a VM that re-renders after an `await`, a timer, or a background event is automatically
   marshaled onto the renderer's thread — no "not associated with the Dispatcher" exceptions.

2. **It cleans up after itself.** Those delegates capture the component (`this`), so
   `ViewModelComponentBase` is `IDisposable`: on dispose it unwires them *and* calls the VM's
   `OnDisposed()`, so a view model can unsubscribe events or dispose timers it set up in
   `OnInitializedAsync` — no leak.

3. **One wrapper for busy + errors.** Route VM work through `Process(...)`:

   ```csharp
   private Task Load() => Process(
       async () => People = await _service.GetPeopleAsync(),
       busyMessage: "Loading people…");
   ```

   You get `IsProcessing` (spinner), `ErrorMessage` (caught exceptions), and a re-render on
   completion — without repeating the try/finally in every method. It ignores
   `OperationCanceledException` (a component going away isn't an error) and routes real failures
   through an overridable `OnError(...)`, so you can log them instead of losing them.

## Wiring it up

Register the VM (usually transient — one per component) and any services it needs:

```csharp
builder.Services.AddMvvmSample();          // see MvvmSampleServiceCollectionExtensions
// or, per view model:
builder.Services.AddTransient<MyViewModel>();
```

## A note on `INotifyPropertyChanged`

There isn't any — and that's deliberate. Blazor re-renders a component wholesale on
`StateHasChanged`, so you don't need per-property change notification the way WPF/MAUI MVVM does.
The VM just sets its state and calls `StateHasChanged` (directly, or via `Process`).
