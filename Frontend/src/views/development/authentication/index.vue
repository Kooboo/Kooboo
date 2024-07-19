<template>
  <div class="p-24">
    <Breadcrumb :name="t('common.authentication')" />

    <div class="flex items-center py-24 space-x-16">
      <el-button
        v-hasPermission="{
          feature: 'authentication',
          action: 'edit',
        }"
        round
        data-cy="create"
        @click="edit()"
      >
        <el-icon class="iconfont icon-a-addto" />
        {{ t("common.create") }}
      </el-button>
    </div>
    <KTable
      :data="list"
      show-check
      :permission="{
        feature: 'authentication',
        action: 'delete',
      }"
      @delete="onDelete"
    >
      <el-table-column :label="t('common.name')" width="150">
        <template #default="{ row }">
          <span class="ellipsis" :title="row.name" data-cy="name">{{
            row.name
          }}</span>
        </template></el-table-column
      >
      <el-table-column :label="t('common.matcher')">
        <template #default="{ row }">
          <span data-cy="matcher">{{ getConditionStr(row) }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.action')" width="150">
        <template #default="{ row }">
          <span data-cy="action">{{ row.action }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.failedAction')" width="150">
        <template #default="{ row }"
          ><span data-cy="failed-action">{{
            row.action === "CustomCode" ? "CustomCode" : row.failedAction
          }}</span></template
        >
      </el-table-column>
      <el-table-column :label="t('common.parameters')" width="180">
        <template #default="{ row }">
          <div class="ellipsis" data-cy="parameters">
            {{ getParameter(row) }}
          </div>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.lastModified')" width="180">
        <template #default="{ row }">{{ useTime(row.lastModified) }}</template>
      </el-table-column>
      <el-table-column width="70" align="right">
        <template #default="{ row }">
          <IconButton
            icon="icon-a-setup"
            :tip="t('common.setting')"
            data-cy="setting"
            @click="edit(row)"
          />
        </template>
      </el-table-column>
    </KTable>
    <EditDialog
      v-model="visibleEditDialog"
      :current="currentEdit"
      @save-success="load"
    />
  </div>
</template>

<script lang="ts" setup>
import KTable from "@/components/k-table";
import { onMounted, ref } from "vue";
import type { Authentication } from "@/api/development/authentication";
import { getList, deletes } from "@/api/development/authentication";
import { useTime } from "@/hooks/use-date";
import EditDialog from "./components/edit-dialog.vue";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { cloneDeep } from "lodash-es";
import { showDeleteConfirm } from "@/components/basic/confirm";

import { useI18n } from "vue-i18n";

const { t } = useI18n();
const list = ref<Authentication[]>([]);
const visibleEditDialog = ref(false);
const currentEdit = ref<Authentication>();

onMounted(() => {
  load();
});

async function load() {
  list.value = (await getList()).sort((a, b) =>
    a.lastModified! > b.lastModified! ? -1 : 1
  );
}
function getParameter(row: Authentication): string {
  if (row.action === "CustomCode") return row.customCode + "";
  if (row.failedAction === "ResultCode") return row.httpCode + "";
  if (row.failedAction === "Redirect") return row.url;
  return "";
}
function getConditionStr(row: Authentication): string {
  if (row.matcher !== "Condition") {
    return row.matcher;
  }
  return row.conditions
    .map((m) => `${m.left} ${m.operator} '${m.right}'`)
    .join(" && ");
}

async function onDelete(rows: Authentication[]) {
  await showDeleteConfirm(rows.length);
  const ids = rows.map((item) => item.id);
  await deletes({ ids });
  load();
}

async function edit(row?: Authentication) {
  currentEdit.value = row ? cloneDeep(row) : undefined;
  visibleEditDialog.value = true;
}
</script>
