<script lang="ts" setup>
import { useRoute } from "vue-router";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useI18n } from "vue-i18n";
import { deleteProductTypes } from "@/api/commerce/type";
import type { CategoryListItem } from "@/api/commerce/category";
import { onMounted, ref } from "vue";
import { showDeleteConfirm } from "@/components/basic/confirm";
import { useCommerceStore } from "@/store/commerce";
import CreateDialog from "./create-dialog.vue";
import EditDialog from "./edit-dialog.vue";
import type { ProductType } from "@/api/commerce/type";
import BooleanTag from "@/components/k-tag/boolean-tag.vue";

const { t } = useI18n();
const route = useRoute();
const routeName = route.meta.title as string;
const commerceStore = useCommerceStore();
const showCreateDialog = ref(false);
const showEditDialog = ref(false);
const editingItem = ref();

async function onDelete(rows: CategoryListItem[]) {
  await showDeleteConfirm(rows.length);
  await deleteProductTypes(rows.map((m) => m.id));
  commerceStore.loadTypes();
}

onMounted(async () => {
  commerceStore.loadTypes();
});

function onEdit(item: ProductType) {
  editingItem.value = item;
  showEditDialog.value = true;
}
</script>

<template>
  <div class="p-24">
    <Breadcrumb :name="routeName" />
    <div class="flex items-center py-24">
      <el-button
        v-hasPermission="{ feature: 'productTypes', action: 'edit' }"
        round
        @click="showCreateDialog = true"
      >
        <div class="flex items-center">
          <el-icon class="mr-16 iconfont icon-a-addto" />
          {{ t("common.create") }}
        </div>
      </el-button>
    </div>
    <KTable :data="commerceStore.types" show-check @delete="onDelete">
      <el-table-column :label="t('common.name')">
        <template #default="{ row }">
          <div class="flex items-center">
            <span class="text-black dark:text-[#cfd3dc]">{{ row.name }}</span>
          </div>
        </template>
      </el-table-column>

      <el-table-column :label="t('common.attributes')" align="center">
        <template #default="{ row }">
          <div
            v-if="row.attributes.length"
            class="flex items-center space-x-4 justify-center"
          >
            <ElTag
              v-for="item in row.attributes"
              :key="item.name"
              round
              type="warning"
              >{{ item.name }}</ElTag
            >
          </div>
        </template>
      </el-table-column>

      <el-table-column :label="t('commerce.variantOptions')" align="center">
        <template #default="{ row }">
          <div
            v-if="row.options.length"
            class="flex items-center space-x-4 justify-center"
          >
            <ElTag
              v-for="item in row.options"
              :key="item.name"
              round
              type="success"
              >{{ item.name }}</ElTag
            >
          </div>
        </template>
      </el-table-column>

      <el-table-column :label="t('common.isDigitalProduct')">
        <template #default="{ row }">
          <div class="flex items-center">
            <BooleanTag :value="row.isDigital" />
          </div>
        </template>
      </el-table-column>

      <el-table-column align="right" width="60">
        <template #default="{ row }">
          <el-tooltip placement="top" :content="t('common.edit')">
            <el-icon
              class="iconfont icon-a-writein hover:text-blue text-l"
              @click="onEdit(row)"
            />
          </el-tooltip>
        </template>
      </el-table-column>
    </KTable>

    <CreateDialog
      v-if="showCreateDialog"
      v-model="showCreateDialog"
      @reload="commerceStore.loadTypes()"
    />

    <EditDialog
      v-if="showEditDialog"
      v-model="showEditDialog"
      :model="editingItem"
      @reload="commerceStore.loadTypes()"
    />
  </div>
</template>
