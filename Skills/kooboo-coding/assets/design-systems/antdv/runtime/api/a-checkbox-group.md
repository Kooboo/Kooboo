# ACheckboxGroup

Ant Design Vue 4.2.6; native export `CheckboxGroup`. Import `ACheckboxGroup` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Checkbox uses v-model:checked; CheckboxGroup uses v-model:value (array). Disabled controls cannot establish business authorization.

## Props and defaults



## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ACheckbox v-model:checked="checked">已阅读</ACheckbox><ACheckboxGroup v-model:value="selection" :options="options"/><p>{{ checked }} / {{ selection }}</p>
```

Source: pinned upstream es/components.js, es/checkbox, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
