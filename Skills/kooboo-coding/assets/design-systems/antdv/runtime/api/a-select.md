# ASelect

Ant Design Vue 4.2.6; native export `Select`. Import `ASelect` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Use v-model:value. Options are {value,label}; multiple mode binds an array. showSearch/filterOption control local search; remote fetch needs loading, empty, error and late-response protection.

## Props and defaults

- `allowClear`: boolean; default false. Show clear button.
- `autoClearSearchValue`: boolean; default true. Whether the current search will be cleared on selecting an item. Only applies when `mode` is set to `multiple` or `tags`.
- `autofocus`: boolean; default false. Get focus by default
- `bordered`: boolean; default true. Whether has border style
- `clearIcon`: VNode | slot; default -. The custom clear icon
- `defaultActiveFirstOption`: boolean; default true. Whether active first option by default
- `defaultOpen`: boolean; default -. Initial open state of dropdown
- `disabled`: boolean; default false. Whether disabled select
- `popupClassName`: string; default -. className of dropdown menu
- `dropdownMatchSelectWidth`: boolean | number; default true. Determine whether the dropdown menu and the select input are the same width. Default set `min-width` same as input. Will ignore when value less than select width. `false` will disable virtual scroll
- `dropdownMenuStyle`: object; default -. additional style applied to dropdown menu
- `dropdownRender`: ({menuNode: VNode, props}) => VNode | v-slot; default -. Customize dropdown content
- `dropdownStyle`: object; default -. style of dropdown menu
- `fieldNames`: object; default { label: `label`, value: `value`, options: `options` }. Customize node label, value, options field name
- `filterOption`: `boolean` | `function(inputValue, option)`; default true. If true, filter options by input, if function, filter options against it. The function will receive two arguments, `inputValue` and `option`, if the function returns `true`, the option will be included in the filtered set; Otherwise, it will be excluded.
- `filterSort`: (optionA: Option, optionB: Option) => number; default -. Sort function for search options sorting, see [Array.sort](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Array/sort)'s compareFunction
- `firstActiveValue`: string | string[]; default -. Value of action option by default
- `getPopupContainer`: function(triggerNode); default () => document.body. Parent Node which the selector should be rendered to. Default to `body`. When position issues happen, try to modify it into scrollable content and position it relative.
- `labelInValue`: boolean; default false. whether to embed label in value, turn the format of value from `string` to `{key: string, label: vNodes, originLabel: any}`, originLabel (3.1) maintains the original type. If the node is constructed through a-select-option children, the value is a function (the default slot of a-select-option)
- `listHeight`: number; default 256. Config popup height
- `loading`: boolean; default false. indicate loading state
- `maxTagCount`: number; default -. Max tag count to show
- `maxTagPlaceholder`: slot | function(omittedValues); default -. Placeholder for not showing tags
- `maxTagTextLength`: number; default -. Max text length to show
- `menuItemSelectedIcon`: VNode | slot; default -. The custom menuItemSelected icon
- `mode`: 'multiple' | 'tags'; default -. Set mode of Select
- `notFoundContent`: string|slot; default `Not Found`. Specify content to show when no result matches..
- `open`: boolean; default -. Controlled open state of dropdown
- `option`: v-slot:option="{value, label, [disabled, key, title]}"; default -. custom render option by slot
- `optionFilterProp`: string; default value. Which prop value of option will be used for filter if filterOption is true
- `optionLabelProp`: string; default `children` | `label`(when use options). Which prop value of option will render as content of select.
- `options`: Array&lt;{value, label, [disabled, key, title]}>; default \[]. Data of the selectOption, manual construction work is no longer needed if this property has been set
- `placeholder`: string|slot; default -. Placeholder of select
- `placement`: `bottomLeft` `bottomRight` `topLeft` `topRight`; default bottomLeft. The position where the selection box pops up
- `removeIcon`: VNode | slot; default -. The custom remove icon
- `searchValue`: string; default -. The current input "search" text
- `showArrow`: boolean; default single:true, multiple:false. Whether to show the drop-down arrow
- `showSearch`: boolean; default single:false, multiple:true. Whether select is searchable
- `size`: string; default default. Size of Select input. `default` `large` `small`
- `status`: 'error' | 'warning'; default -. Set validation status
- `suffixIcon`: VNode | slot; default -. The custom suffix icon
- `tagRender`: slot | (props) => any; default -. Customize tag render, only applies when `mode` is set to `multiple` or `tags`
- `tokenSeparators`: string[]; default -. Separator used to tokenize, only applies when `mode="tags"`
- `value(v-model)`: string|number|string[]|number[]; default -. Current selected option.
- `virtual`: boolean; default true. Disable virtual scroll when set to false
- `blur()`: see upstream; default unspecified. Remove focus
- `focus()`: see upstream; default unspecified. Get focus

## Events

- `blur`: Called when blur
- `change`: Called when select an option or input value change, or value of input is changed in combobox mode
- `deselect`: Called when a option is deselected, the params are option's value (or key) . only called for multiple or tags, effective in multiple or tags mode only.
- `dropdownVisibleChange`: Call when dropdown open
- `focus`: Called when focus
- `inputKeyDown`: Called when key pressed
- `mouseenter`: Called when mouse enter
- `mouseleave`: Called when mouse leave
- `popupScroll`: Called when dropdown scrolls
- `search`: Callback function that is fired when input changed.
- `select`: Called when a option is selected, the params are option's value (or key) and option instance.

## Slots

- `clearIcon`: The custom clear icon
- `dropdownRender`: Customize dropdown content
- `maxTagPlaceholder`: Placeholder for not showing tags
- `menuItemSelectedIcon`: The custom menuItemSelected icon
- `notFoundContent`: Specify content to show when no result matches..
- `option`: custom render option by slot
- `placeholder`: Placeholder of select
- `removeIcon`: The custom remove icon
- `suffixIcon`: The custom suffix icon
- `tagRender`: Customize tag render, only applies when `mode` is set to `multiple` or `tags`

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ASelect v-model:value="choice" :options="options" allow-clear show-search style="width:220px"/><ASelect v-model:value="selection" mode="multiple" :options="options" style="min-width:220px"/><p>{{ choice }} {{ selection }}</p>
```

Source: pinned upstream es/components.js, es/select, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
