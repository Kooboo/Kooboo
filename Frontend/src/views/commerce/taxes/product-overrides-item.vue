<script lang="ts" setup>
import type { ConditionSchema } from "@/api/commerce/common";
import type { ProductOverrides } from "@/api/commerce/tax";
import { useI18n } from "vue-i18n";
import Condition from "@/views/commerce/components/condition.vue";
import { ref } from "vue";

const props = defineProps<{
  item: ProductOverrides;
  schemas: ConditionSchema[];
  baseTax: number;
}>();
defineEmits<{
  (e: "remove"): void;
}>();
const { t } = useI18n();
const editing = ref(!!(props.item as any).editing);
</script>

<template>
  <div v-if="editing" class="space-y-8">
    <div class="flex items-start">
      <div class="flex gap-12 flex-1">
        <div class="flex items-center gap-4">
          <ElInputNumber v-model="item.tax" :step="0.1" :min="0" />
          <span class="dark:text-[#cfd3dc]">%</span>
        </div>
        <ElSelect v-model="item.type" class="w-300px">
          <ElOption
            value="Added"
            :label="
              t('common.addedToBaseTaxAndRegionOverrides', {
                tax: baseTax,
              })
            "
          />
          <ElOption
            value="Instead"
            :label="
              t('common.insteadOfBaseTaxAndRegionOverrides', {
                tax: baseTax,
              })
            "
          />
          <ElOption
            value="Compounded"
            :label="
              t('common.compoundedOnTopOfBaseTaxAndRegionOverrides', {
                tax: baseTax,
              })
            "
          />
        </ElSelect>
      </div>
      <IconButton
        circle
        class="hover:text-orange text-orange"
        icon="icon-delete "
        :tip="t('common.delete')"
        @click="$emit('remove')"
      />
    </div>
    <div class="flex items-end">
      <Condition
        v-if="schemas.length"
        :condition="item.condition"
        :schemas="schemas"
        :title="t('commerce.product')"
        class="flex-1"
      />
      <ElButton round type="primary" @click="editing = false">{{
        t("common.done")
      }}</ElButton>
    </div>
  </div>
  <div v-else class="cursor-pointer" @click="editing = true">
    <Condition
      :condition="item.condition"
      :schemas="schemas"
      :title="t('commerce.product')"
      readonly
    />
    <div class="flex items-center space-x-4">
      <span>{{ t("common.using") }}</span>
      <span>{{ item.tax }}%</span>
      <template v-if="item.type == 'Added'">
        <span class="dark:text-[#cfd3dc]">{{
          t("common.addedToBaseTaxAndRegionOverrides", {
            tax: baseTax,
          })
        }}</span>
      </template>
      <template v-else-if="item.type == 'Instead'">
        <span class="dark:text-[#cfd3dc]">{{
          t("common.insteadOfBaseTaxAndRegionOverrides", {
            tax: baseTax,
          })
        }}</span>
      </template>
      <template v-else>
        <span class="dark:text-[#cfd3dc]">{{
          t("common.compoundedOnTopOfBaseTaxAndRegionOverrides", {
            tax: baseTax,
          })
        }}</span>
      </template>
    </div>
  </div>
</template>
