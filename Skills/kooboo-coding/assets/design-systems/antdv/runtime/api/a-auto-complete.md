# AAutoComplete

Ant Design Vue 4.2.6; native export `AutoComplete`. Import `AAutoComplete` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `allowClear`: boolean; default false. Show clear button, effective in multiple mode only.
- `autofocus`: boolean; default false. get focus when component mounted
- `backfill`: boolean; default false. backfill selected item the input when using keyboard
- `bordered`: boolean; default true. Whether has border style
- `clearIcon`: slot; default `<CloseCircleFilled />`. Use slot custom clear icon
- `default (for customize input element)`: slot; default `<Input />`. customize input element
- `defaultActiveFirstOption`: boolean; default true. Whether active first option by default
- `defaultOpen`: boolean; default -. Initial open state of dropdown
- `disabled`: boolean; default false. Whether disabled select
- `popupClassName`: string; default -. The className of dropdown menu
- `dropdownMatchSelectWidth`: boolean | number; default true. Determine whether the dropdown menu and the select input are the same width. Default set `min-width` same as input. Will ignore when value less than select width. `false` will disable virtual scroll
- `dropdownMenuStyle`: object; default unspecified. additional style applied to dropdown menu
- `filterOption`: boolean or function(inputValue, option); default true. If true, filter options by input, if function, filter options against it. The function will receive two arguments, `inputValue` and `option`, if the function returns `true`, the option will be included in the filtered set; Otherwise, it will be excluded.
- `open`: boolean; default -. Controlled open state of dropdown
- `option`: v-slot:option="{value, label, [disabled, key, title]}"; default -. custom render option by slot
- `options`: [DataSourceItemType](https://github.com/vueComponent/ant-design-vue/blob/724d53b907e577cf5880c1e6742d4c3f924f8f49/components/auto-complete/index.vue#L9)[]; default unspecified. Data source for autocomplete
- `placeholder`: string; default -. placeholder of input
- `status`: 'error' | 'warning'; default -. Set validation status
- `v-model:value`: string|string[]|{ key: string, label: string|vNodes }|Array&lt;{ key: string, label: string|vNodes }>; default -. selected option

## Events

- `blur`: Called when leaving the component.
- `change`: Called when select an option or input value change, or value of input is changed
- `dropdownVisibleChange`: Call when dropdown open
- `focus`: Called when entering the component
- `search`: Called when searching items.
- `select`: Called when a option is selected. param is option's value and option instance.
- `clear`: Called when clear

## Slots

- `clearIcon`: Use slot custom clear icon
- `default (for customize input element)`: customize input element
- `option`: custom render option by slot

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<AAutoComplete v-model:value="text" :options="options" placeholder="搜索城市" style="width:240px"/><p>{{ text }}</p>
```

Source: pinned upstream es/components.js, es/auto-complete, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
