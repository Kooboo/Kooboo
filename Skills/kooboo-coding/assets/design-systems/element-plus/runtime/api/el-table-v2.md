# ElTableV2

ElTableV2: native Element Plus component.

Kind: component. Import `ElTableV2` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/table-v2.html#tablev2

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| cache | number | 2 | Number of rows rendered in advance to boost the performance |
| estimated-row-height | number | not declared | The estimated row height for rendering dynamic height rows |
| header-class |  | not declared | Customized class name passed to header wrapper |
| header-props |  | not declared | Customized props name passed to header component |
| header-cell-props |  | not declared | Customized props name passed to header cell component |
| header-height | number` / `number[] | 50 | The height of the header is set by `height`. If given an array, it renders header rows equal to its length |
| footer-height | number | 0 | The height of the footer element, when provided, will be part to the calculation of the table's height. |
| row-class |  | not declared | Customized class name passed to row wrapper |
| row-key | string` / `Symbol` / `number | id | The key of each row, if not provided, will be the index of the row |
| row-props |  | not declared | Customized props name passed to row component |
| row-height | number | 50 | The height of each row, used for calculating the total height of the table |
| row-event-handlers |  | not declared | A collection of handlers attached to each row |
| cell-props |  | not declared | extra props passed to each cell (except header cells) |
| columns | [Column[]] | not declared | An array of column definitions. |
| data | [Data[]] | [] | An array of data to be rendered in the table. |
| data-getter |  | not declared | A method to customize data fetch from the data source. |
| fixed-data |  | not declared | Data for rendering rows above the main content and below the header |
| expand-column-key | string | not declared | The column key indicates which row is expandable |
| expanded-row-keys | [KeyType[]] | not declared | An array of keys for expanded rows, can be used with `v-model` |
| default-expanded-row-keys | [KeyType[]] | not declared | An array of keys for default expanded rows, **NON REACTIVE** |
| class | string` / `array` / `object | not declared | Class name for the virtual table, will be applied to all three tables (left, right, main) |
| fixed | boolean | false | Flag indicates the table column's width to be fixed or flexible. |
| width | number | not declared | Width of the table |
| height | number | not declared | Height of the table |
| max-height | number | not declared | Maximum height of the table |
| indent-size | number | 12 | horizontal indentation of tree table |
| h-scrollbar-size | number | 6 | Indicates the horizontal scrollbar's size for the table, used to prevent the horizontal and vertical scrollbar to collapse |
| v-scrollbar-size | number | 6 | Indicates the vertical scrollbar's size for the table, used to prevent the horizontal and vertical scrollbar to collapse |
| scrollbar-always-on | boolean | false | If true, the scrollbar will always be shown instead of when mouse is placed above the table |
| sort-by |  | {} | Sort indicator |
| sort-state |  | undefined | Multiple sort indicator |

## Events

- `column-sort`: Invoked when column sorted
- `expanded-rows-change`: Invoked when expanded rows changed
- `end-reached`: Invoked when the end of the table is reached. The callback contain the remain distance, it is the usually the scrollbar height.
- `scroll`: Invoked after scrolling
- `rows-rendered`: Invoked when rows are rendered
- `row-expand`: Invoked when expand/collapse the tree node by clicking the arrow icon

## Slots

- `cell`:
- `header`:
- `header-cell`:
- `row`:
- `footer`:
- `empty`:
- `overlay`:
