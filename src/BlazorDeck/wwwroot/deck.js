// BlazorDeck JS interop module (concept #18).
// Loaded on demand via IJSObjectReference: keyboard navigation and fullscreen —
// the two things that genuinely need the browser. (Syntax highlighting runs in C#;
// see CodeHighlighter — it doesn't need JS, so it's deliberately not here.)

let handler = null;

const NAV_KEYS = [
    "ArrowRight", "ArrowLeft", "ArrowUp", "ArrowDown",
    "PageUp", "PageDown", "Home", "End", " ", "Enter", "t", "f", "o", "r", "s", "Escape"
];

export function registerKeys(dotNetRef) {
    // Drop any previous listener first, so the Server→WebAssembly auto-swap (which
    // registers again from the new runtime) never stacks two handlers.
    unregisterKeys();
    handler = (e) => {
        if (NAV_KEYS.includes(e.key)) {
            e.preventDefault();
            // The invoke can race the render-mode swap / a circuit reconnect, when the
            // SignalR connection is momentarily gone. Swallow that — the keypress is
            // disposable; the audience can press again.
            dotNetRef.invokeMethodAsync("OnKey", e.key).catch(() => { });
        }
    };
    document.addEventListener("keydown", handler);
}

export function unregisterKeys() {
    if (handler) document.removeEventListener("keydown", handler);
    handler = null;
}

// The overview renders every slide live at full size, then scales each down with a CSS transform.
// The scale factor is (card width / current viewport width) — it can't be expressed in pure CSS
// (you can't divide px by vw), so measure a real card and publish k + the matching thumb height as
// CSS variables. The card width is set by the responsive grid, so we read it from the DOM rather
// than assume it. Re-runs on resize so the thumbnails stay faithful when the window/columns change.
let resizeBound = false;

export function setThumbScale() {
    const thumb = document.querySelector(".ov-thumb");
    if (!thumb) return;
    const k = thumb.clientWidth / window.innerWidth;
    const root = document.documentElement.style;
    root.setProperty("--ov-k", k);
    root.setProperty("--ov-thumb-h", window.innerHeight * k + "px");
    if (!resizeBound) {
        window.addEventListener("resize", () => setThumbScale());
        resizeBound = true;
    }
}

// Center the current slide's card when the overview opens — but scroll ONLY the grid, never any
// ancestor. Element.scrollIntoView() walks every scrollable ancestor, and for a card in the last
// row (which the grid can't scroll far enough to truly center) it reaches up and scrolls the deck's
// overflow:hidden container, shifting the whole deck up with no scrollbar. Setting grid.scrollTop
// directly clamps to the grid's own range and leaves everything else put. Double rAF so it runs
// after the scale change has reflowed the cards to their real height.
export function scrollToCurrentThumb() {
    requestAnimationFrame(() => requestAnimationFrame(() => {
        const grid = document.querySelector(".ov-grid");
        const card = document.querySelector(".ov-card.cur");
        if (!grid || !card) return;
        const gridRect = grid.getBoundingClientRect();
        const cardRect = card.getBoundingClientRect();
        grid.scrollTop += (cardRect.top - gridRect.top) - (grid.clientHeight - cardRect.height) / 2;
    }));
}

export function reload() {
    location.reload();
}

export function toggleFullscreen() {
    if (document.fullscreenElement) {
        document.exitFullscreen();
    } else {
        document.documentElement.requestFullscreen();
    }
}
