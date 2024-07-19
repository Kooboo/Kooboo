<script lang="ts" setup>
import RouterDialog from "./router-dialog.vue";
import { computed, ref } from "vue";
import type { Page } from "@/api/pages/types";

import ImportDialog from "./import-dialog.vue";
import type { KeyValue } from "@/global/types";
import { emptyGuid } from "@/utils/guid";
import { useRouter } from "vue-router";
import { useRouteSiteId } from "@/hooks/use-site-id";
import IconButton from "@/components/basic/icon-button.vue";
import { useLayoutStore } from "@/store/layout";

import { useI18n } from "vue-i18n";
import { useSiteStore } from "@/store/site";
import { usePageStore } from "@/store/page";

defineProps<{ pages?: Page[] }>();
const { t } = useI18n();
const router = useRouter();
const layoutStore = useLayoutStore();
const showRouterDialog = ref(false);
const showImportDialog = ref(false);
const siteStore = useSiteStore();
const pageStore = usePageStore();

const layoutList = computed(() => {
  let list: KeyValue[] = [];
  if (layoutStore.list?.length) {
    list = layoutStore.list.map((m) => ({ key: m.id, value: m.name }));
  } else {
    list.push({
      key: emptyGuid,
      value: t("common.clickToCreateFirstLayout"),
    });
  }
  return list;
});

const onCreateLayoutPage = (command: string) => {
  if (command === emptyGuid) {
    router.push(
      useRouteSiteId({
        name: "layout-edit",
      })
    );
  } else {
    router.push(
      useRouteSiteId({
        name: "layout-page-setting",
        query: {
          layoutId: command,
        },
      })
    );
  }
};

const onCreateLayoutDesignPage = (command: string) => {
  if (!command) {
    router.push(
      useRouteSiteId({
        name: "page-design",
      })
    );
  } else if (command === emptyGuid) {
    router.push(
      useRouteSiteId({
        name: "layout-edit",
      })
    );
  } else {
    router.push(
      useRouteSiteId({
        name: "layout-page-design",
        query: {
          layoutId: command,
        },
      })
    );
  }
};

const openRouteSettingDialog = () => {
  if (siteStore.hasAccess("pages", "router")) {
    showRouterDialog.value = true;
  }
};
</script>

<template>
  <div class="flex items-center py-24 space-x-16">
    <el-button
      v-hasPermission="{
        feature: 'pages',
        action: 'edit',
      }"
      round
      data-cy="new-page"
      @click="router.push(useRouteSiteId({ name: 'page-edit' }))"
    >
      <el-icon class="iconfont icon-a-addto" />
      {{ t("common.newPage") }}
    </el-button>

    <el-button
      v-hasPermission="{
        feature: 'pages',
        action: 'edit',
      }"
      round
      data-cy="new-rich-text-page"
      @click="router.push(useRouteSiteId({ name: 'rich-page-edit' }))"
    >
      <el-icon class="iconfont icon-a-addto" />
      {{ t("common.newRichTextPage") }}
    </el-button>

    <el-dropdown trigger="click" class="ml-10px" @command="onCreateLayoutPage">
      <el-button
        v-hasPermission="{
          feature: 'pages',
          action: 'edit',
        }"
        round
        data-cy="new-layout-page"
      >
        <el-icon class="iconfont icon-a-addto" />
        <span>{{ t("common.newLayoutPage") }}</span>
        <el-icon class="iconfont icon-pull-down text-12px ml-8 !mr-0" />
      </el-button>
      <template #dropdown>
        <el-dropdown-menu>
          <el-dropdown-item
            v-for="item of layoutList"
            :key="item.key"
            :command="item.key"
            :disabled="
              !siteStore.hasAccess('layout', 'edit') &&
              !layoutStore.list?.length
            "
            :title="
              !siteStore.hasAccess('layout', 'edit') &&
              !layoutStore.list?.length
                ? t('common.noPermission')
                : ''
            "
            data-cy="layout"
          >
            <span>{{ item.value }}</span>
          </el-dropdown-item>
        </el-dropdown-menu>
      </template>
    </el-dropdown>

    <el-dropdown
      trigger="click"
      class="ml-10px"
      @command="onCreateLayoutDesignPage"
    >
      <el-button
        v-hasPermission="{
          feature: 'pages',
          action: 'edit',
        }"
        round
        data-cy="new-layout-design-page"
      >
        <el-icon class="iconfont icon-a-addto" />
        <span>{{ t("common.newPageDesign") }}</span>
        <el-icon class="iconfont icon-pull-down text-12px ml-8 !mr-0" />
      </el-button>
      <template #dropdown>
        <el-dropdown-menu>
          <el-dropdown-item command="">
            <span>{{ t("common.normalPage") }}</span>
          </el-dropdown-item>
          <el-dropdown-item
            v-for="item of layoutList"
            :key="item.key"
            :command="item.key"
            :disabled="
              !siteStore.hasAccess('layout', 'edit') &&
              !layoutStore.list?.length
            "
            :title="
              !siteStore.hasAccess('layout', 'edit') &&
              !layoutStore.list?.length
                ? t('common.noPermission')
                : ''
            "
            data-cy="layout-design"
          >
            <span>{{ item.value }}</span>
          </el-dropdown-item>
        </el-dropdown-menu>
      </template>
    </el-dropdown>

    <el-button
      v-hasPermission="{
        feature: 'pages',
        action: 'edit',
      }"
      round
      data-cy="import"
      @click="showImportDialog = true"
    >
      {{ t("common.import") }}
    </el-button>

    <div class="flex-1" />

    <IconButton
      v-if="pages?.length"
      :permission="{ feature: 'pages', action: 'router' }"
      circle
      icon="icon-a-setup"
      :tip="t('common.routeSetting')"
      data-cy="route-setting"
      @click="openRouteSettingDialog"
    />
  </div>
  <RouterDialog
    v-if="pages && showRouterDialog"
    v-model="showRouterDialog"
    :pages="pages"
  />
  <ImportDialog v-if="showImportDialog" v-model="showImportDialog" />
</template>
