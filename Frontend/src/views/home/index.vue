<template>
  <div
    class="home w-1120px mx-auto py-32"
    :class="{ 'home--list': listType !== 'grid' }"
  >
    <div class="home__title">{{ t("common.mySites") }}</div>
    <div v-if="hasSelectFolder" class="mt-32">
      <Breadcrumb
        hide-header
        :crumb-path="[{ name: t('common.allSites') }, { name: currentFolder!.key }]"
        @handle-click-path="
          (item) => {
            if (item.name === t('common.allSites')) {
              router.push({ query: {} });
            }
          }
        "
      />
    </div>
    <div class="home__head">
      <div>
        <el-button
          type="primary"
          round
          class="shadow-s-10"
          data-cy="new-site"
          @click="handleCreateSite"
        >
          <el-icon class="iconfont icon-a-addto" />
          {{ t("common.newSite") }}
        </el-button>
        <el-button
          v-if="!hasSelectFolder"
          round
          data-cy="new-folder"
          @click="handleCreateFolder"
        >
          <el-icon class="iconfont icon-a-addfile" />
          {{ t("common.newFolder") }}
        </el-button>
      </div>
      <div class="home__toolbar">
        <SearchInput
          v-model="keywordsTemp"
          :placeholder="t('common.searchWebsite')"
          data-cy="search"
          class="w-224px"
        />
        <ListSwitchButton
          v-model:current="listType"
          class="home__toolbar-item ml-6"
        />
      </div>
    </div>
    <template v-if="listType === 'grid' && hasFolders">
      <FolderList
        :data="folderList"
        @select="handleSelectFolder"
        @remove-success="refreshListAndFolders"
        @rename="handleRenameFolder"
      />
      <div v-if="hasSites" class="home__line" />
    </template>
    <template v-if="initialized">
      <div
        v-if="hasSites || (hasFolders && listType === 'list')"
        class="home__list-head"
      >
        <el-checkbox
          v-model="selectAll"
          size="large"
          :indeterminate="isIndeterminate"
          data-cy="total-selected-sites"
        >
          {{
            totalSelectedSites === 0
              ? t("common.selectAll")
              : totalSelectedSites > 1
              ? t("common.sitesSelected", { total: totalSelectedSites })
              : t("common.siteSelected")
          }}
        </el-checkbox>
        <div class="home__list-toolbar">
          <template v-if="totalSelectedSites > 0">
            <el-button
              round
              class="text-blue"
              data-cy="batch-export"
              @click="handleExportSelectedSites"
              >{{ t("common.export") }}</el-button
            >
            <el-button
              round
              class="text-blue"
              data-cy="batch-move-to-folder"
              @click="handleMoveSitesToFolder"
              >{{ t("common.moveToFolder") }}
            </el-button>
            <IconButton
              v-if="!siteStore.permissions.length"
              circle
              class="text-orange text-18px hover:text-orange shadow-s-10"
              icon="icon-delete"
              :tip="t('common.delete')"
              data-cy="batch-delete"
              @click="handleDeleteSelectedSites"
            />
          </template>
          <template v-else>
            {{
              t("common.allLoaded", {
                total: pagination.totalCount,
              })
            }}
          </template>
        </div>
      </div>
      <SiteList
        v-if="listType === 'list'"
        :data="siteList"
        :folders="folderList"
        :current-folder="currentFolder"
        @delete="refreshList"
        @export="handleExport"
        @move-to="handleMoveToFolder"
        @select="handleSelectFolder"
        @remove-success="refreshListAndFolders"
        @rename="handleRenameFolder"
        @share="handleShare"
      />
      <SiteGrid
        v-else
        :data="siteList"
        @delete="refreshList"
        @share="handleShare"
        @export="handleExport"
        @move-to="handleMoveToFolder"
      />
      <div v-if="siteList.length" class="home__pagination py-24">
        <el-pagination
          v-if="!route.query?.currentFolder"
          v-model:currentPage="pagination.pageNr"
          :page-size="pagination.pageSize"
          layout="prev, pager, next"
          :total="pagination.totalCount"
          hide-on-single-page
          @current-change="handlePageChange"
        />
        <el-pagination
          v-else
          v-model:currentPage="siteInFolderPagination.pageNr"
          :page-size="siteInFolderPagination.pageSize"
          layout="prev, pager, next"
          :total="siteInFolderPagination.totalCount"
          hide-on-single-page
          @current-change="handlePageChange"
        />
      </div>
      <template v-if="!hasSites">
        <EmptySiteInFolder
          v-if="hasSelectFolder && currentFolder"
          :folder="currentFolder"
          :keywords-length="keywords.length"
        />

        <EmptySite
          v-else
          :keywords-length="keywords.length"
          :all-sites-in-folder="allSitesInFolder"
          @create-site="handleCreateSite"
        />
      </template>
    </template>
  </div>

  <ShareSiteDialog v-model="visibleShareSiteDialog" :site="currentSite" />
  <ExportSiteDialog
    v-if="visibleExportSiteDialog"
    v-model="visibleExportSiteDialog"
    :site-id="currentSite.siteId"
  />
  <AddFolderDialog
    v-model="visibleCreateFolderDialog"
    :folders="folderList"
    @create-success="getFolderList"
  />
  <RenameFolderDialog
    v-model="visibleRenameFolderDialog"
    :folders="folderList"
    :folder="currentEditFolder"
    @rename-success="getFolderList"
  />
  <MoveToFolderDialog
    v-model="visibleMoveToFolderDialog"
    :sites="wantToMoveSites"
    :folders="folderList"
    :current-folder="currentFolder"
    @move-success="refreshListAndFolders"
  />
