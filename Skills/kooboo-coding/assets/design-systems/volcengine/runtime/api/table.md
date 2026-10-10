# YlTable

Import `{ YlTable } from '/yuanli.js'`; register in `components`.

Props: `columns: Array<{key:string,label:string}>` required; `rows: object[] = []`; `rowKey: string = 'id'`; `caption: string` required; `loading: boolean = false`; `striped: boolean = false` uses alternating source row backgrounds; `error: string = ''`; `emptyText = '暂无数据'`; `loadingText = '正在加载'`; `selectable: boolean = false`; `selectedKeys: (string|number)[] = []` (`v-model:selected-keys`); `selectAllLabel = '选择当前页全部记录'`; `selectionLabel: (row)=>string` defaults to `选择记录 ${row.id}` (override when rowKey differs).

Every row needs a unique, stable non-null key; every column needs a unique key. Cell slot `cell-{key}` receives `{row,value}`; plain text is rendered when absent. Never pass HTML strings for cells. Selection uses YlCheckbox. Optional `rowDisabled: (row)=>boolean = ()=>false` disables individual rows. Event: `update:selectedKeys(keys)`; page select-all affects only enabled current rows and preserves selections on disabled rows and other pages. Caller clears obsolete selections after dataset/filter changes.

```html
<YlTable :rows="rows" :columns="columns" caption="订单列表" :loading="loading" :error="error" selectable v-model:selected-keys="selected">
  <template #cell-actions="{row}"><YlButton variant="text" @click="edit(row)">编辑</YlButton></template>
</YlTable>
```

Include `{key:'actions',label:'操作'}` in columns for that slot. Loading/error hides stale rows and disables selection. Empty/loading/error span all columns with centered feedback. Only loading spins; empty/error use a brief entrance. Both respect prefers-reduced-motion. Horizontal overflow remains scrollable and keyboard focusable. Caller owns pagination (pass only the page's rows), filtering, sorting, mutations and retry controls. Set column.sortable=true and bind sort={key,order:"asc"|"desc"}|null. Header clicks emit update:sort through ascending/descending/none and are blocked while loading/error; caller sorts or requests rows. toolbar slot receives {selectedKeys}; pagination slot can compose YlPagination below the table. These slots do not request data.
