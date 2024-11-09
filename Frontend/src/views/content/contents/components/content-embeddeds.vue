<template>
  <el-form-item
    v-for="(embedded, index) in embeddeds"
    :key="index"
    :label="embedded.display || embedded.alias"
  >
    <SortableList
      class="!p-0 min-w-600px"
      :list="embedded.contents"
      id-prop="id"
      @sort="onSort(embedded.contents, $event)"
      @delete="remove"
      @add="edit(embedded)"
    >
      <template #default="{ item }">
        {{ getText(item, embedded) }}
      </template>
      <template #right="{ item }">
        <a
          class="cursor-pointer px-4 hover:text-blue"
          data-cy="edit"
          @click="edit(embedded, item)"
        >
          <el-icon class="iconfont icon-a-writein" />
        </a>
      </template>
    </SortableList>
  </el-form-item>
  <Teleport to="body">
    <EditEmbeddedDialog
      v-model="visibleEdit"
      :current="current"
      :current-content="currentContent"
      :paths="[...(paths ?? []), getText(currentContent!, current)]"
      @save-success="onSaveSuccess"
    />
  </Teleport>
</template>

<script lang="ts" setup>
import type { SortEvent } from "@/global/types";
import { deletes } from "@/api/content/textContent";
import type {
  ContentEmbedded,
  TextContentItem,
} from "@/api/content/textContent";
import { useSync } from "@/hooks/use-sync";
import { ref } from "vue";
import EditEmbeddedDialog from "./edit-embedded-dialog.vue";

import SortableList from "@/components/sortable-list/index.vue";
import { showConfirm } from "@/components/basic/confirm";
import { useI18n } from "vue-i18n";
interface PropsType {
  modelValue: ContentEmbedded[];
  id?: string;
  paths?: string[];
}
const props = defineProps<PropsType>();
const emits = defineEmits<{
  (e: "update:modelValue"): void;
}>();

const { t } = useI18n();
const embeddeds = useSync(props, "modelValue", emits);
const visibleEdit = ref(false);
const current = ref<ContentEmbedded>({} as ContentEmbedded);
const currentContent = ref<TextContentItem>();

function getText(content: TextContentItem, embedded: ContentEmbedded) {
  if (!content) return "";
  let key =
    content.summaryField ??
    embedded.columns.find((f: any) => f.isSummaryField)?.name ??
    Object.keys(content.textValues)[0];
  key =
    Object.keys(content.textValues).find(
      (f) => f.toLowerCase() == key.toLowerCase()
    ) ?? "";
  var value = content.textValues[key];

  const column = embedded.columns.find(
    (f: any) => f.name?.toLowerCase() == key?.toLowerCase()
  );
  if (column?.selectionOptions) {
    try {
      const options = JSON.parse(column.selectionOptions);
      let values = [value];
      if (column.controlType == "CheckBox") {
        values = JSON.parse(value);
      }
      const displayValues = [];
      for (const i of values) {
        const option = options.find((f: any) => f.value == i);
        if (option) displayValues.push(option.key);
        else displayValues.push(i);
      }
      value = displayValues.join(",");
    } catch {
      //
    }
  }
  return value;
}
async function remove(
  _id: string,
  _item: any,
  context: { list: unknown[]; index: number }
) {
  await showConfirm(t("common.deleteEmbeddedFolderTips"));
  const removedItems = context.list.splice(
    context.index,
    1
  ) as TextContentItem[];
  deletes({
    ids: removedItems.map((m) => m.id),
    parentId: props.id,
  });
}

function edit(embedded: ContentEmbedded, content?: TextContentItem) {
  current.value = embedded;
  currentContent.value = content;
  visibleEdit.value = true;
}

function onSaveSuccess(content: TextContentItem) {
  if (currentContent.value) {
    currentContent.value.textValues = content.textValues;
  } else {
    current.value.contents.push(content);
  }
}

function onSort(list: TextContentItem[], e: SortEvent) {
  const oldItem = list.splice(e.oldIndex, 1)[0];
  list.splice(e.newIndex, 0, oldItem);
  list.forEach((it: TextContentItem, i: number) => {
    it.order = i;
  });
}
</script>
