<script lang="ts" setup>
import type { ConditionSchema } from "@/api/commerce/common";
import { getTaxSchemas, type ProductOverrides } from "@/api/commerce/tax";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import ProductOverridesItem from "./product-overrides-item.vue";

const props = defineProps<{ list: ProductOverrides[]; baseTax: number }>();
const schemas = ref<ConditionSchema[]>([]);

async function loadSchemas() {
  schemas.value = await getTaxSchemas();
}

loadSchemas();

const { t } = useI18n();

function add() {
  props.list.push({
    tax: 0,
    type: "Added",
    condition: {
      isAny: false,
      items: [],
    },
    editing: true,
  } as any);
}
function remove(index: number) {
  props.list.splice(index, 1);
}
</script>

<template>
  <ElFormItem :label="t('common.productOverrides')">
    <div class="space-y-8 w-full">
      <ElCard
        v-for="(item, index) of list"
        :key="index"
        class="relative mb-8 w-820px"
        :shadow="false"
      >
        <ProductOverridesItem
          :base-tax="baseTax"
          :item="item"
          :schemas="schemas"
          @remove="remove(index)"
        />
      </ElCard>
      <IconButton
        circle
        class="text-blue"
        icon="icon-a-addto"
        :tip="t('common.add')"
        @click="add"
      />
    </div>
  </ElFormItem>
</template>
