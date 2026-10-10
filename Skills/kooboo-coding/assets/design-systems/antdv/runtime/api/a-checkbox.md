# ACheckbox

Ant Design Vue 4.2.6; native export `Checkbox`. Import `ACheckbox` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Checkbox uses v-model:checked; CheckboxGroup uses v-model:value (array). Disabled controls cannot establish business authorization.

## Props and defaults

- `autofocus`: boolean; default false. get focus when component mounted
- `checked(v-model)`: boolean; default false. Specifies whether the checkbox is selected.
- `disabled`: boolean; default false. Disable checkbox
- `indeterminate`: boolean; default false. indeterminate checked state of checkbox
- `value`: boolean | string | number; default -. value of checkbox in CheckboxGroup
- `disabled`: boolean; default false. Disable all checkboxes
- `name`: string; default -. The `name` property of all `input[type="checkbox"]` children
- `options`: string[] | Array&lt;{ label: string value: string disabled?: boolean, indeterminate?: boolean, onChange?: function }>; default \[]. Specifies options, you can customize `label` with slot = "label" slot-scope="option"
- `value(v-model)`: (boolean | string | number)[]; default \[]. Used for setting the currently selected value.
- `blur()`: see upstream; default unspecified. remove focus
- `focus()`: see upstream; default unspecified. get focus

## Events

- `change`: The callback function that is triggered when the state changes.
- `change`: The callback function that is triggered when the state changes.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ACheckbox v-model:checked="checked">已阅读</ACheckbox><ACheckboxGroup v-model:value="selection" :options="options"/><p>{{ checked }} / {{ selection }}</p>
```

Source: pinned upstream es/components.js, es/checkbox, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
