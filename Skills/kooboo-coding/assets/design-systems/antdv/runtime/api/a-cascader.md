# ACascader

Ant Design Vue 4.2.6; native export `Cascader`. Import `ACascader` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `allowClear`: boolean; default true. whether allow clear
- `autofocus`: boolean; default false. get focus when component mounted
- `bordered`: boolean; default true. Whether has border style
- `clearIcon`: slot; default -. The custom clear icon
- `changeOnSelect`: boolean; default false. (Work on single select) change value on each selection if set to true, see above demo for details
- `disabled`: boolean; default false. whether disabled select
- `displayRender`: `({labels, selectedOptions}) => VNode`; default `labels => labels.join(' / ')`. render function of displaying selected options, you can use #displayRender="{labels, selectedOptions}".
- `popupClassName`: string; default -. additional className of popup overlay
- `dropdownStyle`: CSSProperties; default {}. additional style of popup overlay
- `expandIcon`: slot; default -. Customize the current item expand icon
- `expandTrigger`: `click` | `hover`; default 'click'. expand current item when click or hover
- `fieldNames`: object; default `{ label: 'label', value: 'value', children: 'children' }`. custom field name for label and value and children
- `getPopupContainer`: Function(triggerNode); default () => document.body. Parent Node which the selector should be rendered to. Default to `body`. When position issues happen, try to modify it into scrollable content and position it relative.
- `loadData`: `(selectedOptions) => void`; default -. To load option lazily, and it cannot work with `showSearch`
- `maxTagCount`: number | `responsive`; default -. Max tag count to show. `responsive` will cost render performance
- `maxTagPlaceholder`: v-slot | function(omittedValues); default -. Placeholder for not showing tags
- `multiple`: boolean; default -. Support multiple or not
- `notFoundContent`: string | slot; default 'Not Found'. Specify content to show when no result matches.
- `open`: boolean; default -. set visible of cascader popup
- `options`: [Option](#option)[]; default -. data options of cascade
- `placeholder`: string; default 'Please select'. input placeholder
- `placement`: `bottomLeft` | `bottomRight` | `topLeft` | `topRight`; default `bottomLeft`. Use preset popup align config from builtinPlacements
- `removeIcon`: slot; default -. The custom remove icon
- `searchValue`: string; default -. Set search value, Need work with `showSearch`
- `showSearch`: boolean | [object](#showsearch); default false. Whether show search input in single mode.
- `size`: `large` | `default` | `small`; default `default`. input size
- `status`: 'error' | 'warning'; default -. Set validation status
- `suffixIcon`: string | VNode | slot; default -. The custom suffix icon
- `showCheckedStrategy`: `Cascader.SHOW_PARENT` | `Cascader.SHOW_CHILD`; default `Cascader.SHOW_PARENT`. The way show selected item in box. ** `SHOW_CHILD`: ** just show child treeNode. **`Cascader.SHOW_PARENT`:** just show parent treeNode (when all child treeNode under the parent treeNode are checked)
- `tagRender`: slot; default -. Customize tag render when `multiple`
- `value(v-model)`: string[] | number[]; default -. selected value

## Events

- `change`: callback when finishing cascader select
- `dropdownVisibleChange`: callback when popup shown or hidden
- `search`: callback when input value change

## Slots

- `clearIcon`: The custom clear icon
- `expandIcon`: Customize the current item expand icon
- `maxTagPlaceholder`: Placeholder for not showing tags
- `notFoundContent`: Specify content to show when no result matches.
- `removeIcon`: The custom remove icon
- `suffixIcon`: The custom suffix icon
- `tagRender`: Customize tag render when `multiple`

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ACascader v-model:value="pathValue" :options="[{value:'cn',label:'中国',children:[{value:'hz',label:'杭州'}]}]" placeholder="选择地区"/><p>{{ pathValue }}</p>
```

Source: pinned upstream es/components.js, es/cascader, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
