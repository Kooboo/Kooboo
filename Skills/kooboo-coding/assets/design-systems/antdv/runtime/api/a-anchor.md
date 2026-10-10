# AAnchor

Ant Design Vue 4.2.6; native export `Anchor`. Import `AAnchor` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `affix`: boolean; default true. Fixed mode of Anchor
- `bounds`: number; default 5(px). Bounding distance of anchor area
- `getContainer`: () => HTMLElement; default () => window. Scrolling container
- `getCurrentAnchor`: (activeLink: string) => string; default -. Customize the anchor highlight
- `offsetBottom`: number; default -. Pixels to offset from bottom when calculating position of scroll
- `offsetTop`: number; default 0. Pixels to offset from top when calculating position of scroll
- `showInkInFixed`: boolean; default false. Whether show ink-square when `：affix="false"`
- `targetOffset`: number; default `offsetTop`. Anchor scroll offset, default as `offsetTop`, [example](#components-anchor-demo-targetoffset)
- `wrapperClass`: string; default -. The class name of the container
- `wrapperStyle`: object; default -. The style of the container
- `items`: { key, href, title, target, children }[] [see](#anchoritem); default -. Data configuration option content, support nesting through children
- `direction`: `vertical` | `horizontal`; default `vertical`. Set Anchor direction
- `customTitle`: v-slot="AnchorItem"; default -. custom option title by slot

## Events

No events listed in the pinned upstream metadata.

## Slots

- `customTitle`: custom option title by slot

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<AAnchor :affix="false" :items="[{key:1,href:'#details',title:'详情'}]"/><section id="details">锚点目标</section>
```

Source: pinned upstream es/components.js, es/anchor, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