</template>

<script lang="ts" setup>
import { ref, onMounted, computed, watch, reactive, onUnmounted } from "vue";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import EmptySite from "./components/empty-site.vue";
import EmptySiteInFolder from "./components/empty-site-in-folder.vue";
import SiteGrid from "./components/site-grid.vue";
import SiteList from "./components/site-list.vue";
import ShareSiteDialog from "./components/share-site-dialog.vue";
import ExportSiteDialog from "@/components/export-site-dialog/index.vue";
import FolderList from "./components/folder-list.vue";
import AddFolderDialog from "./components/create-folder-dialog.vue";
import RenameFolderDialog from "./components/rename-folder-dialog.vue";
import MoveToFolderDialog from "./components/move-to-folder-dialog.vue";
import ListSwitchButton from "@/components/list-switch-button/index.vue";
import type { ListType } from "@/components/list-switch-button/types";
import {
  getSitePagedlist,
  exportSite,
  deleteSite,
  getFolderList as getFolderListAPI,
} from "@/api/site";
import type { FolderItem, SiteItem } from "./type";
import { useSiteStore } from "@/store/site";
import SearchInput from "@/components/basic/search-input.vue";
import { showDeleteConfirm } from "@/components/basic/confirm";
import { useRouter, useRoute } from "vue-router";
import { useI18n } from "vue-i18n";
import { getQueryString, openInHiddenFrame, searchDebounce } from "@/utils/url";
import { useAppStore } from "@/store/app";
import { batchSitesDelete } from "@/api/site";

const siteStore = useSiteStore();
const router = useRouter();
const route = useRoute();
const { t } = useI18n();
const keywords = ref("");
const keywordsTemp = ref("");

const listType = ref<ListType>("grid");
const initialized = ref(false);
const siteList = ref<SiteItem[]>([]);
const folderList = ref<FolderItem[]>([]);
const allSitesInFolder = ref(0);
const currentSite = ref({ siteId: "" } as SiteItem);
const getPagination = computed(() => {
  if (route.query?.currentFolder) {
    return siteInFolderPagination;
  } else {
    return pagination;
  }
});
const pagination = reactive({
  pageNr: 1,
  pageSize: 6,
  totalCount: 0,
});

const siteInFolderPagination = reactive({
  pageNr: 1,
  pageSize: 6,
  totalCount: 0,
});
const selectAll = ref(false);
const visibleCreateFolderDialog = ref(false);
const visibleShareSiteDialog = ref(false);
const visibleExportSiteDialog = ref(false);
const hasSites = computed(() => siteList.value.length > 0);
const hasFolders = computed(
  () => folderList.value.length > 0 && !hasSelectFolder.value
);
const selectedSites = computed(() =>
  siteList.value.filter((item) => item.selected)
);
const totalSelectedSites = computed(() => selectedSites.value.length);
const isIndeterminate = ref(false);
const currentFolder = ref<FolderItem | null>(null);
const hasSelectFolder = computed(() => currentFolder.value !== null);
const visibleRenameFolderDialog = ref(false);
const currentEditFolder = ref({} as FolderItem);
const visibleMoveToFolderDialog = ref(false);

