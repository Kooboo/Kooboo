# APopconfirm

Ant Design Vue 4.2.6; native export `Popconfirm`. Import `APopconfirm` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Use @confirm/@cancel; async persistence and loading belong to business code. Trigger in default slot; do not treat cancel as request failure.

## Props and defaults

- `cancelButton`: slot; default -. custom render cancel button
- `cancelButtonProps`: [ButtonProps](/components/button/#api); default -. The cancel button props
- `cancelText`: string|slot; default `Cancel`. text of the Cancel button
- `disabled`: boolean; default false. is show popconfirm when click its childrenNode
- `icon`: vNode|slot; default &lt;Icon type="exclamation-circle" />. customize icon of confirmation
- `okButton`: slot; default -. custom render confirm button
- `okButtonProps`: [ButtonProps](/components/button/#api); default -. The ok button props
- `okText`: string|slot; default `Confirm`. text of the Confirm button
- `okType`: string; default `primary`. Button `type` of the Confirm button
- `showCancel`: boolean; default true. Show cancel button
- `title`: string|slot; default -. title of the confirmation box
- `description`: string|slot; default -. The description of the confirmation box title
- `visible (v-model)`: boolean; default -. hide or show

## Events

- `cancel`: callback of cancel
- `confirm`: callback of confirmation
- `openChange`: Callback executed when visibility of the tooltip card is changed

## Slots

- `cancelButton`: custom render cancel button
- `cancelText`: text of the Cancel button
- `icon`: customize icon of confirmation
- `okButton`: custom render confirm button
- `okText`: text of the Confirm button
- `title`: title of the confirmation box
- `description`: The description of the confirmation box title

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<APopconfirm title="确认删除？" @confirm="event = '已确认'" @cancel="event = '已取消'"><AButton danger>删除</AButton></APopconfirm><p>{{ event }}</p>
```

Source: pinned upstream es/components.js, es/popconfirm, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
