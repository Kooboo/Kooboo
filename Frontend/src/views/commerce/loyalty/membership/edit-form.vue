<script lang="ts" setup>
import { useI18n } from "vue-i18n";
import { type Membership, getMembershipSchemas } from "@/api/commerce/loyalty";
import TimeDuration from "@/views/commerce/components/time-duration.vue";
import { useCommerceStore } from "@/store/commerce";
import Condition from "@/views/commerce/components/condition.vue";
import type { ConditionSchema } from "@/api/commerce/common";
import { ref } from "vue";
import { requiredRule } from "@/utils/validate";

defineProps<{ model: Membership }>();
const { t } = useI18n();
const commerceStore = useCommerceStore();
const schemas = ref<ConditionSchema[]>([]);
const form = ref();

async function loadSchemas() {
  schemas.value = await getMembershipSchemas();
}

loadSchemas();

defineExpose({
  validate: async () => await form.value.validate(),
});
</script>

<template>
  <ElForm ref="form" label-position="top" :model="model">
    <ElFormItem
      :label="t('common.name')"
      prop="name"
      :rules="[requiredRule(t('common.inputNameTips'))]"
    >
      <ElInput v-model="model.name" />
    </ElFormItem>

    <ElFormItem :label="t('common.description')">
      <ElInput v-model="model.description" type="textarea" />
    </ElFormItem>
    <div class="grid grid-cols-2 gap-8">
      <ElFormItem>
        <template #label>
          <div class="inline-flex items-center space-x-4">
            <div>{{ t("common.level") }}</div>
            <Tooltip
              :tip="t('common.membershipLevelTip')"
              custom-class="ml-4"
            />
          </div>
        </template>
        <ElInputNumber v-model="model.priority" :min="0" />
      </ElFormItem>
      <ElFormItem :label="t('common.duration')">
        <TimeDuration
          v-model.number="model.duration"
          v-model:unit="model.durationUnit"
        />
      </ElFormItem>
    </div>
    <ElFormItem :label="t('common.upgradeMethods')">
      <div class="space-y-12 w-full">
        <div class="flex flex-col gap-4">
          <ElCheckbox
            v-model="model.allowPurchase"
            :label="t('common.subscription')"
          />
          <div v-if="model.allowPurchase" class="flex gap-4 items-center">
            <ElInputNumber v-model="model.price" />
            <span>{{ commerceStore.settings.currencyCode }}</span>
          </div>
        </div>

        <div class="space-y-4">
          <div class="flex items-center">
            <ElCheckbox
              v-model="model.allowAutoUpgrade"
              :label="t('common.upgradeAccordingToTheRules')"
            />
          </div>
          <Condition
            v-if="schemas.length && model.allowAutoUpgrade"
            class="bg-gray/30 dark:bg-[#252526] px-24 py-16 rounded-normal relative mb-8 w-full"
            :condition="model.condition"
            :schemas="schemas"
            :title="t('common.customer')"
          />
        </div>
      </div>
    </ElFormItem>
  </ElForm>
</template>
