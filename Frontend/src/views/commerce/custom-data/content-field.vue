<script lang="ts" setup>
import SelectContentDialog from "./select-content-dialog.vue";
import { computed, ref } from "vue";
import type { SortEvent } from "@/global/types";
import { getByIds } from "@/api/content/textContent";
import defaultImage from "@/assets/images/commerce_default_image.svg";
import { combineUrl } from "@/utils/url";
import { useSiteStore } from "@/store/site";
import { getValueIgnoreCase } from "@/utils/string";
import type { TextContentDetails, TextContentColumn } from "./content-field";
import ContentDialog from "@/views/content/contents/content-dialog.vue";
import { emptyGuid } from "@/utils/guid";

const props = defineProps<{
  modelValue: string[];
  contentFolder: string;
  multiple?: boolean;
  allowRepetition?: boolean;
  embedded?: boolean;
}>();
const emit = defineEmits<{
  (e: "update:model-value", value: string[]): void;
}>();
const showSelectDialog = ref(false);
const showContentDialog = ref(false);
const selected = ref();
const contents = ref<TextContentDetails>({
  columns: [],
  list: [],
});

function onAdd() {
  if (props.embedded) {
    selected.value = { id: emptyGuid, folderId: props.contentFolder };
    showContentDialog.value = true;
  } else {
    showSelectDialog.value = true;
  }
}

async function load() {
  const rsp = await getByIds(props.modelValue || [], props.contentFolder ?? "");
  contents.value = rsp;
  emit(
    "update:model-value",
    contents.value.list.map((m) => m.id)
  );
}

load();

const columns = computed(() => {
  const result: any[] = [];
  if (!contents.value) return result;
  for (const i of contents.value.columns) {
    if (i.controlType == "MediaFile") {
      result.unshift(i);
    } else {
      result.push(i);
    }
  }
  return result;
});

function onUpdate(model: any[]) {
  let result = [...props.modelValue];

  if (props.multiple) {
    for (const i of model) {
      if (!contents.value.list.find((f: any) => f.id == i.id))
        contents.value.list.push(i);
      result.push(i.id);
    }
  } else {
    result = model.map((m) => m.id);
    contents.value.list = model;
  }

  emit("update:model-value", result);
}

function onDelete(id: string) {
  const index = props.modelValue.findIndex((f) => f == id);
  if (index > -1) props.modelValue.splice(index, 1);
  emit("update:model-value", props.modelValue);
}

function onShort(e: SortEvent) {
  const item = props.modelValue.splice(e.oldIndex, 1)[0];
  props.modelValue.splice(e.newIndex, 0, item);
  emit("update:model-value", props.modelValue);
}

function getContent(id: string, key: string) {
  const item = contents.value?.list?.find((f: any) => f.id == id);
  if (!item) return "";
  return getValueIgnoreCase(item.textValues, key);
}

const siteStore = useSiteStore();

function previewUrl(value: string) {
  const url = value ? combineUrl(siteStore.site.prUrl, value) : defaultImage;
  return `url("${url}")`;
}

function getImages(id: string, column: TextContentColumn) {
  if (column.controlType !== "MediaFile") {
    return [];
  }

  const content = getContent(id, column.name);
  if (!content) {
    return [];
  }
  if (!column.multipleValue) {
    return [content];
  }

  try {
    return JSON.parse(content);
  } catch (error) {
    console.warn(["JSON parse error", error, content]);
    return [];
  }
}

function onEditContent(item: string) {
  selected.value = contents.value.list.find((f) => f.id == item);
  if (selected.value) showContentDialog.value = true;
}

function onContentSave(id: string) {
  const index = props.modelValue.findIndex((f) => f == id);
  if (index == -1) props.modelValue.push(id);
  emit("update:model-value", props.modelValue);
  load();
}
</script>

<template>
  <div v-if="contents?.columns?.length" class="w-504px">
    <SortableList
      wrapper-class="space-y-4 w-full"
      :list="modelValue"
      @add="onAdd"
      @delete="onDelete"
      @sort="onShort"
    >
      <template #default="{ item }">
        <el-scrollbar>
          <div class="flex space-x-4">
            <template v-for="i of columns" :key="i.name">
              <template v-if="i.controlType == 'MediaFile'">
                <div
                  v-for="(url, ix) in getImages(item, i)"
                  :key="ix"
                  class="w-32 h-32 bg-contain bg-center bg-no-repeat rounded-4px overflow-hidden inline-block relative group bg-gray"
                  :style="{
                    backgroundImage: previewUrl(url),
                  }"
                />
              </template>
              <div v-else>
                {{ getContent(item, i.name) }}
              </div>
            </template>
          </div>
        </el-scrollbar>
      </template>
      <template #right="{ item }">
        <a
          class="cursor-pointer px-4 hover:text-blue"
          data-cy="edit"
          @click="onEditContent(item)"
        >
          <el-icon class="iconfont icon-a-writein" />
        </a>
      </template>
    </SortableList>
    <Teleport to="body">
      <SelectContentDialog
        v-if="showSelectDialog"
        v-model="showSelectDialog"
        :folder-id="contentFolder"
        :multiple="multiple"
        :exclude="allowRepetition ? [] : modelValue"
        @success="onUpdate"
      />
    </Teleport>
  </div>
  <ContentDialog
    v-if="showContentDialog"
    :id="selected?.id ?? emptyGuid"
    v-model="showContentDialog"
    :folder="selected?.folderId ?? emptyGuid"
    @reload="onContentSave"
  />
</template>
