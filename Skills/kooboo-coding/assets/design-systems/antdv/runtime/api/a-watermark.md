# AWatermark

Ant Design Vue 4.2.6; native export `Watermark`. Import `AWatermark` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `width`: number; default 120. The width of the watermark, the default value of `content` is its own width
- `height`: number; default 64. The height of the watermark, the default value of `content` is its own height
- `rotate`: number; default -22. When the watermark is drawn, the rotation Angle, unit `°`
- `zIndex`: number; default 9. The z-index of the appended watermark element
- `image`: string; default -. Image source, it is recommended to export 2x or 3x image, high priority
- `content`: string | string[]; default -. Watermark text content
- `font`: [Font](#font); default [Font](#font). Text style
- `gap`: [number, number]; default \[100, 100\]. The spacing between watermarks
- `offset`: [number, number]; default \[gap\[0\]/2, gap\[1\]/2\]. The offset of the watermark from the upper left corner of the container. The default is `gap/2`

## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<AWatermark content="内部资料"><div style="height:220px">水印内容区</div></AWatermark>
```

Source: pinned upstream es/components.js, es/watermark, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
