<template>
  <el-dialog
    v-model="show"
    width="650px"
    :title="t('common.organizationAdmin')"
    custom-class="el-dialog--zero-padding"
    @closed="emit('update:modelValue', false)"
  >
    <el-scrollbar max-height="240px" always>
      <ul class="px-16 mt-8 space-y-8">
        <li
          v-for="org in orgList"
          :key="org.id"
          class="pl-16 pr-8 cursor-pointer border-transparent hover:border-blue border-1 border-solid rounded-normal hover:bg-[#2296F3] hover:bg-opacity-10"
          :class="
            org.id === selectOrganizationId
              ? '  bg-[#2296F3] bg-opacity-10 !border-blue'
              : ''
          "
          @click="selectOrganizationId = org.id"
        >
          <div class="flex justify-between items-center py-24 pr-4">
            <div class="flex items-center justify-between w-[460px]">
              <div class="flex items-center">
                <span class="ellipsis max-w-200px leading-5" :title="org.name">
                  {{ org.name }}
                </span>
                <span
                  v-if="org.id === appStore.currentOrg!.id"
                  class="flex-1 text-12px flex items-center bg-blue/10 text-blue px-10px h-20px rounded-full mx-12"
                >
                  {{ t("common.currentOrganization") }}
                </span>
              </div>
              <span>{{ org.members }} {{ t("organization.members") }}</span>
            </div>
            <div>
              <el-icon
                v-if="org.id === selectOrganizationId"
                class="iconfont icon-Select2 text-green text-xl"
              />
            </div>
          </div>
        </li>
      </ul>
    </el-scrollbar>
    <template #footer>
      <el-button data-cy="cancel-in-dialog" @click="show = false">
        {{ t("common.cancel") }}
      </el-button>
      <el-button
        round
        :disabled="isOrganizationAdmin || !selectOrganizationId"
        @click="departOrg"
        >{{ t("common.depart") }}</el-button
      >
      <el-button v-if="showCreate" round @click="createOwnOrganization">{{
        t("common.createOwnOrg")
      }}</el-button>
      <el-button
        v-hasPermission="{ feature: 'style', action: 'edit' }"
        round
        type="primary"
        :disabled=" appStore.currentOrg!.id=== selectOrganizationId || !selectOrganizationId"
        @click="handleChangeUserOrg"
        >{{ t("domain.switch") }}</el-button
      >
    </template>
  </el-dialog>
</template>
<script lang="ts" setup>
import {
  changeUserOrg,
  getOrganizations,
  userDepartOrg,
  createOwnOrg,
} from "@/api/organization";
import { showConfirm } from "@/components/basic/confirm";
import { ref, computed } from "vue";
import { useI18n } from "vue-i18n";
import { useAppStore } from "@/store/app";
import { useRouter } from "vue-router";
import type { OrganizationList } from "@/api/organization/types";

const { t } = useI18n();
const appStore = useAppStore();
const router = useRouter();
const selectOrganizationId = ref();
const isOrganizationAdmin = computed(() => {
  const matchedOrg = orgList.value?.find(
    (org) => org.id === selectOrganizationId.value
  );
  return matchedOrg ? matchedOrg.isAdmin === true : false;
});

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

defineProps<{ modelValue: boolean }>();
const orgList = ref<OrganizationList[]>();
const show = ref(true);

const handleChangeUserOrg = async () => {
  await showConfirm(t("common.switchOrganizationTips"));
  await changeUserOrg(selectOrganizationId.value);
  appStore.logout();
  router.push({
    name: "login",
  });
};

const departOrg = async () => {
  await showConfirm(t("common.departOrganizationTips"));
  await userDepartOrg(selectOrganizationId.value);
  appStore.logout();
  router.push({
    name: "login",
  });
};

const showCreate = computed(() => {
  if (orgList.value?.some((org) => org.isAdmin === true)) {
    return false;
  } else {
    return true;
  }
});

const createOwnOrganization = async () => {
  await createOwnOrg();
  load();
};
const load = async () => {
  orgList.value = await getOrganizations();
  selectOrganizationId.value = appStore.currentOrg!.id;
};
load();
</script>
