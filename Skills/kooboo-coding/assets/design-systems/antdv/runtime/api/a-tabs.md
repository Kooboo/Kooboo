# ATabs

Ant Design Vue 4.2.6; native export `Tabs`. Import `ATabs` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `activeKey(v-model)`: string; default -. Current TabPane's key
- `animated`: boolean | {inkBar:boolean, tabPane:boolean}; default `true`, `false` when `type="card"`. Whether to change tabs with animation. Only works while tabPosition=`"top"` | `"bottom"`
- `destroyInactiveTabPane`: boolean; default false. Whether destroy inactive TabPane when change tab
- `hideAdd`: boolean; default `false`. Hide plus icon or not. Only works while `type="editable-card"`
- `size`: `large` | `middle` | `small`; default `middle`. preset tab bar size
- `tabBarGutter`: number; default -. The gap between tabs
- `tabBarStyle`: CSSProperties; default -. Tab bar style object
- `tabPosition`: `top` | `right` | `bottom` | `left`; default `top`. Position of tabs
- `type`: `line` | `card` | `editable-card`; default `line`. Basic style of tabs
- `addIcon`: -; default -. Customize add icon
- `leftExtra`: -; default -. Extra content in tab bar left
- `moreIcon`: -; default -. The custom icon of ellipsis
- `renderTabBar`: { DefaultTabBar }; default unspecified. Replace the TabBar
- `rightExtra`: -; default -. Extra content in tab bar right
- `change`: Function(activeKey) {}; default unspecified. Callback executed when active tab is changed
- `edit`: (action === 'add' ? event : targetKey, action): void; default unspecified. Callback executed when tab is added or removed. Only works while `type="editable-card"`
- `nextClick`: Function; default unspecified. Callback executed when next button is clicked
- `prevClick`: Function; default unspecified. Callback executed when prev button is clicked
- `tabClick`: Function; default unspecified. Callback executed when tab is clicked

## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ATabs v-model:activeKey="tabKey"><ATabPane key="1" tab="概览">概览内容</ATabPane><ATabPane key="2" tab="历史">历史内容</ATabPane></ATabs>
```

Source: pinned upstream es/components.js, es/tabs, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
