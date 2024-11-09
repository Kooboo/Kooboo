<template>
  <el-form-item
    v-for="(category, index) in categories"
    :key="index"
    :label="category.display || category.alias"
  >
    <SortableList
      class="!p-0 min-w-600px"
      :list="category.contents"
      id-prop="id"
      @sort="onSort(category.contents, $event)"
      @delete="remove"
      @add="edit(category)"
    >
      <template #default="{ item }">
        {{ getText(item, category) }}
      </template>
    </SortableList>
  </el-form-item>
  <Teleport to="body">
    <EditCategoryDialog v-model="visibleEdit" :current="current" />
  </Teleport>
</template>

<script lang="ts" setup>
import type { SortEvent } from "@/global/types";

import type {
  ContentCategory,
  TextContentItem,
} from "@/api/content/textContent";
import { useSync } from "@/hooks/use-sync";
import { ref } from "vue";
import EditCategoryDialog from "./edit-category-dialog.vue";

import SortableList from "@/components/sortable-list/index.vue";
interface PropsType {
  modelValue: ContentCategory[];
}
const props = defineProps<PropsType>();
const emits = defineEmits<{
  (e: "update:modelValue"): void;
}>();

const categories = useSync(props, "modelValue", emits);
const visibleEdit = ref(false);
const current = ref<ContentCategory>({} as ContentCategory);
function getText(content: TextContentItem, category: ContentCategory) {
  let key = content.summaryField ?? Object.keys(content.textValues)[0];
  key =
    Object.keys(content.textValues).find(
      (f) => f.toLowerCase() == key.toLowerCase()
    ) ?? "";
  let value = content.textValues[key];

  const column = category.columns.find(
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

function remove(
  _id: string,
  _item: any,
  context: { list: unknown[]; index: number }
) {
  context.list.splice(context.index, 1);
}

function edit(category: ContentCategory) {
  current.value = category;
  visibleEdit.value = true;
}

function onSort(list: TextContentItem[], e: SortEvent) {
  const oldItem = list.splice(e.oldIndex, 1)[0];
  list.splice(e.newIndex, 0, oldItem);
  list.forEach((it: TextContentItem, i: number) => {
    it.order = i;
  });
}
</script>
