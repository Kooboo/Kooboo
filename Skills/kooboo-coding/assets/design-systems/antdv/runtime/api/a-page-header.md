# APageHeader

Ant Design Vue 4.2.6; native export `PageHeader`. Import `APageHeader` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `avatar`: [avatar props](/components/avatar/); default -. Avatar next to the title bar
- `backIcon`: string|slot; default `<ArrowLeft />`. custom back icon, if false the back icon will not be displayed
- `breadcrumb`: [breadcrumb](/components/breadcrumb/); default -. Breadcrumb configuration
- `extra`: string|slot; default -. Operating area, at the end of the line of the title line
- `footer`: string|slot; default -. PageHeader's footer, generally used to render TabBar
- `ghost`: boolean; default true. PageHeader type, will change background color
- `subTitle`: string|slot; default -. custom subTitle text
- `tags`: [Tag](/components/tag/)[] | [Tag](/components/tag/); default -. Tag list next to title
- `title`: string|slot; default -. custom title text

## Events

No events listed in the pinned upstream metadata.

## Slots

- `backIcon`: custom back icon, if false the back icon will not be displayed
- `extra`: Operating area, at the end of the line of the title line
- `footer`: PageHeader's footer, generally used to render TabBar
- `subTitle`: custom subTitle text
- `title`: custom title text

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<APageHeader title="订单 PO-001" sub-title="采购详情" @back="event = '返回'"><template #extra><AButton @click="count++">保存 {{ count }}</AButton></template><p>页头属于业务内容，不重复全局标题。</p></APageHeader><p>{{ event }}</p>
```

Source: pinned upstream es/components.js, es/page-header, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
