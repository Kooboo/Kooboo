<script lang="ts" setup>
import { useI18n } from "vue-i18n";
import type { ProductVariant, VariantOption } from "@/api/commerce/product";
import EditVariantDialog from "./edit-variant-dialog.vue";
import CreateVariantDialog from "./create-variant-dialog.vue";
import type { Ref } from "vue";
import { ref } from "vue";
import { buildOptionsDisplay } from "./product-variant";
import { useCommerceStore } from "@/store/commerce";

const { t } = useI18n();
const props = defineProps<{
  variants: ProductVariant[];
  options: string[];
  defaultImage: string;
  variantOptions: VariantOption[];
}>();
const showEditVariantDialog = ref(false);
const showCreateVariantDialog = ref(false);
const editingItem = ref<ProductVariant>() as Ref<ProductVariant>;
const commerceStore = useCommerceStore();

function onDelete(id: string) {
  const index = props.variants.findIndex((f) => f.id == id);
  props.variants.splice(index, 1);
}

function onEdit(item: ProductVariant) {
  editingItem.value = item;
  showEditVariantDialog.value = true;
}

function updateVariant(item: ProductVariant) {
  const old = props.variants.find((f) => f.id == item.id);
  if (old) {
    for (const option of old.selectedOptions) {
      const variantOption = props.variantOptions.find(
        (f) => f.name == option.name
      );
      if (variantOption) {
        const variantOptionItem = variantOption.items.find(
          (f) => f.name == option.value
        );
        if (variantOptionItem) {
          const variantOptionItemValue = item.selectedOptions.find(
            (f) => f.name == option.name
          )?.value;
          if (variantOptionItemValue) {
            variantOptionItem.name = variantOptionItemValue;
          }
        }
      }
    }
  }
  const index = props.variants.findIndex((f) => f.id == item.id);
  props.variants.splice(index, 1, item);
}
</script>

<template>
  <div class="space-y-12">
    <div v-if="options.length && variants[0]?.selectedOptions?.length">
      <el-button
        v-hasPermission="{ feature: 'productManagement', action: 'edit' }"
        round
        @click="showCreateVariantDialog = true"
      >
        <div class="flex items-center">
          <el-icon class="mr-16 iconfont icon-a-addto" />
          {{ t("commerce.addVariant") }}
        </div>
      </el-button>
    </div>
    <div>
      <ElTable :data="variants" class="el-table--gray">
        <el-table-column :label="t('common.cover')" width="80" align="left">
          <template #default="{ row }">
            <ImageCover v-model="row.image" />
          </template>
        </el-table-column>
        <el-table-column :label="t('common.options')">
          <template #default="{ row }">
            <div class="flex items-center">
              <span class="text-black dark:text-[#cfd3dc]">{{
                buildOptionsDisplay(row.selectedOptions, true)
              }}</span>
            </div>
          </template>
        </el-table-column>

        <el-table-column
          :label="`${t('common.price')} (${
            commerceStore.settings.currencySymbol
          })`"
          align="center"
          prop="inventory"
        >
          <template #default="{ row }">
            <ElInput v-model.number="row.price" class="w-120px text-center" />
          </template>
        </el-table-column>

        <el-table-column :label="t('common.inventory')" align="center">
          <template #default="{ row }">
            <ElInput
              v-model.number="row.newInventory"
              class="w-120px text-center"
            />
          </template>
        </el-table-column>

        <el-table-column :label="t('common.active')" width="100" align="center">
          <template #default="{ row }">
            <span :class="row.active ? 'text-green' : ''">
              <ElSwitch v-model="row.active" />
            </span>
          </template>
        </el-table-column>
        <el-table-column align="right" width="80">
          <template #default="{ row }">
            <div class="flex space-x-12 justify-end">
              <el-tooltip placement="top" :content="t('common.edit')">
                <el-icon
                  class="iconfont icon-a-writein hover:text-blue text-l"
                  @click="onEdit(row)"
                />
              </el-tooltip>
              <el-tooltip
                v-if="variants.length > 1"
                placement="top"
                :content="t('common.delete')"
              >
                <el-icon
                  class="iconfont icon-delete hover:text-orange text-l"
                  @click="onDelete(row.id)"
                />
              </el-tooltip>
            </div>
          </template>
        </el-table-column>
      </ElTable>
      <EditVariantDialog
        v-if="showEditVariantDialog"
        v-model="showEditVariantDialog"
        :model="editingItem"
        :variants="variants"
        @update:model="updateVariant"
      />
      <CreateVariantDialog
        v-if="showCreateVariantDialog"
        v-model="showCreateVariantDialog"
        :variants="variants"
        :default-image="defaultImage"
      />
    </div>
  </div>
</template>

<style scoped>
:deep(.text-center .el-input__inner) {
  text-align: center;
}
</style>
