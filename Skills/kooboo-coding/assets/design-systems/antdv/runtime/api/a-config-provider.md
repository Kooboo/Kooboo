# AConfigProvider

Ant Design Vue 4.2.6; native export `ConfigProvider`. Import `AConfigProvider` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Pass :locale="antZhCN" and :theme="{token:{colorPrimary}}". Components inject their own styles at runtime; antdv.css is reset only. Use theme.darkAlgorithm/compactAlgorithm explicitly. Do not assume Element Plus CSS variables exist.

## Props and defaults

- `autoInsertSpaceInButton`: boolean; default true. Set `false` to remove space between 2 chinese characters on Button
- `componentSize`: `small` | `middle` | `large`; default -. Config antd component size
- `csp`: { nonce: string }; default -. Set [Content Security Policy](https://developer.mozilla.org/en-US/docs/Web/HTTP/CSP) config
- `direction`: `ltr` | `rtl`; default `ltr`. Set direction of layout. See [demo](#components-config-provider-demo-direction)
- `dropdownMatchSelectWidth`: boolean | number; default -. Determine whether the dropdown menu and the select input are the same width. Default set `min-width` same as input. Will ignore when value less than select width. `false` will disable virtual scroll
- `form`: { validateMessages?: [ValidateMessages](/components/form/#validatemessages), requiredMark?: boolean | `optional` }; default -. Set Form common props
- `getPopupContainer`: Function(triggerNode, dialogContext); default `() => document.body`. to set the container of the popup element. The default is to create a `div` element in `body`.
- `getTargetContainer`: () => HTMLElement; default () => window. Config Affix, Anchor scroll target container
- `input`: { autocomplete?: string }; default -. Set Input common props
- `locale`: object; default -. language package setting, you can find the packages in [ant-design-vue/es/locale](http://unpkg.com/ant-design-vue/es/locale/)
- `pageHeader`: { ghost:boolean }; default 'true'. Unify the ghost of pageHeader ,Ref [pageHeader]\(&lt;(/components/page-header)>
- `prefixCls`: string; default ant. set prefix class
- `renderEmpty`: slot-scope | Function(componentName: string): VNode; default -. set empty content of components. Ref [Empty](/components/empty/)
- `space`: { size: `small` | `middle` | `large` | `number` }; default -. Set Space `size`, ref [Space](/components/space)
- `transformCellText`: Function({ text, column, record, index }) => any; default -. Table data can be changed again before rendering. The default configuration of general user empty data.
- `virtual`: boolean; default true. Disable virtual scroll when set to false
- `wave`: { disabled?: boolean }; default -. Config wave effect

## Events

No events listed in the pinned upstream metadata.

## Slots

- `renderEmpty`: set empty content of components. Ref [Empty](/components/empty/)

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<AConfigProvider :theme="{token:{colorPrimary:'#722ed1'}}"><AButton type="primary">局部紫色主题</AButton><AInput placeholder="原生主题令牌"/></AConfigProvider>
```

Source: pinned upstream es/components.js, es/config-provider, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
