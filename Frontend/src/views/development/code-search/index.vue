<script lang="ts" setup>
import KTable from "@/components/k-table";
import type { SearchResult } from "@/api/development/code-search";
import { search as codeSearch } from "@/api/development/code-search";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { computed, onMounted, ref, watch, toRaw } from "vue";
import SearchInput from "@/components/basic/search-input.vue";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { useI18n } from "vue-i18n";
import { useSiteStore } from "@/store/site";
import { openInNewTab } from "@/utils/url";
import { useRouter } from "vue-router";
import { searchDebounce } from "@/utils/url";
import { useCodeStore } from "@/store/code";
import { onBeforeRouteLeave } from "vue-router";
import { useShortcut } from "@/hooks/use-shortcuts";

const { t } = useI18n();
const list = ref<SearchResult[]>([]);
const codeStore = useCodeStore();
const siteStore = useSiteStore();
const codeTypes: Record<string, string> = {
  Page: "pages",
  HtmlBlock: "htmlBlock",
  TextContent: "content",
};
const router = useRouter();

const getPermissionName = computed(() => (name: string) => {
  if (Object.getOwnPropertyNames(codeTypes).includes(name)) {
    return codeTypes[name];
  } else {
    return name.toLowerCase();
  }
});

async function searchEvent() {
  const keyword = codeStore.keywords?.trim();
  if (codeStore.isRegex) {
    try {
      RegExp(keyword);
    } catch (error) {
      return;
    }
  }
  if (!keyword) {
    list.value = [];
    return;
  }
  list.value = await codeSearch(
    keyword,
    codeStore.isRegex,
    codeStore.ignoreCase
  );
}
onMounted(async () => {
  list.value = await codeSearch(
    codeStore.keywords,
    codeStore.isRegex,
    codeStore.ignoreCase
  );
});
onBeforeRouteLeave((to) => {
  if (!to.query.id) {
    codeStore.keywords = "";
  }
});

function getEditRoute(row: SearchResult) {
  const routeNames: Record<string, string> = {
    Page: "page-edit",
    View: "view-edit",
    Style: "style-edit",
    Script: "script-edit",
    Code: "code-edit",
    Layout: "layout-edit",
    HtmlBlock: "htmlBlock-edit",
    Form: "form-edit",
    TextContent: "content",
  };
  const layout = row.params.layout;
  // TODO: Page types & Code types
  let editRoute = routeNames[row.type];
  let layoutId: string | undefined;
  let folder: string | undefined;
  if (row.type === "Page") {
    switch (row.params.type) {
      case 3:
        layoutId = layout;
        editRoute = layout ? "layout-page-design" : "page-design";
        break;
      case 2:
        editRoute = "rich-page-edit";
        break;
      case 1:
        editRoute = "layout-page-edit";
        layoutId = layout;
        break;
    }
  }
  // else if (row.type === "TextContent") {
  //   folder = row.folderId;
  // }
  return {
    name: editRoute,
    query: { id: row.id, layoutId, folder },
  };
}

const goEdit = (row: SearchResult) => {
  let path = router.resolve(useRouteSiteId(getEditRoute(row))).href;
  openInNewTab(path);
};
const search = searchDebounce(searchEvent, 1000);
watch(
  () => [codeStore.keywords, codeStore.isRegex, codeStore.ignoreCase],
  () => {
    search();
  }
);

useShortcut("matchCase", () => (codeStore.ignoreCase = !codeStore.ignoreCase));
useShortcut("useRegularExpression", () => {
  codeStore.isRegex = !codeStore.isRegex;
});
</script>
<template>
  <div class="p-24">
    <Breadcrumb :name="t('common.codeSearch')" />
    <div class="flex items-center py-24">
      <div class="relative">
        <SearchInput
          v-model="codeStore.keywords"
          :clearable="false"
          class="w-320px"
          :placeholder="t('common.searchCode')"
          data-cy="search"
        >
          <span
            class="cursor-pointer w-24"
            :class="{ 'text-blue': !codeStore.ignoreCase }"
            :title="t('common.matchCase')"
            @click="codeStore.ignoreCase = !codeStore.ignoreCase"
            >Aa</span
          >
          <span
            class="cursor-pointer w-24"
            :title="t('common.useRegularExpression')"
            :class="{ 'text-blue': codeStore.isRegex }"
            @click="codeStore.isRegex = !codeStore.isRegex"
            >.*</span
          >
        </SearchInput>
      </div>
    </div>
    <KTable :data="list">
      <el-table-column width="10" />
      <el-table-column :label="t('common.name')" prop="name" width="300">
        <template #default="{ row }">
          <span data-cy="name">{{ row.name }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.type')" width="150">
        <template #default="{ row }">
          <span class="text-blue" data-cy="type">{{ row.type }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.line')">
        <template #default="{ row }">
          <div
            v-for="(line, index) in row.matched"
            :key="index"
            class="flex flex-wrap"
            data-cy="code-line"
          >
            <span class="flex-none mr-4"
              >{{ t("common.line") }} {{ line.lineNumber + 1 }}:</span
            >
            {{ line.summary }}
          </div>
          {{ row.line }}
        </template>
      </el-table-column>

      <el-table-column width="100" align="right">
        <template #default="{ row }">
          <IconButton
            v-if="siteStore.hasAccess(getPermissionName(row.type))"
            :permission="{
              feature: getPermissionName(row.type),
              action: 'view',
            }"
            icon="icon-a-writein"
            class="hover:text-blue cursor-pointer"
            :tip="t('common.edit')"
            data-cy="edit"
            @click="goEdit(row)"
          />
          <IconButton
            v-else
            :permission="{
              feature: getPermissionName(row.type),
              action: 'view',
            }"
            icon="icon-a-writein"
            class="hover:text-blue cursor-pointer"
            :tip="t('common.edit')"
            data-cy="edit"
          />
        </template>
      </el-table-column>
    </KTable>
  </div>
</template>
