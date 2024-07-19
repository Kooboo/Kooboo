<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { cancelOrder } from "@/api/commerce/order";

const { t } = useI18n();
const show = ref(true);

const props = defineProps<{
  modelValue: boolean;
  id: string;
}>();

const model = ref({
  id: props.id,
  reason: "",
});

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

async function onSave() {
  await cancelOrder(model.value);
  emit("reload");
  show.value = false;
}
</script>

<template>
  <el-dialog
    :model-value="show"
    width="600px"
    :title="t('commerce.cancelOrder')"
    :close-on-click-modal="false"
    @closed="emit('update:modelValue', false)"
  >
    <ElForm v-if="model" label-position="top">
      <ElFormItem :label="t('common.cancelReason')">
        <ElInput v-model="model.reason" type="textarea" />
      </ElFormItem>
    </ElForm>
    <template #footer>
      <DialogFooterBar @confirm="onSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
