# AAvatar

Ant Design Vue 4.2.6; native export `Avatar`. Import `AAvatar` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `alt`: string; default -. This attribute defines the alternative text describing the image
- `crossOrigin`: `'anonymous'` | `'use-credentials'` | `''`; default -. cors settings attributes
- `draggable`: boolean | `'true'` | `'false'`; default -. Whether the picture is allowed to be dragged
- `gap`: number; default 4. Letter type unit distance between left and right sides
- `icon`: VNode | slot; default -. the `Icon` type for an icon avatar, see `Icon` Component
- `loadError`: () => boolean; default -. handler when img load error, return false to prevent default fallback behavior
- `shape`: `circle` | `square`; default `circle`. the shape of avatar
- `size`: number | `large` | `small` | `default` | { xs: number, sm: number, ...}; default `default`. The size of the avatar
- `src`: string; default -. the address of the image for an image avatar
- `srcset`: string; default -. a list of sources to use for different screen resolutions

## Events

No events listed in the pinned upstream metadata.

## Slots

- `icon`: the `Icon` type for an icon avatar, see `Icon` Component

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<AAvatarGroup><AAvatar style="background:#1677ff">陈</AAvatar><AAvatar>李</AAvatar></AAvatarGroup>
```

Source: pinned upstream es/components.js, es/avatar, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
