# Motion and Static Interaction Guidelines

Reference implementation for Kooboo Site motion and static JavaScript interactions. Use it when creating or modifying Layout, Page, View, Style, or Script objects to turn `ui-ux-pro-max-pp` intent for motion / animation / reveal / hover / tabs / accordion / carousel / menu into maintainable, accessible frontend code that does not block content.

## Scope

- Lightweight interactions for static-site first screens, View sections, product cards, portfolio cards, FAQs, testimonials, pricing cards, and form states
- Presentation interactions that do not depend on backend data/API: mobile navigation, dropdown, accordion, tabs, gallery/carousel, static filter chips, expand/collapse, and hover/focus/active states
- When motion-related rules are returned from `ui-ux-pro-max-pp`'s `ux-guidelines.csv` / `anti-slop.csv`
- When a Page Plan block's `preflightTags`, `variantBrief`, `visualTreatment`, or `Layout Intent` mentions reveal, stagger, hover, motion, or animation

Not applicable:

- Interactions that depend on backend data, accounts, orders, payment, form submission, persistence, or business APIs; these belong to S6-S9 dynamicization.
- Complex runtime-state animation, dragging, real-time visualization, video players, and 3D interaction. These are specialized frontend tasks and require separate design.

## Core Principles

1. **Motion serves the narrative**: motion must reinforce entry order, visual hierarchy, state feedback, content comparison, or brand voice; do not animate merely to “have animation”.
2. **Kooboo HTML first, CSS first**: during static site building, prefer CSS keyframes, transition, `animation-delay`, and CSS custom properties. Use lightweight native JS for state changes; do not introduce Framer Motion, GSAP, or other frontend-framework dependencies.
3. **Content is visible by default**: core content must not permanently depend on JS to appear. Do not initialize every section with `opacity: 0` and wait for a script to unlock it.
4. **Prefer transform / opacity for performance**: prefer lightweight uses of `transform`, `opacity`, and `filter`; do not animate `width`, `height`, `top`, `left`, `margin`, or other properties that trigger layout jitter.
5. **Accessibility is required**: provide a `prefers-reduced-motion` fallback; when motion is reduced, preserve the final layout, content order, and visual hierarchy.
6. **One motion language per page**: reuse one entrance reveal and one hover/active feedback pattern on the same page. Avoid a different animation for every component.
7. **Static JS belongs to S4**: interactions that do not depend on APIs, such as mobile menus, FAQs, tabs, galleries, and static filters, must be completed in the first static round. Static JS must not use `fetch`, `k.*`, real submissions, persistence, or business APIs.

## Recommended Patterns

### 1. Orchestrated First-Screen Load

Suitable for a Hero, first proof strip, first-screen CTA, and the first product cards. One carefully orchestrated page load is more effective than scattered micro-interactions everywhere.

```html
<section class="kb-motion-scope">
  <div class="kb-reveal" style="--kb-delay: 0ms">...</div>
  <div class="kb-reveal" style="--kb-delay: 90ms">...</div>
  <div class="kb-reveal" style="--kb-delay: 180ms">...</div>
</section>
```

```css
.kb-motion-scope .kb-reveal {
  animation: kb-reveal-up 560ms cubic-bezier(.16, 1, .3, 1) both;
  animation-delay: var(--kb-delay, 0ms);
}

@keyframes kb-reveal-up {
  from {
    opacity: 0;
    transform: translateY(18px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@media (prefers-reduced-motion: reduce) {
  .kb-motion-scope .kb-reveal {
    animation: none;
  }
}
```

Constraints:

- Keep the total duration of one stagger group within about 700ms.
- Increase delays by about 40-90ms; do not create waits longer than 1s.
- Do not write complex motion during the skeleton Page stage; add it after the real View is complete.

### 2. Narrative Hover

Suitable for portfolio cards, product cards, service cards, and project cards. Hover should reveal useful information or reinforce focus, not merely shake the element.

```html
<article class="kb-project-card group">
  <img src="/_api/v2/AiSiteBuilder/ImagePlaceholder?keyword=interior%20project&orientation=landscape&size=regular" alt="Interior project" class="kb-project-image">
  <div class="kb-project-meta">
    <p>Residential / 2026</p>
    <h3>Courtyard renovation</h3>
  </div>
</article>
```

```css
.kb-project-card {
  overflow: hidden;
}

.kb-project-image,
.kb-project-meta {
  transition:
    transform 220ms cubic-bezier(.16, 1, .3, 1),
    opacity 220ms cubic-bezier(.16, 1, .3, 1);
}

.kb-project-card:hover .kb-project-image {
  transform: scale(1.035);
}

.kb-project-meta {
  opacity: .88;
}

.kb-project-card:hover .kb-project-meta {
  opacity: 1;
  transform: translateY(-2px);
}

@media (hover: none) {
  .kb-project-card:hover .kb-project-image,
  .kb-project-card:hover .kb-project-meta {
    transform: none;
  }
}
```

Constraints:

- Price, CTA, and key explanations must not appear only after hover; mobile has no hover.
- Do not use a grid-card `hover:scale-105` that pushes neighboring cards. If scaling is needed, scale only the inner image or overlay.
- Interactive elements must retain `focus-visible` styling; hover alone is insufficient.

### 3. Scroll Reveal as Progressive Enhancement

Suitable for the middle and later parts of a long page, but complex JS is not the default during static site building. When scroll triggering is needed, prefer CSS / lightweight progressive enhancement and keep content visible by default.

