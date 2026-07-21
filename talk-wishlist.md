# Talk wishlist — things to include

Running list of content/ideas Brent wants worked into the deck. Newest additions at the bottom.

- [x] **Static SSR — how a component behaves with no interactivity.** Shipped as **slide 7
      (`A6bStaticSsr`)**. The deck is interactive so it can't be static from the inside — instead a
      genuinely static page (`/demo/static-ssr`, a `RazorComponentResult` served outside the deck's
      router) is embedded live in an iframe: inert `@onclick`, working form POST with an antiforgery
      token. (Future: could still show `[StreamRendering]` / enhanced-nav behaviors; not covered yet.)

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

- [x] **Per-component `@rendermode`.** Shipped as **slide 8 (`A6cRenderModes`)**. A static host page
      (`/demo/render-modes`, served outside the deck's router) carries an `InteractiveServer` island
      and an `InteractiveWebAssembly` island side by side, each reporting its own mode — the whole
      instance-level concept on one screen, embedded live via iframe. (Couldn't be an island *inside*
      the deck itself: the deck's router is globally interactive, so a static host had to be carved out.)

- [ ] **Show the different ways to bind.** Slide 13 (B4Binding) covers the core two-way
      `@bind` + `@bind:event`. Consider showing the other modifiers: `@bind:format`,
      `@bind:after` (.NET 7+), `@bind:get`/`@bind:set`, and `@bind-Value` (binding a
      component's own `Value`/`ValueChanged`). Open question: keep slide 13 focused and add a
      compact "modifiers" strip, or give it its own dedicated slide.
