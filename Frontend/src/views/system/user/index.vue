<script lang="ts" setup>
import ActionBar from "./action-bar.vue";
import KTable from "@/components/k-table";
import type { User } from "@/api/site/user";
import { getCurrentUsers, deletes } from "@/api/site/user";
import { ref } from "vue";

import Breadcrumb from "@/components/basic/breadcrumb.vue";

import { useI18n } from "vue-i18n";
import { showDeleteConfirm } from "@/components/basic/confirm";
const { t } = useI18n();
const data = ref<User[]>();

const load = async () => {
  data.value = await getCurrentUsers();
};

const onDelete = async (rows: User[]) => {
  await showDeleteConfirm(rows.length);
  await deletes(rows.map((m) => (m as User).userId));
  load();
};

load();
</script>

<template>
  <div class="p-24">
    <Breadcrumb :name="t('common.siteUser')" />
    <ActionBar @reload="load" />
    <KTable
      :data="data ?? []"
      show-check
      :permission="{ feature: 'siteUser', action: 'delete' }"
      @delete="onDelete"
    >
      <el-table-column :label="t('common.username')" prop="userName" />
      <el-table-column :label="t('common.email')" prop="email" />
      <el-table-column :label="t('common.role')">
        <template #default="{ row }">
          <el-tag
            size="small"
            class="rounded-full cursor-pointer"
            data-cy="role"
            >{{ row.role }}</el-tag
          >
        </template>
      </el-table-column>
    </KTable>
  </div>
</template>
