<script lang="ts" setup>
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { buildOptionsDisplay } from "../products-management/product-variant";
import type { ProductVariantItem } from "@/api/commerce/product";
import {
  getProductVariantItems,
  type ProductVariant,
} from "@/api/commerce/product";
import SelectVariantDialog from "../components/select-variant-dialog.vue";

const props = defineProps<{
  modelValue: string[];
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: string[]): void;
}>();

const { t } = useI18n();
const showSelectVariantDialog = ref(false);
const list = ref<ProductVariantItem[]>([]);

function onDelete(id: string) {
  emit(
    "update:model-value",
    props.modelValue.filter((f) => f !== id)
  );
}

function addVariant(row: ProductVariant) {
  emit("update:model-value", props.modelValue.concat([row.id]));
}

watch(
  [() => props.modelValue],
  async () => {
    if (!props.modelValue.length) return;
    list.value = await getProductVariantItems(props.modelValue);
  },
  {
    deep: true,
    immediate: true,
  }
);
</script>

<template>
  <div class="space-y-12 w-full">
    <el-button
      v-hasPermission="{ feature: 'discounts', action: 'edit' }"
      round
      @click="showSelectVariantDialog = true"
    >
      <div class="flex items-center">
        <el-icon class="mr-16 iconfont icon-a-addto" />
        {{ t("common.addProduct") }}
      </div>
    </el-button>
    <ElTable :data="list" class="el-table--gray">
      <el-table-column :label="t('common.cover')" width="80" align="left">
        <template #default="{ row }">
          <div class="flex items-center justify-center">
            <ImageCover v-model="row.image" />
          </div>
        </template>
      </el-table-column>

      <el-table-column :label="t('common.title')">
        <template #default="{ row }">
          <div>
            <div>{{ row.title }}</div>
            <div class="text-s">{{ buildOptionsDisplay(row.options) }}</div>
          </div>
        </template>
      </el-table-column>

      <el-table-column
        :label="t('common.price')"
        align="center"
        prop="inventory"
      >
        <template #default="{ row }">
          {{ row.price }}
        </template>
      </el-table-column>

      <el-table-column align="right" width="80">
        <template #default="{ row }">
          <div class="flex space-x-12 justify-end">
            <el-tooltip placement="top" :content="t('common.delete')">
              <el-icon
                class="iconfont icon-delete hover:text-orange text-l"
                @click="onDelete(row.variantId)"
              />
            </el-tooltip>
          </div>
        </template>
      </el-table-column>
    </ElTable>
  </div>
  <SelectVariantDialog
    v-if="showSelectVariantDialog"
    v-model="showSelectVariantDialog"
    @selected="addVariant"
  />
</template>
