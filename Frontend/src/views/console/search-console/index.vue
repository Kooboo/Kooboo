<script lang="ts" setup>
import {
  getAuthUrl,
  getSiteList,
  deleteSite,
  validSite,
} from "@/api/search-console";
import { ref } from "vue";
import GuidInfo from "@/components/guid-info/index.vue";
import { useI18n } from "vue-i18n";
import type { Site } from "@/api/search-console/types";
import { showDeletesConfirm } from "@/components/basic/confirm";
import AddDialog from "./add-dialog.vue";
import SitemapDialog from "./sitemap-dialog.vue";
import AnalyticsDialog from "./analytics-dialog.vue";

const authUrl = ref<string>();
const sites = ref<Site[]>([]);
const { t } = useI18n();
const showAddDialog = ref(false);
const showSitemapDialog = ref(false);
const showAnalyticsDialog = ref(false);
const selected = ref<Site>();

async function getList() {
  try {
    sites.value = (await getSiteList()).sort((a, b) =>
      a.siteUrl > b.siteUrl ? 1 : -1
    );
  } catch (error) {
    authUrl.value = await getAuthUrl();
  }
}

getList();

function openAuthUrl() {
  location.href = authUrl.value!;
}

async function onValidSite(siteUrl: string, onGoogle: boolean) {
  await validSite(siteUrl, onGoogle);
  getList();
}

async function onDelete(items: Site[]) {
  await showDeletesConfirm();

  for (const item of items) {
    await deleteSite(item.siteUrl);
  }

  getList();
}

function isValid(item: Site) {
  return item.permissionLevel != "siteUnverifiedUser";
}

function onSitemap(item: Site) {
  selected.value = item;
  showSitemapDialog.value = true;
}

function onAnalytics(item: Site) {
  selected.value = item;
  showAnalyticsDialog.value = true;
}

const permissions: Record<string, string> = {
  siteOwner: t("common.siteOwner"),
  siteUnverifiedUser: t("common.siteUnverifiedUser"),
};
</script>

<template>
  <div>
    <GuidInfo v-if="authUrl">
      <p>{{ t("common.searchConsoleLoginTip") }}</p>
      <template #button>
        <ElButton type="primary" @click="openAuthUrl">{{
          t("common.login")
        }}</ElButton>
      </template>
    </GuidInfo>
    <div v-else class="px-24">
      <div class="flex items-center py-24 space-x-16">
        <!-- <el-button round data-cy="new-page" @click="showAddDialog = true">
          <el-icon class="iconfont icon-a-addto" />
          {{ t("common.addSite") }}
        </el-button> -->
      </div>
      <KTable :data="sites" @delete="onDelete">
        <el-table-column :label="t('common.site')" prop="siteUrl">
          <template #default="{ row }">
            <span
              v-if="isValid(row)"
              class="text-blue cursor-pointer"
              @click="onAnalytics(row)"
              >{{ row.siteUrl?.replace("sc-domain:", "") }}</span
            >
            <span v-else>{{ row.siteUrl }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="t('common.permission')" align="center">
          <template #default="{ row }">
            <ElTag class="rounded-full">{{
              permissions[row.permissionLevel] || row.permissionLevel
            }}</ElTag>
          </template>
        </el-table-column>
        <el-table-column align="right">
          <template #default="{ row }">
            <ElButton
              v-if="!isValid(row)"
              type="danger"
              round
              size="small"
              @click="onValidSite(row.siteUrl, row.onGoogle)"
              >{{ t("common.validation") }}</ElButton
            >
            <ElButton
              v-else
              type="primary"
              round
              plain
              size="small"
              @click="onSitemap(row)"
              >{{ t("common.sitemap") }}
            </ElButton>
          </template>
        </el-table-column>
      </KTable>
    </div>

    <AddDialog
      v-if="showAddDialog"
      v-model="showAddDialog"
      :excludes="sites.map((m) => m.siteUrl)"
      @reload="getList"
    />
    <SitemapDialog
      v-if="showSitemapDialog"
      v-model="showSitemapDialog"
      :site-url="selected!.siteUrl"
    />
    <AnalyticsDialog
      v-if="showAnalyticsDialog"
      v-model="showAnalyticsDialog"
      :site-url="selected!.siteUrl"
    />
  </div>
</template>
