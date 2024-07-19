<script lang="ts" setup>
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import type { PurchaseDomain } from "@/api/console/types";
import { createDomainOrder } from "@/api/market-order";
import { useRouter } from "vue-router";

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

const props = defineProps<{ modelValue: boolean; domain: PurchaseDomain }>();
const { t } = useI18n();
const router = useRouter();
const show = ref(true);
const form = ref();
const model = ref({
  domainName: props.domain.domain,
  year: 1,
  client: {
    name: "",
    address: "",
    phone: "",
    email: "",
  },
  useKoobooClient: true,
});

const amount = computed(() => {
  return props.domain.price.product.price * model.value.year;
});

const yearList = ref<{ value: number; label: string }[]>([]);
for (var i = 1; i <= 10; i++) {
  yearList.value.push({
    value: i,
    label: i + " " + (i === 1 ? t("common.year") : t("common.years")),
  });
}

const onSave = async () => {
  const orderId = await createDomainOrder({
    domains: [
      {
        domainName: model.value.domainName,
        year: model.value.year,
      },
    ],
    Address: model.value.useKoobooClient ? undefined : model.value.client,
    totalAmount: amount.value,
  });
  router.push({
    name: "checkOrder",
    query: {
      orderId: orderId as string,
    },
  });
};
</script>

<template>
  <el-dialog
    :model-value="show"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.createOrder')"
    @closed="emit('update:modelValue', false)"
  >
    <el-form
      ref="form"
      label-position="top"
      :model="model"
      @keydown.enter="onSave"
    >
      <el-form-item :label="t('common.domain')">
        <el-input v-model="model.domainName" disabled />
      </el-form-item>
      <el-form-item
        :label="`${t('common.period')} (${t('common.year')})`"
        prop="year"
      >
        <el-select v-model="model.year" class="w-30" default-first-option>
          <el-option
            v-for="item in yearList"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
        </el-select>
      </el-form-item>
      <el-form-item>
        <div class="flex space-x-8 items-center">
          <div class="font-bold">{{ t("domain.amount") }}:</div>
          <div class="flex">
            <span>{{ amount.toFixed(2) }}</span>
            <span>{{ domain.price.product.currency }}</span>
          </div>
        </div>
      </el-form-item>
      <el-form-item
        :label="t('common.contactInformation')"
        prop="useKoobooClient"
      >
        <el-radio-group v-model="model.useKoobooClient" class="ml-4">
          <ElRadio :label="true">{{ t("common.auto") }}</ElRadio>
          <ElRadio :label="false">{{ t("common.custom") }}</ElRadio>
        </el-radio-group>
      </el-form-item>
      <div v-if="!model.useKoobooClient">
        <el-form-item :label="t('common.name')" required>
          <el-input v-model="model.client.name" />
        </el-form-item>
        <el-form-item :label="t('common.phone')" required>
          <el-input v-model="model.client.phone" />
        </el-form-item>
        <el-form-item :label="t('common.email')" required>
          <el-input v-model="model.client.email" />
        </el-form-item>
        <el-form-item :label="t('common.address')" required>
          <el-input v-model="model.client.address" type="textarea" />
        </el-form-item>
      </div>
    </el-form>
    <template #footer>
      <DialogFooterBar
        :confirm-label="t('common.ok')"
        @confirm="onSave"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
