<template>
  <div class="p-24">
    <Breadcrumb :name="t('common.contentFolders')" />
    <div class="flex items-center py-24 space-x-16">
      <el-dropdown
        trigger="click"
        class="ml-10px"
        @command="$event == 'multi-content' ? newFolder() : newContent()"
      >
        <el-button
          v-hasPermission="{
            feature: 'contentType',
            action: 'edit',
          }"
          round
          data-cy="new-folder"
        >
          <el-icon class="iconfont icon-a-addto" />
          <span>{{ t("common.new") }}</span>
          <el-icon class="iconfont icon-pull-down text-12px ml-8 !mr-0" />
        </el-button>
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item
              command="multi-content"
              :disabled="!siteStore.hasAccess('contentType', 'edit')"
              :title="
                !siteStore.hasAccess('contentType', 'edit')
                  ? t('common.noPermission')
                  : ''
              "
              data-cy="multi-content"
            >
              <span>{{ t("common.folder") }}</span>
            </el-dropdown-item>
            <el-dropdown-item
              command="single-content"
              :disabled="!siteStore.hasAccess('contentType', 'edit')"
              :title="
                !siteStore.hasAccess('contentType', 'edit')
                  ? t('common.noPermission')
                  : ''
              "
              data-cy="single-content"
            >
              <span>{{ t("common.singleContent") }}</span>
            </el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>

      <div class="flex-1" />
      <div
        v-if="list.some((f) => f.hidden)"
        class="flex items-center text-s space-x-4 text-999"
      >
        <div>{{ t("common.showAll") }}</div>
        <el-switch v-model="showHidden" />
      </div>
    </div>

    <KTable
      v-model:selectedData="selectedData"
      :data="computedList"
      show-check
      custom-check
      :permission="{
        feature: 'contentType',
        action: 'edit',
      }"
      draggable=".sortable-elements"
      row-key="id"
      :row-class-name="({ row }) => (row.isChildren ? `` : 'sortable-elements')"
      @delete="onDelete"
      @sorted="onSorted"
    >
      <el-table-column :label="t('common.name')">
        <template #default="{ row }">
          <span v-if="row.isFolder">{{ row.displayName }}</span>
          <div v-else class="inline-flex items-center space-x-4 gap-8">
            <el-checkbox
              size="large"
              :model-value="selectedData.some((f) => f === row)"
              data-cy="checkbox-label"
              @change="
                () => {
                  if (selectedData.includes(row)) {
                    selectedData = selectedData.filter((f) => f !== row);
                  } else {
                    selectedData.push(row);
                  }
                }
              "
            />
            <router-link
              class="ellipsis text-blue cursor-pointer"
              :to="
                useRouteSiteId({
                  name: row.isContent ? 'content' : 'textcontentsbyfolder',
                  query: {
                    folder: row.id,
                    id: row.defaultContentId,
                    isContent: row.isContent,
                  },
                })
              "
              data-cy="name"
              >{{ row.displayName || row.name }}</router-link
            >
            <el-icon
              v-if="row.hidden"
              class="text-20px iconfont icon-yincang-px- text-999/60"
            />
          </div>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.usedBy')">
        <template #default="{ row }">
          <template v-if="!row.isFolder">
            <RelationsTag
              :id="row.id"
              :relations="row.relations"
              type="ContentFolder"
            />
          </template>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.type')" width="140" align="center">
        <template #default="{ row }">
          <template v-if="!row.isFolder">
            <el-tag v-if="row.isContent" type="success" round>{{
              t("common.singleContent")
            }}</el-tag>
            <el-tag v-else type="warning" round>{{
              t("common.folder")
            }}</el-tag>
          </template>
        </template>
      </el-table-column>

      <el-table-column width="150" align="right">
        <template #default="{ row }">
          <template v-if="!row.isFolder">
            <IconButton
              v-if="row.type !== 'RichText'"
              :permission="{
                feature: 'contentType',
                action: 'edit',
              }"
              icon="icon-a-setup"
              :tip="t('common.folderSetting')"
              data-cy="setting"
              @click="newFolder(row)"
            />

            <router-link
              v-if="siteStore.hasAccess('contentType', 'edit')"
              :to="useRouteSiteId({ name: 'contenttype', query: { id: row.contentTypeId, fromRouter: route.name as string, }, })"
              data-cy="edit-content-type"
              class="mx-8"
            >
              <el-tooltip
                placement="top"
                :content="t('common.editContentType')"
              >
                <el-icon class="iconfont icon-a-writein" />
              </el-tooltip>
            </router-link>
            <IconButton
              v-else
              :permission="{
                feature: 'contentType',
                action: 'edit',
              }"
              icon="icon-a-writein"
              :tip="t('common.editContentType')"
            />
          </template>

          <IconButton
            v-if="!row.isChildren"
            icon="icon-move js-sortable cursor-move"
            :tip="t('common.move')"
            data-cy="move"
          />
        </template>
      </el-table-column>
    </KTable>
    <EditFolderDialog
      v-model="visibleEditDialog"
      :current="currentEdit"
      :folders="list"
      :is-content="isContent"
      @create-success="load"
    />
  </div>
