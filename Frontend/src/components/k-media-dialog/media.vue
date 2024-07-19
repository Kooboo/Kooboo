<template>
  <div class="media">
    <div
      v-if="!dialogInfo"
      class="flex items-center justify-between px-24 pt-0 relative"
      :class="{ 'pb-24': provider === 'default' }"
    >
      <Breadcrumb
        :class="{
          'w-[calc(100%-400px)]': providers.length > 1,
          'w-[calc(100%-270px)]': providers.length <= 1,
        }"
        :crumb-path="getCrumbPath"
        @handle-click-path="handleClickBreadcrumb"
      />

      <div
        :class="{
          'max-w-400px': providers.length > 1,
          'max-w-270px': providers.length <= 1,
        }"
      >
        <SearchProvider
          v-model:provider="provider"
          v-model:searchKey="searchKey"
          :providers="providers"
          @update:provider="onProviderChanged"
          @update:search-key="updateRoute"
        />
      </div>
    </div>
    <el-tabs v-if="showTabs" v-model="curImgType" class="el-tabs--hide-content">
      <template v-for="tab in imgTypes">
        <el-tab-pane
          v-if="tab.value === 'all' || !searchKey"
          :key="tab.value"
          :label="tab.displayName"
          :name="tab.value"
        />
      </template>
    </el-tabs>
    <div class="p-24px media__body">
      <div
        class="flex items-center justify-between media__head"
        :class="{
          'mb-24px': !dialogInfo,
        }"
      >
        <div class="flex items-center space-x-24">
          <template v-if="isAllTab">
            <el-checkbox
              v-if="!dialogInfo"
              v-model="selectAll"
              :indeterminate="isIndeterminate"
              size="large"
              data-cy="check-all"
            >
              {{ t("common.selectAllFiles") }}
            </el-checkbox>
            <MediaUpload
              :disabled="!!searchKey"
              :folder="currentFolderPath"
              :provider="provider"
              :multiple="true"
              @after-upload="resetList(true)"
            />

            <el-button
              v-if="!dialogInfo"
              v-hasPermission="{
                feature: 'mediaLibrary',
                action: 'edit',
              }"
              :disabled="!!searchKey"
              round
              data-cy="new-folder"
              @click="handleNewFolder"
            >
              <el-icon class="iconfont icon-a-addfile" />
              {{ t("common.newFolder") }}
            </el-button>
            <el-button
              v-if="!dialogInfo"
              v-hasPermission="{
                feature: 'mediaLibrary',
                action: 'view',
              }"
              :disabled="!!searchKey"
              round
              data-cy="search"
              @click="handleSearchOnline"
            >
              <el-icon class="iconfont icon-search" />
              {{ t("common.search") }}
            </el-button>
            <el-button
              v-if="totalSelectedItems > 0 && !dialogInfo"
              v-hasPermission="{
                feature: 'mediaLibrary',
                action: 'view',
              }"
              round
              data-cy="download"
              @click="handleDownload"
            >
              <el-icon class="iconfont icon-xiazai-wenjianxiazai-05" />
              {{ t("common.download") }}
            </el-button>
            <el-button
              v-if="totalSelectedItems > 0 && !dialogInfo"
              v-hasPermission="{
                feature: 'mediaLibrary',
                action: 'delete',
              }"
              round
              class="text-orange"
              data-cy="delete"
              @click="handleDelete"
            >
              <el-icon class="iconfont icon-delete" />
              {{ t("common.delete") }}
            </el-button>
          </template>
        </div>
        <ListSwitchButton
          v-if="!dialogInfo"
          v-model:current="listType"
          @change="updateRoute"
        />
        <div v-else>
          <SearchProvider
            v-model:provider="provider"
            v-model:searchKey="searchKey"
            :providers="providers"
            @update:provider="onProviderChanged"
            @update:search-key="updateRoute"
          />
        </div>
      </div>

      <div
        v-if="listType === 'list'"
        class="rounded-normal overflow-hidden shadow-s-10"
      >
        <MediaList
          :folders="folders"
          :files="files"
          :has-checkbox="isAllTab"
          :hide-open-folder="!searchKey"
          :provider="provider"
          @click-folder="handleClickFolder"
          @edit-image="handleEditImage"
        />
        <KPagination
          v-if="files.length || folders.length"
          class="bg-fff dark:bg-444 py-24"
          :pagination="pagination"
          :infinite="infinite"
          @current-change="handlePageChange"
        />
      </div>

      <div v-else>
        <MediaGrid
          :folders="folders"
          :files="files"
          :has-checkbox="isAllTab"
          :hide-open-folder="!searchKey"
          :grid-col="gridCol ?? 6"
          :pagination="pagination"
          :provider="provider"
          @click-folder="handleClickFolder"
          @edit-image="handleEditImage"
          @change-page="handlePageChange"
        />
        <KPagination
          v-if="files.length || folders.length"
          class="dark:bg-[#1e1e1e] py-24"
          :class="!gridCol ? 'bg-[#f3f5f5]' : 'bg-fff dark:bg-[#222222]'"
          :pagination="pagination"
          :infinite="infinite"
          @current-change="handlePageChange"
        />
      </div>
    </div>
    <CreateFolderDialog
      v-model="visibleCreateFolderDialog"
      :path="currentFolderPath"
      :folders="folders"
      :provider="provider"
      @create-success="refreshList"
    />
    <AltEditorDialog
      v-model="visibleEditImageDialog"
      :file="editingFile"
      :provider="provider"
    />
    <SearchOnlineDialog
      ref="searchDialog"
      :provider="provider"
      @reload="refreshList(true)"
    />
  </div>
