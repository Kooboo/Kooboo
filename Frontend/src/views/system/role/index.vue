<script lang="ts" setup>
import KTable from "@/components/k-table";
import type { Role } from "@/api/site/role";
import { getList, deletes } from "@/api/site/role";
import { ref } from "vue";
import EditRoleDialog from "./edit-role-dialog.vue";
import Breadcrumb from "@/components/basic/breadcrumb.vue";

import { useI18n } from "vue-i18n";
import { showDeleteConfirm } from "@/components/basic/confirm";
const { t } = useI18n();
const data = ref<(Role & { $DisabledSelect?: boolean })[]>();
const name = ref<string>("");
const showEditDialog = ref(false);

const load = async () => {
  data.value = await getList();
  data.value.forEach((item) => {
    if (["master", "developer", "contentmanager"].includes(item.name)) {
      item.$DisabledSelect = true;
    }
  });
};

const onDelete = async (rows: Role[]) => {
  await showDeleteConfirm(rows.length);
  await deletes(rows.map((m) => (m as Role).id));
  load();
};

const onAdd = () => {
  name.value = "";
  showEditDialog.value = true;
};

const onEdit = (role: Role) => {
  name.value = role.name;
  showEditDialog.value = true;
};

load();
</script>

<template>
  <div class="p-24">
    <Breadcrumb :name="t('common.roles')" />
    <div class="flex items-center py-24 space-x-16">
      <el-button
        v-hasPermission="{ feature: 'role', action: 'edit' }"
        round
        data-cy="add-role"
        @click="onAdd"
      >
        <div class="flex items-center">
          <el-icon class="mr-16 iconfont icon-a-addto" />
          {{ t("common.addRole") }}
        </div>
      </el-button>
    </div>
    <KTable
      v-if="data"
      :data="data"
      sort="creationDate"
      show-check
      :permission="{ feature: 'role', action: 'delete' }"
      @delete="onDelete"
    >
      <el-table-column :label="t('common.roleName')">
        <template #default="{ row }">
          <el-tag size="small" class="rounded-full" data-cy="name">{{
            row.name
          }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column align="right" width="80px" prop="creationDate">
        <template #default="{ row }">
          <IconButton
            icon="icon-a-writein"
            :tip="t('common.editRole')"
            data-cy="edit"
            @click="onEdit(row)"
          />
        </template>
      </el-table-column>
    </KTable>
    <EditRoleDialog
      v-if="showEditDialog"
      v-model="showEditDialog"
      :name="name"
      @reload="load"
    />
  </div>
</template>
