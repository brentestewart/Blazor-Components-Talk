// BlazorDeck JS interop module (concept #18).
// Loaded on demand via IJSObjectReference: keyboard navigation and fullscreen —
// the two things that genuinely need the browser. (Syntax highlighting runs in C#;
// see CodeHighlighter — it doesn't need JS, so it's deliberately not here.)

let handler = null;

// The deck is authored at this fixed canvas and scaled as a whole to fit any screen.
const DESIGN_W = 1280, DESIGN_H = 720;

// Scale the whole canvas to fit the viewport (min ratio → letterbox, never crop). Publishes
// --deck-scale, which .deck reads. Recomputes on resize.
let fitBound = false;
export function fitDeck() {
    const k = Math.min(window.innerWidth / DESIGN_W, window.innerHeight / DESIGN_H);
    document.documentElement.style.setProperty("--deck-scale", k);
    if (!fitBound) {
        window.addEventListener("resize", () => fitDeck());
        fitBound = true;
    }
}

// Shrink a slide's CONTENT to fit the canvas when it's taller than the space left under the title.
// Only .slide-body scales — the title and the <Tag /> chrome above it are outside that box, so they
// never move however far the content shrinks.
//
// Fitting can't wait on .NET. A JS interop call from OnAfterRenderAsync lands after the browser has
// already painted the incoming slide at full size, which reads as a flicker: the content appears
// large, then snaps down. A MutationObserver callback runs as a microtask at the end of the task
// that swapped the slide — before the next paint — so the fitted size is the first thing shown. The
// interop calls stay as a backstop (fitSlide is idempotent).
let slideObserver = null;
let fitQueued = false;

export function observeSlides() {
    if (slideObserver) return;
    const stage = document.querySelector(".stage");
    if (!stage) return;
    slideObserver = new MutationObserver(() => {
        // A slide's own content can mutate constantly (live demos, counters), so measure at most once
        // per frame — but synchronously on the first mutation, so it still beats the paint.
        if (fitQueued) return;
        fitQueued = true;
        requestAnimationFrame(() => { fitQueued = false; });
        fitSlide();
    });
    // childList only. fitSlide writes inline styles, and attribute mutations aren't observed, so it
    // can't retrigger itself.
    slideObserver.observe(stage, { childList: true, subtree: true });
}

// Fits every layer on the stage, not just the incoming one: during a transition the outgoing slide
// is a separate @key'd subtree, built fresh, so it starts with no factor of its own — leaving it
// alone makes it snap to full size as it fades out. There are at most two. Scoped to .slide-layer so
// the overview's thumbnails (.ov-stage) are untouched.
export function fitSlide() {
    document.querySelectorAll(".slide-layer .slide").forEach(fitOneSlide);
}

function fitOneSlide(slide) {
    const body = slide.querySelector(".slide-body");
    if (!body) return;

    // Measure from a clean slate — a factor left over from a previous fit would skew the numbers.
    body.style.removeProperty("--content-fit");
    body.style.removeProperty("height");

    const slideStyle = getComputedStyle(slide);
    const head = slide.querySelector(".slide-head");
    const headHeight = head
        ? head.offsetHeight + parseFloat(getComputedStyle(head).marginBottom)
        : 0;
    const available = slide.clientHeight
        - parseFloat(slideStyle.paddingTop)
        - parseFloat(slideStyle.paddingBottom)
        - headHeight;
    // The body is a flex item, so it may already have been shrunk below its content; scrollHeight
    // reports the true content height in that case, offsetHeight when it wasn't shrunk.
    const natural = Math.max(body.scrollHeight, body.offsetHeight);
    if (available <= 0 || natural <= available) return;   // already fits — leave the slide alone

    body.style.setProperty("--content-fit", available / natural);
    // Give the box the layout height its scaled content actually occupies. Without this the slide
    // still lays out against the unscaled height, overflows, and .slide's justify-content:center
    // drags the title up into the tag — which is the bug this replaces.
    body.style.height = available + "px";
}

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
    // The thumbnail stage is the full 1280×720 canvas, so scale by (card width / canvas width).
    // clientWidth is layout px inside the canvas, unaffected by the outer --deck-scale transform.
    const k = thumb.clientWidth / DESIGN_W;
    const root = document.documentElement.style;
    root.setProperty("--ov-k", k);
    root.setProperty("--ov-thumb-h", DESIGN_H * k + "px");
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
