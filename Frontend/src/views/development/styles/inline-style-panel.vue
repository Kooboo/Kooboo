<script lang="ts" setup>
import { deletes, getList } from "@/api/css-rule";
import KTable from "@/components/k-table";
import { useTime } from "@/hooks/use-date";
import type { ScriptItem } from "@/api/script/types";
import { ref } from "vue";
import type { InlineItem } from "@/api/css-rule/types";
import EditDeclarationDialog from "./edit-declaration-dialog.vue";

import { useI18n } from "vue-i18n";
import { showDeleteConfirm } from "@/components/basic/confirm";
const { t } = useI18n();
const list = ref<InlineItem[]>([]);
const selected = ref<InlineItem>();
const showEditDialog = ref(false);

const load = async () => {
  list.value = (await getList()).sort((a, b) =>
    a.lastModified > b.lastModified ? -1 : 1
  );
};

const onDelete = async (rows: ScriptItem[]) => {
  await showDeleteConfirm(rows.length);

  await deletes({
    ids: rows.map((m) => m.id),
  });
  load();
};
load();
</script>

<template>
  <div>
    <KTable
      :data="list"
      show-check
      :permission="{
        feature: 'style',
        action: 'delete',
      }"
      @delete="onDelete"
    >
      <el-table-column :label="t('common.style')">
        <template #default="{ row }">
          <span
            :title="row.name"
            class="text-blue cursor-pointer ellipsis"
            data-cy="name"
            @click="
              selected = row;
              showEditDialog = true;
            "
            >{{ row.name }}</span
          >
        </template>
      </el-table-column>
      <el-table-column :label="t('common.ownerType')" width="120px">
        <template #default="{ row }">
          <el-tag class="rounded-full" size="small" data-cy="owner-type">{{
            row.ownerType
          }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.siteObject')">
        <template #default="{ row }">
          <span class="ellipsis" data-cy="site-object">{{
            row.ownerName
          }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.lastModified')" width="180px">
        <template #default="{ row }">{{ useTime(row.lastModified) }}</template>
      </el-table-column>
    </KTable>
    <EditDeclarationDialog
      v-if="showEditDialog"
      :id="selected!.id"
      v-model="showEditDialog"
      @reload="load"
    />
  </div>
</template>
