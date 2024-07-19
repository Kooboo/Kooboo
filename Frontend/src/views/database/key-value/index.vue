<script lang="ts" setup>
import KTable from "@/components/k-table";
import EditValueDialog from "./edit-value-dialog.vue";
import { ref } from "vue";
import { getList, deletes } from "@/api/database/key-value";
import type { KeyValue } from "@/global/types";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { showDeleteConfirm } from "@/components/basic/confirm";

import { useI18n } from "vue-i18n";
import { useSiteStore } from "@/store/site";
const { t } = useI18n();
const list = ref<KeyValue[]>([]);
const siteStore = useSiteStore();
const showEditDialog = ref(false);
const key = ref("");

async function load() {
  const record = await getList();
  const items: KeyValue[] = [];
  for (const key in record) {
    items.push({ key, value: record[key] });
  }
  list.value = items.sort((a, b) => (a.key < b.key ? -1 : 1));
}

async function onDelete(rows: KeyValue[]) {
  await showDeleteConfirm(rows.length);
  const ids = rows.map((item) => item.key);
  await deletes({ ids });
  load();
}

const openEditValueDialog = (row: any) => {
  if (siteStore.hasAccess("keyValue", "edit")) {
    key.value = row.key;
    showEditDialog.value = true;
  }
};

load();
</script>

<template>
  <div class="p-24">
    <Breadcrumb :name="t('common.keyValueStore')" />
    <div class="flex items-center py-24 space-x-16">
      <el-button
        v-hasPermission="{
          feature: 'keyValue',
          action: 'edit',
        }"
        round
        data-cy="create-key-value"
        @click="
          key = '';
          showEditDialog = true;
        "
      >
        <el-icon class="iconfont icon-a-addto" />
        {{ t("common.new") }}
      </el-button>
    </div>
    <KTable
      :data="list"
      show-check
      :permission="{
        feature: 'keyValue',
        action: 'delete',
      }"
      @delete="onDelete"
    >
      <el-table-column :label="t('common.key')">
        <template #default="{ row }">
          <span data-cy="key">{{ row.key }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.value')">
        <template #default="{ row }">
          <span data-cy="value">{{ row.value }}</span>
        </template>
      </el-table-column>
      <el-table-column width="70" align="right">
        <template #default="{ row }">
          <IconButton
            :permission="{ feature: 'keyValue', action: 'edit' }"
            icon="icon-a-writein"
            :tip="t('common.edit')"
            data-cy="edit"
            @click="openEditValueDialog(row)"
          />
        </template>
      </el-table-column>
    </KTable>
    <EditValueDialog
      v-if="showEditDialog"
      v-model="showEditDialog"
      :current="key!"
      @success="load"
    />
  </div>
</template>
