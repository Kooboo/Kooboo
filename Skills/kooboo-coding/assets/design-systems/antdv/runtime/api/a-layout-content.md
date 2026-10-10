# ALayoutContent

Ant Design Vue 4.2.6; native export `LayoutContent`. Import `ALayoutContent` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Layout/Header/Sider/Content/Footer compose freely. Content height grows by default; choose viewport scrolling explicitly. KbAntShell is an optional workspace preset; route layout belongs in layout.default.js, never app.js.

## Props and defaults



## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ALayout style="min-height:260px"><ALayoutHeader style="color:white">顶部导航</ALayoutHeader><ALayout><ALayoutSider><AMenu theme="dark" :items="menuItems"/></ALayoutSider><ALayoutContent style="padding:24px">独立布局区域</ALayoutContent></ALayout><ALayoutFooter>页脚</ALayoutFooter></ALayout>
```

Source: pinned upstream es/components.js, es/layout, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
