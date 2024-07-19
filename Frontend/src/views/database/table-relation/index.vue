<template>
  <div class="p-24">
    <Breadcrumb :name="t('common.tableRelation')" />
    <div class="flex items-center py-24 space-x-16">
      <el-button
        v-hasPermission="{
          feature: 'tableRelation',
          action: 'edit',
        }"
        round
        data-cy="create-table-relation"
        @click="visibleRelationDialog = true"
      >
        <el-icon class="iconfont icon-a-addto" />
        {{ t("common.new") }}
      </el-button>
    </div>
    <KTable
      :data="list"
      show-check
      :permission="{
        feature: 'tableRelation',
        action: 'delete',
      }"
      @delete="onDelete"
    >
      <el-table-column :label="t('common.name')" prop="name">
        <template #default="{ row }">
          <span class="ellipsis" :title="row.name" data-cy="name">{{
            row.name
          }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.tableA')" prop="tableA">
        <template #default="{ row }">
          <span class="ellipsis" :title="row.tableA" data-cy="table-A">{{
            row.tableA
          }}</span>
        </template></el-table-column
      >
      <el-table-column :label="t('common.fieldA')" prop="fieldA">
        <template #default="{ row }">
          <span class="ellipsis" :title="row.fieldA" data-cy="field-A">{{
            row.fieldA
          }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.relation')">
        <template #default="{ row }">
          <span class="text-green" data-cy="relation">{{
            row.relationName
          }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.tableB')" prop="tableB">
        <template #default="{ row }">
          <span class="ellipsis" :title="row.tableB" data-cy="table-B">{{
            row.tableB
          }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.fieldB')" prop="fieldB">
        <template #default="{ row }">
          <span class="ellipsis" :title="row.fieldB" data-cy="field-B">{{
            row.fieldB
          }}</span>
        </template></el-table-column
      >
    </KTable>
    <CreateRelationDialog
      v-model="visibleRelationDialog"
      @create-success="load"
    />
  </div>
</template>

<script lang="ts" setup>
import KTable from "@/components/k-table";
import CreateRelationDialog from "./components/create-relation-dialog.vue";
import { onMounted, ref } from "vue";
import type { TableRelation } from "@/api/database/table-relation";
import { getList, deleteRelation } from "@/api/database/table-relation";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { showDeleteConfirm } from "@/components/basic/confirm";
import { useI18n } from "vue-i18n";
const { t } = useI18n();
const list = ref<TableRelation[]>();
const visibleRelationDialog = ref(false);

onMounted(() => {
  load();
});

async function load() {
  list.value = await getList();
}
async function onDelete(rows: TableRelation[]) {
  await showDeleteConfirm(rows.length);
  const ids = rows.map((item) => item.id);
  await deleteRelation({ ids });
  load();
}
</script>
