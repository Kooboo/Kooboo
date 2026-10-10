# ASlider

Ant Design Vue 4.2.6; native export `Slider`. Import `ASlider` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `autofocus`: boolean; default false. get focus when component mounted
- `disabled`: boolean; default false. If true, the slider will not be intractable.
- `dots`: boolean; default false. Whether the thumb can drag over tick only.
- `handleStyle`: CSSProperties; default -. The style of slider handle
- `included`: boolean; default true. Make effect when `marks` not null, `true` means containment and `false` means coordinative
- `mark`: v-slot:mark; default { point: number, label: any }. Custom tick mark of Slider,
- `marks`: object; default { number: string|VNode } or { number: { style: object, label: string|VNode } } or { number: () => VNode }. Tick mark of Slider, type of key must be `number`, and must in closed interval \[min, max], each mark can declare its own style.
- `max`: number; default 100. The maximum value the slider can slide to
- `min`: number; default 0. The minimum value the slider can slide to.
- `range`: boolean; default false. dual thumb mode
- `reverse`: boolean; default false. reverse the component
- `step`: number|null; default 1. The granularity the slider can step through values. Must greater than 0, and be divided by (max - min) . When `marks` no null, `step` can be `null`.
- `trackStyle`: CSSProperties; default -. The style of slider track
- `value(v-model)`: number|number[]; default unspecified. The value of slider. When `range` is `false`, use `number`, otherwise, use `[number, number]`
- `vertical`: Boolean; default false. If true, the slider will be vertical.
- `tipFormatter`: Function|null; default IDENTITY. Slider will pass its value to `tipFormatter`, and display its value in Tooltip, and hide Tooltip when return value is null.
- `tooltipPlacement`: string; default unspecified. Set Tooltip display position. Ref [`Tooltip`](/components/tooltip/).
- `tooltipOpen`: Boolean; default unspecified. If true, Tooltip will show always, or it will not show anyway, even if dragging or hovering.
- `getTooltipPopupContainer`: Function; default () => document.body. The DOM container of the Tooltip, the default behavior is to create a div element in body.

## Events

- `change`: Callback function that is fired when the user changes the slider's value.
- `afterChange`: Fire when `mouseup` is fired.

## Slots

- `mark`: Custom tick mark of Slider,

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ASlider v-model:value="count"/><p>{{ count }}</p>
```

Source: pinned upstream es/components.js, es/slider, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
