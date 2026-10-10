# AFlex

Ant Design Vue 4.2.6; native export `Flex`. Import `AFlex` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `vertical`: boolean; default `false`. Is direction of the flex vertical, use `flex-direction: column`
- `wrap`: reference [flex-wrap](https://developer.mozilla.org/en-US/docs/Web/CSS/flex-wrap); default nowrap. Set whether the element is displayed in a single line or in multiple lines
- `justify`: reference [justify-content](https://developer.mozilla.org/en-US/docs/Web/CSS/justify-content); default normal. Sets the alignment of elements in the direction of the main axis
- `align`: reference [align-items](https://developer.mozilla.org/en-US/docs/Web/CSS/align-items); default normal. Sets the alignment of elements in the direction of the cross axis
- `flex`: reference [flex](https://developer.mozilla.org/en-US/docs/Web/CSS/flex); default normal. flex CSS shorthand properties
- `gap`: `small` | `middle` | `large` | string | number; default -. Sets the gap between grids
- `component`: Component; default `div`. custom element type

## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<AFlex gap="middle" wrap="wrap"><AButton>一</AButton><AButton>二</AButton><AButton>三</AButton></AFlex>
```

Source: pinned upstream es/components.js, es/flex, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
