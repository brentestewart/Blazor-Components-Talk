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

// Bring the current slide's card into view when the overview opens. Double rAF so it runs after the
// scale change above has actually reflowed the cards to their real height — a single frame can fire
// before the layout settles and scroll against a not-yet-grown grid.
export function scrollToCurrentThumb() {
    requestAnimationFrame(() => requestAnimationFrame(() => {
        document.querySelector(".ov-card.cur")?.scrollIntoView({ block: "center" });
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
