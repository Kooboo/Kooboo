<script lang="ts" setup>
import type { ProductListItem } from "@/api/commerce/product";
import { getProducts, editProducts } from "@/api/commerce/category";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import SelectProductDialog from "../components/select-product-dialog.vue";
import { useProductFields } from "../useFields";
import DynamicColumns from "@/components/dynamic-columns/index.vue";
const { t } = useI18n();
const show = ref(true);
const products = ref<ProductListItem[]>();
const showSelectProductDialog = ref(false);

const { getColumns } = useProductFields();
const columns = getColumns([
  {
    name: "featuredImage",
    attrs: {
      width: 80,
      align: "center",
    },
  },
  {
    name: "title",
  },
  {
    name: "variantsCount",
    attrs: {
      align: "center",
    },
  },
  {
    name: "inventory",
    attrs: {
      align: "center",
    },
  },
  {
    name: "active",
    attrs: {
      width: 100,
      align: "center",
    },
  },
]);
const props = defineProps<{
  editable?: boolean;
  categoryId: string;
  modelValue: boolean;
}>();

getProducts(props.categoryId).then((rsp) => {
  products.value = rsp;
});

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

function addProduct(selected: ProductListItem[]) {
  products.value?.push(...selected);
}

async function onSave() {
  await editProducts(
    props.categoryId,
    products.value!.map((m) => m.id)
  );
  show.value = false;
  emit("reload");
}
</script>

<template>
  <el-dialog
    :model-value="show"
    width="850px"
    :title="t('common.products')"
    :close-on-click-modal="false"
    @closed="emit('update:modelValue', false)"
  >
    <div class="space-y-12">
      <el-button
        v-if="editable"
        v-hasPermission="{ feature: 'productCategories', action: 'edit' }"
        round
        @click="showSelectProductDialog = true"
      >
        <div class="flex items-center">
          <el-icon class="mr-16 iconfont icon-a-addto" />
          {{ t("common.addProduct") }}
        </div>
      </el-button>
      <el-scrollbar max-height="400px">
        <ElTable :data="products" class="el-table--gray">
          <DynamicColumns :columns="columns" />
          <el-table-column v-if="editable" align="right" width="60">
            <template #default="{ row }">
              <el-tooltip placement="top" :content="t('common.delete')">
                <el-icon
                  class="iconfont icon-delete hover:text-blue text-l"
                  @click="products = products!.filter((f) => f.id != row.id)"
                />
              </el-tooltip>
            </template>
          </el-table-column>
        </ElTable>
      </el-scrollbar>
    </div>

    <template #footer>
      <DialogFooterBar @confirm="onSave" @cancel="show = false" />
    </template>
  </el-dialog>

  <SelectProductDialog
    v-if="showSelectProductDialog"
    v-model="showSelectProductDialog"
    :excludes="products?.map((m) => m.id)"
    @selected="addProduct"
  />
</template>
