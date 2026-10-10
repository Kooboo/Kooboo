# ASpace

Ant Design Vue 4.2.6; native export `Space`. Import `ASpace` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `align`: `start` | `end` |`center` |`baseline`; default -. Align items
- `direction`: `vertical` | `horizontal`; default `horizontal`. The space direction
- `size`: `small` | `middle` | `large` | `number`; default `small`. The space size
- `split`: VueNode | v-slot; default -. Set split
- `wrap`: boolean; default false. Auto wrap line, when `horizontal` effective

## Events

No events listed in the pinned upstream metadata.

## Slots

- `split`: Set split

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ASpace wrap><AButton>第一项</AButton><AButton>第二项</AButton></ASpace><ACompact><AInput v-model:value="text"/><AButton @click="event = text">提交</AButton></ACompact><p>{{ event }}</p>
```

Source: pinned upstream es/components.js, es/space, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
