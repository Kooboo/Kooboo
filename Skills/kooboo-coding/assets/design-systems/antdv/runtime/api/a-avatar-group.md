# AAvatarGroup

Ant Design Vue 4.2.6; native export `AvatarGroup`. Import `AAvatarGroup` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `maxCount`: number; default -. Max avatars to show
- `maxPopoverPlacement`: `top` | `bottom`; default `top`. The placement of excess avatar Popover
- `maxPopoverTrigger`: `hover` | `focus` | `click`; default `hover`. Set the trigger of excess avatar Popover
- `maxStyle`: CSSProperties; default -. The style of excess avatar style
- `size`: number | `large` | `small` | `default` | { xs: number, sm: number, ...}; default `default`. The size of the avatar
- `shape`: `circle` | `square`; default `circle`. The shape of the avatar

## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<AAvatarGroup><AAvatar style="background:#1677ff">陈</AAvatar><AAvatar>李</AAvatar></AAvatarGroup>
```

Source: pinned upstream es/components.js, es/avatar, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
