<template>
  <div
    v-if="appStore.header"
    class="profile flex items-center justify-center cursor-pointer px-16 !mx-0 h-full border-r-1 border-[#e1e2e8] dark:border-opacity-50"
    data-cy="username-dropdown"
    @click="triggerDropdown"
  >
    <KAvatar :username="logoText" :size="30" />
    <el-dropdown
      ref="dropdown"
      trigger="click"
      class="h-full"
      data-cy="username-abbr"
      @command="goto"
      @visible-change="handleVisible"
    >
      <span class="flex items-center h-full" data-cy="username">
        <el-icon class="iconfont icon-pull-down text-s ml-8" />
      </span>
      <div class="w-full bg-666" />
      <template #dropdown>
        <el-dropdown-menu>
          <el-dropdown-item
            disabled
            :title="`${t('common.username')}: ${appStore.header.user.name}`"
          >
            <div class="flex space-x-4 items-center">
              <el-icon class="iconfont icon-wode" />
              <span data-cy="username">
                {{ appStore.header.user.name }}
              </span>
            </div>
          </el-dropdown-item>
          <el-dropdown-item
            v-if="!appStore.limitSite"
            command="organization"
            :title="`${t('common.currentOrganization')}: ${
              appStore.currentOrg?.displayName
            }`"
          >
            <div class="flex space-x-4 items-center">
              <el-icon class="iconfont icon-organization" />
              <span data-cy="organization">
                {{ appStore.currentOrg?.displayName }}
              </span>
            </div>
          </el-dropdown-item>
          <div class="bg-[#e1e2e8] dark:bg-opacity-60 h-1px mx-16 my-8" />

          <el-dropdown-item
            v-if="!appStore.limitSite"
            command="profile"
            data-cy="setting"
          >
            <div class="flex space-x-4 items-center">
              <el-icon
                class="iconfont icon-shezhixitongshezhigongnengshezhishuxing-xian"
              />
              <span>
                {{ t("common.setting") }}
              </span>
            </div>
          </el-dropdown-item>
          <el-dropdown-item command="login" data-cy="logout">
            <div class="flex space-x-4 items-center">
              <el-icon class="iconfont icon-tuichu" />
              <span>
                {{ t("common.logout") }}
              </span>
            </div>
          </el-dropdown-item>
        </el-dropdown-menu>
      </template>
    </el-dropdown>
  </div>
</template>

<script lang="ts" setup>
import { computed, ref } from "vue";
import { useAppStore } from "@/store/app";
import { useRouter } from "vue-router";
import KAvatar from "@/components/basic/avatar.vue";

import { useI18n } from "vue-i18n";
import type { ElDropdown } from "element-plus";
const { t } = useI18n();
const appStore = useAppStore();
const router = useRouter();
const dropdown = ref<InstanceType<typeof ElDropdown>>();
const dropdownVisible = ref(false);

const logoText = computed(() => {
  return appStore.header!.user.name.substring(0, 2);
});

function goto(command: string) {
  if (command === "login") {
    appStore.logout();
  }

  router.push({
    name: command,
  });
}

function handleVisible(visible: boolean) {
  dropdownVisible.value = visible;
}
function triggerDropdown() {
  if (dropdownVisible.value) {
    dropdown.value?.handleClose();
  } else {
    dropdown.value?.handleOpen();
  }
}

appStore.load();
</script>
