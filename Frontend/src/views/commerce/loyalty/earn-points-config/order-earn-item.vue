<script lang="ts" setup>
import type { ConditionSchema } from "@/api/commerce/common";
import type { OrderEarnPointRule } from "@/api/commerce/loyalty";
import { useI18n } from "vue-i18n";
import Condition from "@/views/commerce/components/condition.vue";
import { ref } from "vue";

const props = defineProps<{
  item: OrderEarnPointRule;
  schemas: ConditionSchema[];
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
      <div class="flex flex-col gap-8 flex-1">
        <ElRadioGroup v-model="item.isPercent">
          <ElRadio :label="false">{{ t("commerce.getFixedPoints") }}</ElRadio>
          <ElRadio :label="true">{{
            t("commerce.getByOrderAmountPercents")
          }}</ElRadio>
        </ElRadioGroup>
        <ElInput v-model.number="item.value" class="w-280px">
          <template #suffix>
            <div
              class="mr-[10px] w-16 h-16 flex items-center justify-center transform origin-center scale-75"
            >
              {{ item.isPercent ? "%" : t("commerce.points") }}
            </div>
          </template>
        </ElInput>
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
        :title="t('commerce.paidOrder')"
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
      :title="t('commerce.paidOrder')"
      readonly
    />
    <div class="flex items-center space-x-4">
      <span class="dark:text-[#cfd3dc]">{{ t("commerce.paidOrder") }}</span>
      <span class="dark:text-[#cfd3dc]">{{ t("common.earn") }}</span>
      <span v-if="item.isPercent" class="text-blue"
        >{{ t("common.totalAmount") }} x {{ item.value }}%</span
      >
      <span v-else class="text-blue">{{ item.value }}</span>
      <span class="dark:text-[#cfd3dc]">{{ t("commerce.points") }}</span>
    </div>
  </div>
</template>
