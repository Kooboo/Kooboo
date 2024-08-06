<script lang="ts" setup>
import SearchDialog from "./layout.vue";
import { ref, computed } from "vue";
import { useI18n } from "vue-i18n";
import { useOnlineSearchApi } from "@/api/third-party";
import type {
  SearchRequest,
  SearchProvider,
  SearchPackageFileItem,
} from "@/api/third-party/types";
import { ElMessage } from "element-plus";

import { getQueryString } from "@/utils/url";
import type SearchDialogType from "@/components/search-dialog/index.vue";
import type { OnlineModule } from "@/api/third-party/index";
import type { InfiniteResponse } from "@/global/types";
import { removeAllModules } from "monaco-editor-ex";
import { highLight } from "@/utils/dom";
import TopPanel from "./top-panel.vue";

const props = defineProps<{
  module: OnlineModule;
}>();

const { getSearchProviders, onlineSearch, download, tops } = computed(() =>
  useOnlineSearchApi(props.module)
).value;

const showDialog = ref(false);

const { t } = useI18n();

const emit = defineEmits<{
  (e: "reload"): void;
}>();

const searchDialogRef = ref<InstanceType<typeof SearchDialogType>>();
const topList = ref<any>();
const providers = ref<SearchProvider[]>([]);

const load = async () => {
  providers.value = await getSearchProviders();
};

async function fetchData(
  formData: SearchRequest
): Promise<InfiniteResponse<any>> {
  const data = await onlineSearch(formData);
  return data;
}

function downloadFile(item: SearchPackageFileItem) {
  item.loading = true;
  download(getQueryString("folder") ?? "/", item)
    .then(() => {
      item.installed = true;
      emit("reload");
    })
    .catch(() => {
      item.installed = false;
      ElMessage.error(t("common.installationFailed"));
    })
    .finally(() => {
      removeAllModules();
      item.loading = false;
    });
}

load();

function show() {
  showDialog.value = true;
  topList.value = null;
  (searchDialogRef.value as any)?.reset();
  if (props.module == "module") {
    tops().then((rsp) => {
      topList.value = rsp;
    });
  }
}

defineExpose({
  show,
});
</script>

<template>
  <SearchDialog
    ref="searchDialogRef"
    v-model="showDialog"
    :fetch-data="fetchData"
    :providers="providers"
  >
    <template #placeholder>
      <TopPanel :top="topList" @selected="searchDialogRef.search($event)" />
    </template>
    <template #default="{ list, keyword }">
      <div
        v-for="item of list"
        :key="item.id"
        class="hover:bg-[#EFF6FF] dark:hover:bg-666 px-32"
      >
        <div
          class="flex items-center pt-12 pb-4"
          :title="item.name + '\n' + item.description"
        >
          <div class="flex-1 text-l font-bold ellipsis">
            <p
              class="ellipsis h-auto"
              :data-cy="`${module}-name`"
              v-html="highLight(item.name, keyword)"
            />
            <p
              class="font-light text-m text-999 p-4 ellipsis"
              :data-cy="`${module}-description`"
              v-html="highLight(item.description, keyword)"
            />
          </div>
          <el-button
            v-hasPermission="{ feature: module, action: 'edit' }"
            type="primary"
            size="small"
            round
            data-cy="install"
            :loading="item.loading"
            :disabled="item.loading || item.installed"
            @click="downloadFile(item)"
            >{{ item.installed ? t("common.hasInstall") : t("common.install") }}
          </el-button>
        </div>
        <el-divider class="m-0" />
      </div>
    </template>
  </SearchDialog>
</template>
