<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { payOrder } from "@/api/commerce/order";

const { t } = useI18n();
const show = ref(true);

const props = defineProps<{
  modelValue: boolean;
  id: string;
}>();

const model = ref({
  orderId: props.id,
  method: "Cash",
  cardNumber: "",
  expirationDate: "",
  cvc: "",
  nameOnCard: "",
});

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

async function onSave() {
  await payOrder(model.value);
  emit("reload");
  show.value = false;
}
</script>

<template>
  <el-dialog
    :model-value="show"
    width="600px"
    :title="t('common.pay')"
    :close-on-click-modal="false"
    @closed="emit('update:modelValue', false)"
  >
    <div class="space-y-8">
      <el-radio-group v-model="model.method">
        <el-radio label="Cash">Cash</el-radio>
        <el-radio label="CreditCard" disabled>Credit card</el-radio>
      </el-radio-group>
      <ElForm v-if="model.method == 'CreditCard'" label-position="top">
        <ElFormItem :label="t('common.cardNumber')">
          <ElInput v-model="model.cardNumber" />
        </ElFormItem>
        <div class="grid grid-cols-2 gap-8">
          <ElFormItem :label="t('common.expirationDate')">
            <ElInput v-model="model.expirationDate" />
          </ElFormItem>
          <ElFormItem :label="t('common.cvc')">
            <ElInput v-model="model.cvc" />
          </ElFormItem>
        </div>
        <ElFormItem :label="t('common.nameOnCard')">
          <ElInput v-model="model.nameOnCard" />
        </ElFormItem>
      </ElForm>
    </div>

    <template #footer>
      <DialogFooterBar
        :confirm-label="t('common.pay')"
        @confirm="onSave"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
