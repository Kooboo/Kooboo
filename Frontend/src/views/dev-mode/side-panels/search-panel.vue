<script lang="ts">
import { i18n } from "@/modules/i18n";
import { useRoute } from "vue-router";
import { searchDebounce } from "@/utils/url";
import { useCodeStore } from "@/store/code";
import { useShortcut } from "@/hooks/use-shortcuts";
const $t = i18n.global.t;

export const define = {
  name: "code search",
  icon: "icon-search",
  display: $t("common.search"),
  route: "layouts",
  order: 85,
  permission: "code",
  actions: [
    {
      name: "refresh",
      display: $t("common.refresh"),
      icon: "icon-Refresh",
      visible: true,
      async invoke() {
        const activity = useDevModeStore().activeActivity;
        if (!activity) return;
        const instance = await activity.panelInstance.promise;
        instance.onSearch();
      },
    },
  ] as Action[],
} as Activity;
</script>

<script lang="ts" setup>
import type { SearchResult } from "@/api/development/code-search";
import { search as searchApi } from "@/api/development/code-search";
import type { Action, Activity } from "@/store/dev-mode";
import { useDevModeStore } from "@/store/dev-mode";

import type { ElInput } from "element-plus";
import { inject, nextTick, onMounted, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import FileItem from "../components/file-item.vue";
import type { ResolvedRoute } from "@/api/route/types";
import { onFileOpenInjectionFlag } from "@/components/monaco-editor/config";
import { getDesignerPage } from "@/utils/page";
import { openInNewTab } from "@/utils/url";
import { router } from "@/modules/router";

const openFile = inject<(req: ResolvedRoute) => void>(onFileOpenInjectionFlag);

const { t } = useI18n();
const devModeStore = useDevModeStore();
const codeStore = useCodeStore();
const route = useRoute();

const list = ref<SearchResult[]>();
const keyword = ref("");
const inputKeyword = ref("");

const onSearch = async () => {
  keyword.value = inputKeyword.value.trim();
  if (!keyword.value) {
    onClear();
    return;
  }

  if (codeStore.isRegex) {
    try {
      RegExp(keyword.value);
    } catch (error) {
      return;
    }
  }

  list.value = await searchApi(
    keyword.value,
    codeStore.isRegex,
    codeStore.ignoreCase
  );
};

const search = searchDebounce(onSearch, 1000);
watch(
  () => [inputKeyword.value, codeStore.ignoreCase, codeStore.isRegex],
  () => {
    search();
  }
);

const onOpen = async (item: SearchResult, line?: number) => {
  const req: ResolvedRoute = {
    id: item.id,
    name: item.name,
    params: item.params,
    type: item.type,
  };
  if (req.type === "Page" && req.params?.type === 3) {
    const pageRoute = getDesignerPage(req.id, req.params?.layout);
    openInNewTab(router.resolve(pageRoute).href);
    return;
  }
  openFile!(req);
  const tab = await devModeStore.activeTab?.panelInstance?.promise;
  if (!tab?.goToLine) return;
  if (line !== undefined) {
    tab.goToLine(line, keyword.value);
    devModeStore.setActiveSearch(item.id + "-" + String(line));
  } else {
    tab.goToLine(1, keyword.value);
    devModeStore.setActiveSearch(item.id);
  }
};

const onClear = () => {
  keyword.value = "";
  inputKeyword.value = "";
  list.value = undefined;
  inputKeywordEl.value?.focus();
};

const inputKeywordEl = ref<InstanceType<typeof ElInput>>();

const keywordFocus = () => {
  nextTick(() => {
    // 有值选中 无值聚焦
    if (inputKeyword.value) {
      inputKeywordEl.value?.select();
    } else {
      inputKeywordEl.value?.focus();
    }
  });
};

onMounted(keywordFocus);
watch(
  () => route.query.activity,
  (activity) => {
    if (activity === "code search") {
      keywordFocus();
    }
  }
);

defineExpose({ onSearch });

useShortcut("matchCase", () => (codeStore.ignoreCase = !codeStore.ignoreCase));
useShortcut("useRegularExpression", () => {
  codeStore.isRegex = !codeStore.isRegex;
});
</script>

<template>
  <div>
    <div
      class="text-s px-8 py-4 flex items-center bg-gray/30 dark:bg-[#333] space-x-4 dark:( text-fff/86)"
    >
      <el-icon class="iconfont icon-search text-666 dark:( text-fff/86)" />
      <input
        ref="inputKeywordEl"
        v-model="inputKeyword"
        class="outline-none w-full bg-transparent"
        :placeholder="t('common.inputKeyword')"
        data-cy="search"
        @click.stop
      />
      <span
        class="cursor-pointer"
        :class="{ 'text-blue': !codeStore.ignoreCase }"
        :title="t('common.matchCase')"
        @click="codeStore.ignoreCase = !codeStore.ignoreCase"
        >Aa</span
      >
      <span
        class="cursor-pointer"
        :class="{ 'text-blue': codeStore.isRegex }"
        :title="t('common.useRegularExpression')"
        @click="codeStore.isRegex = !codeStore.isRegex"
        >.*</span
      >
      <el-icon
        v-if="inputKeyword"
        class="iconfont icon-delete5 text-666/50 dark:( text-fff/50) cursor-pointer"
        @click="onClear"
      />
    </div>
    <template v-if="list?.length">
      <template v-for="item of list" :key="item.id">
        <FileItem
          :id="item.id"
          :title="item.name"
          ignore-search
          permission="search"
          @click="onOpen(item)"
        >
          <template #collapse>
            <div
              v-for="line of item.matched"
              :key="line.lineNumber + line.summary"
              class="cursor-pointer hover:bg-blue/30 px-8"
              :class="
                devModeStore.activeSearch ===
                item.id + '-' + String(line.lineNumber + 1)
                  ? 'bg-blue/30'
                  : ''
              "
              @click.stop="onOpen(item, line.lineNumber + 1)"
            >
              <p
                class="text-s text-666 ellipsis cursor-default select-none pointer-events-none"
                :title="line.summary"
                data-cy="line-summary"
              >
                {{ line.lineNumber + 1 }}:
                <Highlight
                  :keyword="keyword"
                  :text="line.summary"
                  pattern="all"
                  :start="line.start"
                  :end="line.end"
                />
              </p>
            </div>
          </template>
        </FileItem>
      </template>
    </template>
    <template v-else-if="list?.length === 0">
      <p class="text-s text-999 break-all p-4" data-cy="no-search-result">
        {{ t("common.searchResultEmpty", { keyword: keyword }) }}
      </p>
    </template>
  </div>
</template>
