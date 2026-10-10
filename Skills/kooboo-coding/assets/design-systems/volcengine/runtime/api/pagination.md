# YlPagination

Import `{ YlPagination } from '/yuanli.js'`; register in `components`.

Props: `modelValue: number = 1` (`v-model`, integer, 1-based); `total: number` required (finite, >=0); `pageSize: number = 20` (positive integer); `disabled: boolean = false`; `label = '分页'`; `previousLabel = '上一页'`; `nextLabel = '下一页'`.
Event: `update:modelValue(number)`. At least one page exists even when total=0. Total/pageSize changes clamp the current page and emit the corrected value. First/previous/nearby/last/next navigation stays bounded; the component never requests or slices data.

```html
<YlPagination v-model="page" :total="total" :page-size="20" :disabled="loading" />
```

Watch page in the business layer to load rows. Reset page to 1 when filters change. Optional showTotal, showSizeChanger, showJumper and simple flags default false. size=md (md/sm); pageSizes=[10,20,50,100]. The size chooser emits update:pageSize and resets modelValue to 1. Bind v-model:pageSize and coordinate one business reload when both values change. Jumper commits on Enter; out-of-range or fractional values do not change the page.

Previous/next render Lucide chevrons; previousLabel and nextLabel remain their accessible names and tooltips.
