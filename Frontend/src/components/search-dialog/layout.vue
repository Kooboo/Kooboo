<script lang="ts" setup>
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { searchDebounce, openInNewTab } from "@/utils/url";
import type { SearchHandler } from "./types";
import type { SearchRequest, SearchProvider } from "@/api/third-party/types";
import type { ElScrollbar } from "element-plus";

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "list-changed", value: any[]): void;
}>();

const props = defineProps<{
  modelValue: boolean;
  providers: SearchProvider[];
  fetchData: SearchHandler;
}>();

const loading = ref(false);
const hasMore = ref(false);

const { t } = useI18n();

const form = ref<SearchRequest>({
  provider: "",
  keyword: "",
  pageNr: 1,
});

const list = ref<any[]>([]);

const scrollbarRef = ref<InstanceType<typeof ElScrollbar>>();

watch(
  () => props.providers,
  () => {
    form.value.provider = props.providers[0]?.value;
  }
);

watch(
  () => list.value,
  (data) => emit("list-changed", data)
);

const searchEvent = async () => {
  if (form.value.keyword.trim() == "") return;
  loading.value = true;
  const searchResult = await props.fetchData(form.value);
  loading.value = false;
  hasMore.value = searchResult.hasMore;
  if (form.value.pageNr <= 1) {
    list.value = searchResult.list;
    (scrollbarRef?.value as any)?.scrollTo(0, 0);
  } else {
    list.value = [...list.value, ...searchResult.list];
  }
};
const onSearch = searchDebounce(searchEvent, 1000);

function loadMore() {
  if (!form.value.keyword || loading.value) {
    return;
  }
  form.value.pageNr++;
  searchEvent();
}
function onResetSearch() {
  form.value.pageNr = 1;
  hasMore.value = true;
  loading.value = true;
  form.value.keyword && onSearch();
}

function reset() {
  form.value.pageNr = 1;
  form.value.provider = props.providers[0]?.value;
  hasMore.value = true;
}

function search(keyword: string) {
  form.value.keyword = keyword;
  searchEvent();
}

function onClosed() {
  form.value.keyword = "";
  form.value.provider = props.providers[0]?.value;
  form.value.pageNr = 1;
  list.value = [];
  emit("update:modelValue", false);
}
const notFoundHighLight = (enterKeyword: string) => {
  let keyword =
    "<span style='color:rgba(34, 150, 243, 1)'>" + enterKeyword + "</span>";
  return keyword;
};

defineExpose({
  loadMore,
  reset,
  search,
});
const searchInputRef = ref();
watch(
  () => props.modelValue,
  (n) => {
    if (n) {
      setTimeout(() => {
        searchInputRef.value.focus();
      }, 0);
    }
  }
);
</script>

<template>
  <el-dialog
    :model-value="modelValue"
    :width="props.providers.length > 1 ? '800px' : '600px'"
    :close-on-click-modal="false"
    :title="t('common.search')"
    custom-class="search_dialog"
    @closed="onClosed"
    @update:model-value="(v) => emit('update:modelValue', v)"
  >
    <div class="mx-32 flex justify-between space-x-16">
      <el-tooltip
        v-if="props.providers.length > 1"
        style="flex: 1"
        placement="top"
        :show-after="300"
        :content="t('common.selectSourceTips')"
      >
        <el-select v-model="form.provider" @change="onResetSearch">
          <el-option
            v-for="{ value, label, url } in providers"
            :key="value"
            :value="value"
            :label="label"
            :data-cy="value"
          >
            <div class="flex justify-between">
              <div>{{ label }}</div>
              <div>
                <el-icon
                  v-if="url"
                  class="text-blue iconfont icon-link1"
                  @click.stop="openInNewTab(url)"
                />
              </div>
            </div>
          </el-option>
        </el-select>
      </el-tooltip>

      <div style="flex: 3">
        <SearchInput
          ref="searchInputRef"
          v-model="form.keyword"
          class="w-full"
          :placeholder="t('common.searchKeywordTip')"
          data-cy="search"
          @input="onResetSearch"
          @search="onSearch"
        />
      </div>
    </div>
    <slot v-if="!list || !list.length" name="placeholder" />
    <el-scrollbar
      ref="scrollbarRef"
      class="mt-8"
      max-height="45vh"
      style="height: 100%"
    >
      <div
        v-infinite-scroll="loadMore"
        :infinite-scroll-disabled="!hasMore"
        :infinite-scroll-distance="50"
      >
        <slot
          v-if="list && list.length > 0"
          :list="list"
          :keyword="form.keyword"
        />
        <div
          v-if="!loading && list && form.keyword && list.length == 0"
          class="px-32 leading-4 text-center mt-8 whitespace-normal"
          data-cy="no-search-result"
          v-html="
            t('common.searchResultEmpty', {
              keyword: notFoundHighLight(form.keyword),
            })
          "
        />
      </div>
    </el-scrollbar>
  </el-dialog>
</template>
<style>
.search_dialog .el-dialog__body {
  padding-left: 0;
  padding-right: 0;
}
</style>
