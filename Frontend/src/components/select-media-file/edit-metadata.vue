<template>
  <el-dialog
    v-model="visible"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.editCommonName', { name: t('common.meta') })"
    custom-class="el-dialog--zero-padding"
    @close="handleClose"
  >
    <div class="px-32 py-24">
      <el-scrollbar max-height="50vh">
        <el-form :model="model" label-position="top" @submit.prevent>
          <el-form-item
            v-for="item in meta"
            :key="item.key"
            :prop="item.key"
            :label="item.key"
          >
            <el-input v-model="model[item.key]" />
          </el-form-item>
        </el-form>
      </el-scrollbar>
    </div>
    <template #footer>
      <DialogFooterBar
        :confirm-label="t('common.save')"
        @confirm="handleSave"
        @cancel="handleClose"
      />
    </template>
  </el-dialog>
</template>

<script lang="ts" setup>
import type { UPDATE_MODEL_EVENT } from "@/constants/constants";
import type { KeyValue } from "@/global/types";
import useOperationDialog from "@/hooks/use-operation-dialog";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { cloneDeep } from "lodash-es";

interface PropsType {
  modelValue: boolean;
  meta: KeyValue[];
}

interface EmitsType {
  (e: typeof UPDATE_MODEL_EVENT, value: boolean): void;
  (e: "save", value: Record<string, string>): void;
}
const props = defineProps<PropsType>();
const emits = defineEmits<EmitsType>();
const { t } = useI18n();
const { visible, handleClose } = useOperationDialog(props, emits);

const model = ref<Record<string, string>>({});

function showDialog(data: Record<string, string>) {
  const modelData: Record<string, string> = {};
  for (const item of props.meta) {
    modelData[item.key] = data[item.key] || item.value;
  }
  model.value = modelData;
  visible.value = true;
}

function handleSave() {
  emits("save", cloneDeep(model.value));
  handleClose();
}

defineExpose({
  showDialog,
});
</script>
