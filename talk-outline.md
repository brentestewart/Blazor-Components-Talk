# Unleashing the Power of Blazor Components

**Duration:** 60 minutes (~48 min content · ~7 min live code tour · ~5 min Q&A)
**Audience:** Mixed (some new to Blazor, some experienced)
**Style:** Mainly code slides; meta live demo woven throughout + a live code tour at the end
**Library focus:** MudBlazor (the deck's themes *are* `MudTheme`s; Segment F shows Mud components
sharing the slide palette)
**Big idea:** The talk is *meta* — the slide deck itself is a Blazor app (`BlazorDeck`), and the
components we teach ARE the deck's own components. Revealed upfront as the hook.
**Hosting:** Unified Blazor Web App (.NET 10). Globally interactive router in `Talk.Client` so
navigation is client-side and slide transitions survive a move. Three static-SSR demo endpoints are
carved out *outside* the router and embedded by iframe, so the deck can still show non-interactive
render modes from the inside. See Slide technology below.

> This document describes the deck **as built**. It was rewritten to match the shipped deck;
> earlier revisions were a plan and had drifted from the code.

---

## Status

- **44 slides**, segments **A–H** plus a close (`Z`). Source of truth is the `Slides` table in
  `src/Talk.Client/Present.razor` — that array *is* the table of contents.
- **Concepts #2–#26**, plus lettered/fractional additions that earned their own slides
  (`#11b` named cascading, `#13½` `StateHasChanged`, `#18a`/`#18b` the two interop directions) and
  several that carry no number at all (Static SSR, per-component render mode, prerendering,
  `PersistentComponentState`, `ErrorBoundary`, forms).
