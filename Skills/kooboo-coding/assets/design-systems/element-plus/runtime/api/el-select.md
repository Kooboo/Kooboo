# ElSelect

When there are plenty of options, use a drop-down menu to display and select desired ones.

Kind: component. Import `ElSelect` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/select.html#select

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| model-value | string / number / boolean / object / array | not declared | binding value |
| multiple | boolean | false | whether multiple-select is activated |
| options | Array<{[key: string]: any}> | not declared | data of the options, the key of `value` and `label` and `disabled` can be customize by `props` |
| [props] | object | not declared | configuration options |
| disabled | boolean | false | whether Select is disabled |
| value-key | string | value | unique identity key name for value, required when value is an object |
| size | '' \| 'large' \| 'default' \| 'small' | not declared | size of Input |
| clearable | boolean | false | whether select can be cleared |
| collapse-tags | boolean | false | whether to collapse tags to a text when multiple selecting |
| collapse-tags-tooltip | boolean | false | whether show all selected tags when mouse hover text of collapse-tags. To use this, `collapse-tags` must be true |
| [tag-tooltip] | TagTooltipProps | {} | configuration object for the collapse-tags tooltip. To use this, `collapse-tags` and `collapse-tags-tooltip` must be true |
| multiple-limit | number | 0 | maximum number of options user can select when `multiple` is `true`. No limit when set to 0 |
| id | string | not declared | native input id input |
| name | string | not declared | the name attribute of select input |
| effect | 'dark' \| 'light' / string | light | tooltip theme, built-in theme: `dark` / `light` |
| autocomplete | string | off | the autocomplete attribute of select input |
| placeholder | string | not declared | placeholder, default is 'Select' |
| filterable | boolean | false | whether Select is filterable |
| allow-create | boolean | false | whether creating new items is allowed. To use this, `filterable` must be true |
| filter-method | (query: string) => void | not declared | custom filter method, the first parameter is the current input value. To use this, `filterable` must be true |
| remote | boolean | false | whether options are loaded from server |
| debounce | number | 300 | debounce delay during remote search, in milliseconds |
| remote-method | (query: string) => void | not declared | function that gets called when the input value changes. Its parameter is the current input value. To use this, `filterable` must be true |
| remote-show-suffix | boolean | false | in remote search method show suffix icon |
| loading | boolean | false | whether Select is loading data from server |
| loading-text | string | not declared | displayed text while loading data from server, default is 'Loading' |
| no-match-text | string | not declared | displayed text when no data matches the filtering query, you can also use slot `empty`, default is 'No matching data' |
| no-data-text | string | not declared | displayed text when there is no options, you can also use slot `empty`, default is 'No data' |
| popper-class | string | '' | custom class name for Select's dropdown and tags' tooltip |
| popper-style | string / object | not declared | custom style for Select's dropdown and tags' tooltip |
| reserve-keyword | boolean | true | when `multiple` and `filterable` is true, whether to reserve current keyword after selecting an option |
| default-first-option | boolean | false | select first matching option on enter key. Use with `filterable` or `remote` |
| teleported | boolean | true | whether select dropdown is teleported, if `true` it will be teleported to where `append-to` sets |
| append-to | CSSSelector / HTMLElement | not declared | which element the select dropdown appends to |
| persistent | boolean | true | when select dropdown is inactive and `persistent` is `false`, select dropdown will be destroyed |
| automatic-dropdown | boolean | false | for non-filterable Select, this prop decides if the option menu pops up when the input is focused |
| clear-icon | string / Component | CircleClose | custom clear icon component |
| fit-input-width | boolean | false | whether the width of the dropdown is the same as the input |
| suffix-icon | string / Component | ArrowDown | custom suffix icon component |
| tag-type | '' \| 'success' \| 'info' \| 'warning' \| 'danger' | info | tag type |
| tag-effect | '' \| 'light' \| 'dark' \| 'plain' | light | tag effect |
| validate-event | boolean | true | whether to trigger form validation |
| offset | number | 12 | offset of the dropdown |
| show-arrow | boolean | true | whether the dropdown has an arrow |
| placement | 'top' \| 'top-start' \| 'top-end' \| 'bottom' \| 'bottom-start' \| 'bottom-end' \| 'left' \| 'left-start' \| 'left-end' \| 'right' \| 'right-start' \| 'right-end' | bottom-start | position of dropdown |
| fallback-placements | Placement[] | ['bottom-start', 'top-start', 'right', 'left'] | list of possible positions for dropdown [popper.js](https://popper.js.org/docs/v2/modifiers/flip/#fallbackplacements) |
| max-collapse-tags | number | 1 | the max tags number to be shown. To use this, `collapse-tags` must be true |
| popper-options | objectrefer to  doc | {} | [popper.js](https://popper.js.org/docs/v2/) parameters |
| aria-label | string | not declared | same as `aria-label` in native input |
| empty-values | array | not declared | empty values of component, [see config-provider](./config-provider.md#empty-values-configurations) |
| value-on-clear | string / number / boolean / Function | not declared | clear return value, [see config-provider](./config-provider.md#empty-values-configurations) |
| suffix-transition | boolean | true | animation when dropdown appears/disappears icon |
| tabindex | string / number | not declared | tabindex for input |

## Events

- `change`: triggers when the selected value changes
- `visible-change`: triggers when the dropdown appears/disappears
- `remove-tag`: triggers when a tag is removed in multiple mode
- `clear`: triggers when the clear icon is clicked in a clearable Select
- `blur`: triggers when Input blurs
- `focus`: triggers when Input focuses
- `popup-scroll`: triggers when dropdown scrolls
- `end-reached`: triggers when dropdown scroll reaches an end

## Slots

- `default`: option component list
- `header`: content at the top of the dropdown
- `footer`: content at the bottom of the dropdown
- `prefix`: content as Select prefix
- `empty`: content when there is no options
- `tag`: content as Select tag, subTags `data`, `selectDisabled` and `deleteTag` introduced in
- `loading`: content as Select loading
- `label`: content as Select label. `index` introduced in