</template>

<script lang="ts" setup>
import KTable from "@/components/k-table";
import { computed, onMounted, ref } from "vue";
import type { ContentFolder } from "@/api/content/content-folder";
import { getList, deletes, sort } from "@/api/content/content-folder";
import { useRouteSiteId } from "@/hooks/use-site-id";
import EditFolderDialog from "./components/edit-folder-dialog.vue";
import { cloneDeep } from "lodash-es";
import RelationsTag from "@/components/relations/relations-tag.vue";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useRoute } from "vue-router";
import { showDeleteConfirm } from "@/components/basic/confirm";
import { useI18n } from "vue-i18n";
import { useSiteStore } from "@/store/site";
import { newGuid } from "@/utils/guid";

const { t } = useI18n();
const siteStore = useSiteStore();
const list = ref<ContentFolder[]>([]);
const visibleEditDialog = ref(false);
const currentEdit = ref<ContentFolder>();
const route = useRoute();
const showHidden = ref(false);
const isContent = ref(false);
const selectedData = ref<any[]>([]);

onMounted(() => {
  load();
});

async function load() {
  const records = await getList();
  list.value = [];
  setTimeout(() => {
    list.value = records.sort((a, b) =>
      (a.order ?? 0) < (b.order ?? 0) ? -1 : 1
    );
  });
}

const computedList = computed(() => {
  let items: ContentFolder[] = JSON.parse(
    JSON.stringify(
      showHidden.value ? list.value : list.value.filter((f) => !f.hidden)
    )
  );

  var result = [];
  while (items.length) {
    const item = items.shift();
    if (!item) break;
    if (item.group) {
      const sameGroup = items.filter((f) => f.group == item.group);
      if (sameGroup.length) {
        const children = [item, ...sameGroup] as any;
        children.forEach((f: any) => (f.isChildren = true));
        result.push({
          isFolder: true,
          group: item.group,
          name: item.name,
          displayName: item.group,
          children: children,
          id: newGuid(),
        });
        items = items.filter((f) => f.group != item.group);
        continue;
      }
    }
    result.push(item);
  }
  return result;
});

async function onDelete(rows: ContentFolder[]) {
  await showDeleteConfirm(rows.length);
  const ids = rows.map((item) => item.id);
  await deletes({ ids });
  load();
}

function newFolder(row?: ContentFolder) {
  isContent.value = row?.isContent || false;
  if (!siteStore.hasAccess("contentType", "edit")) return;
  visibleEditDialog.value = true;
  if (row) {
    currentEdit.value = cloneDeep(row);
  } else {
    currentEdit.value = undefined;
  }
}

function newContent(row?: ContentFolder) {
  isContent.value = true;
  if (!siteStore.hasAccess("contentType", "edit")) return;
  visibleEditDialog.value = true;
  if (row) {
    currentEdit.value = cloneDeep(row);
  } else {
    currentEdit.value = undefined;
  }
}

async function onSorted(data: ContentFolder[]) {
  const names: string[] = [];
  for (const i of data) {
    if (!i || names.includes(i.name)) continue;
    names.push(i.name);
    if ((i as any).children) {
      for (const c of (i as any).children) {
        if (names.includes(c.name)) continue;
        names.push(c.name);
      }
    }
  }
  await sort(names);
  load();
}
</script>
