# ACollapse

Ant Design Vue 4.2.6; native export `Collapse`. Import `ACollapse` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `accordion`: boolean; default `false`. If `true`, `Collapse` renders as `Accordion`
- `activeKey(v-model)`: string[] | string <br> number[] | number; default No default value. In [accordion mode](#components-collapse-demo-accordion), it's the key of the first panel.. Key of the active panel
- `bordered`: boolean; default `true`. Toggles rendering of the border around the collapse block
- `collapsible`: `header` | `icon` | `disabled`; default -. Specify whether the panels of children be collapsible or the trigger area of collapsible
- `destroyInactivePanel`: boolean; default `false`. Destroy Inactive Panel
- `expandIcon`: Function(props):VNode | v-slot:expandIcon="props"; default unspecified. allow to customize collapse icon
- `expandIconPosition`: `start` | `end`; default -. Set expand icon position
- `ghost`: boolean; default false. Make the collapse borderless and its background transparent

## Events

- `change`: Callback function executed when active panel is changed

## Slots

- `expandIcon`: allow to customize collapse icon

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ACollapse v-model:activeKey="keys"><ACollapsePanel key="1" header="基本信息">可展开内容</ACollapsePanel><ACollapsePanel key="2" header="详细信息">详细内容</ACollapsePanel></ACollapse>
```

Source: pinned upstream es/components.js, es/collapse, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
