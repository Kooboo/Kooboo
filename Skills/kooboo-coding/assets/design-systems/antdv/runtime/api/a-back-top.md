# ABackTop

Ant Design Vue 4.2.6; native export `BackTop`. Import `ABackTop` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults



## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<div style="min-height:100px"><p>浮动按钮位于右下角</p><AFloatButtonGroup style="right:32px;bottom:32px"><AFloatButton description="帮助" @click="event = '帮助'"/><ABackTop/></AFloatButtonGroup><p>{{ event }}</p></div>
```

Source: pinned upstream es/components.js, es/float-button, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
