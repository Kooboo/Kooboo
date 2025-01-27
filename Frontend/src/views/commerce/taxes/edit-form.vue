<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import type { Tax } from "@/api/commerce/tax";
import { getRegions } from "@/api/commerce/address";
import RegionOverrides from "./region-overrides.vue";
import ProductOverrides from "./product-overrides.vue";

const props = defineProps<{ model: Tax }>();
const { t } = useI18n();
const regions = ref<any[]>([]);

getRegions(props.model.country).then((rsp) => (regions.value = rsp));
</script>

<template>
  <div class="px-24 pt-0 pb-84px space-y-12">
    <div class="bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal">
      <ElForm label-position="top">
        <ElFormItem :label="t('common.baseTax')">
          <div class="flex items-center gap-4">
            <ElInputNumber v-model="model.baseTax" :step="0.1" :min="0" />
            <span class="dark:text-[#cfd3dc]">%</span>
          </div>
        </ElFormItem>
        <ElFormItem v-if="regions.length" :label="t('common.regionOverrides')">
          <RegionOverrides
            :regions="regions"
            :list="model.regionOverrides"
            :base-tax="model.baseTax"
          />
        </ElFormItem>
        <ProductOverrides
          :list="model.productOverrides"
          :base-tax="model.baseTax"
        />
      </ElForm>
    </div>
  </div>
</template>