const wantToMoveSites = ref<SiteItem[]>([]);
let setTimeoutId = 0;
onMounted(() => {
  if (!route.query?.currentFolder) {
    getList();
  }
  getFolderList().then(() => {
    // 如果query携带文件夹参数 则切换到文件夹下
    const currentFolderKey = getQueryString("currentFolder");
    if (currentFolderKey) {
      const folderItem = folderList.value.find(
        (item) => item.key === currentFolderKey
      );
      if (folderItem) {
        handleSelectFolder(folderItem);
        refreshList();
      } else {
        router.replace({
          query: {},
        });
      }
    }
  });
  siteStore.clear();
});

function countTotal(arr: any[], keyName: any) {
  let total = 0;
  total = arr.reduce(function (total: number, currentValue: any) {
    return currentValue[keyName] ? total + currentValue[keyName] : total;
  }, 0);
  return total;
}

onUnmounted(() => clearTimeout(setTimeoutId));

watch(
  () => route.query?.currentFolder,
  (n) => {
    if (route.query?.currentFolder) {
      // 如果query携带文件夹参数 则切换到文件夹下
      const currentFolderKey = getQueryString("currentFolder");
      if (currentFolderKey) {
        if (currentFolderKey) {
          const folderItem = folderList.value.find(
            (item) => item.key === currentFolderKey
          );
          handleSelectFolder(folderItem!);
          refreshList();
        }
      }
    } else {
      siteInFolderPagination.pageNr = 1;
      pagination.pageNr = 1;
      gotoAllSites();
    }
  }
);

watch(
  () => selectAll.value,
  (val) => {
    siteList.value.forEach((site) => {
      site.selected = val;
    });
  }
);
watch(
  () => totalSelectedSites.value,
  (val) => {
    if (val === 0) {
      selectAll.value = false;
      isIndeterminate.value = false;
    } else if (val < siteList.value.length) {
      isIndeterminate.value = true;
    } else if (val === siteList.value.length) {
      isIndeterminate.value = false;
      selectAll.value = true;
    }
  }
);
async function getList(hiddenLoading?: boolean) {
  try {
    const response = await getSitePagedlist(
      {
        pageNr: getPagination.value.pageNr,
        pageSize: getPagination.value.pageSize,
        keyword: keywords.value,
        folder: currentFolder.value?.key,
      },
      hiddenLoading
    );

    if (hiddenLoading) {
      for (const iterator of response.list) {
        const found = siteList.value.find((f) => f.siteId === iterator.siteId);
        if (found) found.inProgress = iterator.inProgress;
      }
    } else {
      siteList.value = response.list;
    }

    getPagination.value.totalCount = response.totalCount;
    if (response.list.some((s) => s.inProgress)) {
      setTimeoutId = setTimeout(() => {
        getList(true);
      }, 3000) as any;
    }
  } catch (error: any) {
    if (error?.response?.status === 401) {
      useAppStore().logout();
      router.replace({ name: "login" });
    }
  } finally {
    initialized.value = true;
  }
}
async function getFolderList() {
  folderList.value = await getFolderListAPI();
  allSitesInFolder.value = countTotal(folderList.value, "value");
}
async function refreshList() {
  await getList();
  selectAll.value = false;
}
function refreshListAndFolders() {
  refreshList();
  if (!hasSelectFolder.value) {
    getFolderList();
  }
}
function handleCreateSite() {
  router.push({
    name: "create-site",
    query: { currentFolder: currentFolder.value?.key },
  });
}
function handleCreateFolder() {
  visibleCreateFolderDialog.value = true;
}
function handlePageChange(pageNr: number) {
  getPagination.value.pageNr = pageNr;
  getList();
}
async function handleExportSelectedSites() {
  for (const item of selectedSites.value) {
    const file = await exportSite({ siteId: item.siteId, copyMode: "normal" });
    openInHiddenFrame(
      `${import.meta.env.VITE_API}/Site/export?siteId=${
        item.siteId
      }&exportfile=${file}`
    );
  }
}
async function handleDeleteSelectedSites() {
  const ids = selectedSites.value.map((x) => x.siteId);
  await showDeleteConfirm(ids.length);
  await batchSitesDelete(ids);
  siteStore.sites = [];
  getList();
}
function handleShare(site: SiteItem) {
  currentSite.value = site;
  visibleShareSiteDialog.value = true;
}
function handleExport(site: SiteItem) {
  currentSite.value = site;
  visibleExportSiteDialog.value = true;
}
function handleSelectFolder(folder: FolderItem) {
  router.push({
    query: {
      currentFolder: folder.key,
    },
  });
  currentFolder.value = folder;
  keywords.value = "";
  siteList.value = [];
  initialized.value = false;
}
function gotoAllSites() {
  currentFolder.value = null;
  keywords.value = "";
  siteList.value = [];
  refreshListAndFolders();
}
function handleRenameFolder(folder: FolderItem) {
  currentEditFolder.value = folder;
  visibleRenameFolderDialog.value = true;
}
function handleMoveSitesToFolder() {
  wantToMoveSites.value = selectedSites.value;
  visibleMoveToFolderDialog.value = true;
}
function handleMoveToFolder(site: SiteItem) {
  wantToMoveSites.value = [site];
  visibleMoveToFolderDialog.value = true;
}

