# ATour

Ant Design Vue 4.2.6; native export `Tour`. Import `ATour` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `arrow`: `boolean`|`{ pointAtCenter: boolean}`; default `true`. Whether to show the arrow, including the configuration whether to point to the center of the element
- `placement`: `left` `leftTop` `leftBottom` `right` `rightTop` `rightBottom` `top` `topLeft` `topRight` `bottom` `bottomLeft` `bottomRight`; default `bottom`. Position of the guide card relative to the target element
- `mask`: `boolean` | `{ style?: CSSProperties; color?: string; }`; default `true`. Whether to enable masking, change mask style and fill color by pass custom props
- `type`: `default` `primary`; default `default`. Type, affects the background color and text color
- `open`: `boolean`; default -. Open tour
- `current(v-model)`: `number`; default -. What is the current step
- `scrollIntoViewOptions`: `boolean` | `ScrollIntoViewOptions`; default `true`. support pass custom scrollIntoView options
- `indicatorsRender`: `v-slot:indicatorsRender="{current, total}"`; default -. custom indicator
- `zIndex`: `number`; default `1001`. Tour's zIndex

## Events

- `close`: Callback function on shutdown
- `finish`: Callback function on finished
- `change`: Callback when the step changes. Current is the previous step

## Slots

- `indicatorsRender`: custom indicator

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<AButton @click="open = true">开始引导</AButton><AButton ref="tourTarget">引导目标</AButton><ATour v-model:open="open" :steps="[{title:'操作入口',description:'在这里处理订单',target:() => tourTarget?.$el}]"/>
```

Source: pinned upstream es/components.js, es/tour, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
