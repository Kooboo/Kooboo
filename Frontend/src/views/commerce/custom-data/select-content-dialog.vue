<script setup lang="ts">
import { onMounted, ref, computed } from "vue";
import KTable from "@/components/k-table";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { useTime } from "@/hooks/use-date";
import { useI18n } from "vue-i18n";
import { useContentEffects } from "@/views/content/contents/content-effect";

import DynamicColumns, {
  type SummaryColumn,
} from "@/components/dynamic-columns/index.vue";

interface PropsType {
  modelValue: boolean;
  folderId: string;
  multiple?: boolean;
  exclude?: string[];
}
interface EmitsType {
  (e: "update:model-value", value: boolean): void;
  (e: "success", value: any[]): void;
}

const props = defineProps<PropsType>();
const emits = defineEmits<EmitsType>();
const { t } = useI18n();
const show = ref(true);
const table = ref();

const {
  list,
  columns,
  keywords,
  columnLoaded,
  pagination,
  fetchList,
  searchCategories,
  categoryOptions,
  rawList,
} = useContentEffects(props.folderId, true, props.exclude);
const listColumns = computed<SummaryColumn[]>(() =>
  columns.value.map((it) => {
    const column: SummaryColumn = {
      name: it.name,
      displayName: it.displayName,
      controlType: it.controlType,
      multipleValue: it.multipleValue,
      attrs: {
        sortable: "custom",
        "min-width": 180,
      },
    };

    return column;
  })
);
onMounted(async () => {
  fetchList(1, 10);
});

function handleSave() {
  const value = [];
  for (const i of table.value.selected) {
    const item = rawList.value.find((f) => f.id == i.id);
    if (item) value.push(item);
  }
  emits("success", value);
  show.value = false;
}
</script>
<template>
  <el-dialog
    v-model="show"
    width="900px"
    :close-on-click-modal="false"
    :title="t('common.content')"
    @closed="emits('update:model-value', false)"
  >
    <div class="flex items-center pb-24 justify-between">
      <div class="space-x-16 flex items-center">
        <el-select
          v-for="f in categoryOptions"
          :key="f.id"
          v-model="searchCategories[f.id]"
          clearable
          multiple
          :placeholder="f.display ?? f.alias"
          @change="() => fetchList(1, 10)"
        >
          <el-option
            v-for="o in f.options"
            :key="o.key"
            :label="o.value"
            :value="o.key"
          />
        </el-select>
        <SearchInput
          v-model="keywords"
          :placeholder="t('common.searchContents')"
          class="w-238px"
          clearable
          data-cy="search"
        />
      </div>
    </div>
    <KTable
      v-if="columnLoaded"
      ref="table"
      :data="list"
      show-check
      hide-header
      :is-radio="!multiple"
      :pagination="pagination"
      hide-delete
      @change="fetchList($event, 10)"
    >
      <DynamicColumns :columns="listColumns" />
      <el-table-column
        prop="online"
        sortable="custom"
        :label="t('content.online')"
        width="120"
        align="center"
      >
        <template #default="{ row }">
          <span
            :class="row.online ? 'text-green' : 'text-999'"
            data-cy="online"
            >{{ row.online ? t("common.yes") : t("common.no") }}</span
          >
        </template>
      </el-table-column>
      <el-table-column
        :label="t('common.lastModified')"
        width="180"
        align="center"
        prop="lastModified"
        sortable="custom"
      >
        <template #default="{ row }">
          {{ useTime(row.lastModified) }}
        </template>
      </el-table-column>
    </KTable>
    <template #footer>
      <DialogFooterBar @confirm="handleSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
