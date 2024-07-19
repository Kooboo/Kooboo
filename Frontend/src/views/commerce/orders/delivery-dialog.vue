<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { deliveryOrder } from "@/api/commerce/order";

const { t } = useI18n();
const show = ref(true);

const props = defineProps<{
  modelValue: boolean;
  id: string;
}>();

const model = ref({
  id: props.id,
  shippingCarrier: "",
  trackingNumber: "",
});

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

async function onSave() {
  await deliveryOrder(model.value);
  emit("reload");
  show.value = false;
}
</script>

<template>
  <el-dialog
    :model-value="show"
    width="600px"
    :title="t('commerce.delivery')"
    :close-on-click-modal="false"
    @closed="emit('update:modelValue', false)"
  >
    <ElForm v-if="model" label-position="top">
      <ElFormItem :label="t('commerce.shippingCarrier')">
        <ElInput v-model="model.shippingCarrier" />
      </ElFormItem>

      <ElFormItem :label="t('commerce.trackingNumber')">
        <ElInput v-model="model.trackingNumber" />
      </ElFormItem>
    </ElForm>
    <template #footer>
      <DialogFooterBar @confirm="onSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
