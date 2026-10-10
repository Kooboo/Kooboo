# AQRCode

Ant Design Vue 4.2.6; native export `QRCode`. Import `AQRCode` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `value`: string; default -. scanned link
- `type`: `'canvas'` | `'svg'`; default `canvas`. render type
- `icon`: string; default -. include image url (only image link are supported)
- `size`: number; default 128. QRCode size
- `iconSize`: number; default 32. include image size
- `color`: string; default `#000`. QRCode Color
- `bgColor`: string; default `transparent`. QRCode Background Color
- `bordered`: boolean; default `true`. Whether has border style
- `errorLevel`: `'L'` | `'M'` | `'Q'` | `'H'`; default `'M'`. Error Code Level
- `status`: `active` | `expired` | `loading` | `scanned`; default `active`. QRCode status

## Events

- `refresh`: callback

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<AQRCode value="https://www.antdv.com"/><p>Ant Design Vue 官方文档二维码</p>
```

Source: pinned upstream es/components.js, es/qrcode, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
