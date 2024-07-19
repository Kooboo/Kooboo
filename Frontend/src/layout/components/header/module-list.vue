<template>
  <div
    v-show="display"
    v-if="appStore.header"
    class="flex items-center justify-center h-full cursor-pointer border-l border-r border-solid border-line dark:border-opacity-50"
  >
    <el-dropdown trigger="click" class="h-full">
      <div
        class="text-black dark:text-fff/60 flex items-center px-16 h-full hover:bg-[#EFF6FF] dark:hover:bg-444"
      >
        <div class="mr-8" data-cy="selected-header-menu">{{ display }}</div>
        <el-icon class="iconfont icon-pull-down text-s leading-none" />
      </div>
      <template #dropdown>
        <el-dropdown-menu>
          <div v-for="item of appStore.header.menu" :key="item.url">
            <router-link
              :to="{ path: routes.find( (f: RouteRecordRaw) =>
            f.meta?.oldPath === item.url )?.path || '', }"
            >
              <el-dropdown-item
                :command="item.url"
                :class="{ selected: route.meta.oldPath === item.url }"
                data-cy="header-menu"
                >{{ item.displayName }}</el-dropdown-item
              ></router-link
            >
          </div>
        </el-dropdown-menu>
      </template>
    </el-dropdown>
  </div>
</template>
<script lang="ts" setup>
import { computed } from "vue";
import type { RouteRecordRaw } from "vue-router";
import { useRoute } from "vue-router";
import { useAppStore } from "@/store/app";
import routes from "@/router/routes";
const appStore = useAppStore();
const route = useRoute();

const display = computed(() => {
  var module = appStore.header!.menu.find((f) => f.url === route.meta.oldPath);
  return module?.displayName || "";
});
</script>
