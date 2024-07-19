<template>
  <div class="p-24">
    <div class="flex items-center pb-24 space-x-16">
      <el-button round @click="addSubAccount('new')">
        <div class="flex items-center">
          {{ t("common.addSubAccount") }}
        </div>
      </el-button>
      <el-button round @click="addSubAccount('existing')">
        <div class="flex items-center">
          {{ t("common.addExisting") }}
        </div>
      </el-button>
    </div>

    <KTable :data="userList">
      <el-table-column :label="t('common.username')">
        <template #default="{ row }">
          <span>
            {{ row.userName }}
          </span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.remark')">
        <template #default="{ row }">
          <span>
            {{ row.remark }}
          </span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.email')">
        <template #default="{ row }">
          <span>
            {{ row.email }}
          </span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.phone')">
        <template #default="{ row }">
          <span>
            {{ row.phone }}
          </span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.joinDate')">
        <template #default="{ row }">
          <span>
            {{ useTime(row.joinDate) }}
          </span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.action')" align="center" width="260">
        <template #default="{ row }">
          <div class="flex items-center space-x-8 justify-center">
            <span
              class="text-blue cursor-pointer"
              @click="changePassword(row.userName)"
            >
              {{ t("partner.resetPassword") }}
            </span>
            <span
              class="text-blue cursor-pointer"
              @click="impersonate(row.userName)"
            >
              {{ t("common.impersonate") }}
            </span>
          </div>
        </template>
      </el-table-column>
    </KTable>
  </div>

  <AddSubAccountDialog
    v-if="addSubAccountDialog"
    v-model="addSubAccountDialog"
    :add-type="addType"
    @add-success="load"
  />
  <ChangePasswordDialog
    v-if="changePasswordDialog"
    v-model="changePasswordDialog"
    :username="currentUserName"
  />
</template>

<script lang="ts" setup>
import { ref } from "vue";
import KTable from "@/components/k-table";
import { useI18n } from "vue-i18n";
import { impersonateApi, getUsers } from "@/api/partner";
import { useTime } from "@/hooks/use-date";
import AddSubAccountDialog from "./add-subaccout-dialog.vue";
import ChangePasswordDialog from "./change-password-dialog.vue";
import type { partnerUser } from "@/api/partner/type";
import { openInNewTab } from "@/utils/url";

const { t } = useI18n();

const userList = ref<partnerUser[]>([]);
const currentUserName = ref();
const subAccountUrl = ref();
const load = async () => {
  userList.value = await getUsers();
};
const addType = ref();
const changePasswordDialog = ref(false);
const addSubAccountDialog = ref(false);
const addSubAccount = (type: string) => {
  addType.value = type;
  addSubAccountDialog.value = true;
};
const changePassword = (username: string) => {
  currentUserName.value = username;
  changePasswordDialog.value = true;
};

const impersonate = async (username: string) => {
  subAccountUrl.value = await impersonateApi(username);
  openInNewTab(subAccountUrl.value);
};
load();
</script>
