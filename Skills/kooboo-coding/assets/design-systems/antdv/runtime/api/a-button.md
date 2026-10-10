# AButton

Ant Design Vue 4.2.6; native export `Button`. Import `AButton` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `block`: boolean; default `false`. option to fit button width to its parent width
- `danger`: boolean; default `false`. set the danger status of button
- `disabled`: boolean; default `false`. disabled state of button
- `ghost`: boolean; default `false`. make background transparent and invert text and border colors
- `href`: string; default -. redirect url of link button
- `htmlType`: string; default `button`. set the original html `type` of `button`, see: [MDN](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/button#attr-type)
- `icon`: v-slot; default -. set the icon of button, see: Icon component
- `loading`: boolean | { delay: number }; default `false`. set the loading status of button
- `shape`: `default` | `circle` | `round`; default `default`. Can be set button shape
- `size`: `large` | `middle` | `small`; default `middle`. set the size of button
- `target`: string; default -. same as target attribute of a, works when href is specified
- `type`: `primary` | `ghost` | `dashed` | `link` | `text` | `default`; default `default`. can be set button type

## Events

- `click`: set the handler to handle `click` event

## Slots

- `icon`: set the icon of button, see: Icon component

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ASpace wrap><AButton type="primary" @click="count++">操作 {{ count }}</AButton><AButton disabled>禁用</AButton><AButton loading>加载中</AButton><AButton danger>删除</AButton></ASpace>
```

Source: pinned upstream es/components.js, es/button, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
