<script lang="ts" setup>
import { ref } from "vue";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useI18n } from "vue-i18n";
import type { UserPublish } from "@/api/publish/types";
import { getServers, removeServer } from "@/api/publish";
import { getOrganizations } from "@/api/organization";
import type { OrganizationList } from "@/api/organization/types";
import AddServer from "./add-server.vue";
import { showDeleteConfirm } from "@/components/basic/confirm";
const { t } = useI18n();

const servers = ref<UserPublish[]>([]);
const orgList = ref<OrganizationList[]>();
const showAddDialog = ref(false);

const loadData = async () => {
  const result = await Promise.all([
    await getServers(),
    await getOrganizations(),
  ]);
  servers.value = result[0].map((m) => {
    if (m.reserved) (m as any).$DisabledSelect = true;
    return m;
  });
  orgList.value = result[1];
};

loadData();

async function onDelete(items: UserPublish[]) {
  await showDeleteConfirm();
  for (const item of items) {
    await removeServer(item.id);
  }
  loadData();
}
</script>

<template>
  <div class="p-24">
    <Breadcrumb
      :crumb-path="[
        { name: t('common.sync'), route: { name: 'sync' } },
        { name: t('common.server') },
      ]"
    />
    <div class="flex items-center py-24 space-x-16">
      <el-button
        v-hasPermission="{ feature: 'sync', action: 'edit' }"
        round
        @click="showAddDialog = true"
      >
        <div class="flex items-center">
          <el-icon class="mr-16 iconfont icon-a-addto" />
          {{ t("common.addServer") }}
        </div>
      </el-button>
    </div>
    <KTable
      :data="servers"
      show-check
      :permission="{ feature: 'sync', action: 'delete' }"
      @delete="onDelete"
    >
      <el-table-column :label="t('common.name')">
        <template #default="{ row }">
          <span data-cy="remote-site">{{ row.name }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.server')">
        <template #default="{ row }">
          <span data-cy="server">{{ row.serverUrl }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.organization')">
        <template #default="{ row }">
          {{ orgList?.find((f) => f.id == row.orgId)?.displayName ?? "-" }}
        </template>
      </el-table-column>
    </KTable>
    <AddServer
      v-if="showAddDialog && orgList"
      v-model="showAddDialog"
      :organizations="orgList"
      @reload="loadData"
    />
  </div>
</template>
