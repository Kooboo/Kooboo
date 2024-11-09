<script lang="ts" setup>
import type { ConditionSchema } from "@/api/commerce/common";
import {
  getOrderRedeemSchemas,
  type RedeemPointSettings,
} from "@/api/commerce/loyalty";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import OrderRedeemItem from "./order-redeem-item.vue";

const props = defineProps<{ redeemPoint: RedeemPointSettings }>();
const schemas = ref<ConditionSchema[]>([]);

async function loadSchemas() {
  schemas.value = await getOrderRedeemSchemas();
}

loadSchemas();

const { t } = useI18n();

function addOrderRedeemRule() {
  props.redeemPoint.orderRedeemRules.push({
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
  <ElFormItem :label="t('common.pointRedeemRules')">
    <div class="space-y-8 w-full">
      <ElCard
        v-for="(item, index) of redeemPoint.orderRedeemRules"
        :key="index"
        class="relative mb-8 w-820px"
        :shadow="false"
      >
        <OrderRedeemItem
          :item="item"
          :schemas="schemas"
          @remove="
            redeemPoint.orderRedeemRules = redeemPoint.orderRedeemRules.filter(
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
        @click="addOrderRedeemRule"
      />
    </div>
  </ElFormItem>
</template>
