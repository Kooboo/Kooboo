<template>
  <el-dialog
    v-model="show"
    width="400px"
    :close-on-click-modal="false"
    :title="t('common.recharge')"
    @closed="emits('update:modelValue', false)"
  >
    <el-form ref="form" label-position="top" :rules="rules" :model="model">
      <el-form-item prop="amount" :label="t('domain.amount')">
        <div class="flex items-center">
          <el-input
            v-model.number="model.amount"
            class="w-180px"
            @input="
              model.amount = model.amount
                .replace(/\s/g, '')
                .replace(/[^\d]/g, '')
            "
          />
          <span class="w-50px ml-12">{{ model.currency }}</span>
        </div>
      </el-form-item>
    </el-form>
    <template #footer>
      <DialogFooterBar
        :confirm-label="t('common.recharge')"
        @confirm="topUp"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
<script setup lang="ts">
import { ref } from "vue";
import { useRouter } from "vue-router";
import { useI18n } from "vue-i18n";
import { newTopup } from "@/api/console";
import { rechargeRule, requiredRule } from "@/utils/validate";
import type { Rules } from "async-validator";

const { t } = useI18n();
const router = useRouter();
interface PropsType {
  modelValue: boolean;
  balance: any;
}
interface EmitsType {
  (e: "update:modelValue", value: boolean): void;
}

const rules = {
  amount: [rechargeRule(), requiredRule(t("common.valueRequiredTips"))],
} as Rules;

const props = defineProps<PropsType>();
const emits = defineEmits<EmitsType>();

const show = ref(true);
const form = ref();

const model = ref();
const load = () => {
  model.value = JSON.parse(JSON.stringify(props.balance));
  model.value.amount = 50;
};
const topUp = async () => {
  await form.value.validate();
  const orderId = await newTopup(model.value.amount);
  show.value = false;
  router.push({
    name: "checkOrder",
    query: {
      orderId: orderId as string,
    },
  });
};

load();
</script>
