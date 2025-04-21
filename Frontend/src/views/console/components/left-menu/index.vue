<script lang="ts" setup>
import { useRoute } from "vue-router";
import { computed } from "vue";
import { useI18n } from "vue-i18n";
import { useAppStore } from "@/store/app";

const appStore = useAppStore();

const route = useRoute();
const { t } = useI18n();

// 激活当前文件夹的menu 未完成
// const folder = computed(() => route.query?.folder);

const activeName = computed(
  () => route.meta.activeMenu || (route.name as string)
);

const menu = computed(() => [
  {
    label: t("common.domain"),
    name: "kconsole",
    show: true,
    icon: "icon-Domain",
  },
  {
    label: t("common.organization"),
    name: "organization",
    show: true,
    icon: "icon-tissue",
  },
  {
    label: t("common.dataCenter"),
    name: "serviceDataCenter",
    show: appStore.currentOrg?.isAdmin,
    icon: "icon-Datacenter",
  },
  // {
  //   label: t("common.bandwidth"),
  //   name: "bandwidth",
  //   show: true,
  // },
  // {
  //   label: t("common.domainCDN"),
  //   name: "cdn",
  //   show: appStore.header?.isOnlineServer,
  //   icon: "icon-a-domaincdn",
  // },
  {
    label: "Google SC",
    name: "searchConsole",
    show: appStore.currentOrg?.isAdmin,
    icon: "icon-gaojisousuo1",
  },
  // {
  //   label: t("common.resourceCDN"),
  //   name: "resourceCDN",
  //   show: appStore.header?.isOnlineServer,
  //   icon: "icon-a-assetscdn",
  // },
  {
    label: t("console.member"),
    name: "member",
    show: appStore.currentOrg?.isAdmin,
    icon: "icon-member",
  },
  {
    label: t("common.orders"),
    name: "consoleOrder",
    show: appStore.currentOrg?.isAdmin,
    icon: "icon-Order",
  },
  {
    label: t("common.balance"),
    name: "domain-balance",
    show: appStore.currentOrg?.isAdmin,
    icon: "icon-balance",
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
              <el-icon class="iconfont" :class="item.icon" /><span>{{
                item.label
              }}</span>
            </el-menu-item>
          </router-link>
        </div>
      </el-menu>
    </el-scrollbar>
  </aside>
</template>
