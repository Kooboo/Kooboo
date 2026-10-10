# AComment

Ant Design Vue 4.2.6; native export `Comment`. Import `AComment` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `actions`: Array | slot; default -. List of action items rendered below the comment content
- `author`: string|slot; default -. The element to display as the comment author
- `avatar`: string|slot; default -. The element to display as the comment avatar - generally an antd `Avatar` or src
- `content`: string|slot; default -. The main content of the comment
- `datetime`: string|slot; default -. A datetime element containing the time to be displayed

## Events

No events listed in the pinned upstream metadata.

## Slots

- `actions`: List of action items rendered below the comment content
- `author`: The element to display as the comment author
- `avatar`: The element to display as the comment avatar - generally an antd `Avatar` or src
- `content`: The main content of the comment
- `datetime`: A datetime element containing the time to be displayed

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<AComment author="陈" content="请核对订单" datetime="今天"><template #avatar><AAvatar>陈</AAvatar></template><AComment author="李" content="已核对"/></AComment>
```

Source: pinned upstream es/components.js, es/comment, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
