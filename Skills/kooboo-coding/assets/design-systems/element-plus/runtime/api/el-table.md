# ElTable

Display multiple data with similar format. You can sort, filter, compare your data in a table.

Kind: component. Import `ElTable` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/table.html#table

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| data | any[] | [] | table data |
| height | string / number | not declared | table's height. By default it has an `auto` height. If its value is a number, the height is measured in pixels; if its value is a string, the value will be assigned to element's style.height, the height is affected by external styles |
| max-height | string / number | not declared | table's max-height. The legal value is a number or the height in px |
| stripe | boolean | false | whether Table is striped |
| border | boolean | false | whether Table has vertical border |
| size | '' \| 'large' \| 'default' \| 'small' | not declared | size of Table |
| fit | boolean | true | whether width of column automatically fits its container |
| show-header | boolean | true | whether Table header is visible |
| highlight-current-row | boolean | false | whether current row is highlighted |
| current-row-key | string / number | not declared | key of current row, a set only prop |
| row-class-name | (data: { row: any, rowIndex: number }) => string / string | not declared | function that returns custom class names for a row, or a string assigning class names for every row |
| row-style | (data: { row: any, rowIndex: number }) => CSSProperties / CSSProperties | not declared | function that returns custom style for a row, or an object assigning custom style for every row |
| cell-class-name | (data: { row: any, column: TableColumnCtx<T>, rowIndex: number, columnIndex: number }) => string / string | not declared | function that returns custom class names for a cell, or a string assigning class names for every cell |
| cell-style | (data: { row: any, column: TableColumnCtx<T>, rowIndex: number, columnIndex: number }) => CSSProperties / CSSProperties | not declared | function that returns custom style for a cell, or an object assigning custom style for every cell |
| header-row-class-name | (data: { row: any, rowIndex: number }) => string / string | not declared | function that returns custom class names for a row in table header, or a string assigning class names for every row in table header |
| header-row-style | (data: { row: any, rowIndex: number }) => CSSProperties / CSSProperties | not declared | function that returns custom style for a row in table header, or an object assigning custom style for every row in table header |
| header-cell-class-name | (data: { row: any, column: TableColumnCtx<T>, rowIndex: number, columnIndex: number }) => string / string | not declared | function that returns custom class names for a cell in table header, or a string assigning class names for every cell in table header |
| header-cell-style | (data: { row: any, column: TableColumnCtx<T>, rowIndex: number, columnIndex: number }) => CSSProperties / CSSProperties | not declared | function that returns custom style for a cell in table header, or an object assigning custom style for every cell in table header |
| row-key | (row: any) => string / string | not declared | key of row data, used for optimizing rendering. Required if `reserve-selection` is on or display tree data. When its type is String, multi-level access is supported, e.g. `user.info.id`, but `user.info[0].id` is not supported, in which case `Function` should be used |
| empty-text | string | No Data | displayed text when data is empty. You can customize this area with `#empty` |
| default-expand-all | boolean | false | whether expand all rows by default, works when the table has a column type="expand" or contains tree structure data |
| expand-row-keys | Array<string> | not declared | set expanded rows by this prop, prop's value is the keys of expand rows, you should set row-key before using this prop. |
| default-sort | Sort | if `prop` is set, and `order` is not set, then `order` is default to ascending | set the default sort column and order. property `prop` is used to set default sort column, property `order` is used to set default sort order |
| tooltip-effect | 'dark' \| 'light' | dark | the `effect` of the overflow tooltip |
| tooltip-options | Pick<ElTooltipProps, 'effect' \| 'enterable' \| 'hideAfter' \| 'offset' \| 'placement' \| 'popperClass' \| 'popperOptions' \| 'showAfter' \| 'showArrow'> | ^[object]`{ enterable: true, placement: 'top', showArrow: true, hideAfter: 200, popperOptions: { strategy: 'fixed' } }` | the options for the overflow tooltip, [see the following tooltip component](tooltip.html#attributes) |
| append-filter-panel-to | string | not declared | which element the filter panels appends to |
| show-summary | boolean | false | whether to display a summary row |
| sum-text | string | Sum | displayed text for the first column of summary row |
| summary-method | (data: { columns: any[], data: any[] }) => [] | not declared | custom summary method |
| span-method | (data: { row: any, column: TableColumnCtx<T>, rowIndex: number, columnIndex: number }) => number[] \| { rowspan: number, colspan: number } / void | not declared | method that returns rowspan and colspan |
| select-on-indeterminate | boolean | true | controls the behavior of master checkbox in multi-select tables when only some rows are selected (but not all). If true, all rows will be selected, else deselected |
| indent | number | 16 | horizontal indentation of tree data |
| lazy | boolean | false | whether to lazy loading data |
| load | (row: any, treeNode: TreeNode, resolve: (data: any[]) => void) => void | not declared | method for loading child row data, only works when `lazy` is true |
| tree-props | { hasChildren?: string, children?: string, checkStrictly?: boolean } | ^[object]`{ hasChildren: 'hasChildren', children: 'children', checkStrictly: false }` | configuration for rendering nested data |
| table-layout | 'fixed' \| 'auto' | fixed | sets the algorithm used to lay out table cells, rows, and columns |
| scrollbar-always-on | boolean | false | always show scrollbar |
| show-overflow-tooltip | boolean | not declared | whether to hide extra content and show them in a tooltip when hovering on the cell.It will affect all the table columns, refer to table [tooltip-options](#table-attributes) |
| flexible | boolean | false | ensure main axis minimum-size doesn't follow the content |
| scrollbar-tabindex | string / number | not declared | body scrollbar's wrap container tabindex |
| allow-drag-last-column | boolean | true | whether to allow drag the last column |
| tooltip-formatter | (data: { row: any, column: TableColumnCtx<T>, cellValue: any }) => VNode / string | not declared | customize tooltip content when using `show-overflow-tooltip` |
| preserve-expanded-content | boolean | false | whether to preserve expanded row content in DOM when collapsed |
| native-scrollbar | boolean | false | whether to use native scrollbars |
| row-expandable | (row: any, index: number) => boolean | not declared | enable expandable rows, works when the table has a column type="expand" |

## Events

- `select`: triggers when user clicks the checkbox in a row
- `select-all`: triggers when user clicks the checkbox in table header
- `selection-change`: triggers when selection changes
- `cell-mouse-enter`: triggers when hovering into a cell
- `cell-mouse-leave`: triggers when hovering out of a cell
- `cell-click`: triggers when clicking a cell
- `cell-dblclick`: triggers when double clicking a cell
- `cell-contextmenu`: triggers when user right clicks on a cell
- `row-click`: triggers when clicking a row
- `row-contextmenu`: triggers when user right clicks on a row
- `row-dblclick`: triggers when double clicking a row
- `header-click`: triggers when clicking a column header
- `header-contextmenu`: triggers when user right clicks on a column header
- `sort-change`: triggers when Table's sorting changes
- `filter-change`: triggers when the table's filter changes
- `current-change`: triggers when current row changes
- `header-dragend`: triggers after changing a column's width by dragging the column header's border
- `expand-change`: triggers when user expands or collapses a row (for expandable table, second param is expandedRows; for tree Table, second param is expanded)
- `scroll`: Invoked after scrolled

## Slots

- `default`: customize default content
- `append`: Contents to be inserted after the last row. You may need this slot if you want to implement infinite scroll for the table. This slot will be displayed above the summary row if there is one.
- `empty`: you can customize content when data is empty.
