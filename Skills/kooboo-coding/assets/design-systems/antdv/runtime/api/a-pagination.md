# APagination

Ant Design Vue 4.2.6; native export `Pagination`. Import `APagination` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `current(v-model)`: number; default -. current page number
- `defaultCurrent`: number; default 1. default initial page number
- `defaultPageSize`: number; default 10. default number of data items per page
- `disabled`: boolean; default -. Disable pagination
- `hideOnSinglePage`: boolean; default false. Whether to hide pager on single page
- `itemRender`: (page, type: 'page' | 'prev' | 'next', originalElement) => vNode | v-slot; default -. to customize item innerHTML
- `pageSize(v-model)`: number; default -. number of data items per page
- `pageSizeOptions`: string[] | number[]; default \['10', '20', '50', '100']. specify the sizeChanger options
- `responsive`: boolean; default -. If `size` is not specified, `Pagination` would resize according to the width of the window
- `showLessItems`: boolean; default false. Show less page items
- `showQuickJumper`: boolean; default false. determine whether you can jump to pages directly
- `showSizeChanger`: boolean; default -. Determine whether to show `pageSize` select, it will be true when `total > 50`
- `showTitle`: boolean; default true. Show page item's title
- `showTotal`: Function(total, range); default -. to display the total number and range
- `simple`: boolean; default -. whether to use simple mode
- `size`: string; default "". specify the size of `Pagination`, can be set to `small`
- `total`: number; default 0. total number of data items

## Events

- `change`: Called when the page number or `pageSize` is changed, and it takes the resulting page number and pageSize as its arguments
- `showSizeChange`: a callback function, executed when `pageSize` is changed

## Slots

- `itemRender`: to customize item innerHTML

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<APagination v-model:current="page" :total="100" show-size-changer @change="event = '页码：' + $event"/><p>{{ event }}</p>
```

Source: pinned upstream es/components.js, es/pagination, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
