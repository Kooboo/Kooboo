<script lang="ts" setup>
import ActionBar from "./action-bar.vue";
import type PageTree from "./page-tree.vue";
import PageTreeVue from "./page-tree.vue";
import KTable from "@/components/k-table";
import type { Page, NodeGroup, SearchParam } from "@/api/pages/types";
import { SearchType, Operator } from "@/api/pages/types";
import { ref, onUnmounted } from "vue";
import { useTime } from "@/hooks/use-date";
import CopyDialog from "./copy-dialog.vue";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { useRouter } from "vue-router";
import { emptyGuid } from "@/utils/guid";
import RelationsTag from "@/components/relations/relations-tag.vue";
import IconButton from "@/components/basic/icon-button.vue";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { usePageStore } from "@/store/page";
import { useLayoutStore } from "@/store/layout";
import { combineUrl } from "@/utils/url";
import Cookies from "universal-cookie";
import { useI18n } from "vue-i18n";
import { useSiteStore } from "@/store/site";
import { getDefaultLanguage } from "@/modules/i18n";
import { showDeleteConfirm } from "@/components/basic/confirm";
import { usePreviewUrl } from "@/hooks/use-preview-url";
import {
  getPageTree,
  getSiteRunningTask,
  getTaskStatus,
  searchPageTree,
} from "@/api/pages";
import { ElMessage } from "element-plus";
import type { ElPopover } from "element-plus";

const { t } = useI18n();
const router = useRouter();
const pageStore = usePageStore();
const siteStore = useSiteStore();
const cookies = new Cookies();

const { onPreview } = usePreviewUrl();

const onDelete = async (rows: Page[]) => {
  await showDeleteConfirm(rows.length);
  await pageStore.deletePages(rows.map((m) => m.id));
};

const showCopyDialog = ref(false);
const copyPage = ref<Page>();

const onCopy = (page: Page) => {
  if (siteStore.hasAccess("pages", "edit")) {
    showCopyDialog.value = true;
    copyPage.value = page;
  }
};

const getDesignUrl = (page: Page) => {
  let path = router.resolve(
    useRouteSiteId({
      name: "inline-design",
      query: {
        path: page.path,
        access_token: cookies.get("jwt_token"),
        inline_design_lang: getDefaultLanguage(),
        scheme: localStorage.getItem("vueuse-color-scheme"),
        type: page.type,
        id: page.id,
      },
    })
  ).href;

  if (import.meta.env.PROD) {
    path = combineUrl(siteStore.site.baseUrl, path);
  }

  return path;
};

async function openDesignPage(page: Page) {
  if (!siteStore.hasAccess("pages", "edit")) return;
  onPreview(getDesignUrl(page), {
    title: t("common.inlineEditor"),
  });
}

function getEditPage(row: Page) {
  const query: Record<string, string> = {
    id: row.id,
  };
  if (row.layoutId !== emptyGuid) {
    query["layoutId"] = row.layoutId;
  }

  const data: Record<string, any> = {
    query,
  };

  if (row.type === "Designer") {
    data["name"] = query["layoutId"] ? "layout-page-design" : "page-design";
  } else {
    data["name"] = query["layoutId"] ? "layout-page-edit" : "page-edit";
  }

  return data;
}
const popover = ref<typeof ElPopover>();
const pageTree = ref<NodeGroup>();
const isShowPageTree = ref(false);

const getDefaultQuery = () => [
  {
    searchType: SearchType.TagName,
    operator: Operator.Equal,
    value: "",
  },
  {
    searchType: SearchType.Id,
    operator: Operator.Equal,
    value: "",
  },
  {
    searchType: SearchType.Class,
    operator: Operator.Equal,
    value: "",
  },
];

const query = ref<SearchParam[]>(getDefaultQuery());

const tempQuery = ref<SearchParam[]>(getDefaultQuery());

function restore() {
  query.value.forEach((_, index) => {
    tempQuery.value[index] = { ...query.value[index] };
  });
}

function removeQueryItem(index: number) {
  tempQuery.value.splice(index, 1);
}

function addQueryItem() {
  tempQuery.value.push({
    searchType: SearchType.TagName,
    operator: Operator.Equal,
    value: "",
  });
}

const pageTreeTask = ref({
  id: "",
  currentStatus: 0,
  isFinish: true,
  totalPages: 0,
});

