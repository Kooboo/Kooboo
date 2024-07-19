<script lang="ts" setup>
import { ref } from "vue";
import MonacoEditor from "@/components/monaco-editor/index.vue";
import { useI18n } from "vue-i18n";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { Uri } from "monaco-editor";
import { newGuid } from "@/utils/guid";

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "update:code", value: string): void;
}>();

const props = defineProps<{ modelValue: boolean; code: string }>();
const { t } = useI18n();
const show = ref(true);
const innerCode = ref(props.code);

const onSave = async () => {
  emit("update:code", innerCode.value);
  show.value = false;
};
</script>

<template>
  <el-dialog
    :model-value="show"
    custom-class="el-dialog--zero-padding"
    width="800px"
    :close-on-click-modal="false"
    :title="t('common.code')"
    @closed="emit('update:modelValue', false)"
  >
    <div class="p-24 h-500px">
      <MonacoEditor
        v-model="innerCode"
        language="typescript"
        k-script
        :uri="Uri.file(newGuid())"
      />
    </div>
    <template #footer>
      <DialogFooterBar @confirm="onSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
