<script lang="ts" setup>
import KTable from "@/components/k-table";
import { useRouteSiteId } from "@/hooks/use-site-id";
import RelationsTag from "@/components/relations/relations-tag.vue";
import { useTime } from "@/hooks/use-date";
import type { ScriptItem } from "@/api/script/types";
import { useScriptStore } from "@/store/script";

import { useI18n } from "vue-i18n";
import { showDeleteConfirm } from "@/components/basic/confirm";
const { t } = useI18n();
const scriptStore = useScriptStore();

const onDelete = async (rows: ScriptItem[]) => {
  await showDeleteConfirm(rows.length);
  scriptStore.deleteScripts(rows.map((m) => m.id));
};

scriptStore.loadEmbedded();
</script>

<template>
  <div>
    <KTable
      :data="scriptStore.embedded"
      show-check
      sort="lastModified"
      :permission="{ feature: 'script', action: 'delete' }"
      @delete="onDelete"
    >
      <el-table-column :label="t('common.name')">
        <template #default="{ row }">
          <router-link
            :to="
              useRouteSiteId({
                name: 'script-edit',
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
          <RelationsTag
            :id="row.id"
            :relations="row.references"
            type="script"
          />
        </template>
      </el-table-column>
      <el-table-column :label="t('common.lastModified')" prop="lastModified">
        <template #default="{ row }">{{ useTime(row.lastModified) }}</template>
      </el-table-column>
    </KTable>
  </div>
</template>
