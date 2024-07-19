<template>
  <el-dialog
    v-model="show"
    width="800px"
    :title="t('common.departmentMembers')"
    @closed="emit('update:modelValue', false)"
  >
    <div class="space-y-16">
      <div>
        <el-button
          v-if="appStore.currentOrg?.isAdmin"
          round
          @click="showAddUsersDialog = true"
        >
          <el-icon class="iconfont icon-a-addto" />
          <div class="flex items-center">
            {{ t("common.addUser") }}
          </div>
        </el-button>
      </div>
      <KTable
        :data="users"
        :show-check="appStore.currentOrg?.isAdmin"
        @delete="onDeleteUsers"
      >
        <el-table-column :label="t('common.userName')">
          <template #default="{ row }">
            <div class="flex items-center">
              <span>{{ row.userName }}</span>
            </div>
          </template>
        </el-table-column>

        <el-table-column :label="t('common.email')">
          <template #default="{ row }">
            <div class="flex items-center">
              <span>{{ row.emailAddress }}</span>
            </div>
          </template>
        </el-table-column>

        <el-table-column :label="t('common.function')">
          <template #default="{ row }">
            <div class="flex items-center">
              <span>{{ row.function }}</span>
            </div>
          </template>
        </el-table-column>

        <el-table-column
          :label="t('common.isManager')"
          width="200"
          align="center"
        >
          <template #default="{ row }">
            <span :class="row.isManager ? 'text-green' : ''">
              {{ row.isManager ? t("common.yes") : t("common.no") }}
            </span>
          </template>
        </el-table-column>
      </KTable>
      <AddUsersDialog
        v-if="showAddUsersDialog"
        :id="id!"
        v-model="showAddUsersDialog"
        :excludes="users.map((m) => m.id)"
        @reload="load"
      />
    </div>
  </el-dialog>
</template>
<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { useAppStore } from "@/store/app";
import { getDepartmentUsers, deleteUsers } from "@/api/organization/department";
import { showConfirm } from "@/components/basic/confirm";
import AddUsersDialog from "./add-users-dialog.vue";

const { t } = useI18n();

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

const props = defineProps<{
  modelValue: boolean;
  id: string;
}>();
const appStore = useAppStore();
const showAddUsersDialog = ref(false);
const users = ref<any[]>([]);

const load = async () => {
  users.value = await getDepartmentUsers(props.id);
  emit("reload");
};

const onDeleteUsers = async (row: any[]) => {
  await showConfirm(t("common.removeMemberTips"));
  await deleteUsers(
    props.id!,
    row.map((m) => m.userName)
  );
  load();
};

load();

const show = ref(true);
</script>
