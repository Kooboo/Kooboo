<script lang="ts" setup>
import KTable from "@/components/k-table";
import { useModuleStore } from "@/store/module";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useI18n } from "vue-i18n";
import AddDialog from "./add-dialog.vue";
import { computed, ref } from "vue";
import { exportModule, updateStatus } from "@/api/module";
import ImportDialog from "./import-dialog.vue";
import ReadMeDialog from "./read-me-dialog.vue";
import SettingDialog from "./setting-dialog.vue";
import { shareModule } from "@/api/module";
import { showConfirm, showDeleteConfirm } from "@/components/basic/confirm";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { useSiteStore } from "@/store/site";
import Cookies from "universal-cookie";
import { openInNewTab } from "@/utils/url";
import SearchOnlineDialog from "@/components/search-dialog/package-search.vue";
import type SearchOnlineDialogType from "@/components/search-dialog/package-search.vue";
const searchDialog = ref<InstanceType<typeof SearchOnlineDialogType>>();
function handleSearchOnline() {
  (searchDialog.value as any)?.show();
}
const cookies = new Cookies();
const { t } = useI18n();
const moduleStore = useModuleStore();
const siteStore = useSiteStore();
const showAddDialog = ref(false);
const showImportDialog = ref(false);
const showReadMeDialog = ref(false);
const showSettingDialog = ref(false);
const showManageDialog = ref(false);
const selected = ref<string>();
const backUrl = ref<string>();

const onDelete = async (rows: { id: string }[]) => {
  await showDeleteConfirm(rows.length);
  await moduleStore.deleteModules(rows.map((m) => m.id));
};

const onShare = async (id: string) => {
  if (!siteStore.hasAccess("module", "edit")) return;
  await showConfirm(t("common.shareModuleConfirm"));
  await shareModule(id);
};

const onReadMe = (id: string) => {
  selected.value = id;
  showReadMeDialog.value = true;
};

const onSetting = (id: string) => {
  selected.value = id;
  showSettingDialog.value = true;
};

const changeState = async (row: any) => {
  if (!siteStore.hasAccess("module", "edit")) return;
  await updateStatus(row.id, !row.online);
  moduleStore.load();
};

const onExportModule = (name: string) => {
  if (!siteStore.hasAccess("module", "edit")) return;
  exportModule(name);
};

const authBackUrl = computed(() => {
  if (!backUrl.value) return;
  const token = cookies.get("jwt_token") || localStorage.getItem("TOKEN");
  return backUrl.value + "?access_token=" + token;
});

const onBackendView = (url: string) => {
  const token = cookies.get("jwt_token") || localStorage.getItem("TOKEN");
  const u = new URL(url);
  const isPublic = siteStore.site.siteType == "p";
  const isNoSecure = location.protocol == "https:" && u.protocol == "http:";
  if (isPublic) {
    if (isNoSecure) {
      openInNewTab(url);
    } else {
      backUrl.value = url;
      showManageDialog.value = true;
    }
  } else {
    url = `${u.origin}/_Admin/login?permission=${siteStore.site.siteType}&returnurl=${u.pathname}&&access_token=${token}`;
    openInNewTab(url);
  }
};
const onReload = async () => {
  await moduleStore.load();
};

onReload();
</script>

<template>
  <div class="p-24">
    <Breadcrumb :name="t('common.modules')" />
    <div class="flex items-center py-24 space-x-16">
      <el-button
        v-hasPermission="{ feature: 'module', action: 'edit' }"
        round
        data-cy="create-module"
        @click="showAddDialog = true"
      >
        <div class="flex items-center">
          <el-icon class="mr-16 iconfont icon-a-addto" />
          {{ t("common.create") }}
        </div>
      </el-button>

      <el-button
        v-hasPermission="{ feature: 'module', action: 'edit' }"
        round
        data-cy="import"
        @click="showImportDialog = true"
      >
        <div class="flex items-center">
          <el-icon class="mr-16 iconfont icon-a-Pullin" />
          {{ t("common.import") }}
        </div>
      </el-button>
      <el-button
        v-hasPermission="{
          feature: 'module',
          action: 'edit',
        }"
        round
        data-cy="search"
        @click="handleSearchOnline"
      >
        <div class="flex items-center">
          <el-icon class="mr-16 iconfont icon-search" />
          {{ t("common.search") }}
        </div>
      </el-button>
    </div>
    <KTable
      v-if="moduleStore.list"
      :data="moduleStore.list"
      show-check
      sort="creationDate"
      :permission="{ feature: 'module', action: 'delete' }"
      @delete="onDelete"
    >
      <el-table-column :label="t('common.name')">
        <template #default="{ row }">
          <router-link
            :to="
              useRouteSiteId({
                name: 'dev-mode',
                query: {
                  activity: 'modules',
                  moduleId: row.id,
                },
              })
            "
            data-cy="name"
            ><span class="text-blue cursor-pointer">{{
              row.name
            }}</span></router-link
          >
        </template>
      </el-table-column>
      <el-table-column :label="t('common.online')" width="150">
        <template #default="{ row }">
          <ElSwitch
            v-hasPermission="{ feature: 'module', action: 'edit' }"
            :model-value="row.online"
            data-cy="online"
            @click="changeState(row)"
          />
        </template>
      </el-table-column>
      <el-table-column width="100" :label="t('common.backend')">
        <template #default="{ row }">
          <el-button
            v-if="row.backendViewUrl"
            size="small"
            type="primary"
            round
            data-cy=""
            class="leading-6"
            @click="onBackendView(row.backendViewUrl)"
            >{{ t("common.manage") }}</el-button
          >
        </template>
      </el-table-column>

      <el-table-column align="right" width="180px" prop="creationDate">
        <template #default="{ row }">
          <IconButton
            icon="icon-a-setup"
            :tip="t('common.settings')"
            data-cy="setting"
            @click="onSetting(row.id)"
          />
          <IconButton
            icon="icon-page"
            :tip="t('common.description')"
            data-cy="description"
            @click="onReadMe(row.id)"
          />
          <IconButton
            :permission="{ feature: 'module', action: 'edit' }"
            icon="icon-xiazai-wenjianxiazai-05"
            :tip="t('common.export')"
            data-cy="export"
            @click="onExportModule(row.name)"
          />
          <IconButton
            :permission="{ feature: 'module', action: 'edit' }"
            icon="icon-link !bg-transparent"
            :tip="t('common.share')"
            data-cy="share"
            @click="onShare(row.id)"
          />
        </template>
      </el-table-column>
    </KTable>
    <AddDialog v-if="showAddDialog" v-model="showAddDialog" />
    <ImportDialog v-if="showImportDialog" v-model="showImportDialog" />
    <ReadMeDialog
      v-if="showReadMeDialog"
      :id="selected!"
      v-model="showReadMeDialog"
    />
    <SettingDialog
      v-if="showSettingDialog"
      :id="selected!"
      v-model="showSettingDialog"
    /><el-dialog
      v-model="showManageDialog"
      width="1000px"
      :close-on-click-modal="false"
      custom-class="el-dialog--zero-padding"
      :title="t('common.manage')"
      @closed="showManageDialog = false"
    >
      <iframe :src="authBackUrl" class="w-full h-500px" />
    </el-dialog>
    <SearchOnlineDialog ref="searchDialog" module="module" @reload="onReload" />
  </div>
</template>
