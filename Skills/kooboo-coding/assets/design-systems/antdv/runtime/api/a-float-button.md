# AFloatButton

Ant Design Vue 4.2.6; native export `FloatButton`. Import `AFloatButton` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `icon`: slot; default -. Set the icon component of button
- `description`: string | slot; default -. Text and other
- `tooltip`: string | slot; default unspecified. The text shown in the tooltip
- `type`: `default` | `primary`; default `default`. Setting button type
- `shape`: `circle` | `square`; default `circle`. Setting button shape
- `href`: string; default -. The target of hyperlink
- `target`: string; default -. Specifies where to display the linked URL
- `badge`: [BadgeProps](/components/badge#api); default -. Attach Badge to FloatButton. `status` and other props related are not supported.

## Events

- `click`: Set the handler to handle `click` event

## Slots

- `icon`: Set the icon component of button
- `description`: Text and other
- `tooltip`: The text shown in the tooltip

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<div style="min-height:100px"><p>浮动按钮位于右下角</p><AFloatButtonGroup style="right:32px;bottom:32px"><AFloatButton description="帮助" @click="event = '帮助'"/><ABackTop/></AFloatButtonGroup><p>{{ event }}</p></div>
```

Source: pinned upstream es/components.js, es/float-button, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
