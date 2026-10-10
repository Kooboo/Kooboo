# ADescriptions

Ant Design Vue 4.2.6; native export `Descriptions`. Import `ADescriptions` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `bordered`: boolean; default false. whether to display the border
- `colon`: boolean; default true. change default props `colon` value of `Descriptions.Item`
- `column`: number; default 3. the number of `DescriptionItems` in a row,could be a number or a object like `{ xs: 8, sm: 16, md: 24}`,(Only set `bordered={true}` to take effect)
- `contentStyle`: CSSProperties; default -. Customize content style
- `extra`: string | VNode | slot; default -. The action area of the description list, placed at the top-right
- `labelStyle`: CSSProperties; default -. Customize label style
- `layout`: `horizontal` | `vertical`; default `horizontal`. Define description layout
- `size`: `default` | `middle` | `small`; default `default`. set the size of the list. Can be set to `middle`,`small`, or not filled
- `title`: string | VNode | slot; default -. The title of the description list, placed at the top

## Events

No events listed in the pinned upstream metadata.

## Slots

- `extra`: The action area of the description list, placed at the top-right
- `title`: The title of the description list, placed at the top

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ADescriptions title="订单详情" bordered><ADescriptionsItem label="订单">PO-001</ADescriptionsItem><ADescriptionsItem label="状态">已确认</ADescriptionsItem></ADescriptions>
```

Source: pinned upstream es/components.js, es/descriptions, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
