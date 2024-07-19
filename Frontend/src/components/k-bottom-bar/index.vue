<template>
  <div
    v-bind="$attrs"
    class="k-bottom-bar bg-gray/10 py-15px pr-62px text-right space-x-32 fixed bottom-0 w-full left-0 z-3 backdrop-filter backdrop-blur-md shadow-m-20"
  >
    <el-button
      v-if="!hiddenCancel"
      :disabled="cancelDisabled"
      round
      data-cy="cancel"
      @click="emits('cancel')"
      >{{ back ? t("common.back") : t("common.cancel") }}</el-button
    >
    <el-button
      v-if="!hiddenConfirm"
      v-hasPermission="{
        feature: permission?.feature,
        action: permission?.action,
      }"
      round
      type="primary"
      :disabled="disabled"
      data-cy="save"
      @click="emits('save')"
    >
      {{ confirmLabel ? confirmLabel : t("common.save") }}
    </el-button>
    <slot name="extra-buttons" />
  </div>
</template>
<script lang="ts" setup>
import { useI18n } from "vue-i18n";
interface PropsType {
  disabled?: boolean;
  cancelDisabled?: boolean;
  back?: boolean;
  hiddenCancel?: boolean;
  hiddenConfirm?: boolean;
  confirmLabel?: string;
  permission?: {
    feature: string;
    action: string;
  };
}
interface EmitsType {
  (e: "cancel"): void;
  (e: "save"): void;
}
const emits = defineEmits<EmitsType>();
defineProps<PropsType>();
const { t } = useI18n();
</script>
<style lang="scss">
.k-bottom-bar {
  .el-button {
    min-width: 120px;
    height: 34px;
    min-height: auto;
  }
}
</style>