```css
@supports (animation-timeline: view()) {
  @media (prefers-reduced-motion: no-preference) {
    .kb-scroll-reveal {
      animation: kb-reveal-up both;
      animation-timeline: view();
      animation-range: entry 10% cover 28%;
    }
  }
}
```

Constraints:

- Scroll-jacking is forbidden.
- Content must not require external JS in order to be visible.
- Do not use strong parallax or flashy reveals in high-trust contexts such as medical, legal, or financial forms.

### 4. Static JS Presentation Interactions

Suitable for mobile menus, FAQ accordions, tabs, galleries/carousels, and static filter chips. Change only the current DOM's classes, ARIA states, focus, and visibility; do not request data, submit, or persist.

```html
<section class="kb-faq" data-kb-faq>
  <button class="kb-faq-trigger" type="button" aria-expanded="false" aria-controls="kb-faq-panel-1">
    Delivery options
  </button>
  <div id="kb-faq-panel-1" class="kb-faq-panel" hidden>
    <p>Choose pickup, local courier, or scheduled delivery at checkout.</p>
  </div>
</section>
```

```html
<script>
document.querySelectorAll('[data-kb-faq]').forEach(function (root) {
  root.querySelectorAll('.kb-faq-trigger').forEach(function (trigger) {
    trigger.addEventListener('click', function () {
      var panel = document.getElementById(trigger.getAttribute('aria-controls'));
      var expanded = trigger.getAttribute('aria-expanded') === 'true';
      trigger.setAttribute('aria-expanded', String(!expanded));
      if (panel) panel.hidden = expanded;
    });
  });
});
</script>
```

Constraints:

- JS must select elements by View or component namespace to avoid affecting the whole site.
- Key content must not appear only after hover; touch and keyboard users must be able to access it.
- Use basic accessibility states such as `aria-expanded`, `aria-controls`, `hidden`, and `focus-visible`.
- When an interaction is reused across pages, prefer a shared Script; when it belongs to one View, it may be placed at the bottom of that View.

## Kooboo Organization

### Local Motion inside a View

Package page-specific motion with its corresponding View:

- Use a namespace in class names, such as `kb-home-hero-*` and `kb-product-grid-*`.
- Use a unique prefix for keyframes, such as `kb-home-hero-reveal`.
- Do not write generic `.card`, `.btn`, or `.reveal` rules that affect the whole site.

### Site-Wide Shared Motion

Put CSS in global CSS created by `create_style` only when it must be reused site-wide:

- shared reveal utility classes;
- shared easing / duration tokens;
- reduced-motion fallback;
- basic button active / focus / hover feedback.

Example:

```css
:root {
  --kb-ease-out: cubic-bezier(.16, 1, .3, 1);
  --kb-motion-fast: 160ms;
  --kb-motion-base: 240ms;
}

@media (prefers-reduced-motion: reduce) {
  *, *::before, *::after {
    animation-duration: 1ms !important;
    animation-iteration-count: 1 !important;
    scroll-behavior: auto !important;
    transition-duration: 1ms !important;
  }
}
```

## Use in the Site-Building Flow

- S3: `ui-ux-pro-max-pp` decides only whether motion is needed, its intensity, and the applicable components.
- S4 step ④: put only lightweight placeholders in the Page skeleton; do not write final motion.
- S4 step ⑤: when generating the real View, if the block's design intent includes motion or static interaction, add local CSS / native JS according to this document, or use global motion tokens / a shared Script.
- S4 step ⑥: during refinement, check that motion is consistent and not excessive, that a reduced-motion fallback exists, and that static interactions such as mobile menus, FAQs, tabs, and galleries work.
- S9: handle only business dynamics that depend on real data, server APIs, submission, persistence, authentication, orders, or payment; do not defer static presentation interactions that belong in S4 to S9.

## Forbidden Patterns

| Forbidden | Reason |
|------|------|
| Introduce Framer Motion / GSAP into a static Kooboo View | Static site building does not need framework dependencies and this adds failure surface |
| `opacity: 0` while waiting for JS to unlock all content | A script failure creates a blank page |
| Large numbers of `animate-bounce` / `animate-pulse` decorative elements | Cheap-looking and distracting |
| Animate `width` / `height` / `top` / `left` | Triggers layout and repaint; mobile can stutter |
| Show prices, CTAs, FAQ answers, or other key information only on hover | Touch and keyboard users cannot reliably access it |
| Use the same flashy motion system for every industry | Motion intensity must match business trust and brand voice |
| Defer FAQ/tabs/menu/gallery static interactions to dynamicization | The static version should be an experienceable page, not an inert screenshot |
| Call `fetch`, `k.*`, or real submission from static JS | This is data/API dynamicization and belongs to S6-S9 |

## Self-Check

- [ ] Does motion serve content entry order, state feedback, or visual narrative?
- [ ] Does it use only low-cost properties such as transform / opacity?
- [ ] Is there a `prefers-reduced-motion` fallback?
- [ ] Is content still fully visible when JS fails or reduced motion is enabled?
- [ ] Do mobile and touch scenarios avoid depending on hover?
- [ ] Does the same page use a consistent easing, duration, and reveal language?
- [ ] Are menu, FAQ, tabs, gallery, and static-filter interactions that do not depend on the backend complete in S4?
- [ ] Does static JS avoid `fetch`, `k.*`, real submission, persistence, and business APIs?
