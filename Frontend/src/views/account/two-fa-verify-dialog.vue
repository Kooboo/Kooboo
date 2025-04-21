<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";

defineProps<{ modelValue: boolean; tip: string }>();

const emit = defineEmits<{
  (e: "update:model-value", value: boolean): void;
  (e: "ok", value: string): boolean;
}>();

const { t } = useI18n();
const code = ref();

function onOk() {
  if (emit("ok", code.value)) {
    emit("update:model-value", false);
  }
}
</script>

<template>
  <el-dialog
    :model-value="modelValue"
    width="400px"
    :close-on-click-modal="false"
    :title="t('common.verification')"
    @update:model-value="emit('update:model-value', $event)"
    @closed="code = ''"
  >
    <div class="space-y-16">
      <el-alert
        :title="tip"
        type="info"
        :closable="false"
        style="word-break: normal"
      />
      <el-input v-model="code" :placeholder="t('common.verificationCode')" />
    </div>
    <template #footer>
      <el-button
        type="primary"
        class="w-full"
        :disabled="!code"
        @click="onOk"
        >{{ t("common.confirm") }}</el-button
      >
    </template>
  </el-dialog>
</template>