</template>

<script lang="ts" setup>
import type { MediaPagedList, CrumbPathItem } from "@/api/content/media";
import {
  getMediaPagedList,
  getMediaPagedListBy,
  deleteFolders,
  deleteImages,
} from "@/api/content/media";
import type { DialogInfo, MediaFileItem, MediaFolderItem } from "./";

import type { ComputedRef } from "vue";
import { computed, inject, onMounted, reactive, ref, watch } from "vue";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import ListSwitchButton from "@/components/list-switch-button/index.vue";
import type { ListType } from "@/components/list-switch-button/types";
import CreateFolderDialog from "./create-folder-dialog.vue";
import MediaList from "./media-list.vue";
import MediaGrid from "./media-grid.vue";
import SearchOnlineDialog from "@/components/search-dialog/media-search.vue";
import type SearchOnlineDialogType from "@/components/search-dialog/media-search.vue";
import { useRoute, useRouter } from "vue-router";
import { showDeleteConfirm } from "../basic/confirm";
import { useI18n } from "vue-i18n";
import type { Pagination } from "../k-table/types";
import { downloadMediaFile } from "@/api/content/media";
import { useUrlSiteId } from "@/hooks/use-site-id";
import { openInNewTab } from "@/utils/url";
import MediaUpload from "./media-upload.vue";
import { last, uniqBy, trim } from "lodash-es";
import KPagination from "../k-pagination/index.vue";
import SearchProvider from "./search-provider.vue";
import { errorMessage } from "@/components/basic/message";
import AltEditorDialog, { type EditingMediaFile } from "./alt-editor.vue";

defineProps<{
  gridCol?: number;
}>();

const { t } = useI18n();
const dialogInfo = inject<ComputedRef<DialogInfo>>("dialogInfo");
const router = useRouter();
const route = useRoute();

const imgTypes = [
  {
    displayName: t("common.all"),
    value: "all",
  },
  {
    displayName: t("common.page"),
    value: "page",
  },
  {
    displayName: t("common.style"),
    value: "style",
  },
  {
    displayName: t("common.view"),
    value: "view",
  },
  {
    displayName: t("common.layout"),
    value: "layout",
  },
  {
    displayName: t("common.htmlBlock"),
    value: "HTMLBlock",
  },
  {
    displayName: t("common.content"),
    value: "TextContent",
  },
] as const;

type ImageType = typeof imgTypes[number]["value"];
const curImgType = ref<ImageType>("all");
const selectAll = ref(false);
const isIndeterminate = ref(false);
const listType = ref<ListType>((route.query.listType as ListType) ?? "grid");

