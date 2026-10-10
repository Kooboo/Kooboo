# ATooltip

Ant Design Vue 4.2.6; native export `Tooltip`. Import `ATooltip` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `title`: string|slot; default -. The text shown in the tooltip
- `align`: Object; default -. this value will be merged into placement's config, please refer to the settings [dom-align](https://github.com/yiminghe/dom-align)
- `arrowPointAtCenter`: boolean; default `false`. Whether the arrow is pointed at the center of target
- `arrow`: boolean | { pointAtCenter: boolean}; default `true`. Change arrow's visible state and change whether the arrow is pointed at the center of target.
- `autoAdjustOverflow`: boolean; default `true`. Whether to adjust popup placement automatically when popup is off screen
- `color`: string; default -. The background color
- `destroyTooltipOnHide`: boolean; default false. Whether to destroy tooltip on hide
- `getPopupContainer`: (triggerNode: HTMLElement) => HTMLElement; default () => document.body. The DOM container of the tip, the default behavior is to create a `div` element in `body`.
- `mouseEnterDelay`: number; default 0.1. Delay in seconds, before tooltip is shown on mouse enter
- `mouseLeaveDelay`: number; default 0.1. Delay in seconds, before tooltip is hidden on mouse leave
- `overlayClassName`: string; default -. Class name of the tooltip card
- `overlayStyle`: object; default -. Style of the tooltip card
- `overlayInnerStyle`: object; default -. Style of the tooltip inner content
- `placement`: string; default `top`. The position of the tooltip relative to the target, which can be one of `top` `left` `right` `bottom` `topLeft` `topRight` `bottomLeft` `bottomRight` `leftTop` `leftBottom` `rightTop` `rightBottom`
- `trigger`: `hover` | `focus` | `click` | `contextmenu`; default `hover`. Tooltip trigger mode
- `open(v-model)`: boolean; default `false`. Whether the floating tooltip card is open or not, Use `visible` under 4.0.0

## Events

- `openChange`: Callback executed when visibility of the tooltip card is changed

## Slots

- `title`: The text shown in the tooltip

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ATooltip title="这是提示信息"><AButton>悬停或聚焦</AButton></ATooltip>
```

Source: pinned upstream es/components.js, es/tooltip, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
