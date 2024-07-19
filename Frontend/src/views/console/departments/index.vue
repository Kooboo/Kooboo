<script setup lang="ts">
import type { Department } from "@/api/organization/department";
import {
  getDepartments,
  deleteDepartments,
} from "@/api/organization/department";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { useAppStore } from "@/store/app";
import { showConfirm } from "@/components/basic/confirm";
import AddDepartmentDialog from "./add-department-dialog.vue";
import { useRouteSiteId } from "@/hooks/use-site-id";
import UsersDialog from "./users-dialog.vue";

const { t } = useI18n();

const appStore = useAppStore();
const showAddDepartmentDialog = ref(false);
const showUsersDialog = ref(false);
const departments = ref<Department[]>();
const selected = ref<Department>();

const load = async () => {
  departments.value = await getDepartments();
};

const onDeleteUsers = async (row: Department[]) => {
  await showConfirm(t("common.removeDepartmentTips"));
  await deleteDepartments(row.map((m) => m.id));
  load();
};

load();
</script>

<template>
  <div class="p-24 space-y-16">
    <div>
      <el-button
        v-if="appStore.currentOrg?.isAdmin"
        round
        @click="showAddDepartmentDialog = true"
      >
        <el-icon class="iconfont icon-a-addto" />
        <div class="flex items-center">
          {{ t("common.addDepartment") }}
        </div>
      </el-button>
    </div>
    <KTable
      :data="departments"
      :show-check="appStore.currentOrg?.isAdmin"
      @delete="onDeleteUsers"
    >
      <el-table-column :label="t('common.name')">
        <template #default="{ row }">
          <div class="flex items-center">
            <span>{{ row.name }}</span>
          </div>
        </template>
      </el-table-column>

      <el-table-column :label="t('common.members')">
        <template #default="{ row }">
          <div class="flex items-center">
            <ElTag
              round
              type="success"
              class="cursor-pointer"
              @click="
                selected = row;
                showUsersDialog = true;
              "
              >{{ row.userCount }}</ElTag
            >
          </div>
        </template>
      </el-table-column>
    </KTable>
    <AddDepartmentDialog
      v-if="showAddDepartmentDialog"
      v-model="showAddDepartmentDialog"
      @reload="load"
    />
    <UsersDialog
      v-if="showUsersDialog"
      :id="selected!.id"
      v-model="showUsersDialog"
      @reload="load"
    />
  </div>
</template>