const folders = ref<MediaFolderItem[]>([]);
const files = ref<MediaFileItem[]>([]);
const infinite = ref(false);
const currentFolderPath = ref("/");
const pagination = reactive<Pagination>({
  currentPage: 1,
  pageSize: 24,
  totalCount: 0,
});
const visibleCreateFolderDialog = ref(false);
const visibleEditImageDialog = ref(false);
const editingFile = ref<EditingMediaFile>({
  key: "",
  alt: "",
  name: "",
  url: "",
  thumbnail: "",
});

const searchKey = ref("");
const provider = ref<string>((route.query.provider as string) || "default");
const providers = ref<string[]>([]);
type SearchData = {
  keyword: string;
  providers: string[];
  data: MediaPagedList;
  page: number;
  infinite?: boolean;
};
const emit = defineEmits<{
  (e: "loaded", value: SearchData): void;
}>();
const selectedFiles = computed(() =>
  files.value.filter((item) => item.selected)
);
const selectedFolders = computed(() =>
  folders.value.filter((item) => item.selected)
);
const totalSelectedItems = computed(
  () => selectedFiles.value.length + selectedFolders.value.length
);
const totalItems = computed(() => files.value.length + folders.value.length);
const isAllTab = computed(() => curImgType.value === "all");
const crumbPath = ref<CrumbPathItem[]>([]);
const getCrumbPath = computed(() => {
  // 修改根路径名
  return crumbPath.value.map((item) => {
    if (item.name === "searchResult") {
      item.name = t("common.searchResult");
    } else if (item.name === "root" && !dialogInfo) {
      item.name = t("common.mediaLibrary");
    }
    return item;
  });
});

const showTabs = computed(() => {
  return !dialogInfo && (provider.value || "default") === "default";
});

onMounted(() => {
  if (!dialogInfo) {
    watch(
      () => route.query,
      (query) => {
        if (route.name === "media") {
          currentFolderPath.value = (query.folder as string) || "/";
          searchKey.value = (query.keyword as string) || "";
          provider.value = (query.provider as string) || "default";
          refreshList();
        }
      },
      { immediate: true, deep: true }
    );
  } else {
    refreshList();
  }
});
watch(
  () => curImgType.value,
  () => {
    currentFolderPath.value = "/";
    refreshList();
  }
);
watch(
  () => selectAll.value,
  (val) => {
    folders.value.forEach((item) => {
      item.selected = val;
    });
    files.value.forEach((item) => {
      item.selected = val;
    });
  }
);

watch(
  () => totalSelectedItems.value,
  (val) => {
    if (val === 0) {
      selectAll.value = false;
      isIndeterminate.value = false;
    } else if (val < totalItems.value) {
      isIndeterminate.value = true;
    } else if (val === totalItems.value) {
      isIndeterminate.value = false;
      selectAll.value = true;
    }
  }
);

watch(
  () => route.query.listType,
  () => {
    listType.value = route.query.listType as ListType;
  }
);
watch(
  () => searchKey.value,
  () => {
    pagination.currentPage = 1;
  }
);
async function fetchPagedList(currentPage: number, hideLoading?: boolean) {
  const response = await getMediaPagedList(
    {
      pageNr: currentPage,
      pageSize: pagination.pageSize!,
      provider: provider.value,
      path: currentFolderPath.value,
      keyword: searchKey.value?.trim(),
      startAfter: currentPage > 1 ? last(files.value)?.key ?? "" : "",
    },
    hideLoading
  );
  afterFetchList(response, currentPage);
}
function afterFetchList(response: MediaPagedList, currentPage: number) {
  providers.value = response.providers ?? [];
  crumbPath.value = response.crumbPath;
  if (searchKey.value) {
    curImgType.value = "all";
  }

  if (response.errorMessage) {
    folders.value = [];
    files.value = [];
    pagination.totalCount = 0;
    pagination.currentPage = 1;
    console.warn(response.errorMessage);
    errorMessage(
      t("common.configError", {
        message: response.errorMessage.replace(
          "{troubleshoot}",
          t("common.troubleshoot")
        ),
      }),
      true,
      {
        dangerouslyUseHTMLString: true,
      }
    );
    return;
  }
  infinite.value = response.files.infinite || false;
  if (infinite.value) {
    currentPage = response.files.pageNr;
  }
  const folderList = (response.folders || []).map((it) => {
    it.selected = false;
    return it;
  });
  if (!infinite.value || currentPage <= 1) {
    folders.value = uniqBy(folderList, "id");
  } else {
    folders.value = uniqBy([...folders.value, ...folderList], "id");
  }

  const fileList = response.files.list.map((it) => {
    it.selected = false;
    return it;
  });
  files.value =
    !infinite.value || currentPage <= 1
      ? fileList
      : [...files.value, ...fileList];

  pagination.totalCount = response.files.totalCount;
  pagination.currentPage = currentPage;

  emit("loaded", {
    keyword: searchKey.value,
    data: response,
    page: currentPage,
    providers: providers.value,
    infinite: infinite.value,
  });
}

