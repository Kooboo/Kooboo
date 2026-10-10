# ASwitch

Ant Design Vue 4.2.6; native export `Switch`. Import `ASwitch` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Use v-model:checked (boolean), loading and disabled. Persist changes explicitly and restore state on failed writes.

## Props and defaults

- `autofocus`: boolean; default false. get focus when component mounted
- `checked(v-model)`: checkedValue | unCheckedValue; default false. determine whether the `Switch` is checked
- `checkedChildren`: string|slot; default unspecified. content to be shown when the state is checked
- `checkedValue`: boolean | string | number; default true. value for checked state
- `disabled`: boolean; default false. Disable switch
- `loading`: boolean; default false. loading state of switch
- `size`: string; default default. the size of the `Switch`, options: `default` `small`
- `unCheckedChildren`: string|slot; default unspecified. content to be shown when the state is unchecked
- `unCheckedValue`: boolean | string | number; default false. value for unchecked state

## Events

No events listed in the pinned upstream metadata.

## Slots

- `checkedChildren`: content to be shown when the state is checked
- `unCheckedChildren`: content to be shown when the state is unchecked

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ASwitch v-model:checked="checked" checked-children="开" un-checked-children="关"/><ASwitch disabled/><ASwitch loading/><p>{{ checked }}</p>
```

Source: pinned upstream es/components.js, es/switch, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
