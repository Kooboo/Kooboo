<template>
  <table class="table-fixed w-full text-center text-[#666] dark:text-fff/50">
    <thead class="bg-[#fafafa] dark:bg-444 shadow-s-10 relative">
      <tr v-if="canEdit">
        <th width="50" />
        <th class="text-left">{{ t("common.fileName") }}</th>
        <th class="text-left">URL</th>
        <th width="250" class="text-left">
          {{ t("common.usedBy") }}
        </th>
        <th width="100">{{ t("common.size") }}</th>
        <th width="150">{{ t("common.lastModified") }}</th>
        <th :width="hideOpenFolder ? 140 : 160" />
      </tr>
      <tr v-else>
        <th width="50" />
        <th class="text-left">{{ t("common.fileName") }}</th>
        <th v-if="!dialogInfo" class="text-left">URL</th>
        <th width="10" />
        <th width="90">{{ t("common.size") }}</th>
        <th width="160">{{ t("common.lastModified") }}</th>
        <th :width="hideOpenFolder ? 110 : 130" />
      </tr>
    </thead>
    <tbody class="bg-[#fff] dark:bg-[#333] text-m">
      <tr
        v-for="item in folders"
        :key="item.fullPath"
        :class="{ '!bg-blue/30': item.selected && hasCheckbox && !dialogInfo }"
      >
        <td
          :class="dialogInfo ? '' : 'cursor-pointer'"
          @click="item.selected = !item.selected"
        >
          <el-checkbox
            v-if="hasCheckbox && !dialogInfo"
            v-model="item.selected"
            class="pointer-events-none"
            data-cy="folder-checkbox"
          />
        </td>
        <td>
          <div
            class="text-left flex items-center cursor-pointer min-h-48px"
            @click="handleClickFolder(item)"
          >
            <img src="@/assets/images/folder.svg" class="w-28px mr-8px" />
            <div
              class="ellipsis text-black dark:text-fff/50 pr-12"
              :title="item.name"
              data-cy="folder-name"
            >
              {{ item.name }}
            </div>
          </div>
        </td>
        <td :class="dialogInfo ? 'hidden' : ''" />
        <td class="text-left" />
        <td>-</td>
        <td class="text-m">{{ useTime(item.lastModified) }}</td>
        <td>
          <div class="flex items-center justify-end cursor-pointer px-16">
            <el-tooltip
              placement="top"
              :show-after="300"
              :content="t('common.download')"
            >
              <span
                class="iconfont icon-xiazai-wenjianxiazai-05 hover:text-blue font-bold"
                @click="handleDownloadFolder(item.fullPath)"
              />
            </el-tooltip>
          </div>
        </td>
      </tr>
      <tr
        v-for="item in files"
        :key="item.id"
        :class="{ '!bg-blue/30': item.selected }"
      >
        <td class="cursor-pointer">
          <el-checkbox
            v-if="hasCheckbox"
            v-model="item.selected"
            data-cy="file-checkbox"
            @change="handleCheckFile(item, files)"
          />
        </td>
        <td>
          <div class="text-left flex items-center min-h-48px">
            <img
              v-if="item.thumbnail"
              :src="item.thumbnail"
              class="w-30px h-26px mr-8px object-contain"
              data-cy="thumbnail"
            />
            <div
              class="ellipsis text-black dark:text-fff/50 pr-12"
              data-cy="file-name"
              :title="item.name"
            >
              {{ item.name }}
            </div>
          </div>
        </td>
        <td v-if="!dialogInfo">
          <div class="text-left flex items-center">
            <div
              class="ellipsis text-black dark:text-fff/50 pr-12"
              data-cy="file-previewUrl"
              :title="item.previewUrl"
            >
              {{ combineUrl(siteStore.site.baseUrl, item.url) }}
            </div>
          </div>
        </td>
        <td v-else />
        <td class="py-8" :class="dialogInfo ? 'hidden' : ''">
          <RelationsTag
            :id="item.id"
            :relations="item.references??item.relations as Record<string, number>"
            :type="item.references ? 'Image' : 'CmsFile'"
          />
        </td>
        <td data-cy="size">{{ bytesToSize(item.size) }}</td>
        <td class="text-m" data-cy="last-modified-in-list">
          {{ useTime(item.lastModified) }}
        </td>
        <td v-if="canEdit">
          <div class="inline-flex items-center cursor-pointer space-x-16">
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
              placement="top"
              :show-after="300"
              :content="t('common.copyUrl')"
            >
              <span
                class="iconfont icon-copy cursor-pointer hover:text-blue"
                data-cy="copy-in-grid"
                @click="copyText(combineUrl(siteStore.site.baseUrl, item.url))"
              />
            </el-tooltip>

            <el-tooltip
              placement="top"
              :show-after="300"
              :content="t('common.preview')"
            >
              <span
                class="hover:text-blue iconfont icon-eyes"
                data-cy="preview-in-list"
                @click="handlePreviewFile(item)"
              />
            </el-tooltip>

            <el-tooltip
              v-if="siteStore.hasAccess('mediaLibrary', 'edit')"
              placement="top"
              :show-after="300"
              :content="t('common.edit')"
            >
              <span
                class="hover:text-blue iconfont icon-a-writein"
                data-cy="edit-in-list"
                @click="handleEditImage(item, provider)"
              />
            </el-tooltip>

            <el-tooltip
              placement="top"
              :show-after="300"
              :content="t('common.download')"
            >
              <span
                class="hover:text-blue iconfont icon-xiazai-wenjianxiazai-05 font-bold"
                data-cy="download-in-list"
                @click="handleDownloadFile(item)"
              />
            </el-tooltip>
          </div>
        </td>
        <td v-else>
          <div
            class="flex items-center cursor-pointer space-x-16 justify-end px-16"
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
              placement="top"
              :show-after="300"
              :content="t('common.copyUrl')"
            >
              <span
                class="iconfont icon-copy hover:text-blue"
                @click="copyText(combineUrl(siteStore.site.baseUrl, item.url))"
              />
            </el-tooltip>

            <el-tooltip
              placement="top"
              :show-after="300"
              :content="t('common.preview')"
            >
              <span
                class="iconfont icon-eyes hover:text-blue"
                @click="handlePreviewFile(item)"
              />
            </el-tooltip>
            <el-tooltip
              placement="top"
              :show-after="300"
              :content="t('common.download')"
            >
              <span
                class="iconfont icon-xiazai-wenjianxiazai-05 hover:text-blue font-bold"
                @click="handleDownloadFile(item)"
              />
            </el-tooltip>
          </div>
        </td>
      </tr>
      <tr v-if="isEmpty" class="isEmpty">
        <td :colspan="!dialogInfo ? 7 : 6">
          <div class="el-table__empty-block w-full">
            <span class="el-table__empty-text">{{ t("common.noData") }}</span>
          </div>
        </td>
      </tr>
    </tbody>
  </table>

  <el-image-viewer
    v-if="showImageViewer"
    :url-list="imageList"
    hide-on-click-modal
    @close="showImageViewer = false"
  />
