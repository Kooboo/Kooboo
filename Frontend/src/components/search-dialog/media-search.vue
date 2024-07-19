<script lang="ts" setup>
import SearchDialogLayout from "./layout.vue";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import {
  getSearchProviders,
  onlineSearch,
  download,
} from "@/api/third-party/media";
import type { SearchRequest, SearchProvider } from "@/api/third-party/types";
import type {
  SearchMediaFileItem,
  DownloadItem,
} from "@/api/third-party/media";

import { getQueryString } from "@/utils/url";
import type SearchDialogType from "@/components/search-dialog/index.vue";
import { showConfirm } from "@/k-components/utils";
import type { InfiniteResponse } from "@/global/types";

const showDialog = ref(false);

const { t } = useI18n();

const props = defineProps<{
  provider: string; // storage provider
}>();

const emit = defineEmits<{
  (e: "reload"): void;
}>();

const searchDialogRef = ref<InstanceType<typeof SearchDialogType>>();

const providers = ref<SearchProvider[]>([]);

const mediaList = ref<SearchMediaFileItem[]>([]);

const load = async () => {
  providers.value = await getSearchProviders();
};

async function fetchData(
  formData: SearchRequest
): Promise<InfiniteResponse<any>> {
  const data = await onlineSearch(formData);
  return data;
}

function listChanged(list: SearchMediaFileItem[]) {
  mediaList.value = list;
}

function downloadFile(item: SearchMediaFileItem) {
  item.loading = true;
  const file: DownloadItem = {
    id: item.id,
    name: item.name,
    url: item.url,
    alt: item.alt,
  };
  item.status = undefined;
  return download(getQueryString("folder") ?? "/", file, props.provider)
    .then(() => {
      item.status = true;
      emit("reload");
    })
    .catch(() => {
      item.status = false;
    })
    .finally(() => {
      item.selected = true;
      item.loading = false;
    });
}

load();

function show() {
  showDialog.value = true;
  (searchDialogRef.value as any)?.reset();
}

async function beforeClose(done: () => void) {
  if (mediaList.value.some((it) => it.loading === true)) {
    await showConfirm(t("common.downloadingImageTip"), {
      showCancelButton: false,
    });
    return;
  }
  done();
}

defineExpose({
  show,
});
</script>

<template>
  <SearchDialogLayout
    ref="searchDialogRef"
    v-model="showDialog"
    :fetch-data="fetchData"
    :providers="providers"
    :before-close="beforeClose"
    @list-changed="listChanged"
  >
    <template #default="{ list }">
      <MediaGrid
        class="mr-12"
        :folders="[]"
        :files="list || []"
        :has-checkbox="false"
        :hide-open-folder="true"
        :grid-col="4"
        :file-actions="['copy', 'preview']"
        :provider="provider"
      >
        <template #actions="{ item }">
          <el-tooltip
            v-if="item.status !== true"
            placement="top"
            :show-after="300"
            :content="t('common.downloadToMediaLibrary')"
          >
            <span
              class="iconfont icon-xiazai-wenjianxiazai-05 cursor-pointer hover:text-blue font-bold"
              data-cy="download-in-search"
              @click.stop="downloadFile(item)"
            />
          </el-tooltip>
        </template>
        <template #checkbox="{ item }">
          <el-tooltip
            v-if="item.status === true"
            placement="top"
            :show-after="300"
            :content="t('common.downloaded')"
          >
            <span
              class="absolute top-8 left-8 iconfont text-green icon-fasongchenggong"
              data-cy="downloaded"
            />
          </el-tooltip>
          <el-tooltip
            v-if="item.status === false"
            placement="top"
            :show-after="300"
            :content="t('common.downloadFailed')"
          >
            <span
              class="absolute top-8 left-8 iconfont text-orange icon-fasongshibai"
              data-cy="download-failed"
            />
          </el-tooltip>
        </template>
      </MediaGrid>
    </template>
  </SearchDialogLayout>
</template>
