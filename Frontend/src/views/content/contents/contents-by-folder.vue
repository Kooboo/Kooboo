<template>
  <div class="p-24">
    <Breadcrumb
      :crumb-path="[
        {
          name: t('common.contentFolders'),
          route: { name: 'contents' },
        },
        { name: friendlyName },
      ]"
    />
    <div class="flex items-center py-24 justify-between">
      <el-button
        v-hasPermission="{
          feature: 'content',
          action: 'edit',
        }"
        round
        :title="t('common.new') + ' ' + friendlyName"
        data-cy="new-text-content"
        @click="edit()"
      >
        <el-icon class="iconfont icon-a-addto" />
        {{ t("common.new") }}
        <span
          class="max-w-300px overflow-x-clip whitespace-nowrap overflow-ellipsis"
        >
          {{ friendlyName }}</span
        >
      </el-button>
      <div class="space-x-16 flex items-center">
        <el-select
          v-for="f in categoryOptions"
          :key="f.id"
          v-model="searchCategories[f.id]"
          clearable
          multiple
          :placeholder="f.display ?? f.alias"
          @change="() => fetchList()"
        >
          <el-option
            v-for="o in f.options"
            :key="o.id"
            :label="getText(o, f)"
            :value="o.id"
          />
        </el-select>
        <SearchInput
          v-model="keywords"
          :placeholder="t('common.searchContents')"
          class="w-238px"
          clearable
          data-cy="search"
        />
        <router-link
          v-if="siteStore.hasAccess('contentType', 'edit')"
          :to="useRouteSiteId({
          name: 'contenttype',
          query: {
            id: folder?.contentTypeId,
            fromRouter: route.name as string,
            fromFolder: folderId,
          },
        })"
        >
          <el-tooltip
            class="box-item"
            effect="dark"
            :content="t('common.editContentType')"
            placement="top"
          >
            <el-button
              circle
              data-cy="edit-content-type"
              @click="gotoContentType"
            >
              <el-icon class="iconfont icon-a-setup" />
            </el-button>
          </el-tooltip>
        </router-link>
        <el-tooltip
          v-else
          class="box-item"
          effect="dark"
          :content="t('common.editContentType')"
          placement="top"
        >
          <el-button
            v-hasPermission="{ feature: 'contentType', action: 'edit' }"
            circle
            data-cy="edit-content-type"
          >
            <el-icon class="iconfont icon-a-setup" />
          </el-button>
        </el-tooltip>
      </div>
    </div>
    <KTable
      v-if="columnLoaded"
      :data="list"
      show-check
      :draggable="draggable"
      row-key="id"
      :pagination="pagination"
      :permission="{
        feature: 'content',
        action: 'delete',
      }"
      :sort="sortSetting.prop ?? undefined"
      :order="sortSetting.order ?? undefined"
      @delete="onDelete"
      @change="fetchList($event)"
      @sorted="onSort"
      @sort-change="onSortChanged"
    >
      <template #bar="{ selected }">
        <IconButton
          v-if="selected.length === 1"
          :permission="{ feature: 'content', action: 'edit' }"
          circle
          class="text-[#192845] !hover:text-blue"
          icon="icon-copy"
          :tip="t('common.copy')"
          data-cy="copy"
          @click="onCopy(selected[0])"
        />
        <IconButton
          v-if="draggable && selected.length === 1"
          :permission="{ feature: 'content', action: 'edit' }"
          circle
          class="text-[#192845] !hover:text-blue"
          icon="icon-move"
          :tip="t('common.move')"
          data-cy="move"
          @click="onMove(selected[0])"
        />
      </template>
      <DynamicColumns :columns="listColumns" />
      <el-table-column :label="t('common.usedBy')" width="180">
        <template #default="{ row }">
          <template v-if="row.usedBy">
            <el-tooltip
              v-for="usage in row.usedBy"
              :key="usage.folderId"
              class="box-item"
              effect="dark"
              :content="usedByTooltip(usage)"
              placement="top"
            >
              <el-tag
                class="cursor-pointer"
                round
                size="small"
                :title="usage.displayName || usage.folderName"
                @click="onShowUsedBy(usage)"
              >
                {{ usage.displayName || usage.folderName }}
              </el-tag>
            </el-tooltip>
          </template>
        </template>
      </el-table-column>
      <el-table-column
        prop="online"
        :sortable="fieldSortable"
        :label="t('content.online')"
        width="120"
        align="center"
      >
        <template #default="{ row }">
          <span
            :class="row.online ? 'text-green' : 'text-999'"
            data-cy="online"
            >{{ row.online ? t("common.yes") : t("common.no") }}</span
          >
        </template>
      </el-table-column>
      <el-table-column
        :label="t('common.lastModified')"
        width="180"
        align="center"
        prop="lastModified"
        :sortable="fieldSortable"
      >
        <template #default="{ row }">
          {{ useTime(row.lastModified) }}
        </template>
      </el-table-column>
      <el-table-column align="right" :width="showSort ? 80 : 50">
        <template #default="{ row }">
          <div class="flex">
            <router-link
              class="mx-8 cursor-pointer hover:text-blue"
              :to="
                useRouteSiteId({
                  name: 'content',
                  query: {
                    folder: folderId,
                    id: row?.id,
                    copy: undefined,
                  },
                })
              "
              data-cy="edit"
            >
              <el-tooltip
                class="box-item"
                effect="dark"
                :content="t('common.edit')"
                placement="top"
              >
                <el-icon class="iconfont icon-a-writein" />
              </el-tooltip>
            </router-link>
            <IconButton
              v-if="showSort"
              icon="icon-move js-sortable cursor-move"
              :tip="t('common.move')"
              data-cy="move"
            />
          </div>
        </template>
      </el-table-column>
    </KTable>
    <UsedByDialog ref="usedByDialogRef" />
    <MoveContentDialog
      v-if="folder"
      ref="moveRef"
      :folder="folder"
      @reload="fetchList"
    />
  </div>
