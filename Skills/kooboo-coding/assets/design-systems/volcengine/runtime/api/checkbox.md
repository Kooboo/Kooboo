# YlCheckbox

Import and register `{ YlCheckbox }` from `/yuanli.js`. Install it with sync_design_runtime before importing.

Boolean v-model; default slot labels the native checkbox. indeterminate is visual/mixed state; caller owns checkbox groups.

Props and events (Vue declaration; types/defaults are authoritative):
```js
{ modelValue: Boolean, disabled: Boolean, indeterminate: Boolean, invalid: Boolean }, emits: ['update:modelValue']
```
