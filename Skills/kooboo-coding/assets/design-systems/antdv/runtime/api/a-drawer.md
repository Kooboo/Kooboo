# ADrawer

Ant Design Vue 4.2.6; native export `Drawer`. Import `ADrawer` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Use v-model:open and @close. Native placement: top/right/bottom/left. Width/height apply by placement. Manage unsaved edits before closing. Destroy-on-close may discard local state.

## Props and defaults

- `autofocus`: boolean; default true. Whether Drawer should get focused after open
- `bodyStyle`: CSSProperties; default -. Style of the drawer content part
- `class`: string; default -. Config Drawer Panel className. Use `rootClassName` if want to config top dom style
- `closable`: boolean; default true. Whether a close (x) button is visible on top left of the Drawer dialog or not
- `closeIcon`: VNode | slot; default `<CloseOutlined />`. Custom close icon
- `contentWrapperStyle`: CSSProperties; default -. Style of the drawer wrapper of content part
- `destroyOnClose`: boolean; default false. Whether to unmount child components on closing drawer or not
- `extra`: VNode | slot; default -. Extra actions area at corner
- `footer`: VNode | slot; default -. The footer for Drawer
- `footerStyle`: CSSProperties; default -. Style of the drawer footer part
- `forceRender`: boolean; default false. Prerender Drawer component forcely
- `getContainer`: HTMLElement | `() => HTMLElement` | Selectors; default 'body'. mounted node and display window for Drawer
- `headerStyle`: CSSProperties; default -. Style of the drawer header part
- `height`: string | number; default 378. Placement is `top` or `bottom`, height of the Drawer dialog
- `keyboard`: boolean; default true. Whether support press esc to close
- `mask`: Boolean; default true. Whether to show mask or not
- `maskClosable`: boolean; default true. Clicking on the mask (area outside the Drawer) to close the Drawer or not
- `maskStyle`: CSSProperties; default {}. Style for Drawer's mask element
- `placement`: 'top' | 'right' | 'bottom' | 'left'; default 'right'. The placement of the Drawer
- `push`: boolean | {distance: string | number}; default { distance: 180 }. Nested drawers push behavior
- `rootClassName`: string; default -. The class name of the container of the Drawer dialog
- `rootStyle`: CSSProperties; default -. Style of wrapper element which **contains mask** compare to `style`
- `style`: CSSProperties; default -. Style of Drawer panel. Use `bodyStyle` if want to config body only
- `size`: `default` | `large`; default `default`. presetted size of drawer, default `378px` and large `736px`
- `style`: CSSProperties; default -. Style of wrapper element which contains mask compare to drawerStyle
- `title`: string | slot; default -. The title for Drawer
- `open(v-model)`: boolean; default -. Whether the Drawer dialog is visible or not
- `width`: string | number; default 378. Width of the Drawer dialog
- `zIndex`: Number; default 1000. The `z-index` of the Drawer

## Events

No events listed in the pinned upstream metadata.

## Slots

- `closeIcon`: Custom close icon
- `extra`: Extra actions area at corner
- `footer`: The footer for Drawer
- `title`: The title for Drawer

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<AButton @click="open = true">打开抽屉</AButton><ADrawer v-model:open="open" title="编辑订单" placement="right"><AInput v-model:value="text"/><p>{{ text }}</p></ADrawer>
```

Source: pinned upstream es/components.js, es/drawer, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
