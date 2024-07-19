<script lang="ts" setup>
import KTable from "@/components/k-table";
import { useScriptStore } from "@/store/script";
import { openInNewTab } from "@/utils/url";
import RelationsTag from "@/components/relations/relations-tag.vue";
import { useTime } from "@/hooks/use-date";
import type { ScriptItem } from "@/api/script/types";
import EditGroupDialog from "./edit-group-dialog.vue";
import { ref } from "vue";
import type { Group } from "@/api/resource-group/types";

import { useI18n } from "vue-i18n";
import { showDeleteConfirm } from "@/components/basic/confirm";
const { t } = useI18n();
const scriptStore = useScriptStore();
const showEditDialog = ref(false);
const selected = ref<Group>();

const onDelete = async (rows: ScriptItem[]) => {
  await showDeleteConfirm(rows.length);
  scriptStore.deleteGroups(rows.map((m) => m.id));
};

scriptStore.loadGroups();
scriptStore.loadExternal();
</script>

<template>
  <div>
    <div class="flex items-center mb-12 space-x-16">
      <el-button
        v-hasPermission="{ feature: 'script', action: 'edit' }"
        round
        data-cy="new-group"
        @click="
          showEditDialog = true;
          selected = undefined;
        "
      >
        <div class="flex items-center">
          <el-icon class="iconfont icon-a-addto" />
          {{ t("common.newGroup") }}
        </div>
      </el-button>
    </div>
    <KTable
      :data="scriptStore.group"
      show-check
      sort="lastModified"
      :permission="{ feature: 'script', action: 'delete' }"
      @delete="onDelete"
    >
      <el-table-column :label="t('common.name')">
        <template #default="{ row }">
          <span
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
      <el-table-column :label="t('common.children')">
        <template #default="{ row }">
          <el-tag size="small" class="rounded-full" data-cy="children-number">{{
            row.childrenCount
          }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.preview')">
        <template #default="{ row }">
          <span
            class="text-blue cursor-pointer ellipsis"
            :title="row.previewUrl"
            data-cy="preview"
            @click="openInNewTab(row.previewUrl)"
            >{{ row.relativeUrl }}</span
          >
        </template>
      </el-table-column>
      <el-table-column :label="t('common.usedBy')">
        <template #default="{ row }">
          <RelationsTag
            :id="row.id"
            :relations="row.references"
            type="ResourceGroup"
          />
        </template>
      </el-table-column>
      <el-table-column :label="t('common.lastModified')" prop="lastModified">
        <template #default="{ row }">{{ useTime(row.lastModified) }}</template>
      </el-table-column>
    </KTable>
    <EditGroupDialog
      v-if="showEditDialog"
      :id="selected?.id"
      v-model="showEditDialog"
    />
  </div>
</template>
