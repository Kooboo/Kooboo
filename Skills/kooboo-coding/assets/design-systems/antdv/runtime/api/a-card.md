# ACard

Ant Design Vue 4.2.6; native export `Card`. Import `ACard` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `activeTabKey`: string; default -. Current TabPane's key
- `bodyStyle`: object; default -. Inline style to apply to the card content
- `bordered`: boolean; default `true`. Toggles rendering of the border around the card
- `defaultActiveTabKey`: string; default -. Initial active TabPane's key, if `activeTabKey` is not set.
- `extra`: string|slot; default -. Content to render in the top-right corner of the card
- `headStyle`: object; default -. Inline style to apply to the card head
- `hoverable`: boolean; default false. Lift up when hovering card
- `loading`: boolean; default false. Shows a loading indicator while the contents of the card are being fetched
- `size`: `default` | `small`; default `default`. Size of card
- `tabList`: Array&lt;{key: string, tab: any}>; default -. List of TabPane's head, Custom tabs with the customTab(v3.0) slot
- `title`: string|slot; default -. Card title
- `type`: string; default -. Card style type, can be set to `inner` or not set
- `actions`: -; default unspecified. The action list, shows at the bottom of the Card.
- `cover`: -; default unspecified. Card cover
- `customTab`: { item: tabList[number] }; default unspecified. custom tabList tab
- `extra`: -; default unspecified. Content to render in the top-right corner of the card
- `tabBarExtraContent`: -; default unspecified. Extra content in tab bar
- `title`: -; default unspecified. Card title

## Events

- `tabChange`: Callback when tab is switched

## Slots

- `extra`: Content to render in the top-right corner of the card
- `title`: Card title

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ACard title="项目" style="max-width:400px"><template #extra><AButton type="link" @click="count++">操作 {{ count }}</AButton></template><ACardMeta title="订单服务" description="本月运行状态"/><ACardGrid style="width:50%">正常</ACardGrid><ACardGrid style="width:50%">待处理</ACardGrid></ACard>
```

Source: pinned upstream es/components.js, es/card, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
