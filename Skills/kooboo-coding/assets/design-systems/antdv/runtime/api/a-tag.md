# ATag

Ant Design Vue 4.2.6; native export `Tag`. Import `ATag` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `closable`: boolean; default `false`. Whether the Tag can be closed
- `closeIcon`: VNode | slot; default -. Custom close icon
- `color`: string; default -. Color of the Tag
- `icon`: VNode | slot; default -. Set the icon of tag
- `bordered`: boolean; default `true`. Whether has border style
- `close`: (e) => void; default unspecified. Callback executed when tag is closed

## Events

No events listed in the pinned upstream metadata.

## Slots

- `closeIcon`: Custom close icon
- `icon`: Set the icon of tag

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ASpace><ATag color="success">已完成</ATag><ATag color="warning">待处理</ATag><ATag closable>可移除</ATag><ACheckableTag v-model:checked="checked">可选 {{ checked }}</ACheckableTag></ASpace>
```

Source: pinned upstream es/components.js, es/tag, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
