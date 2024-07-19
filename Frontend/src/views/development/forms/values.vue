<script lang="ts" setup>
import KTable from "@/components/k-table";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { ref, computed } from "vue";
import type { Values, Value } from "@/api/form/types";
import { getQueryString } from "@/utils/url";
import { useI18n } from "vue-i18n";
import { useFormStore } from "@/store/form";
import { showDeleteConfirm } from "@/components/basic/confirm";

const { t } = useI18n();
const formStore = useFormStore();

const model = ref<Values>({
  list: [],
  pageNr: 1,
  pageSize: 30,
  totalCount: 0,
  totalPages: 0,
});

const cols = computed(() => {
  let _cols = new Set();
  model.value.list.forEach((item) => {
    Object.keys(item.values)?.forEach((key) => _cols.add(key));
  });
  return Array.from(_cols) as string[];
});

const load = async (pageNr: number) => {
  model.value = await formStore.getFormValues(getQueryString("id")!, pageNr);
  model.value.list || (model.value.list = []);
  model.value.list.sort((a, b) => (a.lastModified > b.lastModified ? -1 : 1));
};

const onDeletes = async (items: Value[]) => {
  await showDeleteConfirm(items.length);
  formStore.deleteFormValues(items.map((m) => m.id));
  load(model.value.pageNr);
};
load(model.value.pageNr);
</script>

<template>
  <div class="p-24 flex items-center">
    <Breadcrumb
      :crumb-path="[
        { name: t('common.forms'), route: { name: 'forms' } },
        { name: t('common.data') },
      ]"
    />
  </div>
  <div class="p-24 pt-0">
    <KTable
      :data="model.list"
      show-check
      table-layout="auto"
      :pagination="{
        pageSize: model.pageSize,
        pageCount: model.totalPages,
        currentPage: model.pageNr,
      }"
      @change="load"
      @delete="onDeletes"
    >
      <el-table-column
        v-for="col in cols"
        :key="col"
        :label="col"
        :prop="`values.${col}`"
      />
    </KTable>
  </div>
</template>
