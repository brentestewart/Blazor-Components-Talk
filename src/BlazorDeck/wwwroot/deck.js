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

// Structural Razor directives whose whole @word is coloured (unlike @expressions such as
// @Title, where only the "@" is coloured and the identifier is left as a plain variable).
const DIRECTIVES = "code|functions|foreach|if|else|for|while|do|switch|using|namespace|inject|page|layout|implements|inherits|typeparam|attributes|rendermode|bind|ref|key|onclick|oninput|onchange";

// Token types, tried left-to-right at each position. The (?<!\w) before a tag's "<" keeps
// generics (IEnumerable<TItem>) from being read as tags. Comments/strings come first so
// keywords inside them aren't separately coloured.
const TOKEN = new RegExp(
    "(\\/\\/[^\\n]*)"                       // 1 line comment
    + "|(\"[^\"]*\"|'[^']*')"               // 2 string / char literal
    + "|(@(?:" + DIRECTIVES + ")\\b)"       // 3 Razor structural directive (@code, @foreach)
    + "|(@)"                                // 4 Razor "@" (start of a C# expression)
    + "|((?<!\\w)<\\/?[A-Za-z][\\w.-]*)"    // 5 HTML / component tag name
    + "|\\b(?:" + KEYWORDS + ")\\b"          // 6 keyword
    + "|\\b[A-Z][A-Za-z0-9_]*\\b"            // 7 Capitalized identifier (type OR member)
    + "|\\b\\d[\\w.]*\\b",                    // 8 number
    "g");

function esc(s) {
    return s.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
}

// A Capitalized identifier is a *type* (coloured) in obvious type position — followed by ?, [,
// or <T> (not </close); inside generic args (preceded by < or ,); or an attribute name right
// after "[" ([Parameter], [JSInvokable]). Otherwise it's a plain member/variable name (Title,
// ChildContent, Name…) and keeps the default text colour.
function isTypePosition(src, start, end) {
    const before = src[start - 1] || "";
    const after = src[end] || "";
    return after === "?" || after === "["
        || (after === "<" && src[end + 1] !== "/")
        || before === "<" || before === "," || before === "[";
}

// Tokenise one line, escaping the gaps between tokens and wrapping each token in a span
// coloured from the --deck-code-* palette (defined per-theme on the .deck element). Building
// output as we go means we never re-scan the markup we inject. A production deck would vendor
// a real grammar (Prism/Shiki); this is a compact stand-in tuned for the Razor/C# on screen.
function tokenize(src) {
    let out = "", last = 0, m;
    TOKEN.lastIndex = 0;
    while ((m = TOKEN.exec(src)) !== null) {
        out += esc(src.slice(last, m.index));
        const end = m.index + m[0].length;
        let kind;
        if (m[1]) kind = "comment";
        else if (m[2]) kind = "string";
        else if (m[3] || m[4]) kind = "directive";
        else if (m[5]) kind = "tag";
        else if (/^\d/.test(m[0])) kind = "number";
        else if (/^[A-Z]/.test(m[0])) kind = isTypePosition(src, m.index, end) ? "type" : "";
        else kind = "keyword";
        out += kind
            ? `<span style="color:var(--deck-code-${kind})">${esc(m[0])}</span>`
            : esc(m[0]);
        last = end;
        if (m[0].length === 0) TOKEN.lastIndex++;
    }
    out += esc(src.slice(last));
    return out;
}

// Highlight, wrapping each source line in its own <span class="cl"> so build-steps can dim
// lines outside the focused region (see setFocus). Comments/strings never span lines, so
// tokenising line-by-line is safe.
export function highlight(el) {
    el.innerHTML = el.textContent
        .split("\n")
        .map(line => `<span class="cl" style="display:block;transition:opacity .25s ease">${tokenize(line) || "​"}</span>`)
        .join("");
}

// Spotlight lines [from, to] (1-based, inclusive) by dimming the rest; from <= 0 clears focus.
export function setFocus(el, from, to) {
    el.querySelectorAll(".cl").forEach((line, i) => {
        const n = i + 1;
        line.style.opacity = (from > 0 && (n < from || n > to)) ? "0.28" : "";
    });
}
