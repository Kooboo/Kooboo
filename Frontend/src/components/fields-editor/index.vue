<template>
  <div class="flex items-center pb-24">
    <el-button round data-cy="new-field" @click="onAdd">
      <el-icon class="iconfont icon-a-addto" />
      {{ t("common.newField") }}
    </el-button>
  </div>
  <KTable
    ref="table"
    :data="properties"
    :row-class-name="
      ({ row, rowIndex }) => (row.isSystemField ? '' : 'draggable')
    "
    draggable=".draggable"
    row-key="name"
    @sorted="updateTableData($event)"
  >
    <el-table-column :label="t('common.name')">
      <template #default="{ row }">
        <span :class="{ 'text-999': row.isSystemField }" data-cy="field-name">
          {{ row.name }}
        </span>
      </template>
    </el-table-column>
    <el-table-column :label="t('common.displayName')"
      ><template #default="{ row }">
        <span data-cy="display-name">{{ row.displayName }}</span>
      </template>
    </el-table-column>
    <el-table-column :label="t('common.controlType')" width="200">
      <template #default="{ row }">
        <span data-cy="control-type">
          {{ getControlType(row.controlType)?.displayName }}
        </span>
      </template>
    </el-table-column>
    <el-table-column width="120" align="right">
      <template #default="{ row, $index }">
        <IconButton
          icon="icon-a-writein"
          :tip="t('common.edit')"
          data-cy="edit"
          @click="onEdit(row)"
        />
        <IconButton
          v-if="!row.isSystemField"
          :permission="{
            feature: 'contentType',
            action: 'edit',
          }"
          icon="icon-delete text-orange hover:text-orange"
          :tip="t('common.delete')"
          data-cy="delete"
          @click="removeItem($index)"
        />
        <IconButton
          icon="icon-move js-sortable cursor-move"
          :tip="t('common.move')"
          data-cy="move"
        />
      </template>
    </el-table-column>
  </KTable>
  <EditFieldDialog
    v-if="showEditFieldDialog"
    v-model="showEditFieldDialog"
    :fields="properties"
    :field="editingField"
    :options="propertyOptions"
    :show-default-value="showDefaultValue"
    :hide-preview-field="hidePreviewField"
    :enable-dynamic-options="enableDynamicOptions"
  />
</template>

<script lang="ts" setup>
import EditFieldDialog from "./edit-field-dialog.vue";
import KTable from "@/components/k-table";
import { onMounted, ref, onBeforeUnmount, watch } from "vue";
import { useControlTypes } from "@/hooks/use-control-types";
import { cloneDeep } from "lodash-es";

import { useI18n } from "vue-i18n";
import type {
  JsonStringField,
  PropertyJsonString,
  Property,
  PropertyOptions,
} from "@/global/control-type";

const editingField = ref<Property>();

const { t } = useI18n();
const { getControlType } = useControlTypes();

const showEditFieldDialog = ref(false);

const properties = ref<Property[]>([]);
const props = defineProps<{
  modelValue: PropertyJsonString[];
  permission?: {
    feature: string;
    action: string;
  };
  propertyOptions?: PropertyOptions;
  showDefaultValue?: boolean;
  hidePreviewField?: boolean;
  enableDynamicOptions?: boolean;
}>();
const emit = defineEmits<{
  (e: "update:model-value", value: PropertyJsonString[]): void;
}>();

onMounted(() => {
  properties.value = sortProperties(
    props.modelValue.map((row: any) => {
      const propertyRow: Property = cloneDeep(row);
      parseJsonFieldToJson(propertyRow, "selectionOptions");
      parseJsonFieldToJson(propertyRow, "validations");
      parseJsonFieldToJson(propertyRow, "settings");
      return propertyRow;
    })
  );
});

onBeforeUnmount(() => {
  properties.value = [];
  editingField.value = undefined;
  showEditFieldDialog.value = false;
});

const updateTableData = (value: Property[]) => {
  const sortedMaps: Record<string, Property> = {};
  value.forEach((it: Property, index: number) => {
    it.order = index;
    sortedMaps[it.name] = it;
  });
  properties.value = sortProperties(properties.value).map((it: Property) => {
    return sortedMaps[it.name] || it;
  });
};
function sortProperties(props: Property[]) {
  return props.sort((a, b) => {
    if (a.isSystemField !== b.isSystemField) {
      if (a.isSystemField) {
        return 1;
      }
      if (b.isSystemField) {
        return -1;
      }
    }
    return a.order > b.order ? 1 : -1;
  });
}

async function removeItem(index: number) {
  properties.value.splice(index, 1);
}

function parseJsonFieldToJson(row: Property, field: JsonStringField) {
  if (row[field]) {
    row[field] =
      typeof row[field] === "string"
        ? JSON.parse(row[field] as unknown as string)
        : row[field];
  } else {
    row[field] = [];
  }
}
function parseJsonFieldToString(row: Property, field: JsonStringField): string {
  return (typeof row[field] === "string"
    ? row[field]
    : JSON.stringify(row[field])) as unknown as string;
}

function onAdd() {
  editingField.value = undefined;
  showEditFieldDialog.value = true;
}

function onEdit(row: Property) {
  editingField.value = cloneDeep(row);
  parseJsonFieldToJson(editingField.value, "selectionOptions");
  parseJsonFieldToJson(editingField.value, "validations");
  parseJsonFieldToJson(editingField.value, "settings");
  showEditFieldDialog.value = true;
}

watch(
  () => properties.value,
  (list) => {
    const fields: PropertyJsonString[] = list.map((item) => {
      return {
        ...item,
        selectionOptions: parseJsonFieldToString(item, "selectionOptions"),
        validations: parseJsonFieldToString(item, "validations"),
        settings: parseJsonFieldToString(item, "settings"),
      };
    });
    emit("update:model-value", fields);
  },
  {
    deep: true,
  }
);
</script>

<style lang="scss" scoped>
:deep(.el-table__row) {
  &:not(.draggable) {
    .js-sortable {
      display: none;
    }
  }
}
</style>
