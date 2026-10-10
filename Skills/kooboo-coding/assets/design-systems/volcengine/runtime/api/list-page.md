# YlListPage

Import `{ YlListPage } from '/yuanli.js'`; register in `components`.

Props: `title: string` required; `description: string = ''` (omit when redundant).
Slots: `actions` for current-page actions; `filters` for labelled filter controls; default for table/content; `pagination` for page navigation. No events or internal business state.

```html
<YlListPage title="订单">
  <template #actions><YlButton variant="primary" @click="create">新建订单</YlButton></template>
  <template #filters><label>搜索 <YlInput v-model="query" /></label></template>
  <YlTable :rows="rows" :columns="columns" caption="订单" :loading="loading" :error="error" />
  <template #pagination><YlPagination v-model="page" :total="total" /></template>
</YlListPage>
```

The Shell supplies outer padding. This component owns one page title, grouping, toolbar and pagination placement; no Shell, sidebar, KPI row, sample data or API calls are included. Empty slots leave no blank region. Read the selected child APIs before binding them.
