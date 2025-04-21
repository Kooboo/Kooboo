<template>
  <el-dialog
    :model-value="show"
    width="340px"
    :close-on-click-modal="false"
    custom-class="el-dialog--zero-padding"
    :title="t('common.transferDomain')"
    @closed="emit('update:modelValue', false)"
  >
    <el-result
      :icon="code ? 'success' : 'info'"
      :title="code"
      :sub-title="code ? t('common.transferDomainTip') : t('common.loading')"
    />
  </el-dialog>
</template>
<script setup lang="ts">
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { getTransferCode } from "@/api/console";

interface PropsType {
  domain: string;
  modelValue: boolean;
}

const props = defineProps<PropsType>();
const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "change-success"): void;
}>();

const { t } = useI18n();
const show = ref(true);
const code = ref();
getTransferCode(props.domain).then((rsp) => (code.value = rsp));
</script>
