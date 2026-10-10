# ACarousel

Ant Design Vue 4.2.6; native export `Carousel`. Import `ACarousel` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `autoplay`: boolean; default `false`. Whether to scroll automatically
- `dotPosition`: string; default `bottom`. The position of the dots, which can be one of `top` `bottom` `left` `right`
- `dots`: boolean; default `true`. Whether to show the dots at the bottom of the gallery
- `dotsClass`: string; default `slick-dots`. Class name of the dots
- `easing`: string; default `linear`. Transition interpolation function name
- `effect`: `scrollx` | `fade`; default `scrollx`. Transition effect
- `afterChange`: function(current); default -. Callback function called after the current index changes
- `beforeChange`: function(from, to); default -. Callback function called before the current index changes

## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ACarousel arrows><div class="demo-slide">第一页</div><div class="demo-slide">第二页</div></ACarousel>
```

Source: pinned upstream es/components.js, es/carousel, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
