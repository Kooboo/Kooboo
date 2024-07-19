<template>
  <div class="media">
    <div v-if="!dialogInfo" class="flex items-center px-24 relative">
      <Breadcrumb
        :crumb-path="getCrumbPath"
        @handle-click-path="handleClickBreadcrumb"
      />
    </div>

    <div class="p-24 media__body">
      <div class="flex items-center justify-between mb-16 media__head">
        <div class="flex items-center space-x-24">
          <el-checkbox
            v-if="!dialogInfo"
            v-model="selectAll"
            :indeterminate="isIndeterminate"
            size="large"
            data-cy="check-all"
            >{{ t("common.selectAllFiles") }}</el-checkbox
          >
          <KUpload
            :permission="{
              feature: 'file',
              action: 'edit',
            }"
            :disabled="!!searchKey"
            :multiple="true"
            :action="uploadUrl"
            :before-upload="handleBeforeUpload"
            :on-success="uploadFinish"
            :data="{ folder: currentFolderPath }"
          />
          <el-button
            v-if="!dialogInfo"
            v-hasPermission="{
              feature: 'file',
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
            v-if="totalSelectedItems > 0 && !dialogInfo"
            v-hasPermission="{
              feature: 'file',
              action: 'view',
            }"
            round
            data-cy="download"
            @click="handleDownload"
            ><el-icon class="iconfont icon-xiazai-wenjianxiazai-05" />
            {{ t("common.download") }}
          </el-button>
          <el-button
            v-if="totalSelectedItems > 0 && !dialogInfo"
            v-hasPermission="{
              feature: 'file',
              action: 'delete',
            }"
            round
            class="text-orange"
            data-cy="delete"
            @click="handleDelete"
          >
            <el-icon class="iconfont icon-delete" />{{ t("common.delete") }}
          </el-button>
        </div>

        <div>
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
        v-if="!dialogInfo"
        class="text-s mb-12 text-666 flex items-center space-x-4"
      >
        <el-icon class="iconfont icon-Tips" />
        <div>
          {{
            t(
              "common.thePicturesAndIconsDisplayedOnYourWebsiteAreSuggestedToBePutInThe"
            )
          }}
          <span
            class="text-blue cursor-pointer"
            @click="
              router.push(
                useRouteSiteId({
                  name: 'media',
                })
              )
            "
            >{{ t("common.mediaLibrary") }}</span
          >
          {{ t("common.forManagement") }}
        </div>
      </div>

      <div class="rounded-normal overflow-hidden shadow-s-10">
        <MediaList
          :folders="folders"
          :files="files"
          :has-checkbox="true"
          :hide-open-folder="!searchKey"
          folder-type="File"
          :provider="provider"
          @click-folder="handleClickFolder"
        />

        <KPagination
          v-if="files.length || folders.length"
          class="dark:bg-[#1e1e1e] py-24"
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
      folder-type="File"
      @create-success="refreshList"
    />
  </div>
</template>

<script lang="ts" setup>
import type { FileList, CrumbPathItem } from "@/api/content/file";
import { downloadFiles, isUniqueKey } from "@/api/content/file";
import {
  getList,
  getUploadActionUrl,
  deleteFolders,
  deleteFiles,
} from "@/api/content/file";
import type {
  DialogInfo,
  MediaFileItem,
  MediaFolderItem,
} from "@/components/k-media-dialog";

import type { ComputedRef } from "vue";
import { computed, inject, onMounted, ref, watch, reactive } from "vue";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import CreateFolderDialog from "@/components/k-media-dialog/create-folder-dialog.vue";
import MediaList from "@/components/k-media-dialog/media-list.vue";
import { useRoute, useRouter } from "vue-router";
import { showFileExistsConfirm, showDeleteConfirm } from "../basic/confirm";
import { errorMessage, uploadMessage } from "../basic/message";
import { openInNewTab, combineUrl } from "@/utils/url";
import { useI18n } from "vue-i18n";
import type { Pagination } from "../k-table/types";
import { ElLoading } from "element-plus";
import { useRouteSiteId, useUrlSiteId } from "@/hooks/use-site-id";
import type { DownloadRequest } from "@/api/content/download-request-type";
import SearchProvider from "../k-media-dialog/search-provider.vue";
import KPagination from "../k-pagination/index.vue";
import { last, uniqBy, trim } from "lodash-es";

let loadingInstance: { close: () => void } | undefined;
const { t } = useI18n();
const dialogInfo = inject<ComputedRef<DialogInfo>>("dialogInfo");
const router = useRouter();
const route = useRoute();