</template>

<script lang="ts" setup>
import KTable from "@/components/k-table";
import { computed, onMounted, ref } from "vue";
import type { TextContentUsedBy } from "@/api/content/textContent";
import { deletes } from "@/api/content/textContent";
import type { ContentFolder } from "@/api/content/content-folder";
import { getFolderInfoById } from "@/api/content/content-folder";
import { useTime } from "@/hooks/use-date";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { getQueryString } from "@/utils/url";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import SearchInput from "@/components/basic/search-input.vue";
import { showDeleteConfirm } from "@/components/basic/confirm";
import { useRoute, useRouter } from "vue-router";
import { useI18n } from "vue-i18n";
import { useSiteStore } from "@/store/site";
import UsedByDialog from "@/components/relations/used-by-dialog.vue";
import type UsedByDialogType from "@/components/relations/used-by-dialog.vue";
import type { SortableEvent } from "sortablejs";
import { moveContent } from "@/api/content/textContent";
import type { TableRowItem } from "./types";
import { useContentEffects } from "./content-effect";
import type MoveContentDialogType from "./components/move-content-dialog.vue";
import MoveContentDialog from "./components/move-content-dialog.vue";
import type { MoveTextContent } from "@/api/content/textContent";
import DynamicColumns, {
  type SummaryColumn,
} from "@/components/dynamic-columns/index.vue";

const { t } = useI18n();
const router = useRouter();
const folderId = getQueryString("folder") as string;

const {
  list,
  columns,
  keywords,
  currentKeyword,
  columnLoaded,
  pagination,
  fetchList,
  sortSetting,
  searchCategories,
  categoryOptions,
  onSortChanged,
} = useContentEffects(folderId);

const listColumns = computed<SummaryColumn[]>(() =>
  columns.value.map((it) => {
    const column: SummaryColumn = {
      name: it.name,
      displayName: it.displayName,
      controlType: it.controlType,
      multipleValue: it.multipleValue,
      attrs: {
        sortable: fieldSortable.value,
        "min-width": 180,
      },
    };

    return column;
  })
);

