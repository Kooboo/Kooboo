# ATable

Ant Design Vue 4.2.6; native export `Table`. Import `ATable` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Use columns/dataSource and stable rowKey. #bodyCell receives {text,record,index,column}. Own pagination, filters, sort and remote request state; @change yields pagination/filters/sorter/extra. TableColumn/Summary exports require their native parent structure.

## Props and defaults

- `bodyCell`: v-slot:bodyCell="{text, record, index, column}"; default -. custom body cell by slot
- `bordered`: boolean; default `false`. Whether to show all table borders
- `childrenColumnName`: string; default `children`. The column contains children to display
- `columns`: array; default -. Columns of table [config](#column)
- `components`: object; default -. Override default table elements
- `customFilterDropdown`: v-slot:customFilterDropdown="[FilterDropdownProps](#filterdropdownprops)"; default -. Customized filter overlay, need set `column.customFilterDropdown`
- `customFilterIcon`: v-slot:customFilterIcon="{filtered, column}"; default -. Customized filter icon
- `customHeaderRow`: Function(columns, index); default -. Set props on per header row
- `customRow`: Function(record, index); default -. Set props on per row
- `dataSource`: any[]; default -. Data record array to be displayed
- `defaultExpandAllRows`: boolean; default `false`. Expand all rows initially
- `defaultExpandedRowKeys`: string[]; default -. Initial expanded row keys
- `emptyText`: v-slot:emptyText; default -. Customize the display content when empty data
- `expandedRowKeys(v-model)`: string[]; default -. Current expanded row keys
- `expandedRowRender`: Function({record, index, indent, expanded}):VNode|v-slot; default -. Expanded container render for each row
- `expandFixed`: boolean | string; default false. Set column to be fixed: `true`(same as left) `'left'` `'right'`
- `expandColumnTitle`: v-slot; default -. Set the title of the expand column
- `expandIcon`: Function(props):VNode | v-slot:expandIcon="props"; default -. Customize row expand Icon.
- `expandRowByClick`: boolean; default `false`. Whether to expand row by clicking anywhere in the whole row
- `footer`: Function(currentPageData)| v-slot:footer="currentPageData"; default unspecified. Table footer renderer
- `getPopupContainer`: (triggerNode) => HTMLElement; default `() => TableHtmlElement`. the render container of dropdowns in table
- `headerCell`: v-slot:headerCell="{title, column}"; default -. custom head cell by slot
- `indentSize`: number; default 15. Indent size in pixels of tree data
- `loading`: boolean|[object](/components/spin); default `false`. Loading status of table
- `locale`: object; default filterConfirm: 'Ok' <br /> filterReset: 'Reset' <br /> emptyText: 'No Data'. i18n text including filter, sort, empty text, etc
- `pagination`: object | `false`; default unspecified. Config of pagination. You can ref table pagination [config](#pagination) or full [`pagination`](/components/pagination/) document, hide it by setting it to `false`
- `rowClassName`: Function(record, index):string; default -. Row's className
- `rowExpandable`: (record) => boolean; default -. Enable row can be expandable
- `rowKey`: string|Function(record, index):string; default `key`. Row's unique key, could be a string or function that returns a string
- `rowSelection`: object; default null. Row selection [config](#rowselection)
- `scroll`: object; default -. Whether the table can be scrollable, [config](#scroll)
- `showExpandColumn`: boolean; default true. Show expand column
- `showHeader`: boolean; default `true`. Whether to show table header
- `showSorterTooltip`: boolean | [Tooltip props](/components/tooltip/#api); default true. The header show next sorter direction tooltip. It will be set as the property of Tooltip if its type is object
- `size`: `middle` | `small` | `large`; default `large`. Size of table
- `sortDirections`: Array; default \[`ascend`, `descend`]. Supported sort way, could be `ascend`, `descend`
- `sticky`: boolean | `{offsetHeader?: number, offsetScroll?: number, getContainer?: () => HTMLElement}`; default -. Set sticky header and scroll bar
- `summary`: v-slot:summary; default -. Summary content
- `tableLayout`: - | 'auto' | 'fixed'; default -<hr />`fixed` when header/columns are fixed, or using `column.ellipsis`. [table-layout](https://developer.mozilla.org/en-US/docs/Web/CSS/table-layout) attribute of table element
- `title`: Function(currentPageData)| v-slot:title="currentPageData"; default unspecified. Table title renderer
- `transformCellText`: Function({ text, column, record, index }) => any, The `text` here is the data processed by other defined cell api, and it may be of type VNode | string | number; default -. The data can be changed again before rendering, generally used for the default configuration of empty data. You can configured globally through [ConfigProvider](/components/config-provider-cn/)

## Events

No events listed in the pinned upstream metadata.

## Slots

- `bodyCell`: custom body cell by slot
- `customFilterDropdown`: Customized filter overlay, need set `column.customFilterDropdown`
- `customFilterIcon`: Customized filter icon
- `emptyText`: Customize the display content when empty data
- `expandedRowRender`: Expanded container render for each row
- `expandColumnTitle`: Set the title of the expand column
- `expandIcon`: Customize row expand Icon.
- `footer`: Table footer renderer
- `headerCell`: custom head cell by slot
- `summary`: Summary content
- `title`: Table title renderer

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ATable :columns="columns" :data-source="rows" row-key="id" :pagination="false"><template #bodyCell="{column,record}"><ATag v-if="column.key === 'status'" color="green">{{ record.status }}</ATag></template></ATable>
```

Source: pinned upstream es/components.js, es/table, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
