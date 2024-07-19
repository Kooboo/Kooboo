<template>
  <el-dialog
    v-model="visible"
    width="800px"
    :close-on-click-modal="false"
    :title="t('common.move')"
    custom-class="el-dialog--fixed-footer editEmbeddedDialog"
    destroy-on-close
    @close="handleClose"
  >
    <KTable
      v-show="columnLoaded"
      :data="tableList"
      row-key="id"
      :pagination="pagination"
      :max-height="300"
      show-check
      hide-delete
      :is-radio="true"
      :permission="{
        feature: 'content',
        action: 'view',
      }"
      @change="fetchList($event)"
    >
      <template #leftBar="{ selected }">
        <div class="h-60px flex items-center">
          <div>
            <el-button
              round
              class="dark:bg-666"
              :disabled="!selected.length"
              @click="handleSave(selected[0])"
            >
              <div class="flex items-center">
                {{ t("common.moveBehind") }}
              </div>
            </el-button>
          </div>
        </div>
      </template>
      <template #bar>
        <SearchInput
          v-model="keywords"
          :placeholder="t('common.searchContents')"
          class="w-238px"
          clearable
          data-cy="search"
        />
      </template>
      <DynamicColumns :columns="listColumns" />
      <el-table-column :label="t('content.online')" width="100" align="center">
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
      >
        <template #default="{ row }">
          {{ useTime(row.lastModified) }}
        </template>
      </el-table-column>
    </KTable>
  </el-dialog>
</template>

<script lang="ts" setup>
import { useI18n } from "vue-i18n";
import { ref, computed } from "vue";
import type { TableRowItem } from "../types";
import { useTime } from "@/hooks/use-date";
import { useContentEffects } from "../content-effect";
import type { ContentFolder } from "@/api/content/content-folder";
import type { MoveTextContent } from "@/api/content/textContent";
import { moveContent } from "@/api/content/textContent";
import DynamicColumns, {
  type SummaryColumn,
} from "@/components/dynamic-columns/index.vue";

interface PropsType {
  folder: ContentFolder;
}

const props = defineProps<PropsType>();
const emits = defineEmits<{
  (e: "reload"): void;
}>();

const visible = ref(false);

const source = ref<string>("");

const { t } = useI18n();
const { list, columnLoaded, pagination, fetchList, columns, keywords } =
  useContentEffects(props.folder!.id);

const listColumns = computed<SummaryColumn[]>(() =>
  columns.value.map((it) => {
    const column: SummaryColumn = {
      name: it.name,
      displayName: it.displayName,
      controlType: it.controlType,
      multipleValue: it.multipleValue,
    };

    return column;
  })
);

const tableList = computed(() => {
  return list.value.map((it: TableRowItem) => {
    it.$DisabledSelect = it.id === source.value;
    return it;
  });
});

const show = (row: TableRowItem) => {
  fetchList(1, props.folder.pageSize);
  source.value = row.id;
  visible.value = true;
};

async function handleSave(selected: TableRowItem) {
  const prevIndex = tableList.value.findIndex((o) => o.id === selected.id) + 1;
  const prev: TableRowItem | undefined = tableList.value[prevIndex];
  const next: TableRowItem | undefined = selected;
  const data: MoveTextContent = {
    source: source.value,
    nextId: next?.id,
    prevId: prev?.id,
    folderId: props.folder.id,
  };
  await moveContent(data);
  emits("reload");
  handleClose();
}

function handleClose() {
  visible.value = false;
}

defineExpose({
  show,
});
</script>