const selectAll = ref(false);
const isIndeterminate = ref(false);
const folders = ref<MediaFolderItem[]>([]);
const files = ref<MediaFileItem[]>([]);
const infinite = ref(false);
const currentFolderPath = ref("/");
const pagination = reactive<Pagination>({
  currentPage: 1,
  pageSize: 50,
  totalCount: 0,
});

const visibleCreateFolderDialog = ref(false);
const searchKey = ref("");
const provider = ref<string>((route.query.provider as string) || "default");
const providers = ref<string[]>([]);
const uploadUrl = computed(() => getUploadActionUrl(provider.value));
type SearchData = {
  keyword: string;
  providers: string[];
  data: FileList;
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
const crumbPath = ref<CrumbPathItem[]>([]);
const getCrumbPath = computed(() => {
  // 修改根路径名
  return crumbPath.value.map((item) => {
    if (item.name === "searchResult") {
      item.name = t("common.searchResult");
    } else if (item.name === "root" && !dialogInfo) {
      item.name = t("common.files");
    }
    return item;
  });
});

function showLoading() {
  if (!loadingInstance) {
    loadingInstance = ElLoading.service({ background: "rgba(0, 0, 0, 0.5)" });
  }
}

function hideLoading() {
  loadingInstance?.close();
  loadingInstance = undefined;
}

onMounted(() => {
  if (!dialogInfo) {
    watch(
      () => route.query,
      (query) => {
        if (route.name === "files") {
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

async function fetchList(currentPage: number) {
  const response = await getList({
    pageNr: currentPage,
    pageSize: pagination.pageSize!,
    path: currentFolderPath.value,
    keyword: searchKey.value?.trim(),
    provider: provider.value,
    startAfter: currentPage > 1 ? last(files.value)?.key : "",
  });
  afterFetchList(response, currentPage);
}
function afterFetchList(response: FileList, currentPage: number) {
  providers.value = response.providers ?? [];
  crumbPath.value = response.crumbPath;
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
  infinite.value = response.files.infinite;
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
    it.references = it.references || it.relations || {};
    return it;
  });
  files.value =
    !infinite.value || currentPage <= 1
      ? fileList
      : [...files.value, ...fileList];

  pagination.totalCount = response.files.totalCount;
  pagination.currentPage = currentPage;
  pagination.pageSize = response.files.pageSize;

  emit("loaded", {
    keyword: searchKey.value,
    data: response,
    providers: providers.value,
    infinite: infinite.value,
  });
}
function handlePageChange(currentPage: number) {
  pagination.currentPage = currentPage;
  fetchList(currentPage);
}
function handleClickFolder(item: MediaFolderItem | CrumbPathItem) {
  currentFolderPath.value = item.fullPath!;
  pagination.currentPage = 1;
  searchKey.value = "";
  files.value = [];
  folders.value = [];
  updateRoute();
}
function uploadFinish() {
  uploadMessage();
  resetList();
  hideLoading();
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
      },
    });
  } else {
    fetchList(pagination.currentPage!);
  }
}

function refreshList() {
  updateRoute();
  fetchList(pagination.currentPage!);
}
function handleNewFolder() {
  visibleCreateFolderDialog.value = true;
}

function resetList() {
  pagination.currentPage = 1;
  refreshList();
}

async function handleDelete() {
  try {
    await showDeleteConfirm(
      selectedFiles.value.length + selectedFolders.value.length
    );
    if (selectedFolders.value.length > 0) {
      await deleteFolders(
        selectedFolders.value.map((item) => item.fullPath),
        provider.value
      );
    }
    if (selectedFiles.value.length > 0) {
      await deleteFiles(
        selectedFiles.value.map((item) => item.key),
        provider.value
      );
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
  const downloadRequest: DownloadRequest = {
    folder: currentFolderPath.value,
    folders,
    files,
  };
  const file = await downloadFiles(downloadRequest, provider.value);
  if (!file) {
    return;
  }
  const url = useUrlSiteId(`Download/Package?file=${file}&type=file`);
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

const maxFileSize = 150;
async function handleBeforeUpload(file: { name: string; size: any }) {
  if (file.size > 1024 * 1024 * maxFileSize) {
    errorMessage(
      t("common.maxUploadFileSizeOutOfLimit", {
        file: file.name,
        size: maxFileSize,
      }),
      true
    );
    return false;
  }

  const fileUrl = combineUrl(currentFolderPath.value, file.name);
  const isValidName = await isUniqueKey(provider.value, fileUrl)
    .then(() => true)
    .catch(() => false);
  if (!isValidName) {
    await showFileExistsConfirm();
  }

  showLoading();
}
function onProviderChanged() {
  handleClickPath({
    name: "root",
    fullPath: "/",
  });
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
