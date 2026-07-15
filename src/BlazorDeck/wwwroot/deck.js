// BlazorDeck JS interop module (concept #18).
// Loaded on demand via IJSObjectReference: keyboard navigation, fullscreen,
// and a lightweight code highlighter.

let handler = null;

const NAV_KEYS = [
    "ArrowRight", "ArrowLeft", "ArrowUp", "ArrowDown",
    "PageUp", "PageDown", "Home", "End", " ", "Enter", "t", "f", "o", "r", "s", "Escape"
];

export function registerKeys(dotNetRef) {
    handler = (e) => {
        if (NAV_KEYS.includes(e.key)) {
            e.preventDefault();
            dotNetRef.invokeMethodAsync("OnKey", e.key);
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

const KEYWORDS = /\b(public|private|protected|internal|class|record|struct|interface|void|async|await|new|return|if|else|for|foreach|while|in|var|string|int|bool|double|null|true|false|namespace|using|get|set|static|readonly|event|partial|override|virtual|abstract|this|typeof|nameof)\b/g;

// Minimal highlighter for the spike: escapes, then wraps line comments and
// keywords in inline-styled spans. A real deck would vendor Prism/Shiki here.
export function highlight(el) {
    let s = el.textContent
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;");
    s = s.replace(/(\/\/[^\n]*)/g, (m) => `<span style="color:var(--deck-comment,#6a9955)">${m}</span>`);
    s = s.replace(KEYWORDS, (m) => `<span style="color:var(--accent)">${m}</span>`);
    el.innerHTML = s;
}
