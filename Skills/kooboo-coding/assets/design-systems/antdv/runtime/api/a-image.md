# AImage

Ant Design Vue 4.2.6; native export `Image`. Import `AImage` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `alt`: string; default -. Image description
- `fallback`: string; default -. Load failure fault-tolerant src
- `height`: string | number; default -. Image height
- `placeholder`: boolean | slot; default -. Load placeholder, use default placeholder when set `true`
- `preview`: boolean | [previewType](#previewtype); default true. preview config, disabled when `false`
- `src`: string; default -. Image path
- `previewMask`: false | function | slot; default -. custom mask
- `width`: string | number; default -. Image width

## Events

- `error`: Load failed callback

## Slots

- `placeholder`: Load placeholder, use default placeholder when set `true`
- `previewMask`: custom mask

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<AImagePreviewGroup><AImage :width="120" :src="imageUrl" alt="示例色块"/></AImagePreviewGroup>
```

Source: pinned upstream es/components.js, es/image, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
