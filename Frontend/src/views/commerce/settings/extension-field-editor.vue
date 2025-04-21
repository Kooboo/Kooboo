<script lang="ts" setup>
import type { ExtensionField } from "@/api/commerce/settings";
import ExtensionFieldDialog from "./extension-field-dialog.vue";
import { ref } from "vue";
import type { SortEvent } from "@/global/types";
import { useI18n } from "vue-i18n";
import { dataTypes } from "./settings";

const { t } = useI18n();
const props = defineProps<{
  data: ExtensionField[];
}>();

const showExtensionFieldDialog = ref(false);
const selected = ref<ExtensionField>();
let editIndex = -1;

function onAdd() {
  selected.value = {
    name: "",
    type: "string",
    displayName: "",
    editable: false,
    isSummaryField: false,
    exportable: false,
    filterable: false,
    width: 120,
  };
  editIndex = -1;
  showExtensionFieldDialog.value = true;
}

function onEdit(id: string, model: ExtensionField) {
  selected.value = model;
  editIndex = props.data.indexOf(model);
  showExtensionFieldDialog.value = true;
}

function onUpdate(model: ExtensionField) {
  if (editIndex == -1) {
    props.data.push(model);
  } else {
    props.data.splice(editIndex, 1, model);
  }
}

function onDelete(id: string) {
  const index = props.data.findIndex((f) => f.name == id);
  if (index > -1) props.data.splice(index, 1);
}

function onShort(e: SortEvent) {
  const item = props.data.splice(e.oldIndex, 1)[0];
  props.data.splice(e.newIndex, 0, item);
}
</script>

<template>
  <div class="w-full">
    <SortableList
      wrapper-class="space-y-4 w-full"
      :list="data"
      id-prop="name"
      editable
      @add="onAdd"
      @delete="onDelete"
      @sort="onShort"
      @edit="onEdit"
    >
      <template #default="{ item }">
        <div class="flex items-center">
          <div class="ellipsis flex-1">{{ item.displayName || item.name }}</div>
          <ElTag v-if="item.isSummaryField" round class="mr-4" type="warning">
            {{ t("common.summaryField") }}
          </ElTag>
          <ElTag round>{{ (dataTypes as any)[item.type] || item.type }}</ElTag>
        </div>
      </template>
    </SortableList>
    <Teleport to="body">
      <ExtensionFieldDialog
        v-if="showExtensionFieldDialog && selected"
        v-model="showExtensionFieldDialog"
        :field="selected"
        :fields="data"
        @success="onUpdate"
      />
    </Teleport>
  </div>
</template>
