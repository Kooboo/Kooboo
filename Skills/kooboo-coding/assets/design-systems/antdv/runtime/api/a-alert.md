# AAlert

Ant Design Vue 4.2.6; native export `Alert`. Import `AAlert` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `action`: slot; default -. The action of Alert
- `afterClose`: () => void; default -. Called when close animation is finished
- `banner`: boolean; default false. Whether to show as banner
- `closable`: boolean; default unspecified. Whether Alert can be closed
- `closeIcon`: slot; default `<CloseOutlined />`. Custom close icon
- `closeText`: string|slot; default -. Close text to show
- `description`: string|slot; default -. Additional content of Alert
- `icon`: vnode | slot; default -. Custom icon, effective when `showIcon` is `true`
- `message`: string|slot; default -. Content of Alert
- `showIcon`: boolean; default false,in `banner` mode default is true. Whether to show icon
- `type`: string; default `info`,in `banner` mode default is `warning`. Type of Alert styles, options: `success`, `info`, `warning`, `error`

## Events

- `close`: Callback when Alert is closed

## Slots

- `action`: The action of Alert
- `closeIcon`: Custom close icon
- `closeText`: Close text to show
- `description`: Additional content of Alert
- `icon`: Custom icon, effective when `showIcon` is `true`
- `message`: Content of Alert

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<AAlert message="保存成功" description="变更已保存在示例状态中" type="success" show-icon closable/><AAlert message="请求失败，请重试" type="error" show-icon/>
```

Source: pinned upstream es/components.js, es/alert, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
