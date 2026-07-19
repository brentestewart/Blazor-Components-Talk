# Unleashing the Power of Blazor Components

**Duration:** 60 minutes (~48 min content · ~7 min live demo · ~5 min Q&A)
**Audience:** Mixed (some new to Blazor, some experienced)
**Style:** Mainly code slides; meta live demo woven throughout + a completeness beat at the end
**Library focus:** MudBlazor (used to build the deck chrome; capstone shows custom slides feeling native beside it)
**Big idea:** The talk is *meta* — the slide deck itself is a Blazor app (`BlazorDeck`), and the
components we teach ARE the deck's own components. Revealed upfront as the hook.
**Hosting:** Unified Blazor Web App (.NET 8+, Microsoft's current direction). Present via
WebAssembly/Auto for reliability (no live circuit to drop mid-talk); keep 1–2 Server-rendered
slides as a live render-modes exhibit. See Slide technology below.

---

## Concept status

- **23 concepts** selected across 7 segments (A–G)
- **Flex items** (skippable live to hit time, placed at segment boundaries): Generic components (#16), bUnit testing (#24). *(Dynamic components #17 is no longer flex — it's the deck's rendering engine, now load-bearing.)*
- **Cut:** Forms & validation · Error boundaries · QuickGrid · PersistentComponentState · Sections

---

## Ordering

### A · Set the stage
1. Cold open — the meta reveal as the hook: "Every slide you'll see today is a Blazor component. This deck is a Blazor Web App, and by the end you'll understand every piece of it." Advance a slide, flip the theme, press `O` for the overview — all live. (See Demo structure below.)
2. Render modes / hosting models — *framing; colors lifecycle, DI lifetimes, JS interop timing later.* Self-demo: show a static-SSR slide, an Interactive Server slide, and the WASM/Auto spine — live, in the deck itself.

### B · Fundamentals
3. Component structure / anatomy
4. Parameters
5. Attribute splatting / `CaptureUnmatchedValues` — *"parameters for everything you didn't declare"*
6. Data binding
7. `EventCallback`
8. Lifecycle methods

### C · Communication & DI
9. Parent/child communication — *reframes params + `EventCallback` as "talking down and up"*
10. Component references (`@ref`)
11. Cascading parameters
12. Dependency injection — *placed here because state containers need it*
13. State container services
14. `StateHasChanged` + `InvokeAsync` threading — *the gotcha, right where it bites*

### D · Advanced composition
15. Templated components / `RenderFragment`
16. Generic components *(flex)* — *pairs with templating*
17. Dynamic components (`DynamicComponent`) — *the deck's engine: `SlideHost` renders the current slide by type*
18. JavaScript interop — *depends on lifecycle (`OnAfterRender`) + render modes*

### E · Performance
19. Rendering / diffing / `@key`
20. Virtualization

### F · Styling, reuse & integration
21. CSS isolation
22. Component libraries (RCL)
23. Integrating with MudBlazor *(capstone)* — *pays off cascading (#11), templating (#15), generics (#16), splatting (#5)*

### G · Payoff & close
24. Testing with bUnit *(flex)*
25. Live demo (the finale — *completeness beat*): step back through the deck and name each component now that the audience understands them — "that's `DeckState`, that's `DynamicComponent`, that's the cascading theme, that's CSS isolation." Payoff line: "You've been inside the demo the whole time — and here's the RCL you can reuse for your own talks."
26. Wrap-up / Q&A

---

## Slide technology — decision

**Build the deck as a unified Blazor Web App** (.NET 8+ template — Microsoft's current direction),
packaged as a reusable framework, `BlazorDeck`. The medium is the message: teaching Blazor
components *with* Blazor components.

- **Hosting decision:** unified Blazor Web App, **presented via WebAssembly/Auto**. Rationale:
  - Aligns with Microsoft's current default (unified render-mode model), *and*
  - Lets the deck **demonstrate render modes on itself** (#2) — a static-SSR slide, an Interactive
    Server slide, and the WASM/Auto spine, live.
  - Presenting via WASM/Auto means **no live SignalR circuit** to drop mid-talk (the big risk with
    Interactive Server: reconnect on laptop sleep or projector display/resolution change).
  - Keep just **1–2 Server-rendered slides** as the teaching exhibit; the spine runs client-side.
- **Risk plan (non-negotiable for a conference):** run it **offline**, pre-warmed; **export a
  static PDF/screenshot deck as a fallback** in case the laptop misbehaves.
- **Effort note:** wiring syntax highlighting (Prism/highlight.js via JS interop) is the one piece
  a tool like Slidev gives for free — budget an afternoon to get Razor/C# crisp.
- Heroes/superhero theme: **dropped.** No domain payload; the deck is the subject.

## Demo structure — meta, revealed upfront

- **Cold open (#1):** state the meta outright — "this deck is a Blazor app; these slides are the
  components." Knowing it doesn't spoil anything; watching it get built *is* the payoff.
- **Snippets throughout:** each segment shows real code from the deck's own components, and you can
  point at the running deck in front of the room.
- **Finale (#25):** the *completeness beat* — walk back through the deck naming each component now
  that the audience understands them, then hand them the `BlazorDeck` RCL to reuse themselves.

## Anchor — the deck itself (`BlazorDeck`)

The reusable framework whose parts are the taught components:

- **`Deck`** — root; owns the slide list, keyboard handling, routing
- **`Slide`** — base slide layout (`Title`, `Number`, content)
- **`CodeSlide` / `CodeBlock`** — code display + syntax highlighting + line-step reveal
- **`SlideHost`** — renders the current slide *by type* via `DynamicComponent`
- **`DeckState`** — service: current index, theme, presenter mode
- **`ThemeProvider`** — cascades accent / light-dark / font-scale to everything
- **`SlideNav`** — prev/next/jump controls + progress
- **`Overview`** — press-`O` thumbnail grid (virtualized)
- **`PlaygroundSlide`** — live `@bind` parameter editor
- **Services** — `ISyntaxHighlighter`, `DeckState` (DI)

**Why it holds up better than a domain anchor:** `DynamicComponent` becomes the deck's engine (not a
throwaway mention), `DeckState`/slide-index is a textbook state container, JS interop has three
genuine uses, and RCL reusability is unarguable (reused every slide + every future talk). Only
soft spot: generics (#16), a mild stretch — already flex, cut-friendly.

### Concept → deck-component mapping

| # | Concept | How the *deck itself* shows it | Carried by | Fit |
|---|---------|-------------------------------|-----------|-----|
| 2 | Render modes | Unified Blazor Web App: show static-SSR + Interactive Server + WASM/Auto slides live in the deck itself | Deck host | ⭐ self-demo |
| 3 | Structure / anatomy | `Slide.razor` — markup + `@code` + directives | Slide | strong |
| 4 | Parameters | `Slide.Title`/`Number`; `CodeSlide.Language`/`HighlightLines` | Slide / CodeSlide | strong |
| 5 | Attribute splatting | `Slide` forwards `class`/`data-*` so one slide can restyle itself | Slide | strong |
| 6 | Data binding | `@bind` on "jump to slide N"; live parameter playground | SlideNav / Playground | genuine |
| 7 | `EventCallback` | `OnNext`/`OnPrev`/`OnSlideChanged` from nav | SlideNav | strong |
| 8 | Lifecycle | Slide-enter animation in `OnAfterRender`; highlight on `OnAfterRenderAsync` | Slide / CodeSlide | strong |
| 9 | Parent/child | `Deck` ↔ `Slide` via params + callbacks | Deck + Slide | strong |
| 10 | `@ref` | `Deck` calls `slide.Advance()` to step through fragments within a slide | Deck → Slide | genuine |
| 11 | Cascading params | Theme (accent / light-dark / font scale) cascades to every slide + code block | ThemeProvider | strong |
| 12 | DI | Inject `ISyntaxHighlighter` + `DeckState` | Services | strong |
| 13 | State container | `DeckState` — current index synced across nav, progress, counter, overview | DeckState | ⭐ textbook |
| 14 | `InvokeAsync`/threading | Auto-advance / rehearsal-timer fires off-thread → `InvokeAsync(StateHasChanged)` | DeckState timer | genuine |
| 15 | Templated components | `Slide` exposes `TitleTemplate` + `ChildContent`; `TwoColumn` layout fragments ⭐ | Slide layouts | strong |
| 16 | Generics *(flex)* | `BulletList<TItem>` / `AgendaList<TItem>` data-driven slide | Content comps | ⚠️ mild stretch |
| 17 | `DynamicComponent` | `SlideHost` renders the current slide **by type** — the deck's engine ⭐ | SlideHost | load-bearing |
| 18 | JS interop | Keyboard nav (`keydown`), fullscreen API, syntax highlighting ⭐ | Deck interop | 3 genuine uses |
| 19 | `@key` | Keying slides / overview thumbnails so transitions diff correctly | Deck / Overview | genuine |
| 20 | Virtualization | The `Overview` thumbnail grid, virtualized | Overview | reasonable |
| 21 | CSS isolation | `Slide.razor.css` / `CodeSlide.razor.css`; `::deep` into highlighted code ⭐ | Slide / CodeSlide | strong |
| 22 | RCL | Package as `BlazorDeck` — reused every slide *and every future talk* ⭐⭐ | The library | the payoff |
| 23 | MudBlazor capstone | Build deck chrome (nav, buttons, overview) with MudBlazor; slides feel native beside Mud | Deck chrome | strong |
| 24 | bUnit *(flex)* | Test `Slide` renders its template; advancing nav increments `DeckState.Index` | Tests | strong |

## Spike — DONE ✓

3-slide `BlazorDeck` skeleton built and verified in the browser (.NET 10, unified Blazor Web App).
Repo layout: `src/BlazorDeck` (RCL) · `src/Talk` + `src/Talk.Client` (host, Auto) ·
`tests/BlazorDeck.Tests` (bUnit) · `BlazorComponents.slnx`. Run with launch profile `deck` (port 5028).

Validated live (build clean, 0 warnings, no console errors):
- `DynamicComponent` renders the current slide by type (#17 — the deck engine)
- `DeckState` container drives nav + position counter (#13)
- JS interop: keyboard nav + fullscreen + code highlighting via `IJSObjectReference` module (#18)
- Cascading theme flip indigo→amber across title, code accent, keywords (#11)
- Render-mode self-demo slide reported **WebAssembly / interactive: True** (#2)
- CSS isolation per slide/code component (#21); RCL served at `_content/BlazorDeck/deck.js` (#22)

Spike files are the real foundation — Step 3 builds on them (not throwaway).

## Slide-type palette — BUILT ✓

The reusable layout components in `BlazorDeck` (compose these inside per-talk slide wrappers):

| Layout | Purpose | Demonstrates |
|--------|---------|--------------|
| `TitleSlide` | Opening / big statement (kicker + title + subtitle) | structure, params |
| `SectionSlide` | Segment divider (marker + title) | — |
| `Slide` | Generic title + freeform body (base others build on) | #3 #4 #5 #11 #21 |
| `CodeSlide` | Full code + JS-interop highlighting | #8 #18 #21 |
| `SplitSlide` | Two columns via named `Left`/`Right` fragments | templating #15 |
| `BulletList<T>` | Typed, templated list (used inside slides) | generics #16, #15, #19 |
| `CompareSlide` + `CompareOption` | Side-by-side options w/ a recommended pick | parent/child #9 |
| `DemoSlide` | Frames a live interactive component inline | the meta moments |
| `PlaygroundSlide` | Preview + controls panes; per-slide wires `@bind` | data binding #6 |
| `Overview` (in `Deck`) | Press-`o` virtualized slide list, click to jump | virtualization #20, #19 |

All verified live in the browser (8 showcase slides, build clean, no console errors): overview jump,
theme flip, live counter, `@bind` slider, split/compare/section layouts.

**MudBlazor integrated (theming aligned).** The deck wraps `MudThemeProvider`; the `t` toggle drives
MudBlazor `IsDarkMode` + primary across 4 presets (Dark/Light × Indigo/Amber). All `--deck-*`
variables resolve *from* `--mud-palette-*`, so custom slides and MudBlazor components share one
palette — the Segment F "feels native" capstone (#23) is now structural, not staged. Verified:
`--deck-bg` === `--mud-palette-background`, `--accent` === `--mud-palette-primary`.

Known content-sizing note: `SplitSlide` code overflows if too wide — keep split-code snippets short.

## Step 3 progress — slide content per segment

- [x] **Segment A · Set the stage** — built & verified (`src/Talk.Client/Slides/SegmentA/`)
  - A1 Title · A2 "It's all components" (reveal hook) · A3 Hello (⚠ placeholder bio to fill)
  - A4 Agenda (bullet list) · A5 Four render modes · A6 Mode-live self-demo (RendererInfo → WebAssembly)
  - A7 Why render mode matters (sets up DI lifetimes + JS interop callbacks)
  - Showcase slides removed (in git history); `Present.razor` now lists Segment A only.
- [x] **Segment B · Fundamentals** — built & verified (`src/Talk.Client/Slides/SegmentB/`)
  - Approach: lean on the deck's own components where it helps, but keep each snippet the
    **minimal version for the current lesson** (teaching code need not match the real code 1:1).
    Concept slides = one focused code window per file (two windows when a lesson spans two files).
  - B1 Anatomy — **two slides**: (1) recommended single-file layout — real `Slide.razor`
    (markup + inline `@code`); (2) "…or split across files" — same component as `.razor` +
    `.razor.cs` + `.razor.css`, with `.cs`/`.css` badged **optional**. (Extracted a reusable
    `CodeWindow` primitive from `CodeSlide`; `Slide` kept single-file to match the recommendation.)
  - B2 Parameters — two windows: `Slide.razor` (declares `Title`) + `A5RenderModes.razor` (parent passes it)
  - B3 Attribute splatting — two windows: `Slide.razor` (`CaptureUnmatchedValues` + `@attributes`) + a caller
  - B4 Data binding — live `@bind` demo **beside its own code** (deck uses a state container, so this is a live toy)
  - B5 EventCallback — child (`Stepper.razor`) + parent code windows **beside the live `<Stepper>`**
  - B6 Lifecycle — a **timeline** (not empty code stubs) of all six hooks in order:
    `SetParametersAsync` (async-only) · `OnInitialized{Async}` · `OnParametersSet{Async}` ·
    `ShouldRender` (sync-only) · `OnAfterRender{Async}` · `Dispose`/`DisposeAsync`, each with a
    frequency badge; most pair sync+async, two don't. NO JS interop here (that's Segment D #18,
    which will reference this lifecycle); `ShouldRender` foreshadows Segment E rendering/#19.
  - Note: deck has no real `@bind`/`EventCallback` usage → B4/B5 are live demos that foreshadow the
    state container (#13, Segment C). A3 bio now filled in (Brent · Alien Arc · GitHub).
- [x] Segment C · Communication & DI — **built & verified** (slides 15–21). Concepts #9–14 (+#13½),
    each dissecting the deck's real state container. Layout guard: keep code windows ≤ ~36 chars
    wide and short enough that one-line content clears the top (the `.cs-pre` clips, doesn't wrap).
  - C1 (#9) — communication map: `[Parameter]` down · `EventCallback` up · `DeckState` anywhere
  - C2 (#10) — `@ref`: the real `CodeWindow` captures its `<code>` ElementReference, hands it to JS
  - C3 (#11) — cascading params: the real `<CascadingValue Value="State.Theme">` provider + a consumer
  - C4 (#12) — DI: `AddScoped<DeckState>()` in **both** Program.cs (Server + WASM — an InteractiveAuto detail)
  - C5 (#13) — ⭐ `DeckState`: state + `OnChange` event; subscribe in `OnInitialized`, unsubscribe in `Dispose`
  - C6 (#13½) — `StateHasChanged`: introduced here (before it's used off-thread). When Blazor re-renders
    for you vs. when you must ask; ties back to the container's `State.OnChange += StateHasChanged` line
  - C7 (#14) — threading: the transition timer's off-thread continuation → `InvokeAsync(StateHasChanged)`
- [x] Segment D · Advanced composition — **built & verified at 1920×1080** (slides 22–25). Concepts #15–18.
    Design target is 1080p (fonts are capped, height is vh-based, so 720p is the tighter constraint).
    Live demos are real components, not fakes — what's on screen IS the code shown.
  - D1 (#15) — templated components: real `Card.razor` (Header + ChildContent slots) + `Page.razor` caller + live `Card`
  - D2 (#16) — generic components: real `TypedList<TItem>` (`@typeparam`, `RenderFragment<TItem> Row`) + caller + live list; `TItem` inferred
  - D3 (#17) — `DynamicComponent`: the deck's real engine — a table of slide types rendered by index (Parameters dict for reuse)
  - D4 (#18) — JS interop: both directions from real deck.js — `IJSObjectReference` (C#→JS) and `DotNetObjectReference`/`[JSInvokable]` (JS→C#)
  - Razor gotcha: a component tag (`<TypedList>`) written inside a CSS comment in a `<style>` block is parsed as markup — keep angle brackets out of `<style>` comments.
- [ ] Segments E–G (next: Segment E · Performance — #19 @key, #20 Virtualization; both real deck features)

## Open items

- [ ] Continue Step 3 with Segment E
- [ ] **Refactor toward original intent** (discuss first): reuse a small set of slide layouts rather than
      a bespoke type per slide. Unused layout components sitting at 0 uses: `SectionSlide`, `CodeSlide`,
      `CompareSlide`/`CompareOption`, `DemoSlide`, `PlaygroundSlide` (also `SplitSlide`/`TitleSlide` at 1).
      `Slide` is used 23/25, `CodeWindow` 15/25 — reuse is real at the base level only. Decide: wire the
      orphaned layouts in, or delete them.
- [ ] Later: real highlighter (vendor Prism/Shiki to replace the spike's minimal one),
      bUnit tests (#24), PDF-export fallback, `DiagramSlide` if the lifecycle timeline needs it
