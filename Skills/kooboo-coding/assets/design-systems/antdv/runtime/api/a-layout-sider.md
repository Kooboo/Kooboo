# ALayoutSider

Ant Design Vue 4.2.6; native export `LayoutSider`. Import `ALayoutSider` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Layout/Header/Sider/Content/Footer compose freely. Content height grows by default; choose viewport scrolling explicitly. KbAntShell is an optional workspace preset; route layout belongs in layout.default.js, never app.js.

## Props and defaults

- `breakpoint`: `xs` | `sm` | `md` | `lg` | `xl` | `xxl`; default -. [breakpoints](/components/grid#api) of the responsive layout
- `class`: string; default -. container className
- `collapsed(v-model)`: boolean; default -. to set the current status
- `collapsedWidth`: number; default 80. width of the collapsed sidebar, by setting to `0` a special trigger will appear
- `collapsible`: boolean; default false. whether can be collapsed
- `defaultCollapsed`: boolean; default false. to set the initial status
- `reverseArrow`: boolean; default false. reverse direction of arrow, for a sider that expands from the right
- `style`: object|string; default -. to customize the styles
- `theme`: `light` | `dark`; default `dark`. color theme of the sidebar
- `trigger`: string|slot; default -. specify the customized trigger, set to null to hide the trigger
- `width`: number|string; default 200. width of the sidebar
- `zeroWidthTriggerStyle`: object; default -. to customize the styles of the special trigger that appears when `collapsedWidth` is 0

## Events

No events listed in the pinned upstream metadata.

## Slots

- `trigger`: specify the customized trigger, set to null to hide the trigger

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ALayout style="min-height:260px"><ALayoutHeader style="color:white">顶部导航</ALayoutHeader><ALayout><ALayoutSider><AMenu theme="dark" :items="menuItems"/></ALayoutSider><ALayoutContent style="padding:24px">独立布局区域</ALayoutContent></ALayout><ALayoutFooter>页脚</ALayoutFooter></ALayout>
```

Source: pinned upstream es/components.js, es/layout, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
