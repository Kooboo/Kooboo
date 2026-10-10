# ATableSummaryRow

Ant Design Vue 4.2.6; native export `TableSummaryRow`. Import `ATableSummaryRow` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Use columns/dataSource and stable rowKey. #bodyCell receives {text,record,index,column}. Own pagination, filters, sort and remote request state; @change yields pagination/filters/sorter/extra. TableColumn/Summary exports require their native parent structure.

## Props and defaults



## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ATable :columns="columns" :data-source="rows" row-key="id" :pagination="false"><template #bodyCell="{column,record}"><ATag v-if="column.key === 'status'" color="green">{{ record.status }}</ATag></template></ATable>
```

Source: pinned upstream es/components.js, es/table, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
