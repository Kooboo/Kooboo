# ElCascader

If the options have a clear hierarchical structure, Cascader can be used to view and select them.

Kind: component. Import `ElCascader` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/cascader.html#cascader

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| model-value | string / number / string[] / number[] / any | not declared | binding value |
| options | CascaderOption[] | not declared | data of the options, the key of `value` and `label` can be customize by `CascaderProps`. |
| [props] | CascaderProps | not declared | configuration options, see the following `CascaderProps` table. |
| size | 'large' \| 'default' \| 'small' | not declared | size of input |
| placeholder | string | not declared | placeholder of input |
| disabled | boolean | not declared | whether Cascader is disabled |
| clearable | boolean | not declared | whether selected value can be cleared |
| clear-icon | string / Component | CircleClose | custom clear icon component |
| show-all-levels | boolean | true | whether to display all levels of the selected value in the input |
| collapse-tags | boolean | not declared | whether to collapse tags in multiple selection mode |
| collapse-tags-tooltip | boolean | false | whether show all selected tags when mouse hover text of collapse-tags. To use this, `collapse-tags` must be true |
| max-collapse-tags-tooltip-height | string / number | not declared | max height of collapse-tags tooltip. |
| separator | string | ' / ' | option label separator |
| filterable | boolean | not declared | whether the options can be searched |
| filter-method | (node: CascaderNode, keyword: string) => boolean | not declared | customize search logic, the first parameter is `node`, the second is `keyword`, and need return a boolean value indicating whether it hits. |
| debounce | number | 300 | debounce delay when typing filter keyword, in milliseconds |
| before-filter | (value: string) => boolean | not declared | hook function before filtering with the value to be filtered as its parameter. If `false` is returned or a `Promise` is returned and then is rejected, filtering will be aborted |
| popper-class | string | '' | custom class name for Cascader's dropdown and tags' tooltip |
| popper-style | string / object | not declared | custom style for Cascader's dropdown and tags' tooltip |
| teleported | boolean | true | whether cascader popup is teleported |
| effect | 'dark' \| 'light' / string | light | tooltip theme, built-in theme: `dark` / `light` |
| tag-type | 'success' \| 'info' \| 'warning' \| 'danger' | info | tag type |
| tag-effect | 'light' \| 'dark' \| 'plain' | light | tag effect |
| validate-event | boolean | true | whether to trigger form validation |
| max-collapse-tags | number | 1 | The max tags number to be shown. To use this, `collapse-tags` must be true |
| empty-values | array | not declared | empty values of component, [see config-provider](./config-provider.md#empty-values-configurations) |
| value-on-clear | string / number / boolean / Function | not declared | clear return value, [see config-provider](./config-provider.md#empty-values-configurations) |
| persistent | boolean | true | when dropdown is inactive and `persistent` is `false`, dropdown will be destroyed |
| fallback-placements | Placement[] | not declared | list of possible positions for Tooltip [popper.js](https://popper.js.org/docs/v2/modifiers/flip/#fallbackplacements) |
| placement | 'top' \| 'top-start' \| 'top-end' \| 'bottom' \| 'bottom-start' \| 'bottom-end' \| 'left' \| 'left-start' \| 'left-end' \| 'right' \| 'right-start' \| 'right-end' | bottom-start | position of dropdown |
| popper-append-to-body | boolean | true | whether to append the popper menu to body. If the positioning of the popper is wrong, you can try to set this prop to false |
| show-checked-strategy | 'parent' \| 'child' | child | strategy for displaying checked nodes in multiple selection mode. Use `parent` when you want things tidy. Use `child` when every single item matters |
| virtual-scroll | boolean | false | whether to enable virtual scrolling for large data |
| fit-input-width | boolean / number | false | whether the width of the suggestion panel is the same as the input, if the value is `number`, then the width is fixed |
| item-size | number | 34 | node height for virtual scrolling (px) |
| height | number | 204 | menu height for virtual scrolling (px) |

## Events

- `change`: triggers when the binding value changes
- `expand-change`: triggers when expand option changes
- `blur`: triggers when Cascader blurs
- `focus`: triggers when Cascader focuses
- `clear`: triggers when the clear icon is clicked in a clearable Select
- `visible-change`: triggers when the dropdown appears/disappears
- `remove-tag`: triggers when remove tag in multiple selection mode

## Slots

- `default`: the custom content of cascader node, which are current Node object and node data respectively.
- `empty`: content when there is no matched options.
- `prefix`: content as Input prefix
- `suggestion-item`: custom content for suggestion item when searching
- `tag`: custom tags style
- `header`: content at the top of the dropdown
- `footer`: content at the bottom of the dropdown
