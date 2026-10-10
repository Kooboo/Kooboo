# ElTransfer

ElTransfer: native Element Plus component.

Kind: component. Import `ElTransfer` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/transfer.html#transfer

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| model-value | Array<string \| number> | [] | binding value |
| data | Record<string, any>[] | [] | data source |
| filterable | boolean | false | whether Transfer is filterable |
| filter-placeholder | string | not declared | placeholder for the filter input |
| filter-method | (query: string, item: Record<string, any>) => boolean | not declared | custom filter method |
| target-order | 'original' \| 'push' \| 'unshift' | original | order strategy for elements in the target list. If set to `original`, the elements will keep the same order as the data source. If set to `push`, the newly added elements will be pushed to the bottom. If set to `unshift`, the newly added elements will be inserted on the top |
| titles | [string, string] | [] | custom list titles |
| button-texts | [string, string] | [] | custom button texts |
| render-content | renderContent | not declared | custom render function for data items |
| format | TransferFormat | {} | texts for checking status in list header |
| [props] | TransferPropsAlias | not declared | prop aliases for data source |
| left-default-checked | Array<string \| number> | [] | key array of initially checked data items of the left list |
| right-default-checked | Array<string \| number> | [] | key array of initially checked data items of the right list |
| validate-event | boolean | true | whether to trigger form validation |
| virtual-scroll | boolean | false | whether to enable virtual scrolling |
| item-size | number | 30 | item height for virtual scrolling |

## Events

- `change`: triggers when data items change in the right list
- `left-check-change`: triggers when end user changes the checked state of any data item in the left list
- `right-check-change`: triggers when end user changes the checked state of any data item in the right list

## Slots

- `default`: Custom content for data items.
- `left-footer`: content of left list footer
- `right-footer`: content of right list footer
- `left-empty`: content when left panel is empty or when no data matches the filter
- `right-empty`: content when right panel is empty or when no data matches the filter
