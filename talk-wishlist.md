# Talk wishlist — things to include

Running list of content/ideas Brent wants worked into the deck. Newest additions at the bottom.

- [ ] **Give the `context` a name.** Show the `Context` attribute on a templated/generic
      component to rename the default `@context` — e.g.
      `<TypedList Items="_people" Context="person">` … `@person.Name — @person.Role`.
      Why it matters: readability, and avoiding `context` name collisions when templated
      components are nested. Relates to D2 (generic components) / #15 templated components.

- [ ] **Show the different ways to bind.** Slide 13 (B4Binding) covers the core two-way
      `@bind` + `@bind:event`. Consider showing the other modifiers: `@bind:format`,
      `@bind:after` (.NET 7+), `@bind:get`/`@bind:set`, and `@bind-Value` (binding a
      component's own `Value`/`ValueChanged`). Open question: keep slide 13 focused and add a
      compact "modifiers" strip, or give it its own dedicated slide.
