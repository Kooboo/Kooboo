# ACollapsePanel

Ant Design Vue 4.2.6; native export `CollapsePanel`. Import `ACollapsePanel` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `collapsible`: `header` | `disabled`; default -. Specify whether the panel be collapsible or the trigger area of collapsible
- `disabled`: boolean; default `false`. If `true`, panel cannot be opened or closed
- `extra`: VNode | slot; default -. extra element in the corner
- `forceRender`: boolean; default `false`. Forced render of content on panel, instead of lazy rending after clicking on header
- `header`: string | slot; default -. Title of the panel
- `key`: string | number; default -. Unique key identifying the panel from among its siblings
- `showArrow`: boolean; default `true`. If `false`, panel will not show arrow icon

## Events

No events listed in the pinned upstream metadata.

## Slots

- `extra`: extra element in the corner
- `header`: Title of the panel

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ACollapse v-model:activeKey="keys"><ACollapsePanel key="1" header="基本信息">可展开内容</ACollapsePanel><ACollapsePanel key="2" header="详细信息">详细内容</ACollapsePanel></ACollapse>
```

Source: pinned upstream es/components.js, es/collapse, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