</template>
<script lang="ts" setup>
import { bytesToSize } from "@/utils/common";
import { useTime } from "@/hooks/use-date";
import { useMedia } from "./use-media";
import type { MediaFileItem, MediaFolderItem } from "./";
import { computed, ref } from "vue";
import RelationsTag from "@/components/relations/relations-tag.vue";
import { useI18n } from "vue-i18n";
import { openInNewTab, combineUrl } from "@/utils/url";
import { useSiteStore } from "@/store/site";
import { copyText } from "@/hooks/use-copy-text";
import { useUrlSiteId } from "@/hooks/use-site-id";
import { downloadFiles } from "@/api/content/file";
import { downloadMediaFile } from "@/api/content/media";
import type { DownloadRequest } from "@/api/content/download-request-type";

const props = defineProps<{
  folders: MediaFolderItem[];
  files: MediaFileItem[];
  hasCheckbox: boolean;
  hideOpenFolder?: boolean;
  folderType?: "File";
  provider: string;
}>();
interface ListEmits {
  (e: "clickFolder", value: MediaFolderItem): void;
  (e: "editImage", file: MediaFileItem, provider: string): void;
}

const showImageViewer = ref(false);
const imageList = ref<string[]>([]);
const siteStore = useSiteStore();
let type = props.folderType ? "file" : "media";

const handlePreviewFile = (item: MediaFileItem) => {
  if (item.mimeType?.split("/")[0] !== "image") {
    openInNewTab(item.previewUrl);
  } else {
    imageList.value = [item.previewUrl];
    showImageViewer.value = true;
  }
};

// 单文件下载
const handleDownloadFile = async (item: MediaFileItem) => {
  if (item.downloadUrl) {
    openInNewTab(item.downloadUrl);
  } else {
    const url = useUrlSiteId(
      `Download/Single?id=${item.key}&type=${type}&provider=${props.provider}`
    );
    openInNewTab(`${import.meta.env.VITE_API}/${url}`);
  }
};

// 单文件夹下载
const handleDownloadFolder = async (folder: string) => {
  const downloadRequest: DownloadRequest = {
    folder: folder,
    folders: [folder],
    files: [],
  };
  const file = props.folderType
    ? await downloadFiles(downloadRequest, props.provider)
    : await downloadMediaFile(downloadRequest, props.provider);
  if (!file) {
    return;
  }
  const url = useUrlSiteId(`Download/Package?file=${file}&type=${type}`);
  openInNewTab(`${import.meta.env.VITE_API}/${url}`);
};

const emits = defineEmits<ListEmits>();
const { t } = useI18n();

const { handleClickFolder, handleEditImage, dialogInfo, handleCheckFile } =
  useMedia(emits);

const openFolder = (item: MediaFileItem) => {
  if (!item["folder"]) {
    const url = item.url;
    item["folder"] = url.substring(0, url.lastIndexOf("/")) || "";
  }
  const folder: MediaFolderItem = {
    fullPath: `${item["folder"] || "/"}`,
    name: "",
    lastModified: new Date(),
    count: 1,
  };
  handleClickFolder(folder);
};

const canEdit = computed(
  () => !dialogInfo?.value && props.folderType !== "File"
);

const isEmpty = computed(
  () => props.folders.length === 0 && props.files.length === 0
);
</script>
<style lang="scss" scoped>
:deep(.el-checkbox) {
  vertical-align: middle;
}
th {
  font-weight: normal;
  padding: 16px 0;
  vertical-align: middle;
  font-size: 14px;
}
tbody {
  tr {
    @apply border-solid border-b-1px border-b-[#EBEEF5] dark:border-444 hover:(bg-blue/10);
  }
  .isEmpty {
    @apply hover: bg-transparent;
  }
  td {
    vertical-align: middle;
  }
}
</style>
