# AInputNumber

Ant Design Vue 4.2.6; native export `InputNumber`. Import `AInputNumber` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `addonAfter`: slot; default -. The label text displayed after (on the right side of) the input field
- `addonBefore`: slot; default -. The label text displayed before (on the left side of) the input field
- `autofocus`: boolean; default false. get focus when component mounted
- `bordered`: boolean; default true. Whether has border style
- `controls`: boolean; default true. Whether to show `+-` controls
- `decimalSeparator`: string; default -. decimal separator
- `defaultValue`: number; default unspecified. initial value
- `disabled`: boolean; default false. disable the input
- `formatter`: function(value: number | string, info: { userTyping: boolean, input: string }): string; default -. Specifies the format of the value presented
- `keyboard`: boolean; default true. If enable keyboard behavior
- `max`: number; default Infinity. max value
- `min`: number; default -Infinity. min value
- `parser`: function( string): number; default -. Specifies the value extracted from formatter
- `precision`: number; default -. precision of input value
- `prefix`: slot; default -. The prefix icon for the Input
- `size`: string; default -. height of input box
- `status`: 'error' | 'warning'; default -. Set validation status
- `step`: number|string; default 1. The number to which the current value is increased or decreased. It can be an integer or decimal.
- `stringMode`: boolean; default false. Set value as string to support high precision decimals. Will return string value by `change`
- `value(v-model)`: number; default unspecified. current value
- `upIcon`: slot; default `<UpOutlined />`. custom up icon
- `downIcon`: slot; default `<DownOutlined />`. custom up down

## Events

- `change`: The callback triggered when the value is changed.
- `pressEnter`: The callback function that is triggered when Enter key is pressed.
- `step`: The callback function that is triggered when click up or down buttons

## Slots

- `addonAfter`: The label text displayed after (on the right side of) the input field
- `addonBefore`: The label text displayed before (on the left side of) the input field
- `prefix`: The prefix icon for the Input
- `upIcon`: custom up icon
- `downIcon`: custom up down

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<AInputNumber v-model:value="count" :min="0" :max="100"/><p>{{ count }}</p>
```

Source: pinned upstream es/components.js, es/input-number, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
