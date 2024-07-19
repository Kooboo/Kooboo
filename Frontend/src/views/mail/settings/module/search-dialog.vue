<script lang="ts" setup>
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { installMailModule, searchMailModule } from "@/api/mail/mail-module";
import type { MailModule, SearchResult } from "@/api/mail/types";
import { searchDebounce } from "@/utils/url";
import { highLight } from "@/utils/dom";
import { ElMessage } from "element-plus";

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

const props = defineProps<{
  list: MailModule[];
  modelValue: boolean;
}>();

const { t } = useI18n();
const show = ref(true);
const keyword = ref();
const enterKeyword = ref("");
const list = ref<SearchResult[]>();
const searchInputRef = ref();

const searchEvent = async () => {
  list.value = undefined;
  keyword.value = enterKeyword.value.trim();
  if (enterKeyword.value.trim() !== "") {
    list.value = await searchMailModule(keyword.value.trim());
    list.value.forEach((f) => {
      f.installed = disabled(f.title);
    });
  }
};
const onSearch = searchDebounce(searchEvent, 1000);

const onInstall = async (item: SearchResult) => {
  item.loading = true;
  installMailModule(item.packageId, item.title)
    .then(() => {
      item.installed = true;
      emit("reload");
    })
    .catch(() => {
      item.installed = false;
      ElMessage.error(t("common.installationFailed"));
    })
    .finally(() => {
      item.loading = false;
    });
};

const disabled = (item: any) => {
  const arr = props.list.map((item) => {
    return item.name;
  });
  return (arr as any).includes(item);
};

const notFoundHighLight = (enterKeyword: string) => {
  let keyword =
    "<span style='color:rgba(34, 150, 243, 1)'>" + enterKeyword + "</span>";
  return keyword;
};
watch(
  () => enterKeyword.value,
  () => {
    onSearch();
  }
);
watch(
  () => show.value,
  (n) => {
    if (n) {
      setTimeout(() => {
        searchInputRef.value.focus();
      }, 0);
    }
  },
  {
    immediate: true,
  }
);
</script>

<template>
  <el-dialog
    :model-value="show"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.search')"
    custom-class="search_dialog"
    @closed="emit('update:modelValue', false)"
  >
    <SearchInput
      ref="searchInputRef"
      v-model="enterKeyword"
      class="w-536px ml-32px"
      :placeholder="t('common.searchKeywordTip')"
      data-cy="search"
    />
    <el-scrollbar class="mt-8" max-height="50vh">
      <div
        v-for="item of list"
        :key="item.packageId"
        class="hover:bg-[#EFF6FF] dark:hover:bg-666 px-32"
      >
        <div
          class="flex items-center pt-12 pb-4"
          :title="item.title + '\n' + item.description"
        >
          <div class="flex-1 text-l font-bold ellipsis">
            <p
              class="ellipsis h-auto"
              data-cy="module-name"
              v-html="highLight(item.title, enterKeyword)"
            />
            <p
              class="font-light text-m text-999 p-4 ellipsis"
              data-cy="module-description"
              v-html="highLight(item.description, enterKeyword)"
            />
          </div>
          <el-button
            type="primary"
            size="small"
            :disabled="item.loading || item.installed"
            :loading="item.loading"
            round
            data-cy="install"
            @click="onInstall(item)"
            >{{ item.installed ? t("common.hasInstall") : t("common.install") }}
          </el-button>
        </div>
        <el-divider class="m-0" />
      </div>
      <div
        v-if="list && keyword && list.length == 0"
        class="px-32 leading-4 text-center mt-8 whitespace-normal"
        data-cy="no-search-result"
        v-html="
          t('common.searchResultEmpty', {
            keyword: notFoundHighLight(keyword),
          })
        "
      />
    </el-scrollbar>
  </el-dialog>
</template>
<style>
.search_dialog .el-dialog__body {
  padding-left: 0;
  padding-right: 0;
}
</style>
