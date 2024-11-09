<script lang="ts" setup>
import type { ConditionSchema } from "@/api/commerce/common";
import type { AdditionalCost, Shipping } from "@/api/commerce/shipping";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { getSchemas } from "@/api/commerce/shipping";
import Condition from "@/views/commerce/components/condition.vue";
import { useCommerceStore } from "@/store/commerce";
import SupportCountries from "./support-countries.vue";

const props = defineProps<{ model: Shipping }>();
const schemas = ref<ConditionSchema[]>([]);
const { t } = useI18n();
const commerceStore = useCommerceStore();
const showAdvanceSettings = ref(false);

async function loadSchemas() {
  schemas.value = await getSchemas();
}

function addAdditionalCost() {
  props.model.additionalCosts.push({
    description: "",
    cost: 0,
    condition: {
      isAny: false,
      items: [],
    },
  });
}

function removeAdditionalCost(item: AdditionalCost) {
  props.model.additionalCosts = props.model.additionalCosts.filter(
    (f) => f != item
  );
}

loadSchemas();
</script>

<template>
  <div class="px-24 pt-0 pb-84px space-y-12">
    <div class="bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal">
      <ElForm label-position="top">
        <ElFormItem :label="t('common.name')">
          <ElInput v-model="model.name" />
        </ElFormItem>
        <ElFormItem :label="t('common.description')">
          <ElInput v-model="model.description" type="textarea" />
        </ElFormItem>
        <ElFormItem
          :label="`${t('common.baseCost')} (${
            commerceStore.settings.currencyCode
          })`"
        >
          <ElInputNumber v-model="model.baseCost" :min="1">
            <template #suffix>
              <div
                class="w-16 h-16 flex items-center justify-center transform origin-center scale-75"
              >
                {{ commerceStore.settings.currencyCode }}
              </div>
            </template>
          </ElInputNumber>
        </ElFormItem>
        <el-tooltip
          placement="top"
          :content="
            !showAdvanceSettings
              ? t('common.showAdvanceSettings')
              : t('common.hideAdvanceSettings')
          "
        >
          <div
            class="flex items-center justify-center h-40px cursor-pointer bg-card dark:bg-444 hover:bg-[#eff6ff] dark:hover:bg-444"
            data-cy="show-advance-settings"
            @click="showAdvanceSettings = !showAdvanceSettings"
          >
            <el-icon
              class="iconfont icon-pull-down text-s leading-none cursor-pointer transform origin-center transition duration-200 dark:text-fff/86"
              :class="showAdvanceSettings ? 'rotate-180' : 'rotate-0'"
            />
          </div>
        </el-tooltip>
        <template v-if="showAdvanceSettings">
          <div class="grid grid-cols-2 gap-8 mt-8">
            <ElFormItem>
              <template #label>
                <span>{{ t("common.companyCode") }}</span>
                <Tooltip
                  :tip="t('common.expressCompanyCodeTip')"
                  custom-class="ml-4"
                />
              </template>
              <ElInput
                v-model.number="model.code"
                class="max-w-280px"
                placeholder="e.g., jtexpress"
              />
            </ElFormItem>
            <ElFormItem>
              <template #label>
                <span>{{ t("common.trackingNumberMatching") }}</span>
                <Tooltip
                  :tip="t('common.trackingNumberMatchingTip')"
                  custom-class="ml-4"
                />
              </template>
              <ElInput
                v-model="model.trackingNumberMatching"
                class="max-w-280px"
                placeholder="e.g., sf.*"
              />
            </ElFormItem>
          </div>
          <ElFormItem :label="t('common.supportCountries')">
            <SupportCountries v-model="model.countries" />
          </ElFormItem>
        </template>
      </ElForm>
    </div>

    <div class="bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal">
      <ElForm label-position="top">
        <div
          v-for="(item, index) of model.additionalCosts"
          :key="index"
          class="bg-gray/30 dark:bg-[#252526] px-24 py-16 rounded-normal relative mb-8"
        >
          <ElForm label-position="top">
            <div class="flex gap-x-8">
              <ElFormItem :label="t('common.costRule')">
                <ElInput v-model.number="item.cost" class="w-280px">
                  <template #suffix>
                    <div
                      class="w-16 h-16 flex items-center justify-center transform origin-center scale-75"
                    >
                      {{ commerceStore.settings.currencyCode }}
                    </div>
                  </template>
                </ElInput>
              </ElFormItem>
              <ElFormItem :label="t('common.description')" class="flex-1">
                <ElInput v-model="item.description" />
              </ElFormItem>
            </div>
          </ElForm>
          <Condition
            v-if="schemas.length"
            :condition="item.condition"
            :schemas="schemas"
            :title="t('commerce.order')"
          />
          <el-icon
            class="text-orange iconfont icon-delete absolute top-12 right-12 cursor-pointer"
            @click="removeAdditionalCost(item)"
          />
        </div>

        <el-button round @click="addAdditionalCost">
          <div class="flex items-center">
            <el-icon class="mr-16 iconfont icon-a-addto" />
            {{ t("common.addCostRule") }}
          </div>
        </el-button>
      </ElForm>
    </div>
  </div>
</template>