function handlePageChange(currentPage: number) {
  pagination.currentPage = currentPage;
  fetchList(pagination.currentPage);
}
function handleClickFolder(item: MediaFolderItem | CrumbPathItem) {
  currentFolderPath.value = item.fullPath!;
  pagination.currentPage = 1;
  searchKey.value = "";
  files.value = [];
  folders.value = [];
  updateRoute();
}

function handleEditImage(file: MediaFileItem) {
  editingFile.value = file;
  visibleEditImageDialog.value = true;
}

async function fetchPagedListBy(currentPage: number, hideLoading?: boolean) {
  const response = await getMediaPagedListBy(
    {
      pageNr: currentPage,
      pageSize: pagination.pageSize!,
      provider: provider.value,
      by: curImgType.value,
    },
    hideLoading
  );
  afterFetchList(response, currentPage);
}
function fetchList(currentPage: number, hideLoading?: boolean) {
  if (isAllTab.value) {
    fetchPagedList(currentPage, hideLoading);
  } else {
    fetchPagedListBy(currentPage, hideLoading);
  }
}

function refreshList(hideLoading?: boolean) {
  updateRoute();
  fetchList(pagination.currentPage!, hideLoading);
}

function handleNewFolder() {
  visibleCreateFolderDialog.value = true;
}

const searchDialog = ref<InstanceType<typeof SearchOnlineDialogType>>();
function handleSearchOnline() {
  (searchDialog.value as any)?.show();
}

function resetList(hideLoading?: boolean) {
  pagination.currentPage = 1;
  refreshList(hideLoading);
}

async function handleDelete() {
  try {
    await showDeleteConfirm(
      selectedFolders.value.length + selectedFiles.value.length
    );
    if (selectedFolders.value.length > 0) {
      await deleteFolders(
        selectedFolders.value.map((item) => item.fullPath),
        provider.value
      );
    }
    if (selectedFiles.value.length > 0) {
      const keys = selectedFiles.value.map((item) => item.key || item.id);
      await deleteImages(keys, provider.value);
    }
    resetList();
  } catch {
    void 0;
  }
}

// 多文件下载
async function handleDownload() {
  const folders = selectedFolders.value.map((item) => item.fullPath);
  const files = selectedFiles.value.map((item) => item.key);
  const downloadRequest: any = {
    root: currentFolderPath.value,
    folders,
    files,
  };
  const file = await downloadMediaFile(downloadRequest, provider.value);
  if (!file) {
    return;
  }
  const url = useUrlSiteId(`Download/Package?file=${file}&type=media`);
  openInNewTab(`${import.meta.env.VITE_API}/${url}`);
}

function handleClickBreadcrumb(item: CrumbPathItem) {
  if (trim(item.fullPath, "/") === trim(currentFolderPath.value, "/")) {
    return;
  }

  handleClickPath(item);
}

function handleClickPath(item: CrumbPathItem) {
  handleClickFolder(item);
}
function onProviderChanged() {
  handleClickPath({
    name: "root",
    fullPath: "/",
  });
}

function updateRoute() {
  if (!dialogInfo) {
    router.push({
      name: route.name as string,
      query: {
        ...route.query,
        folder: currentFolderPath.value,
        keyword: searchKey.value,
        provider: provider.value,
        listType: listType.value,
      },
    });
  } else {
    fetchList(pagination.currentPage!);
  }
}

defineExpose({
  selectedFiles,
  getCrumbPath,
  handleClickPath,
  handleClickBreadcrumb,
});
</script>

<style lang="scss" scoped>
.media--in-dialog {
  .media__body {
    padding: 0;
  }
}
</style>