const folder = ref<ContentFolder>();
const moveRef = ref<InstanceType<typeof MoveContentDialogType>>();

const route = useRoute();
const siteStore = useSiteStore();
const usedByDialogRef = ref<InstanceType<typeof UsedByDialogType>>();

onMounted(async () => {
  await fetchFolderInfo();
  fetchList(1, folder.value?.pageSize);
});

const isDragAndDrop = computed(
  () => folder.value?.sortable && folder.value?.sortField === "dragAndDrop"
);

const draggable = computed(() => {
  return isDragAndDrop.value && list.value?.length > 1;
});
const showSort = computed(() => draggable.value && !currentKeyword.value);
const fieldSortable = computed(() =>
  isDragAndDrop.value ? undefined : "custom"
);

const friendlyName = computed(
  () => folder.value?.displayName || folder.value?.name || ""
);

async function fetchFolderInfo() {
  folder.value = await getFolderInfoById({ id: folderId });
}

async function onDelete(rows: TableRowItem[]) {
  await showDeleteConfirm(rows.length);
  const ids = rows.map((item) => item.id);
  await deletes({ ids });
  fetchList(pagination.currentPage);
}
function gotoContentType() {
  if (siteStore.hasAccess("contentType", "edit")) {
    router.push(
      useRouteSiteId({
        name: "contenttype",
        query: {
          id: folder.value?.contentTypeId,
          fromRouter: route.name as string,
          fromFolder: folderId,
        },
      })
    );
  }
}
function edit(row?: TableRowItem, copy?: boolean) {
  router.push(
    useRouteSiteId({
      name: "content",
      query: { folder: folderId, id: row?.id, copy: copy ? "true" : undefined },
    })
  );
}
function onCopy(row: TableRowItem) {
  if (!siteStore.hasAccess("content", "edit")) return;
  edit(row, true);
}

function usedByTooltip(usage: TextContentUsedBy) {
  switch (usage.type) {
    case 1:
      return t("common.embeddedFolder");
    case 2:
      return t("common.categoryFolder");
    default:
      return t("common.unknown");
  }
}
function onShowUsedBy(usage: TextContentUsedBy) {
  (usedByDialogRef.value as any)?.show(usage);
}

function onMove(row: TableRowItem) {
  (moveRef.value as any)?.show(row);
}

async function onSort(sortedData: any[], e: SortableEvent) {
  const { newIndex, oldIndex } = e;
  if (
    newIndex === undefined ||
    oldIndex === undefined ||
    oldIndex === newIndex
  ) {
    return;
  }

  const target: TableRowItem = sortedData[newIndex];

  // Order by Descending, so prev is newIndex+1, next is newIndex-1
  const prev: TableRowItem | undefined = sortedData[newIndex + 1];
  const next: TableRowItem | undefined = sortedData[newIndex - 1];
  const req: MoveTextContent = {
    source: target.id,
    folderId,
    prevId: prev?.id,
    nextId: next?.id,
  };
  await moveContent(req);
  await fetchList();
}

function getText(content: any, category: any) {
  let key = content.summaryField ?? Object.keys(content.textValues)[0];
  key =
    Object.keys(content.textValues).find(
      (f) => f.toLowerCase() == key.toLowerCase()
    ) ?? "";
  let value = content.textValues[key];

  const column = category.columns.find(
    (f: any) => f.name?.toLowerCase() == key?.toLowerCase()
  );
  if (column?.selectionOptions) {
    try {
      const options = JSON.parse(column.selectionOptions);
      let values = [value];
      if (column.controlType == "CheckBox") {
        values = JSON.parse(value);
      }
      const displayValues = [];
      for (const i of values) {
        const option = options.find((f: any) => f.value == i);
        if (option) displayValues.push(option.key);
        else displayValues.push(i);
      }
      value = displayValues.join(",");
    } catch {
      //
    }
  }
  return value;
}
</script>
