<template>
  <el-button data-cy="cancel-in-dialog" @click="cancelAction">
    {{ t("common.cancel") }}
  </el-button>
  <el-button
    v-if="permission && !hiddenConfirm"
    v-hasPermission="permission"
    type="primary"
    :disabled="disabled"
    data-cy="submit-in-dialog"
    @click="confirmAction"
  >
    {{ confirmLabel ? confirmLabel : t("common.save") }}
  </el-button>
  <el-button
    v-else-if="!hiddenConfirm"
    type="primary"
    :disabled="disabled"
    data-cy="submit-in-dialog"
    @click="confirmAction"
  >
    {{ confirmLabel ? confirmLabel : t("common.save") }}
  </el-button>

  <slot name="extra-buttons" />
</template>
<script setup lang="ts">
import { useI18n } from "vue-i18n";

const { t } = useI18n();
const emit = defineEmits<{ (e: "confirm"): void; (e: "cancel"): void }>();
interface PropsType {
  confirmLabel?: string;
  hiddenConfirm?: boolean | number;
  disabled?: boolean;
  permission?: {
    feature: string;
    action: string;
  };
}
defineProps<PropsType>();

const confirmAction = () => {
  emit("confirm");
};
const cancelAction = () => {
  emit("cancel");
};
</script>
