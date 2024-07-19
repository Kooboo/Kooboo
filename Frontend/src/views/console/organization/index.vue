<script setup lang="ts">
import { deleteUser, getUsers } from "@/api/organization";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { useDate } from "@/hooks/use-date";
import { showConfirm } from "@/components/basic/confirm";
import AddMemberDialog from "./add-member-dialog.vue";
import ChangeOrganizationDialog from "./change-organization-dialog.vue";
import type { UsersList } from "@/api/organization/types";
import { useAppStore } from "@/store/app";
import CreateMemberDialog from "./create-member-dialog.vue";
import { useRouteSiteId } from "@/hooks/use-site-id";

const { t } = useI18n();

const appStore = useAppStore();
const userList = ref([] as (UsersList & { $DisabledSelect?: boolean })[]);
const addUserDialog = ref(false);
const changeOrganizationDialog = ref(false);
const createMemberDialogVisible = ref(false);

const load = async () => {
  userList.value = (await getUsers(appStore.currentOrg?.id)) as UsersList[];
  userList.value.forEach((item) => {
    item.$DisabledSelect = item.isAdmin;
  });
};
const addMember = () => {
  addUserDialog.value = true;
};
const createMember = () => {
  createMemberDialogVisible.value = true;
};

const changeOrganization = () => {
  changeOrganizationDialog.value = true;
};

const handleDeleteUser = async (row: UsersList[]) => {
  await showConfirm(t("common.removeMemberTips"));
  const deleteUserData = {
    organizationId: appStore.currentOrg!.id,
    userName: row[0].userName,
  };
  await deleteUser(deleteUserData);
  load();
};

load();
</script>

<template>
  <div class="p-24 space-y-16">
    <div class="flex gap-8 items-center dark:text-gray">
      <ul class="flex space-x-24">
        <li>
          <span>{{ t("common.currentOrganization") }}:</span
          ><span class="text-blue ml-8">{{ appStore.currentOrg?.name }}</span>
        </li>
        <li>
          {{ t("common.members") }}:<span class="text-blue ml-8">{{
            userList?.length
          }}</span>
        </li>
        <li>
          {{ t("common.identity") }}:<span class="text-blue ml-8">{{
            appStore.currentOrg?.isAdmin
              ? t("common.admin")
              : t("common.member")
          }}</span>
        </li>
      </ul>
      <div class="flex-1" />
      <div>
        <router-link
          :to="
            useRouteSiteId({
              name: 'departments',
            })
          "
          class="mx-8"
        >
          <el-button round>
            <el-icon class="iconfont icon-module1 text-blue" />
            <div class="flex items-center">
              {{ t("common.departments") }}
            </div>
          </el-button>
        </router-link>
      </div>
      <div>
        <el-button round @click="changeOrganization">
          <el-icon class="iconfont icon-qiehuanjiaose text-blue" />
          <div class="flex items-center">
            {{ t("common.organizationAdmin") }}
          </div>
        </el-button>
      </div>
    </div>
    <KTable
      :data="userList"
      :show-check="appStore.currentOrg?.isAdmin"
      :is-radio="true"
      @delete="handleDeleteUser"
    >
      <template v-if="appStore.currentOrg?.isAdmin" #leftBar>
        <div class="h-60px flex items-center justify-between px-16">
          <el-button
            v-if="appStore.currentOrg?.isAdmin"
            round
            @click="addMember"
          >
            <el-icon class="iconfont icon-a-addto" />
            <div class="flex items-center">
              {{ t("common.addMember") }}
            </div>
          </el-button>

          <el-button
            v-if="appStore.currentOrg?.isAdmin"
            round
            @click="createMember"
          >
            <el-icon class="iconfont icon-a-addto" />
            <div class="flex items-center">
              {{ t("common.createMember") }}
            </div>
          </el-button>
        </div>
      </template>
      <el-table-column :label="t('common.userName')">
        <template #default="{ row }">
          <div class="flex items-center">
            <span>{{ row.userName }}</span>
            <span
              v-if="row.isAdmin"
              class="text-12px flex items-center bg-blue/10 text-blue px-10px h-20px rounded-full ml-16"
            >
              {{ t("common.admin") }}
            </span>
          </div>
        </template>
      </el-table-column>
      <el-table-column prop="emailAddress" :label="t('common.email')" />
      <el-table-column :label="t('common.joinDate')">
        <template #default="{ row }">
          <span>{{ useDate(row.joinDate) }}</span>
        </template>
      </el-table-column>
    </KTable>
  </div>

  <AddMemberDialog
    v-if="addUserDialog"
    v-model="addUserDialog"
    @add-success="load"
  />
  <CreateMemberDialog
    v-if="createMemberDialogVisible"
    v-model="createMemberDialogVisible"
    @create-success="load()"
  />
  <ChangeOrganizationDialog
    v-if="changeOrganizationDialog"
    v-model="changeOrganizationDialog"
  />
</template>
