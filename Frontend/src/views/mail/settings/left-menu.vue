<script lang="ts" setup>
import { useRouter, useRoute } from "vue-router";
import { computed } from "vue";
import { useI18n } from "vue-i18n";
import { useAppStore } from "@/store/app";

const appStore = useAppStore();
const router = useRouter();
const route = useRoute();
const { t } = useI18n();

const activeName = computed(() => {
  let name = route.name ?? "";
  return name.toString();
});

const menu = computed(() => {
  const list = [
    {
      label: t("mail.addresses"),
      name: "addresses",
      icon: "icon-a-Emailaddress",
    },
    {
      label: t("common.contacts"),
      name: "contacts",
      icon: "icon-a-Contactperson",
    },
    {
      label: t("common.securityReport"),
      name: "security-report",
      icon: "icon-dmarcreport",
    },

    {
      label: "IMAP",
      name: "imap-setting",
      icon: "icon-imap",
    },

    {
      label: t("common.mailMigration"),
      name: "mail-migration",
      icon: "icon-Datacenter",
    },
  ];

  // TODO isAdmin push it
  if (appStore.currentOrg?.isAdmin) {
    list.push({
      label: t("common.mailModule"),
      name: "mail-module",
      icon: "icon-a-Expansionmodule",
    });
    list.push({
      label: t("common.inboxLogo"),
      name: "inbox-logo",
      icon: "icon-trademark1",
    });
  }

  if (appStore.currentOrg?.isAdmin && !appStore.header?.isOnlineServer) {
    list.push({
      label: "SMTP",
      name: "smtp-setting",
      icon: "icon-imap",
    });
  }

  return list;
});

const changeMenu = async (index: any) => {
  await router.push({
    name: index.name,
    query: { ...router.currentRoute.value.query },
  });
};

const goBack = () => {
  if (route.query.oldFolder?.toString()) {
    router.push({
      name: route.query.oldFolder?.toString(),
      query: {
        ...router.currentRoute.value.query,
      },
    });
  } else {
    router.push({ name: "inbox" });
  }
};
</script>

<template>
  <aside class="w-202px h-full relative z-10 dark:bg-[#252526] pb-[56px]">
    <el-scrollbar>
      <el-menu
        :default-active="activeName"
        class="!dark:text-blue"
        :unique-opened="true"
      >
        <el-menu-item
          v-for="item in menu"
          :key="item.name"
          :index="item.name"
          @click="changeMenu({ name: item.name })"
        >
          <el-icon class="iconfont" :class="item.icon" />
          <span class="dark:text-999">{{ item.label }}</span>
        </el-menu-item>
      </el-menu>
    </el-scrollbar>
    <div
      class="absolute bottom-0 left-0 right-0 h-14 bg-fff flex items-center pl-5 space-x-8 hover:bg-blue/20 cursor-pointer dark:bg-[#252526] text-444 dark:text-999"
      @click="goBack"
    >
      <el-icon class="iconfont icon-fanhui1" />
      <span>{{ t("common.back") }}</span>
    </div>
  </aside>
</template>