async function refreshTask(taskId?: string) {
  if (taskId) {
    pageTreeTask.value.id = taskId;
    pageTreeTask.value.isFinish = false;
  }

  // 如果不存在任务
  if (!pageTreeTask.value.id) {
    pageTreeTask.value.totalPages = 0;
    pageTreeTask.value.currentStatus = 0;
    pageTreeTask.value.isFinish = true;
    return;
  }

  const { totalPages, currentStatus, isFinish } = await getTaskStatus(
    pageTreeTask.value.id
  );

  // 完成提醒
  if (isFinish) {
    ElMessage.success(t("common.TaskExecutionCompleted"));
    pageTreeTask.value.id = "";
    pageTreeTask.value.totalPages = 0;
    pageTreeTask.value.currentStatus = 0;
    pageTreeTask.value.isFinish = true;
    await loadPageTreeData();
    return;
  }

  pageTreeTask.value.totalPages = totalPages;
  pageTreeTask.value.currentStatus = currentStatus;
  pageTreeTask.value.isFinish = isFinish;
}

const timer = ref();

const includeScript = ref(false);
const searchKeyword = ref("");

function advancedSearch() {
  if (tempQuery.value.some((i) => i.value.trim())) {
    popover.value?.hide();

    tempQuery.value.forEach((_, index) => {
      query.value[index] = { ...tempQuery.value[index] };
    });

    searchKeyword.value = "";

    loadPageTreeData();
  } else {
    ElMessage.warning(t("page.pleaseFillValueTips"));
  }
}

function resetAdvancedSearch() {
  query.value = getDefaultQuery();
  tempQuery.value = getDefaultQuery();

  popover.value?.hide();

  loadPageTreeData();
}

async function loadPageTreeData() {
  // 至少有一个筛选条件
  if (query.value.some((i) => i.value.trim())) {
    const data = await searchPageTree(includeScript.value, query.value);
    pageTree.value = data;
  } else {
    const data = await getPageTree(includeScript.value);
    pageTree.value = data;
  }
}

function onChangeIncludeScript() {
  console.log(includeScript.value);
  includeScript.value = !includeScript.value;
  loadPageTreeData();
}

async function onShowPageTree() {
  isShowPageTree.value = !isShowPageTree.value;
  searchKeyword.value = "";
  if (isShowPageTree.value) {
    // 每三秒扫描一次任务状态
    timer.value = setInterval(async () => {
      if (!pageTreeTask.value.isFinish) {
        refreshTask();
      }
    }, 3000);

    // 进入后立刻查询是否有任务在执行
    getSiteRunningTask().then((task) => {
      if (task && task.id) {
        pageTreeTask.value.id = task.id;
        pageTreeTask.value.isFinish = task.isFinish;
        pageTreeTask.value.totalPages = task.totalPage;
        pageTreeTask.value.currentStatus = task.currentStatus;
      }
    });

    if (pageTreeTask.value.isFinish) {
      loadPageTreeData();
    }
  } else {
    pageTree.value = void 0;
    clearInterval(timer.value);
  }
}

const pageTreeRef = ref<typeof PageTree>();

pageStore.load();
useLayoutStore().load();

onUnmounted(() => {
  clearInterval(timer.value);
});
</script>

