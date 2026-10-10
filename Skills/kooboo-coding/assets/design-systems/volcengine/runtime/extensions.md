# Extending Yuanli in an App

Read only when a business feature needs a custom component. First use existing props/events/slots, then compose components. A repeated App need is not permission to modify or publish the shared framework.

## Resources and design contract

Create a normal Kooboo Script named `component.product-picker.js`, routed at `/component.product-picker.js`; import it in the consuming Screen and register it. Keep its scoped business layout under `.product-picker-*` in `/app.css` (extend the existing file). The runtime installer does not own these resources.

Use the installed dependency inventory. For this example add YlDialog, YlInput, YlButton, YlTable and YlPagination using sync_design_runtime before writing imports. Follow its preview/apply protocol; do not paste framework source.

Available extension points: YlDialog default/footer slots, YlTable `cell-{key}` slots, YlListPage actions/filters/pagination slots, YlFormItem's input-attribute slot, component props/events and inherited native attributes. Theme/layout tokens: `--color-primary`, `--color-text-1/2/3`, `--color-bg-content`, `--color-border-2`, `--radius-sm`, `--space-2/3/4/6/8`. Use YlIcon/YlButton.icon for Lucide geometry. Do not assume undocumented variants or replace framework selectors.

## Composition example: product picker

The parent owns fetching/filtering rows and persistence. This picker emits queries and selection; it does not invent an endpoint. Pass the current page's rows and filtered total; page size is 10. On opening, the parent loads the initial query `{query:'',page:1,pageSize:10}` or invokes a retained instance's query state as appropriate.

```js
import { ref } from 'vue'
import { YlDialog, YlInput, YlButton, YlTable, YlPagination } from '/yuanli.js'
export default {
  name: 'ProductPicker',
  components: { YlDialog, YlInput, YlButton, YlTable, YlPagination },
  props: { modelValue: Boolean, rows: { type: Array, default: () => [] }, total: { type: Number, default: 0 }, loading: Boolean, error: String },
  emits: ['update:modelValue', 'search', 'select'],
  setup(props, { emit }) {
    const query = ref(''), page = ref(1)
    const columns = [{ key: 'name', label: '产品名称' }, { key: 'actions', label: '操作' }]
    function search(next = 1) { page.value = next; emit('search', { query: query.value, page: next, pageSize: 10 }) }
    function select(row) { if (!props.loading && !props.error) { emit('select', row); emit('update:modelValue', false) } }
    return { query, page, columns, search, select }
  },
  template: `
    <YlDialog :model-value="modelValue" title="选择产品" size="lg" @update:model-value="$emit('update:modelValue', $event)">
      <form class="product-picker-filters" @submit.prevent="search()">
        <label>产品名称 <YlInput v-model="query" /></label>
        <YlButton type="submit" icon="search" :loading="loading">搜索</YlButton>
      </form>
      <YlTable :rows="rows" :columns="columns" :loading="loading" :error="error" caption="可选产品">
        <template #cell-actions="{row}"><YlButton variant="text" @click="select(row)">选择</YlButton></template>
      </YlTable>
      <template #footer><YlPagination :model-value="page" :total="total" :page-size="10" :disabled="loading" @update:model-value="search($event)" /></template>
    </YlDialog>`
}
```

A parent can mount the picker with `v-if="pickerOpen"` to reset its query on each opening. Keep the triggering button mounted so YlDialog can restore focus. Handle latest-request ordering and error/retry in the parent service; never show stale rows as successful new results. Keep this component outside the framework-managed files.

```css
.product-picker-filters { display: flex; flex-wrap: wrap; align-items: end; gap: var(--space-3); margin-bottom: var(--space-4); }
```

## Gaps and validation

For unique data visualization reuse tokens, text hierarchy and status semantics. For new complex interactions first check existing package capabilities or a suitable Vue-compatible upstream; pin and document any additional dependency and keep it out of the shared framework until maintainers explicitly adopt it. Do not silently replace the selected design system.

Preserve labels, keyboard operation, focus return, disabled/loading/error/empty states, retry and request-order handling. Validate component composition and actual API integration separately. This source example is not browser acceptance; follow the user's manual/browser policy for overlays and responsive layout.
