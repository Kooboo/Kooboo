<script lang="ts" setup>
import type { CustomField } from "@/api/commerce/settings";
import CustomFieldDialog from "./custom-field-dialog.vue";
import { ref } from "vue";
import type { SortEvent } from "@/global/types";
import { fieldTypes } from "../custom-data/custom-field";
import { useI18n } from "vue-i18n";
import { useLabels } from "../useLabels";

const { t } = useI18n();
const props = defineProps<{
  data: CustomField[];
  labels: Record<string, string>;
}>();
const { getFieldDisplayName } = useLabels(props.labels);

const showCustomFieldDialog = ref(false);
const selected = ref<CustomField>();
let editIndex = -1;

function onAdd() {
  selected.value = {
    name: "",
    type: "TextBox",
    displayName: "",
    editable: true,
    validations: [],
    multiple: false,
    selectionOptions: [],
    isSystemField: false,
    isSummaryField: false,
    multilingual: false,
  };
  editIndex = -1;
  showCustomFieldDialog.value = true;
}

function onEdit(id: string, model: CustomField) {
  selected.value = model;
  editIndex = props.data.indexOf(model);
  showCustomFieldDialog.value = true;
}

function onUpdate(model: CustomField) {
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
      :disable-actions="{ 'delete': (row: CustomField) => row.isSystemField }"
      :delete-tip="(row: CustomField) => row.isSystemField ? t('common.systemFieldNotSupportDelete') : ''"
      @add="onAdd"
      @delete="onDelete"
      @sort="onShort"
      @edit="onEdit"
    >
      <template #default="{ item }">
        <div class="flex items-center">
          <div class="ellipsis flex-1">{{ getFieldDisplayName(item) }}</div>
          <ElTag v-if="item.isSummaryField" round class="mr-4" type="warning">
            {{ t("common.summaryField") }}
          </ElTag>
          <ElTag round>{{ (fieldTypes as any)[item.type]?.displayName }}</ElTag>
        </div>
      </template>
    </SortableList>
    <Teleport to="body">
      <CustomFieldDialog
        v-if="showCustomFieldDialog && selected"
        v-model="showCustomFieldDialog"
        :field="selected"
        :fields="data"
        @success="onUpdate"
      />
    </Teleport>
  </div>
</template>