- **Flex items** (skippable live to hit time, at segment boundaries): generic components (#16),
  named cascading (#11b), `ShouldRender` (#22), bUnit (#25).
  *(`DynamicComponent` #17 is not flex — it's the deck's rendering engine.)*
- **Previously cut, since shipped:** `ErrorBoundary` (D3b/D3c) · `PersistentComponentState` (B7).
- **Still cut:** Forms & validation · QuickGrid · `Sections`.
- **Tests:** 6 passing in `tests/BlazorDeck.Tests` (bUnit for components, plain xunit for the
  pure `CodeHighlighter`).

---

## Ordering — the 44 slides as built

Numbers are live slide positions (`/slide/N`).

### A · Set the stage (1–10)

| # | Slide | Beat |
|---|-------|------|
| 1 | `A1Title` | Title + subtitle (with the joke) |
| 2 | `A3Hello` | Who's talking — Brent · Alien Arc · GitHub |
| 3 | `A2Reveal` | ⭐ The meta hook: "It's all components" — this deck is the demo |
| 4 | `A4Agenda` | What we'll cover (`BulletList<T>`) |
| 5 | `A5RenderModes` | #2 — the four render modes, as framing |
| 6 | `A6ModeLive` | Self-demo: `RendererInfo` reports this slide's own mode |
| 7 | `A6bStaticSsr` | Static SSR, live in an iframe: inert `@onclick`, working form POST |
| 8 | `A6cRenderModes` | Render mode is **per-component**: a Server island beside a WASM island |
| 9 | `A6dPrerender` | Prerendering — names the two-phase render, sets up B7's gotcha |
| 10 | `A7WhyItMatters` | Why mode choice colours DI lifetimes and interop timing |

*Intro before the reveal (2 before 3) is deliberate: who's talking, then what the deck is.*

### B · Fundamentals (11–19) — each slide dissects a real deck component

| # | Slide | Concept |
|---|-------|---------|
| 11 | `B1Anatomy` | #3 — one `.razor` file: markup + inline `@code` (the real `Slide`) |
| 12 | `B1bAnatomyFiles` | #3b — the same component split into `.razor` + `.razor.cs` + `.razor.css` (both extras badged **optional**) |
| 13 | `B1cIsolation` | #21 — CSS isolation: the `b-…` attribute rewrite, and why a child's elements dodge it |
| 14 | `B2Parameters` | #4 — `[Parameter]` declared, parent passes it (one-way, down) |
| 15 | `B3Splatting` | #5 — `CaptureUnmatchedValues` + `@attributes` |
| 16 | `B4Binding` | #6 — real `@bind` code beside the live component it drives |
| 17 | `B5EventCallback` | #7 — `Stepper.razor` child + parent, beside the live `<Stepper>` |
| 18 | `B6Lifecycle` | #8 — the full `ComponentBase` hook timeline, with frequency badges |
| 19 | `B7Persist` | `PersistentComponentState` — the prerender double-run; step 2 adds `[PersistentState]` + `??=` |

*CSS isolation moved here from Segment F: it's part of "what a component is made of", so it belongs
next to the anatomy slides rather than in a styling segment at the end.*

### C · Communication & DI (20–27) — dissecting the deck's own state container

| # | Slide | Concept |
|---|-------|---------|
| 20 | `C1CommunicationMap` | #9 — the map: params down, `EventCallback` up, DI service anywhere |
| 21 | `C2Ref` | #10 — `@ref`: capture an element, `FocusAsync` after first render *(illustrative)* |
| 22 | `C3Cascading` | #11 — the real `<CascadingValue Value="State.Theme">` + a consumer |
| 23 | `C3bNamedCascading` | #11b — two cascades of one type, disambiguated by `Name` *(illustrative)* |
| 24 | `C4Di` | #12 — `AddScoped<DeckState>()` in **both** `Program.cs` files (the InteractiveAuto detail) |
| 25 | `C5StateContainer` | #13 ⭐ — `DeckState`: state + `OnChange`; subscribe in `OnInitialized`, unsubscribe in `Dispose` |
| 26 | `C6StateHasChanged` | #13½ — automatic re-render vs. asking for one |
| 27 | `C7Threading` | #14 — the transition timer's off-thread continuation → `InvokeAsync(StateHasChanged)` |

### D · Advanced composition (28–36)

| # | Slide | Concept |
|---|-------|---------|
| 28 | `D1Templated` | #15 — `RenderFragment` slots: the real `Card.razor` (named `Header` + `ChildContent`), live |
| 29 | `D2Generic` | #16 *(flex)* — the deck's real `BulletList<T>`: `@typeparam`, inferred `TItem` |
| 30 | `D2bForms` | Forms vocabulary: `EditForm` cascades an `EditContext`, `DataAnnotationsValidator` feeds it, `ValidationMessage` reads it. Live `InputText` |
| 31 | `D2cInputBase` | #26 — write your own input: `DurationInput : InputBase<TimeSpan>` parsing "1h 30m", live. Step 1 the form, step 2 the component |
| 32 | `D3DynamicComponent` | #17 — the deck's engine: one `<DynamicComponent>` swaps in whichever slide you're on |
| 33 | `D3bErrorBoundary` | `ErrorBoundary` — the real wrap around this deck's slide host |
| 34 | `D3cBoom` | ⭐ The payoff: a real slide throws live; the fallback appears, the deck survives, `→` keeps going |
| 35 | `D4JsCall` | #18a — C# → JS: `IJSObjectReference` module import, `toggleFullscreen` |
| 36 | `D5JsCallback` | #18b — JS → C#: `DotNetObjectReference` + `[JSInvokable]`, the real keyboard nav |

*30 exists for 31's sake: the deck has no other forms content, so without it `InputBase<T>` would be
teaching how to join machinery the room has never seen. 31 shows usage before authorship for the
same reason.*

### E · Performance (37–39)

| # | Slide | Concept |
|---|-------|---------|
| 37 | `E1ShouldRender` | #22 — veto the renders that can't change your output; the deck guards tokenising instead |
| 38 | `E2Key` | #19 — `@key` for stable list identity (the real `BulletList` + overview rows) |
| 39 | `E3Virtualize` | #20 — the overview grid is a real `<Virtualize>` |

### F · Reuse (40–41)

| # | Slide | Concept |
|---|-------|---------|
| 40 | `F1Libraries` | #23 — the RCL: `Sdk="Microsoft.NET.Sdk.Razor"`, assets under `_content/BlazorDeck/` |
| 41 | `F2MudBlazor` | #24 — real MudBlazor components, themed by the shared palette |

### G · Testing (42)

| # | Slide | Concept |
|---|-------|---------|
| 42 | `G1Bunit` | #25 *(flex)* — the deck's real `CaptionTests` under `BunitContext` |

### H · Code tour (43)

| # | Slide | Beat |
|---|-------|------|
| 43 | `H1CodeTour` | `SectionSlide` segue: leave the deck, walk the real project in the editor — the `BlazorDeck` RCL and the MVVM starter in `samples/mvvm` (served live at `/demo/mvvm`) |

### Close (44)

| # | Slide | Beat |
|---|-------|------|
| 44 | `Z1Close` | Thank-you + repo link + scannable QR. No recap — the source *is* the recap |

---

## Slide technology — as built

**A unified Blazor Web App (.NET 10)**, with the reusable deck framework packaged as the
`BlazorDeck` RCL. The medium is the message: teaching Blazor components *with* Blazor components.

- **Repo layout:** `src/BlazorDeck` (RCL) · `src/Talk` (server host) · `src/Talk.Client` (WASM
  client + all slides) · `samples/mvvm/MvvmSample` · `tests/BlazorDeck.Tests` ·
  `BlazorComponents.slnx`.
- **Run it:** `dotnet run` from `src/Talk` — profile `https` (https://localhost:7270,
  http://localhost:5028) or `http` (5028 only).
- **Router:** `Routes.razor` lives in `Talk.Client` so the router itself runs in the interactive
  runtime. That's what keeps navigation client-side (no round-trip, no remount) and lets slide
  transitions survive a move. **Don't** revert this to per-page render modes.
- **The static-SSR carve-out:** because the router is globally interactive, the deck cannot show a
  non-interactive component from the inside. Three endpoints are mapped in `src/Talk/Program.cs`
  *outside* `<Routes>` as `RazorComponentResult` documents, and slides embed them in iframes:
  - `/demo/static-ssr` — GET + POST form round-trip (slide 7)
  - `/demo/render-modes` — a Server island beside a WASM island (slide 8)
  - `/demo/mvvm` — the MVVM sample as an InteractiveServer island (referenced by slide 41)
- **Syntax highlighting is C#, not JS.** `CodeHighlighter` is a pure function (code in → per-line
  tokens out) that runs during render, so code is coloured correctly from the first paint —
  including prerender — with no JS and no flash. It is unit-tested directly.
  *(The original plan called for Prism/highlight.js via interop; that was dropped.)*
- **JS interop is still genuine**, just elsewhere: keyboard nav, fullscreen, canvas fitting, and
  overview thumbnail scaling — all in `_content/BlazorDeck/deck.js`.
- **Risk plan:** run offline, pre-warmed. PDF/screenshot fallback export is still an open item.

### Presenter controls

`←/→` `PageUp/PageDown` `Space` `Enter` move (steps within a slide are consumed before slides) ·
`Home`/`End` jump to ends · `o` overview · `t` theme · `f` fullscreen · `s` settings · `r` reload ·
`Escape` closes overview/settings. Keys are ignored while focus is in a text field, so the `@bind`
demo slides are safe to type in.

- **4 themes:** Dark/Light × Indigo/Amber. Each *is* a `MudTheme`, and all `--deck-*` variables
  resolve from `--mud-palette-*`, so custom slides and MudBlazor components share one palette.
- **5 transitions:** Fade (default) · Slide · Vertical · Scale · None. Per-slide via `SlideInfo`,
  or set the deck default in settings.
- **4 canvas shapes:** Auto (match the screen — default) · 16:9 · 16:10 · 4:3. The canvas is always
  1280 wide, so horizontal `cqw` units are identical whichever shape is picked; only height changes.
- **View state in the URL:** `/slide/N` plus `?theme=`/`?transition=`, read in `OnInitialized`
  before first paint — which is why the deck has no prerender flash and no double-fetch of its own
  (see the B7 note).

## Demo structure — meta, revealed upfront

- **Cold open (slides 1–3):** state the meta outright — "this deck is a Blazor app; these slides are
  the components." Knowing it doesn't spoil anything; watching it get built *is* the payoff.
- **Live moments throughout**, not just at the end: the render-mode probe (6), two live iframes
  (7–8), the `@bind` toy (16), the live `<Stepper>` (17), the live `Card` (28) and `BulletList` (29),
  two working forms with validation firing (30–31), a real slide crashing and recovering (34),
  fullscreen and keyboard interop (35–36), the virtualized overview (39), live MudBlazor (41).
- **Finale (43–44):** drop out of the deck into the editor, walk the real project, then hand them
  the repo.

## Anchor — the deck itself (`BlazorDeck`)

The RCL whose parts are the taught components:

**Layout / content components**
- **`Deck`** — root: slide list, keyboard handling, URL sync, transitions, overview, settings,
  canvas fitting, and the `ErrorBoundary` around the slide host
- **`Slide`** — base slide layout (title, subtitle, body); the anatomy specimen in B1
- **`TitleSlide`** · **`SectionSlide`** — opening statement / segment divider
- **`CodeSlide`** — full-bleed code slide (used by B1)
- **`CodeWindow`** — the workhorse: one filenamed, highlighted, optionally step-revealed code pane
- **`SplitSlide`** — two columns via named `Left`/`Right` fragments (+ `Footer`, `WideLeft`)
- **`LiveFrame`** — the "live" tag + framed stage around a real running component
- **`BulletList<T>`** — typed, templated, `@key`-ed list
- **`Caption`** — inline prose/code annotation
- **`SlideErrorFallback`** — what a thrown slide degrades to

**State & services**
- **`DeckState`** (scoped DI) — index, step/step-count, theme, transition, aspect, overview and
  settings flags, direction; raises `OnChange`
- **`DeckTheme`/`DeckThemes`** · **`SlideTransition(s)`** · **`DeckAspect(s)`** — the presenter-facing
  option sets
- **`SlideInfo`** — one TOC entry: `Type`, `Title`, `Transition`, `Tag` (the corner `<Tag />` label)
- **`ISteppable`/`SteppableSlide`** — how a slide reports its build-step count to the deck
- **`CodeHighlighter`** — pure, testable Razor/C# tokenizer (no DI, no JS)

**Assets:** `deck.js` · `background.png`, served from `_content/BlazorDeck/`.

**Why it holds up:** `DynamicComponent` is the deck's engine (not a throwaway mention), `DeckState`
is a textbook state container, JS interop has four genuine uses, `ErrorBoundary` is dogfooded to the
point that a slide can crash on stage, and RCL reusability is unarguable.

### Concept → deck-component mapping

| # | Concept | Slide | How the *deck itself* shows it | Fit |
|---|---------|-------|-------------------------------|-----|
| 2 | Render modes | 5–10 | The host is Auto; a probe reports the live mode; two iframes show Static SSR and per-component islands | ⭐ self-demo |
| 3 | Anatomy | 11–12 | The real `Slide.razor`, then the same type split across three files | strong |
| 4 | Parameters | 14 | `Slide.Title` declared; a parent slide passes it | strong |
| 5 | Attribute splatting | 15 | `CaptureUnmatchedValues` + `@attributes` on the root element | strong |
| 6 | Data binding | 16 | Live `@bind` toy beside its own source | genuine (toy) |
| 7 | `EventCallback` | 17 | `Stepper` raises, parent decides what a step means | strong (toy) |
| 8 | Lifecycle | 18 | Full `ComponentBase` timeline; `OnAfterRender` pays off at 35 | strong |
| — | `PersistentComponentState` | 19 | Prerender double-run; `[PersistentState]` + `??=` as the whole diff | made-up by design |
| 9 | Parent/child | 20 | The communication map framing the segment | strong |
| 10 | `@ref` | 21 | Capture an `<input>`, `FocusAsync` | illustrative |
| 11 | Cascading params | 22 | The real theme cascade wrapping the whole stage | ⭐ strong |
| 11b | Named cascading | 23 | Two same-typed cascades disambiguated by `Name` | illustrative *(flex)* |
| 12 | DI | 24 | `AddScoped<DeckState>()` in **both** `Program.cs` files | ⭐ real detail |
| 13 | State container | 25 | `DeckState` drives everything you're watching | ⭐ textbook |
| 13½ | `StateHasChanged` | 26 | The missing piece under `State.OnChange += StateHasChanged` | strong |
| 14 | `InvokeAsync`/threading | 27 | The transition timer's off-thread continuation | ⭐ genuine |
| 15 | Templated components | 28 | Real `Card` with a named `Header` slot, rendered live | strong |
| 16 | Generics | 29 | The real `BulletList<T>` that slides 4/5/10 already use | strong *(flex)* |
| — | Forms | 30 | `EditForm` + `DataAnnotationsValidator` + `InputText`, live — the vocabulary 31 needs | context slide |
| 26 | `InputBase<T>` | 31 | `DurationInput : InputBase<TimeSpan>`, live: nonsense in, your own error out | ⭐ converges B2/B3/B4/C3/D2 |
| 17 | `DynamicComponent` | 32 | The deck's engine — renders the current slide **by type** | ⭐ load-bearing |
| — | `ErrorBoundary` | 33–34 | The real wrap around the slide host; slide 34 throws live and recovers | ⭐ dogfooded |
| 18a | JS interop C#→JS | 35 | `IJSObjectReference` → `toggleFullscreen` in the real `deck.js` | strong |
| 18b | JS interop JS→C# | 36 | `DotNetObjectReference` + `[JSInvokable]` — every keypress you make | ⭐ strong |
| 19 | `@key` | 38 | `BulletList` keys by item; overview rows key by index | genuine |
| 20 | Virtualization | 39 | The overview grid is a real `<Virtualize>` | genuine |
| 21 | CSS isolation | 13 | The `b-…` rewrite; why a child's elements dodge it | strong |
| 22 | `ShouldRender` | 37 | Framed as the flip side of #13½; `CodeWindow` guards the work instead | honest caveat *(flex)* |
| 23 | RCL | 40 | `BlazorDeck` itself — reused every slide and every future talk | ⭐⭐ the payoff |
| 24 | MudBlazor | 41 | Deck themes *are* `MudTheme`s; Mud components match with no extra styling | structural |
| 25 | bUnit | 42 | The deck's real `CaptionTests` | strong *(flex)* |

## Slide-type palette — usage as built

| Layout | Purpose | Uses |
|--------|---------|------|
| `CodeWindow` | One filenamed, highlighted, step-revealable code pane | 49 — the workhorse |
| `Caption` | Inline prose/code annotation | 32 |
| `Slide` | Generic title + freeform body | 26 |
| `SplitSlide` | Two columns via named `Left`/`Right` fragments | 24 — the default concept-slide shape |
| `LiveFrame` | Framed live component with a "live" tag | 11 |
| `BulletList<T>` | Typed, templated, keyed list | 3 |
| `TitleSlide` | Opening statement | 1 (A1) |
| `SectionSlide` | Segment divider | 1 (H1) |
| `CodeSlide` | Full-bleed code slide | 1 (B1) |
| `CompareSlide` + `CompareOption` | Side-by-side options with a recommended pick | **0 — orphan** |
| `PlaygroundSlide` | Preview + controls panes | **0 — orphan** |
| `DemoSlide` | Full-slide live component (now just a thin wrapper over `LiveFrame`) | **0 — orphan** |

**Layout guardrails learned the hard way:**
- Keep `SplitSlide` code windows ≤ ~36 chars wide — `.cs-pre` clips, it doesn't wrap. `WideLeft`
  with a `LeftRatio` buys more; at `1.55` the cut lands around 46 chars. Re-wrap the displayed code
  rather than letting a line vanish off the right edge.
- On a stepped slide, keep every step's code **similar in line count**. The `SplitSlide` body scales
  to fit its height, so advancing from a long step to a short one visibly zooms the whole column.
- Scoped CSS can't reach inside a child component — style a built-in like `<InputText>` with
  `::deep`, or give your own component its own `.razor.css` (what `DurationInput` does).
- A related code-window **pair** stacks over-under; side-by-side is reserved for code + live demo.
- A component tag written inside a CSS comment in a `<style>` block is parsed as markup — keep
  angle brackets out of `<style>` comments.
- Design target is 1080p; fonts are capped and height is `cqh`-based, so 720p is the tighter
  constraint. Check 4:3 before presenting on an old projector.

## Open items

- [x] **Forms & `InputBase<T>`** — shipped as **slides 30–31**. 30 is the vocabulary slide the deck
      otherwise lacked (`EditForm`/validator/`ValidationMessage`); 31 writes a real
      `DurationInput : InputBase<TimeSpan>` that parses "1h 30m", shown usage-first. Both run live.
      Illustrative, not dogfooded — a deck has no forms of its own. Closes the biggest content gap
      against the conference abstract's "patterns for real-world apps".
- [ ] **More `@bind` modifiers** — slide 16 covers `@bind` + `@bind:event` only. Consider
      `@bind:format`, `@bind:after`, `@bind:get`/`@bind:set`. (`@bind-Value` is now covered at 31.)
      Open question: a compact "modifiers" strip on slide 16, or its own slide.
- [ ] **Time budget** — the deck is now 44 slides against ~48 minutes of content. Candidate cuts
      flagged earlier: fold `B1bAnatomyFiles` into `B1`, drop `C3bNamedCascading` (flex, and
      explicitly fictional), fold `A6dPrerender` into `A6c`.
- [ ] **Decide keep-or-delete on the three orphan layouts** — `CompareSlide`/`CompareOption`,
      `PlaygroundSlide`, `DemoSlide`. (`CodeSlide` and `SectionSlide` are no longer orphans: B1 and
      H1 use them.)
- [ ] **PDF/screenshot fallback export** for the conference risk plan.
- [ ] Possible future render-mode material: `[StreamRendering]`, enhanced navigation.
- [ ] `AngleSharp` 1.4.0 in the test project has a known moderate advisory (NU1902) — bump when a
      fixed bUnit ships.
