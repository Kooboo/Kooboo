<template>
  <div>
    <div v-if="folders.length" class="flex items-center flex-wrap">
      <div
        v-for="item in folders"
        :key="item.fullPath"
        class="folder-item bg-fff dark:bg-444 dark:text-fff/50 rounded-4px shadow-l-10 w-354px h-147px mr-32 mb-24 flex flex-col justify-between cursor-pointer"
        @click="handleClickFolder(item)"
      >
        <div class="flex flex-1 items-center justify-between px-24">
          <div
            class="flex items-center cursor-pointer min-w-0"
            data-cy="folder"
          >
            <img src="@/assets/images/folder.svg" class="w-48px mr-16" />
            <div class="text-m overflow-hidden">
              <div
                class="font-bold ellipsis"
                :title="item.name"
                data-cy="folder-name"
              >
                {{ item.name }}
              </div>
              <div
                class="text-444 dark:text-fff/60 mt-1px"
                data-cy="total-files"
              >
                {{
                  t(
                    "common.totalFiles",
                    { n: item.count > 99 ? "99+" : item.count },
                    item.count
                  )
                }}
              </div>
            </div>
          </div>
          <div v-if="!dialogInfo" class="flex-none mb-16">
            <el-checkbox
              v-if="hasCheckbox"
              v-model="item.selected"
              size="large"
              class="hidden rounded-full text-0px !h-24"
              :class="{ '!block': item.selected }"
              data-cy="checkbox"
              @click.stop
            />
          </div>
        </div>
        <div
          class="h-38px px-24 text-666 dark:text-fff/50 flex items-center justify-between border-solid border-t-1 border-black/6 dark:border-fff/10"
          data-cy="last-modified-in-grid"
        >
          <span class="text-s">
            {{ t("common.lastModified") }}: {{ useTime(item.lastModified) }}
          </span>
          <el-tooltip
            placement="top"
            :show-after="300"
            :content="t('common.download')"
          >
            <span
              class="iconfont icon-xiazai-wenjianxiazai-05 cursor-pointer hover:text-blue font-bold pr-6px hidden text-14px"
              data-cy="copy-in-grid"
              :class="{ '!block': item.selected }"
              @click.stop="downloadFolder(item.fullPath)"
            />
          </el-tooltip>
        </div>
      </div>
    </div>
    <div v-if="loaded">
      <div
        class="grid gap-2"
        :class="gridCol === 4 ? 'grid-cols-4' : 'grid-cols-6'"
      >
        <div
          v-for="item in files"
          :key="item.id"
          v-loading="item.loading"
          class="file-item relative rounded-4px w-full h-161px px-32 pt-32 mb-12 hover:bg-[#E9EAF0] dark:hover:bg-444 flex flex-col"
          :class="{ 'bg-[#E9EAF0] dark:bg-444': item.selected }"
        >
          <slot name="checkbox" :item="item">
            <el-checkbox
              v-if="hasCheckbox"
              v-model="item.selected"
              size="large"
              class="hidden absolute top-8 left-8"
              :class="{ '!block': item.selected }"
              @click.stop
              @change="handleCheckFile(item, files)"
            />
          </slot>
          <div class="hidden absolute top-8 right-8">
            <div
              class="h-20px flex items-center space-x-8 dark:text-fff/50 text-14px text-666"
            >
              <el-tooltip
                v-if="!hideOpenFolder"
                placement="top"
                :show-after="300"
                :content="t('common.openFileLocation', item)"
              >
                <span
                  class="iconfont icon-folder cursor-pointer hover:text-blue"
                  data-cy="open-folder"
                  @click.stop="openFolder(item)"
                />
              </el-tooltip>
              <el-tooltip
                v-if="actions.includes('copy')"
                placement="top"
                :show-after="300"
                :content="t('common.copyUrl')"
              >
                <span
                  class="iconfont icon-copy cursor-pointer hover:text-blue"
                  data-cy="copy-in-grid"
                  @click.stop="
                    copyText(combineUrl(siteStore.site.baseUrl, item.url))
                  "
                />
              </el-tooltip>

              <el-tooltip
                v-if="actions.includes('preview')"
                placement="top"
                :show-after="300"
                :content="t('common.preview')"
              >
                <span
                  class="iconfont icon-eyes cursor-pointer hover:text-blue"
                  data-cy="preview-in-grid"
                  @click.stop="handlePreviewImage(item)"
                />
              </el-tooltip>
              <el-tooltip
                v-if="
                  siteStore.hasAccess('mediaLibrary', 'edit') &&
                  actions.includes('edit')
                "
                placement="top"
                :show-after="300"
                :content="t('common.edit')"
              >
                <span
                  class="iconfont icon-a-writein cursor-pointer hover:text-blue"
                  data-cy="edit-in-grid"
                  @click.stop="handleEditImage(item, provider)"
                />
              </el-tooltip>
              <el-tooltip
                v-if="actions.includes('download')"
                placement="top"
                :show-after="300"
                :content="t('common.download')"
              >
                <span
                  class="iconfont icon-xiazai-wenjianxiazai-05 cursor-pointer hover:text-blue font-bold"
                  data-cy="download-in-grid"
                  @click.stop="downloadFile(item)"
                />
              </el-tooltip>
              <slot name="actions" :item="item" />
            </div>
          </div>

          <div class="w-full flex-1 rounded-4px overflow-hidden">
            <slot name="thumbnail" :item="item">
              <img
                v-if="item.thumbnail"
                class="select-none w-full h-full object-contain dark:text-fff/60"
                :src="item.thumbnail"
                data-cy="thumbnail"
                :alt="item.alt || item.name"
                :title="
                  t('common.imageTitle', {
                    ...item,
                    size: bytesToSize(item.size),
                  })
                "
              />
            </slot>
          </div>

          <div
            class="ellipsis text-s text-444 dark:text-fff/60 h-32 leading-32px text-center"
            data-cy="file-name"
          >
            <slot name="file-name" :item="item">
              {{ item.name }}
            </slot>
          </div>
        </div>
      </div>
      <div v-if="isEmpty" class="el-table__empty-block w-full">
        <span class="el-table__empty-text">{{ t("common.noData") }}</span>
      </div>

      <el-image-viewer
        v-if="showImageViewer"
        :url-list="imageList"
        hide-on-click-modal
        @close="showImageViewer = false"
      />
    </div>
  </div>