<template>
  <div class="p-24">
    <Breadcrumb :name="t('common.pages')" />
    <ActionBar :pages="pageStore.list" />

    <!-- 页面列表 -->
    <KTable
      v-if="!isShowPageTree"
      :data="pageStore.list"
      show-check
      :permission="{ feature: 'pages', action: 'delete' }"
      @delete="onDelete"
    >
      <template #bar="{ selected }">
        <IconButton
          v-if="selected.length === 1"
          :permission="{ feature: 'pages', action: 'edit' }"
          icon="icon-copy"
          :tip="t('common.copy')"
          circle
          data-cy="copy"
          @click="onCopy(selected[0])"
        />
      </template>

      <template #rightBar>
        <el-radio-group
          :model-value="isShowPageTree"
          class="el-radio-group--rounded ml-12px"
          @update:model-value="onShowPageTree"
        >
          <el-radio-button :label="false" data-cy="page">{{
            t("common.page")
          }}</el-radio-button>
          <el-radio-button :label="true" data-cy="pageTree">{{
            t("common.pageTree")
          }}</el-radio-button>
        </el-radio-group>
      </template>

      <el-table-column
        :label="t('common.name')"
        :label-class-name="
          pageStore.list.find((f) => f.startPage) ? 'ml-30px' : ''
        "
      >
        <template #default="{ row }">
          <div class="flex items-center justify-center">
            <el-icon
              v-if="pageStore.list.find((f) => f.startPage)"
              class="w-30px"
              :class="row.startPage ? 'iconfont icon-home2 ' : ''"
            />
            <span
              class="text-l font-bold flex-1 ellipsis"
              :title="row.name"
              data-cy="name"
            >
              {{ row.name }}
            </span>
          </div>
        </template>
      </el-table-column>
      <el-table-column
        prop="linked"
        :label="t('common.linked')"
        width="80px"
        align="center"
      >
        <template #default="{ row }">
          <span v-if="row.linked" class="text-blue" data-cy="linked">{{
            row.linked
          }}</span>
          <span v-else class="text-card-disabled" data-cy="linked">{{
            row.linked
          }}</span>
        </template>
      </el-table-column>
      <el-table-column
        prop="online"
        :label="t('page.online')"
        align="center"
        width="80px"
      >
        <template #default="{ row }">
          <span
            v-if="row.online && siteStore.site.published"
            class="text-green"
            data-cy="online"
          >
            {{ t("common.online") }}
          </span>
          <span v-else data-cy="online" class="text-card-disabled">
            {{ t("common.offline") }}
          </span>
        </template>
      </el-table-column>
      <el-table-column prop="relations" :label="t('common.references')">
        <template #default="{ row }">
          <RelationsTag :id="row.id" :relations="row.relations" type="Page" />
        </template>
      </el-table-column>
      <el-table-column
        prop="lastModified"
        :label="t('common.lastModified')"
        align="center"
      >
        <template #default="{ row }">{{ useTime(row.lastModified) }}</template>
      </el-table-column>
      <el-table-column :label="t('common.preview')">
        <template #default="{ row }">
          <div class="text-blue ellipsis">
            <a
              href="javascript:;"
              :title="row.previewUrl"
              data-cy="preview"
              @click.prevent="onPreview(row.previewUrl)"
            >
              {{ row.path }}
            </a>
          </div>
        </template>
      </el-table-column>
      <el-table-column width="120" align="right">
        <template #default="{ row }">
          <router-link
            v-if="row.type !== 'Designer'"
            :to="
              useRouteSiteId({
                name:
                  row.type === 'RichText'
                    ? 'rich-page-edit'
                    : row.layoutId === emptyGuid
                    ? 'page-setting'
                    : 'layout-page-setting',
                query: {
                  id: row.id,
                  layoutId:
                    row.layoutId === emptyGuid ? undefined : row.layoutId,
                },
              })
            "
            data-cy="setting"
            class="mx-8"
          >
            <el-tooltip placement="top" :content="t('common.setting')">
              <el-icon class="iconfont icon-a-setup hover:text-blue text-l" />
            </el-tooltip>
          </router-link>

          <router-link
            v-if="row.type !== 'RichText'"
            :to="useRouteSiteId(getEditPage(row))"
            data-cy="edit-code"
            class="mx-8"
          >
            <el-tooltip
              v-if="row.type === 'Designer'"
              placement="top"
              :content="t('common.design')"
            >
              <el-icon class="iconfont icon-code hover:text-blue text-l" />
            </el-tooltip>
            <el-tooltip v-else placement="top" :content="t('common.editCode')">
              <el-icon class="iconfont icon-code hover:text-blue text-l" />
            </el-tooltip>
          </router-link>

          <a
            v-if="siteStore.hasAccess('pages', 'edit')"
            target="_blank"
            :href="getDesignUrl(row)"
            class="mx-8"
          >
            <el-tooltip placement="top" :content="t('common.inlineEditor')">
              <el-icon
                class="iconfont icon-a-writein2 hover:text-blue text-l"
                data-cy="inline-editor"
                @click.prevent="openDesignPage(row)"
              />
            </el-tooltip>
          </a>

          <IconButton
            v-else
            :permission="{ feature: 'pages', action: 'edit' }"
            icon="icon-a-writein2"
            :tip="t('common.inlineEditor')"
            data-cy="inline-editor"
            @click.prevent="openDesignPage(row)"
          />
        </template>
      </el-table-column>
    </KTable>

    <!-- 
      页面树
      高度限制：100vh - (header breadcrumb action-bar padding-y-24)
     -->
    <div
      v-if="isShowPageTree"
      class="shadow-s-10 rounded-normal overflow-hidden flex flex-col bg-fff dark:bg-[#303030]"
      style="height: calc(100vh - 214px)"
    >
      <div
        class="h-72px px-15px flex items-center bg-fff flex-shrink-0 dark:bg-[#303030]"
        data-cy="table-check-all"
      >
        <SearchInput
          v-model="searchKeyword"
          :placeholder="t('common.search')"
          class="w-250px h-10"
          @update:model-value="(keyword:string) => pageTreeRef?.filter(keyword)"
        />

        <el-popover
          ref="popover"
          placement="bottom-start"
          :width="600"
          trigger="click"
          @before-enter="restore"
        >
          <template #reference>
            <el-button
              round
              class="ml-12px"
              :type="query.some((i) => i.value.trim()) ? 'primary' : ''"
            >
              {{ t("common.advancedSearch") }}
            </el-button>
          </template>

          <el-form @submit.prevent>
            <el-scrollbar max-height="50vh" class="pr-12px">
              <div v-for="(item, index) in tempQuery" :key="item.searchType">
                <div class="flex items-center gap-8px">
                  <el-form-item class="flex-shrink-0 w-140px">
                    <el-select
                      v-model="item.searchType"
                      filterable
                      allow-create
                      :teleported="false"
                    >
                      <el-option
                        v-for="item in SearchType"
                        :key="item"
                        :label="item"
                        :value="item"
                      />
                    </el-select>
                  </el-form-item>

                  <el-form-item class="flex-shrink-0 w-140px">
                    <el-select v-model="item.operator" :teleported="false">
                      <el-option
                        v-for="operator in Operator"
                        :key="operator"
                        :label="operator"
                        :value="operator"
                      />
                    </el-select>
                  </el-form-item>

                  <el-form-item class="flex-1">
                    <el-input v-model="item.value" clearable />
                  </el-form-item>

                  <el-form-item>
                    <IconButton
                      :disabled="tempQuery.length < 2"
                      class="!text-orange"
                      icon="icon-delete"
                      :tip="t('common.delete')"
                      circle
                      data-cy="delete"
                      @click.stop="removeQueryItem(index)"
                    />
                  </el-form-item>
                </div>
              </div>

              <el-form-item v-if="tempQuery.length < 10">
                <IconButton
                  icon="icon-a-addto"
                  :tip="t('common.add')"
                  circle
                  data-cy="add"
                  @click.stop="addQueryItem"
                />
              </el-form-item>
            </el-scrollbar>

            <el-form-item>
              <div class="w-full flex items-center justify-end">
                <el-button @click="resetAdvancedSearch">{{
                  t("common.reset")
                }}</el-button>
                <el-button type="primary" @click="advancedSearch">{{
                  t("common.search")
                }}</el-button>
              </div>
            </el-form-item>
          </el-form>
        </el-popover>

        <div class="flex-1" />

        <el-button round class="ml-12px" @click="onChangeIncludeScript">
          <el-checkbox
            :model-value="includeScript"
            :label="t('page.includeScriptTags')"
            class="pointer-events-none"
          />
        </el-button>

        <div
          v-if="pageTreeRef?.selectedKey"
          class="flex items-center ml-12px space-x-12px"
        >
          <IconButton
            icon="icon-view"
            :tip="t('common.convertToType', { type: t('common.view') })"
            :data-cy="t('common.view')"
            circle
            @click.stop="pageTreeRef.convertTo('View')"
          />
          <IconButton
            icon="icon-html"
            :tip="t('common.convertToType', { type: t('common.htmlBlock') })"
            :data-cy="t('common.htmlBlock')"
            circle
            @click.stop="pageTreeRef.convertTo('HtmlBlock')"
          />

          <el-dropdown trigger="hover">
            <div>
              <IconButton
                icon="icon-a-writein"
                :tip="t('common.edit')"
                :data-cy="t('common.edit')"
                circle
              />
            </div>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item @click.stop="pageTreeRef.edit('outerHtml')">{{
                  t("code.editCode", { name: t("page.outerHtml") })
                }}</el-dropdown-item>
                <el-dropdown-item @click.stop="pageTreeRef.edit('innerHtml')">{{
                  t("code.editCode", { name: t("page.inlineHtml") })
                }}</el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>

          <IconButton
            class="!text-orange"
            icon="icon-delete"
            :tip="t('common.delete')"
            circle
            data-cy="delete"
            @click.stop="pageTreeRef.deleteSelected"
          />
        </div>

        <el-radio-group
          :model-value="isShowPageTree"
          class="el-radio-group--rounded ml-12px"
          @update:model-value="onShowPageTree"
        >
          <el-radio-button :label="false" data-cy="page">{{
            t("common.page")
          }}</el-radio-button>
          <el-radio-button :label="true" data-cy="pageTree">{{
            t("common.pageTree")
          }}</el-radio-button>
        </el-radio-group>
      </div>

      <div class="flex-1">
        <PageTreeVue
          v-if="pageTreeTask.isFinish"
          ref="pageTreeRef"
          :tree-data="pageTree!"
          @addTask="refreshTask"
          @reload="loadPageTreeData"
        />

        <div v-else class="flex justify-center items-center h-full">
          <el-result
            :title="t('common.taskIsExecuting')"
            :sub-title="
              t('page.pagesRemaining', {
                total: pageTreeTask.totalPages,
                count: pageTreeTask.currentStatus,
              })
            "
          >
            <template #icon>
              <el-icon
                class="iconfont icon-bufenchenggong text-56px text-green"
              />
            </template>
            <!-- <template #extra>
              <el-button type="primary" @click="refreshTask()">{{
                t("common.refresh")
              }}</el-button>
            </template> -->
          </el-result>
        </div>
      </div>
    </div>

    <CopyDialog
      v-if="showCopyDialog"
      v-model="showCopyDialog"
      :page="copyPage"
    />
  </div>
</template>
