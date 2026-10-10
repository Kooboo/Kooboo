# ElTableColumn

ElTableColumn: native Element Plus component.

Kind: component. Import `ElTableColumn` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/table.html#table-column

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| type | 'default' \| 'selection' \| 'index' \| 'expand' | default | type of the column. If set to `selection`, the column will display checkbox. If set to `index`, the column will display index of the row (staring from 1). If set to `expand`, the column will display expand icon |
| index | number / (index: number) => number | not declared | customize indices for each row, works on columns with `type=index` |
| label | string | not declared | column label |
| column-key | string | not declared | column's key. If you need to use the filter-change event, you need this attribute to identify which column is being filtered |
| prop | string | not declared | field name. You can also use its alias: `property` |
| width | string / number | '' | column width |
| min-width | string / number | '' | column minimum width. Columns with `width` has a fixed width, while columns with `min-width` has a width that is distributed in proportion |
| fixed | 'left' \| 'right' / boolean | false | whether column is fixed at left / right. Will be fixed at left if `true` |
| render-header | (data: { column: TableColumnCtx<T>, $index: number }) => void | not declared | render function for table header of this column |
| sortable | boolean / string | false | whether column can be sorted. Remote sorting can be done by setting this attribute to 'custom' and listening to the `sort-change` event of Table |
| sort-method | <T = any>(a: T, b: T) => number | not declared | sorting method, works when `sortable` is `true`. Should return a number, just like Array.sort |
| sort-by | (row: any, index: number) => string / string / string[] | not declared | specify which property to sort by, works when `sortable` is `true` and `sort-method` is `undefined`. If set to an Array, the column will sequentially sort by the next property if the previous one is equal |
| sort-orders | [] | ['ascending', 'descending', null] | the order of the sorting strategies used when sorting the data, works when `sortable` is `true`. Accepts an array, as the user clicks on the header, the column is sorted in order of the elements in the array |
| resizable | boolean | true | whether column width can be resized, works when `border` of `el-table` is `true` |
| formatter | (row: any, column: TableColumnCtx<T>, cellValue: any, index: number) => VNode / string | not declared | function that formats cell content |
| show-overflow-tooltip | boolean | undefined | whether to hide extra content and show them in a tooltip when hovering on the cell |
| align | 'left' \| 'center' \| 'right' | left | alignment |
| header-align | 'left' \| 'center' \| 'right' | left | alignment of the table header. If omitted, the value of the above `align` attribute will be applied |
| class-name | string | not declared | class name of cells in the column |
| label-class-name | string | not declared | class name of the label of this column |
| selectable | (row: any, index: number) => boolean | not declared | function that determines if a certain row can be selected, works when `type` is 'selection' |
| reserve-selection | boolean | false | whether to reserve selection after data refreshing, works when `type` is 'selection'. Note that `row-key` is required for this to work |
| filters | Array<{text: string, value: string}> | not declared | an array of data filtering options. For each element in this array, `text` and `value` are required |
| filter-placement | 'top' \| 'top-start' \| 'top-end' \| 'bottom' \| 'bottom-start' \| 'bottom-end' \| 'left' \| 'left-start' \| 'left-end' \| 'right' \| 'right-start' \| 'right-end' | not declared | placement for the filter dropdown |
| filter-class-name | string | not declared | className for the filter dropdown |
| filter-multiple | boolean | true | whether data filtering supports multiple options |
| filter-method | (value: any, row: any, column: TableColumnCtx<T>) => void | not declared | data filtering method. If `filter-multiple` is on, this method will be called multiple times for each row, and a row will display if one of the calls returns `true` |
| filtered-value | string[] | not declared | filter value for selected data, might be useful when table header is rendered with `render-header` |
| tooltip-formatter | (data: { row: any, column: TableColumnCtx<T>, cellValue: any }) => VNode / string | not declared | customize tooltip content when using `show-overflow-tooltip` |

## Slots

- `default`: Custom content for table columns
- `header`: Custom content for table header
- `filter-icon`: Custom content for filter icon
- `expand`: Custom content for expand columns. The `expandable` property is supported starting from v2.13.2.
