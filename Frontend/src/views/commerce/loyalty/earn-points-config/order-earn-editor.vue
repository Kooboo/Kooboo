<script lang="ts" setup>
import type { ConditionSchema } from "@/api/commerce/common";
import {
  getOrderEarnSchemas,
  type EarnPointSettings,
} from "@/api/commerce/loyalty";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import OrderEarnItem from "./order-earn-item.vue";

const props = defineProps<{ earnPoint: EarnPointSettings }>();
const schemas = ref<ConditionSchema[]>([]);

async function loadSchemas() {
  schemas.value = await getOrderEarnSchemas();
}

loadSchemas();

const { t } = useI18n();

function addOrderEarnRule() {
  props.earnPoint.orderEarnRules.push({
    value: 10,
    isPercent: false,
    description: "",
    condition: {
      isAny: false,
      items: [],
    },
    editing: true,
  } as any);
}
</script>

<template>
  <ElFormItem :label="t('common.earnPointOnPlaceOrderRules')">
    <div class="space-y-8 w-full">
      <ElCard
        v-for="(item, index) of earnPoint.orderEarnRules"
        :key="index"
        class="relative mb-8 w-820px"
        :shadow="false"
      >
        <OrderEarnItem
          :item="item"
          :schemas="schemas"
          @remove="
            earnPoint.orderEarnRules = earnPoint.orderEarnRules.filter(
              (f) => f != item
            )
          "
        />
      </ElCard>
      <IconButton
        circle
        class="text-blue"
        icon="icon-a-addto"
        :tip="t('common.add')"
        @click="addOrderEarnRule"
      />
    </div>
  </ElFormItem>
</template>
