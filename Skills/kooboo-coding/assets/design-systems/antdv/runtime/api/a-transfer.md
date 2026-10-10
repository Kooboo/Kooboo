# ATransfer

Ant Design Vue 4.2.6; native export `Transfer`. Import `ATransfer` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `dataSource`: [{key: string.isRequired,title: string.isRequired,description: string,disabled: bool}]; default \[]. Used for setting the source data. The elements that are part of this array will be present the left column. Except the elements whose keys are included in `targetKeys` prop.
- `disabled`: boolean; default false. Whether disabled transfer
- `filterOption`: (inputValue, option): boolean; default unspecified. A function to determine whether an item should show in search result list
- `footer`: slot="footer" slot-scope="props"; default unspecified. customize the progress dot by setting a scoped slot
- `listStyle`: CSSProperties; default unspecified. A custom CSS style used for rendering the transfer columns.
- `locale`: object; default `{ itemUnit: 'item', itemsUnit: 'items', notFoundContent: 'The list is empty', searchPlaceholder: 'Search here' }`. i18n text including filter, empty text, item unit, etc
- `oneWay`: boolean; default false. Display as single direction style
- `operations`: string[]; default \['>', '&lt;']. A set of operations that are sorted from top to bottom.
- `operationStyle`: CSSProperties; default -. A custom CSS style used for rendering the operations column
- `pagination`: boolean | { pageSize: number, simple: boolean, showSizeChanger?: boolean, showLessItems?: boolean }; default false. Use pagination. Not work in render props
- `render`: Function(record) | slot; default unspecified. The function to generate the item shown on a column. Based on an record (element of the dataSource array), this function should return a element which is generated from that record. Also, it can return a plain object with `value` and `label`, `label` is a element and `value` is for title
- `selectAllLabels`: VueNode | ((info: { selectedCount: number; totalCount: number }) => VueNode);; default -. A set of customized labels for select all checkboxes on the header
- `selectedKeys(v-model)`: string[]; default \[]. A set of keys of selected items.
- `showSearch`: boolean; default false. If included, a search box is shown on each column.
- `showSelectAll`: boolean; default true. Show select all checkbox on the header
- `status`: 'error' | 'warning'; default -. Set validation status
- `targetKeys(v-model)`: string[]; default \[]. A set of keys of elements that are listed on the right column.
- `titles`: string[]; default -. A set of titles that are sorted from left to right.

## Events

- `change`: A callback function that is executed when the transfer between columns is complete.
- `scroll`: A callback function which is executed when scroll options list
- `search`: A callback function which is executed when search field are changed
- `selectChange`: A callback function which is executed when selected items are changed.

## Slots

- `footer`: customize the progress dot by setting a scoped slot
- `render`: The function to generate the item shown on a column. Based on an record (element of the dataSource array), this function should return a element which is generated from that record. Also, it can return a plain object with `value` and `label`, `label` is a element and `value` is for title

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ATransfer v-model:targetKeys="targetKeys" :data-source="transferData" :render="item => item.title" show-search/><p>{{ targetKeys }}</p>
```

Source: pinned upstream es/components.js, es/transfer, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
