<script lang="ts" setup>
import type { PagingParams } from "@/api/commerce/common";
import type {
  CustomerListItem,
  CustomerPagingResult,
} from "@/api/commerce/customer";
import { getCustomers } from "@/api/commerce/customer";
import { ref, onMounted } from "vue";
import { useI18n } from "vue-i18n";
import CreateDialog from "../customers/create-dialog.vue";

const { t } = useI18n();
const show = ref(true);
const showCreateDialog = ref(false);

defineProps<{
  modelValue: boolean;
}>();

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "selected", value: CustomerListItem): void;
}>();

const pagingResult = ref<CustomerPagingResult>();
const queryParams = ref<PagingParams & { keyword: string }>({
  pageIndex: 1,
  pageSize: 30,
  keyword: "",
});

async function load(index: number) {
  queryParams.value.pageIndex = index;
  pagingResult.value = await getCustomers(queryParams.value);
}

onMounted(() => {
  load(1);
});

function onRowClick(row: CustomerListItem) {
  emit("selected", row);
  show.value = false;
}
</script>

<template>
  <el-dialog
    :model-value="show"
    width="800px"
    :title="t('commerce.selectCustomer')"
    :close-on-click-modal="false"
    @closed="emit('update:modelValue', false)"
  >
    <div class="flex items-center pb-12 space-x-16">
      <el-button
        v-hasPermission="{ feature: 'carts', action: 'edit' }"
        round
        @click="showCreateDialog = true"
      >
        <div class="flex items-center">
          <el-icon class="mr-16 iconfont icon-a-addto" />
          {{ t("common.create") }}
        </div>
      </el-button>
      <div class="flex-1" />
      <SearchInput
        v-model="queryParams.keyword"
        class="w-248px"
        @keydown.enter.prevent="load(1)"
      />
    </div>
    <el-scrollbar max-height="400px">
      <ElTable
        v-if="pagingResult"
        :data="pagingResult.list"
        class="el-table--gray mb-24"
        @row-click="onRowClick"
      >
        <el-table-column :label="t('common.email')">
          <template #default="{ row }">
            {{ row.email }}
          </template>
        </el-table-column>
        <el-table-column :label="t('common.phone')">
          <template #default="{ row }">
            {{ row.phone }}
          </template>
        </el-table-column>

        <el-table-column :label="t('common.firstName')">
          <template #default="{ row }">
            {{ row.firstName }}
          </template>
        </el-table-column>

        <el-table-column :label="t('common.lastName')">
          <template #default="{ row }">
            {{ row.lastName }}
          </template>
        </el-table-column>
      </ElTable>
    </el-scrollbar>
    <div class="text-center">
      <el-pagination
        class="py-8"
        layout="prev, pager, next"
        hide-on-single-page
        :page-size="pagingResult?.pageSize"
        :current-page="pagingResult?.pageIndex"
        :total="pagingResult?.count"
        @current-change="load"
      />
    </div>
  </el-dialog>
  <CreateDialog
    v-if="showCreateDialog"
    v-model="showCreateDialog"
    @reload="load(1)"
  />
</template>
