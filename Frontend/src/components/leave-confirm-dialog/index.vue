<template>
  <el-dialog
    v-model="show"
    width="400px"
    custom-class="leaveConfirmDialog"
    :close-on-click-modal="false"
    @closed="emits('update:modelValue', false)"
  >
    <div class="flex justify-center">
      <div v-html="LogoIcon" />
    </div>

    <div
      class="mt-18px text-s text-[#606266] font-bold leading-17px px-12 dark:text-[#cfd3dc] break-normal"
    >
      {{ t("common.mailUnSaveTips") }}
    </div>
    <div class="flex justify-end items-center mt-16">
      <el-button round @click="emits('closeDialog')">
        {{ t("common.cancel") }}
      </el-button>
      <el-button round @click="emits('cancelSaveDraft')">
        {{ t("common.no") }}
      </el-button>
      <el-button type="primary" round @click="emits('saveDraft')">
        {{ t("common.yes") }}
      </el-button>
    </div>
  </el-dialog>
</template>
<script setup lang="ts">
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import LogoIcon from "@/assets/images/logo-transparent.svg?raw";

const { t } = useI18n();
interface PropsType {
  modelValue: boolean;
}
interface EmitsType {
  (e: "update:modelValue", value: boolean): void;
  (e: "saveDraft"): void;
  (e: "cancelSaveDraft"): void;
  (e: "closeDialog"): void;
}

defineProps<PropsType>();
const emits = defineEmits<EmitsType>();

const show = ref(true);
</script>
<style>
/* 隐藏dialog头部 */
.leaveConfirmDialog header.el-dialog__header {
  display: none;
}

/* 调整dialog暗黑样式 */
.dark .leaveConfirmDialog .el-dialog__body {
  background: #141414;
  border: 1px solid #363637;
  border-radius: 8px;
}
</style>
