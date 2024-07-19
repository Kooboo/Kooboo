<script lang="ts" setup>
import KTable from "@/components/k-table";
import { useRouteSiteId } from "@/hooks/use-site-id";
import RelationsTag from "@/components/relations/relations-tag.vue";
import { useTime } from "@/hooks/use-date";
import type { ScriptItem } from "@/api/script/types";
import { useStyleStore } from "@/store/style";

import { useI18n } from "vue-i18n";
import { showDeleteConfirm } from "@/components/basic/confirm";
const { t } = useI18n();
const styleStore = useStyleStore();

const onDelete = async (rows: ScriptItem[]) => {
  await showDeleteConfirm(rows.length);
  styleStore.deleteStyles(rows.map((m) => m.id));
};
styleStore.loadEmbedded();
</script>

<template>
  <div>
    <KTable
      :data="styleStore.embedded"
      show-check
      :permission="{
        feature: 'style',
        action: 'delete',
      }"
      @delete="onDelete"
    >
      <el-table-column :label="t('common.name')">
        <template #default="{ row }">
          <router-link
            :to="
              useRouteSiteId({
                name: 'style-edit',
                query: {
                  id: row.id,
                },
              })
            "
            data-cy="name"
          >
            <span :title="row.name" class="text-blue cursor-pointer ellipsis">{{
              row.name
            }}</span>
          </router-link>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.usedBy')">
        <template #default="{ row }">
          <RelationsTag :id="row.id" :relations="row.references" type="style" />
        </template>
      </el-table-column>
      <el-table-column :label="t('common.lastModified')">
        <template #default="{ row }">{{ useTime(row.lastModified) }}</template>
      </el-table-column>
    </KTable>
  </div>
</template>
