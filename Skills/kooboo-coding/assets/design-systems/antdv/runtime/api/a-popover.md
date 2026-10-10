# APopover

Ant Design Vue 4.2.6; native export `Popover`. Import `APopover` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Use v-model:open, trigger="click" or hover, and #content. The trigger is the default slot (not Element Plus #reference). placement uses bottomRight etc. Let the component own trigger toggling.

## Props and defaults

- `content`: string|slot|vNode; default -. Content of the card
- `title`: string|slot|VNode; default -. Title of the card

## Events

No events listed in the pinned upstream metadata.

## Slots

- `content`: Content of the card
- `title`: Title of the card

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<APopover title="帮助" trigger="click"><AButton>查看帮助</AButton><template #content><p>点击外部关闭</p></template></APopover>
```

Source: pinned upstream es/components.js, es/popover, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
