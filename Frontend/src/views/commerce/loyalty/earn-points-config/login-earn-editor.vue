<script lang="ts" setup>
import type { ConditionSchema } from "@/api/commerce/common";
import {
  getLoginEarnSchemas,
  type EarnPointSettings,
} from "@/api/commerce/loyalty";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import LoginEarnItem from "./login-earn-item.vue";

const props = defineProps<{ earnPoint: EarnPointSettings }>();
const schemas = ref<ConditionSchema[]>([]);

async function loadSchemas() {
  schemas.value = await getLoginEarnSchemas();
}

loadSchemas();

const { t } = useI18n();

function addLoginEarnRule() {
  props.earnPoint.loginEarnRules.push({
    value: 10,
    durationUnit: "Day",
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
  <ElFormItem :label="t('common.earnPointOnLoginRules')">
    <div class="w-full">
      <ElCard
        v-for="(item, index) of earnPoint.loginEarnRules"
        :key="index"
        class="relative mb-8 w-820px"
        :shadow="false"
      >
        <LoginEarnItem
          :item="item"
          :schemas="schemas"
          @remove="
            earnPoint.loginEarnRules = earnPoint.loginEarnRules.filter(
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
        @click="addLoginEarnRule"
      />
    </div>
  </ElFormItem>
</template>
