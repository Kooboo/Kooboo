<script lang="ts" setup>
import { useRoute } from "vue-router";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useI18n } from "vue-i18n";
import { deleteCustomers } from "@/api/commerce/customer";
import { onMounted, ref } from "vue";
import { showDeleteConfirm } from "@/components/basic/confirm";
import type {
  CustomerListItem,
  CustomerPagingResult,
} from "@/api/commerce/customer";
import type { PagingParams } from "@/api/commerce/common";
import { getCustomers } from "@/api/commerce/customer";
import CreateDialog from "./create-dialog.vue";
import EditDialog from "./edit-dialog.vue";
import AddressesDialog from "./addresses-dialog.vue";

const { t } = useI18n();
const route = useRoute();
const routeName = route.meta.title as string;
const showCreateDialog = ref(false);
const showEditDialog = ref(false);
const showAddressesDialog = ref(false);
const pagingResult = ref<CustomerPagingResult>();
const selected = ref<CustomerListItem>();

const queryParams = ref<PagingParams & { keyword: string }>({
  pageIndex: 1,
  pageSize: 30,
  keyword: "",
});

async function onDelete(rows: any[]) {
  await showDeleteConfirm(rows.length);
  await deleteCustomers(rows.map((m) => m.id));
  load(queryParams.value.pageIndex);
}

async function load(pageIndex = 1) {
  queryParams.value.pageIndex = pageIndex;
  pagingResult.value = await getCustomers(queryParams.value);
}

onMounted(async () => {
  load();
});
</script>

<template>
  <div class="p-24">
    <Breadcrumb :name="routeName" />
    <div class="flex items-center py-24 space-x-16">
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
        :placeholder="t('common.search')"
        class="w-248px"
        @keydown.enter="load()"
      />
    </div>
    <KTable
      v-if="pagingResult"
      :data="pagingResult.list"
      show-check
      :pagination="{
        currentPage: pagingResult.pageIndex,
        totalCount: pagingResult.count,
        pageSize: pagingResult.pageSize,
      }"
      @change="load(pagingResult.pageIndex)"
      @delete="onDelete"
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

      <el-table-column :label="t('common.addresses')" align="center">
        <template #default="{ row }">
          <div class="flex items-center justify-center">
            <ElTag
              round
              class="cursor-pointer"
              type="success"
              @click="
                selected = row;
                showAddressesDialog = true;
              "
              >{{ row.addresses }}</ElTag
            >
          </div>
        </template>
      </el-table-column>

      <el-table-column :label="t('common.tags')" align="center">
        <template #default="{ row }">
          <div class="inline-flex flex-wrap gap-4">
            <ElTag
              v-for="(item, index) of row.tags"
              :key="index"
              round
              type="warning"
              >{{ item }}</ElTag
            >
          </div>
        </template>
      </el-table-column>

      <el-table-column align="right" width="60">
        <template #default="{ row }">
          <el-tooltip placement="top" :content="t('common.edit')">
            <el-icon
              class="iconfont icon-a-writein hover:text-blue text-l"
              @click="
                selected = row;
                showEditDialog = true;
              "
            />
          </el-tooltip>
        </template>
      </el-table-column>
    </KTable>

    <CreateDialog
      v-if="showCreateDialog"
      v-model="showCreateDialog"
      @reload="load"
    />

    <EditDialog
      v-if="showEditDialog"
      :id="selected!.id"
      v-model="showEditDialog"
      @reload="load"
    />

    <AddressesDialog
      v-if="showAddressesDialog"
      :id="selected!.id"
      v-model="showAddressesDialog"
      @reload="load"
    />
  </div>
</template>
