# Element Plus business extensions

Use public native APIs first. Business Scripts and `/app.css` own extensions; installer-managed runtime assets remain unchanged. Read the APIs returned for the selected components rather than reconstructing their DOM.

## Public styling

Use `--el-color-primary`, `--el-color-success`, `--el-color-warning`, `--el-color-danger`, `--el-text-color-primary`, `--el-text-color-regular`, `--el-text-color-secondary`, `--el-border-color`, `--el-border-color-light`, `--el-bg-color`, `--el-bg-color-page`, `--el-fill-color-light`, `--el-border-radius-base`, `--el-font-size-base` and `--el-font-family`. State variants include `--el-color-primary-light-3/5/7/8/9` and `--el-color-primary-dark-2`; changing only primary leaves hover/active colors unchanged. Scope business-specific variables to a wrapper and account for teleported overlays, which may be outside that wrapper. Avoid blanket overrides of native `.el-*` geometry.

Use `ElConfigProvider` for locale, default size and z-index. Overlays need a deliberate stacking context and lifecycle; do not turn off teleport solely to hide a geometry problem. Keep standard keyboard/focus behavior and test actual first-open positioning manually.

## Executable product selector

Use `ElDialog`, `ElInput`, `ElTable`, `ElTableColumn`, `ElButton`. Save this as a business Script such as `component.product-selector.js`; the parent supplies records fetched by its service. The example does not invent a backend or claim that the displayed inventory is current.

```js
import { computed, ref } from 'vue';
import { ElDialog, ElInput, ElTable, ElTableColumn, ElButton } from '/element-plus.js';
export default {
  name: 'ProductSelector',
  components: { ElDialog, ElInput, ElTable, ElTableColumn, ElButton },
  props: { modelValue: Boolean, products: { type: Array, default: () => [] }, busy: Boolean, error: String },
  emits: ['update:modelValue', 'select', 'retry'],
  setup(props, { emit }) {
    const keyword = ref('');
    const rows = computed(() => props.products.filter(row => row.name.toLowerCase().includes(keyword.value.toLowerCase())));
    function select(row) {
      if (props.busy || row.disabled) return;
      emit('select', row);
      emit('update:modelValue', false);
    }
    return { keyword, rows, select };
  },
  template: `<ElDialog :model-value="modelValue" @update:model-value="$emit('update:modelValue', $event)" title="选择产品" width="min(720px, 94vw)">
    <ElInput v-model="keyword" clearable aria-label="搜索产品" />
    <p v-if="busy" role="status">正在加载</p>
    <p v-else-if="error" role="alert">{{ error }} <ElButton @click="$emit('retry')">重试</ElButton></p>
    <ElTable v-else :data="rows" row-key="id" empty-text="没有匹配的产品">
      <ElTableColumn prop="name" label="产品" />
      <ElTableColumn label="操作"><template #default="{ row }"><ElButton :disabled="row.disabled" @click="select(row)">选择</ElButton></template></ElTableColumn>
    </ElTable>
  </ElDialog>`
};
```

For server-side filtering/paging, move filtering into the service and guard stale responses. Keep loading, empty and failure states distinct. Parent business code validates the selected record before mutation and handles permissions. Test cancellation, keyboard focus return, narrow widths and failed requests. Generalize into the framework only after a recurring cross-App need has been evaluated by maintainers.
