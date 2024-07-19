<template>
  <el-dialog
    :model-value="show"
    :title="t('common.result')"
    width="1024px"
    :close-on-click-modal="false"
    @closed="emit('update:modelValue', false)"
  >
    <KTable
      v-model:selectedData="selectedData"
      :data="result"
      show-check
      :max-height="300"
      hide-delete
    >
      <template #leftBar>
        <span class="ml-4">({{ t("common.scanLabelTips") }})</span>
      </template>
      <el-table-column :label="t('common.type')" width="100">
        <template #default="{ row }">
          <ObjectTypeTag :type="row.type" />
        </template>
      </el-table-column>

      <el-table-column :label="t('common.displayName')" width="150">
        <template #default="{ row }">
          {{ row.displayName }}
        </template>
      </el-table-column>

      <el-table-column :label="t('common.attributes')" width="100">
        <template #default="{ row }">
          {{ row.attribute }}
        </template>
      </el-table-column>

      <el-table-column :label="t('common.key')" width="300">
        <template #default="{ row }">
          <el-input v-model="row.suggestKey" autosize type="textarea" />
        </template>
      </el-table-column>

      <el-table-column :label="t('common.value')">
        <template #default="{ row }">
          {{ row.tag }}
        </template>
      </el-table-column>
    </KTable>
    <template #footer>
      <DialogFooterBar
        :permission="{
          feature: 'label',
          action: 'edit',
        }"
        :confirm-label="
          props.type === 'scan' ? t('common.addLabel') : t('common.update')
        "
        :disabled="!selectedData.length"
        @confirm="handleSave"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>

<script lang="ts" setup>
import type { ScanList } from "@/api/content/label";
import { ConfirmI18NAdd } from "@/api/content/label";
import { ScanLabel } from "@/api/content/label";
import { Scan, ConfirmAdd } from "@/api/content/label";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
const { t } = useI18n();
const result = ref<ScanList[]>([]);
const show = ref(true);
const selectedData = ref<ScanList[]>([]);

const props = defineProps<{
  modelValue: boolean;
  type: string;
}>();
const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "load"): void;
}>();

const handleSave = async () => {
  if (props.type === "scan") {
    await ConfirmAdd(selectedData.value);
  } else {
    await ConfirmI18NAdd(selectedData.value);
  }
  show.value = false;
  emit("load");
};

const load = async () => {
  if (props.type === "scan") {
    result.value = await Scan();
  } else {
    result.value = await ScanLabel();
  }
};

load();
</script>