</template>
<script lang="ts" setup>
import { useTime } from "@/hooks/use-date";
import { useMedia } from "./use-media";
import type { MediaFileItem, MediaFolderItem } from "./";
import { useI18n } from "vue-i18n";
import { computed, ref, onMounted } from "vue";
import { useSiteStore } from "@/store/site";
import type { Pagination } from "../k-table/types";
import { copyText } from "@/hooks/use-copy-text";
import { openInNewTab, combineUrl } from "@/utils/url";
import { useUrlSiteId } from "@/hooks/use-site-id";
import { downloadMediaFile } from "@/api/content/media";
import type { DownloadRequest } from "@/api/content/download-request-type";
import { bytesToSize } from "@/utils/common";

type MediaFileAction = "copy" | "preview" | "edit" | "download";

const props = defineProps<{
  folders: MediaFolderItem[];
  files: MediaFileItem[];
  hasCheckbox: boolean;
  gridCol?: number;
  hideOpenFolder?: boolean;
  pagination?: Pagination;
  fileActions?: MediaFileAction[];
  provider: string;
}>();

interface ListEmits {
  (e: "clickFolder", value: MediaFolderItem): void;
  (e: "editImage", file: MediaFileItem, provider: string): void;
  (e: "changePage", value: number): void;
  (e: "loadMore"): void;
}

const loaded = ref(false);
onMounted(() => {
  // why: https://juejin.cn/post/7095925947412512804
  loaded.value = true;
});

const emits = defineEmits<ListEmits>();
const { t } = useI18n();
const siteStore = useSiteStore();
const isEmpty = computed(
  () => props.folders.length === 0 && props.files.length === 0
);
const actions = computed(() => {
  if (!props.fileActions) {
    return ["openFolder", "copy", "preview", "edit", "download"];
  }
  return props.fileActions;
});
const showImageViewer = ref(false);
const imageList = ref<string[]>([]);

const handlePreviewImage = (item: MediaFileItem) => {
  imageList.value = [getPreviewUrl(item.url)];
  showImageViewer.value = true;
};
// 单文件夹下载
const downloadFolder = async (folder: string) => {
  const downloadRequest: DownloadRequest = {
    folder: folder,
    folders: [folder],
    files: [],
  };
  const file = await downloadMediaFile(downloadRequest, props.provider);
  if (!file) {
    return;
  }
  const url = useUrlSiteId(`Download/Package?file=${file}&type=media`);
  openInNewTab(`${import.meta.env.VITE_API}/${url}`);
};

// 单文件下载
const downloadFile = (item: MediaFileItem) => {
  if (item.downloadUrl) {
    openInNewTab(item.downloadUrl);
  } else {
    const url = useUrlSiteId(
      `Download/Single?id=${item.key}&type=media&provider=${props.provider}`
    );
    openInNewTab(`${import.meta.env.VITE_API}/${url}`);
  }
};

const openFolder = (item: MediaFileItem) => {
  if (!item["folder"]) {
    console.warn("folder is empty");
    return;
  }
  const folder: MediaFolderItem = {
    fullPath: `${item["folder"] || "/"}`,
    name: "",
    lastModified: new Date(),
    count: 1,
  };
  handleClickFolder(folder);
};

const {
  handleClickFolder,
  handleEditImage,
  dialogInfo,
  handleCheckFile,
  getPreviewUrl,
} = useMedia(emits);
</script>
<style lang="scss" scoped>
.folder-item,
.file-item {
  &:hover {
    .hidden {
      display: block;
      &.inline-flex {
        display: inline-flex;
      }
    }
  }
}
</style>