function searchEvent() {
  keywords.value = keywordsTemp.value.trim();
  refreshList();
}
const search = searchDebounce(searchEvent, 1000);
watch(
  () => keywordsTemp.value,
  () => {
    search();
  }
);
</script>

<style lang="scss" scoped>
.home {
  &__title {
    .dark & {
      color: rgba(#fff, 0.86);
    }

    font-size: 32px;
    color: $main-color;
    line-height: 45px;
  }

  &__breadcrumb {
    margin-top: 32px;
    display: flex;
    align-items: center;
    font-size: 14px;

    &__item {
      font-weight: bold;
      cursor: pointer;

      &--current {
        font-weight: normal;
        color: #666;
        cursor: default;
      }
    }

    .iconfont {
      font-size: 14px;
      margin: 0 8px;
    }
  }

  &__head {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin: 32px 0;
  }

  &__input {
    box-shadow: 0px 2px 4px 0px rgba(0, 0, 0, 0.1);
    padding: 10px 24px;

    .iconfont {
      font-size: 18px;
      margin-right: 10px;
    }
  }

  &__btn {
    &--blue {
      color: $main-blue;
    }
  }

  .el-button + .el-button {
    margin-left: 24px;
  }

  &__input {
    padding: 0;
    border-radius: 100px;
    min-width: 238px;

    &:deep(.el-input__inner) {
      border-radius: 100px;
      padding: 4px 5px 4px 50px;
    }

    .iconfont {
      color: $main-blue;
      padding: 0 16px;
    }
  }

  &__toolbar {
    display: flex;
    align-items: center;
  }

  &__list {
    &-title {
      color: #666;
      font-size: 12px;
    }

    &-head {
      padding: 0px 24px;
      min-height: 64px;
      border-radius: 8px 8px 0 0;
      background-color: #e9eaf0;

      .dark & {
        background-color: #333;
      }

      box-shadow: 0px 4px 8px 0px rgba(0, 0, 0, 0.2);
      display: flex;
      align-items: center;
      justify-content: space-between;
      position: relative;

      .el-button {
        padding: 0px 24px;
        min-height: 32px;

        &.is-circle {
          padding: 0;
          height: 32px;
          width: 32px;
        }
      }
    }

    &-toolbar {
      color: #666;

      .dark & {
        color: rgba(#fff, 0.6);
      }

      font-size: 12px;
    }
  }

  &__line {
    height: 2px;
    background: rgba($color: #192845, $alpha: 0.06);
    margin: 24px 0;
  }

  &--list {
    .home {
      &__pagination {
        background: #fff;
        border-radius: 0 0 8px 8px;
        box-shadow: 0px 2px 4px 0px rgba(0, 0, 0, 0.1);
      }
    }
  }
}

.dark {
  .home {
    &--list {
      .home {
        &__pagination {
          background: #333;
        }
      }
    }
  }
}
</style>
