<script lang="ts" setup>
import type { ConditionSchema } from "@/api/commerce/common";
import type { LoginEarnPointRule } from "@/api/commerce/loyalty";
import { durationUnits } from "@/utils/date";
import { useI18n } from "vue-i18n";
import Condition from "@/views/commerce/components/condition.vue";
import { ref } from "vue";

const props = defineProps<{
  item: LoginEarnPointRule;
  schemas: ConditionSchema[];
}>();
defineEmits<{
  (e: "remove"): void;
}>();
const { t } = useI18n();
const editing = ref(!!(props.item as any).editing);
</script>

<template>
  <div v-if="editing" class="space-x-8">
    <div class="flex flex-col gap-8">
      <div class="flex items-start">
        <div class="flex-1 flex items-center gap-8">
          <span class="dark:text-[#cfd3dc]">{{ t("common.every") }}</span>
          <el-select v-model="item.durationUnit" class="w-120px !m-0">
            <el-option
              v-for="i of durationUnits"
              :key="i.key"
              :label="i.value"
              :value="i.key"
            />
          </el-select>
          <span class="dark:text-[#cfd3dc]">{{ t("common.earn") }}</span>
          <ElInput v-model.number="item.value" class="w-280px">
            <template #suffix>
              <div
                class="mr-[10px] w-16 h-16 flex items-center justify-center transform origin-center scale-75"
              >
                {{ t("commerce.points") }}
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
          :title="t('common.loginUser')"
          class="flex-1"
        />
        <ElButton round type="primary" @click="editing = false">{{
          t("common.done")
        }}</ElButton>
      </div>
    </div>
  </div>
  <div v-else class="cursor-pointer" @click="editing = true">
    <Condition
      :condition="item.condition"
      :schemas="schemas"
      :title="t('common.loginUser')"
      readonly
    />
    <div class="flex items-center space-x-4">
      <span class="dark:text-[#cfd3dc]">{{ t("common.loginUser") }}</span>
      <span class="dark:text-[#cfd3dc]">{{ t("common.every") }}</span>
      <span class="text-blue">{{
        durationUnits.find((f) => f.key == item.durationUnit)?.value
      }}</span>
      <span class="dark:text-[#cfd3dc]">{{ t("common.earn") }}</span>
      <span class="text-blue">{{ item.value }}</span>
      <span class="dark:text-[#cfd3dc]">{{ t("commerce.points") }}</span>
    </div>
  </div>
</template>
