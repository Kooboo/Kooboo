<script lang="ts" setup>
import { useRoute } from "vue-router";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

const route = useRoute();
const { t } = useI18n();

const activeName = computed(
  () => route.meta.activeMenu || (route.name as string)
);

const menu = computed(() => [
  {
    label: t("common.server"),
    name: "partner",
    show: true,
    icon: "icon-Server",
  },
  {
    label: "DNS",
    name: "dns",
    show: true,
    icon: "icon-dns",
  },
  {
    label: t("common.subAccount"),
    name: "subAccount",
    show: true,
    icon: "icon-subaccount",
  },
]);
</script>

<template>
  <aside class="w-202px h-full relative bg-fff z-10 dark:bg-[#252526]">
    <el-scrollbar>
      <el-menu :default-active="activeName">
        <div v-for="item in menu.filter((f) => f.show)" :key="item.name">
          <router-link
            :to="{
              name: item.name,
            }"
          >
            <el-menu-item :index="item.name">
              <el-icon class="iconfont" :class="item.icon" />
              <span>{{ item.label }}</span>
            </el-menu-item>
          </router-link>
        </div>
      </el-menu>
    </el-scrollbar>
  </aside>
</template>
