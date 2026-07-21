# Talk wishlist — things to include

Running list of content/ideas Brent wants worked into the deck. Newest additions at the bottom.

- [ ] **Static SSR — how a component behaves with no interactivity.** The counterpart to the
      whole interactive story. Under static SSR: `OnInitialized`/`OnParametersSet` run but there's
      **no re-render** (`StateHasChanged` is a no-op, `OnAfterRender` never fires); `@onclick` and
      other handlers are inert → interaction happens via **form POST / enhanced form handling**;
      `[StreamRendering]` changes how async content flushes; enhanced navigation patches the DOM
      instead of reloading. Slot it right after A5 (four render modes) so the audience knows which
      world each later lesson assumes. It's a component-behavior lesson, not plumbing — the same
      components, different rules. Ties back to B6 lifecycle, C6 `StateHasChanged`, B5 `EventCallback`.

- [ ] **PersistentComponentState.** Solves the prerender→hydrate double-render that the deck's own
      **Auto/WASM hosting choice** creates (the classic double-fetch flicker). The deck's own load is
      the demo, so it justifies a hosting decision already made. Fits under lifecycle, not routing.

- [ ] **ErrorBoundary.** Wrap `SlideHost` in `<ErrorBoundary>` — a slide that throws renders a
      fallback instead of killing the whole presentation. Live, memorable, and on-brand for the
      conference risk plan ("what happens when slide 30 has a bug mid-talk?"). It's a component you
      wrap around other components.

- [ ] **Forms & `InputBase<T>`.** Weakest meta-fit (a deck has no forms) but the #1 thing a mixed
      audience actually ships. `InputBase<T>` is component authoring at its core (inheritance +
      `EditContext` cascade), so it earns a place on component grounds. `PlaygroundSlide` could grow
      a validated parameter editor to carry it.

- [ ] **Per-component `@rendermode`.** A5/A6 teach render modes at the *page* level; .NET 8+ lets you
      drop a single `InteractiveServer` island into the WASM spine. One Server-rendered slide inside
      the WASM deck = the whole concept on one screen. Extends A5/A6 from page-level to instance-level.

- [ ] **Show the different ways to bind.** Slide 13 (B4Binding) covers the core two-way
      `@bind` + `@bind:event`. Consider showing the other modifiers: `@bind:format`,
      `@bind:after` (.NET 7+), `@bind:get`/`@bind:set`, and `@bind-Value` (binding a
      component's own `Value`/`ValueChanged`). Open question: keep slide 13 focused and add a
      compact "modifiers" strip, or give it its own dedicated slide.
