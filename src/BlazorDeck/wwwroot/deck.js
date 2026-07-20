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
