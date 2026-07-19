// BlazorDeck JS interop module (concept #18).
// Loaded on demand via IJSObjectReference: keyboard navigation, fullscreen,
// and a lightweight code highlighter.

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

const KEYWORDS = "public|private|protected|internal|class|record|struct|interface|void|async|await|new|return|if|else|for|foreach|while|in|var|string|int|bool|double|null|true|false|namespace|using|get|set|static|readonly|event|partial|override|virtual|abstract|this|typeof|nameof";

// A line comment OR a keyword, matched in one alternation. Comments come first so a
// keyword inside a comment is swallowed by the comment (and not separately coloured).
const TOKEN = new RegExp(`(\\/\\/[^\\n]*)|\\b(?:${KEYWORDS})\\b`, "g");

// Minimal highlighter for the spike: escape, then wrap comments/keywords in styled spans.
// Single pass over the escaped text — crucial so we never re-scan the markup we inject
// (the injected styles contain "var(--…)", and "var" is itself a keyword). A real deck
// would vendor Prism/Shiki here.
export function highlight(el) {
    const escaped = el.textContent
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;");
    el.innerHTML = escaped.replace(TOKEN, (match, comment) =>
        comment
            ? `<span style="color:var(--deck-comment,#6a9955)">${comment}</span>`
            : `<span style="color:var(--accent)">${match}</span>`);
}
