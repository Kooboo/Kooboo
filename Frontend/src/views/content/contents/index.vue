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
      :data="computedList"
      show-check
      :permission="{
        feature: 'contentType',
        action: 'edit',
      }"
      @delete="onDelete"
    >
      <el-table-column :label="t('common.name')">
        <template #default="{ row }">
          <div class="flex items-center space-x-4">
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
              >{{ row.displayName }}</router-link
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
          <RelationsTag
            :id="row.id"
            :relations="row.relations"
            type="ContentFolder"
          />
        </template>
      </el-table-column>
      <el-table-column :label="t('common.type')" width="140" align="center">
        <template #default="{ row }">
          <el-tag v-if="row.isContent" type="success" round>{{
            t("common.singleContent")
          }}</el-tag>
          <el-tag v-else type="warning" round>{{ t("common.folder") }}</el-tag>
        </template>
      </el-table-column>

      <el-table-column width="150" align="right">
        <template #default="{ row }">
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
            <el-tooltip placement="top" :content="t('common.editContentType')">
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
import { getList, deletes } from "@/api/content/content-folder";
import { useRouteSiteId } from "@/hooks/use-site-id";
import EditFolderDialog from "./components/edit-folder-dialog.vue";
import { cloneDeep } from "lodash-es";
import RelationsTag from "@/components/relations/relations-tag.vue";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useRoute } from "vue-router";
import { showDeleteConfirm } from "@/components/basic/confirm";

import { useI18n } from "vue-i18n";
import { useSiteStore } from "@/store/site";
const { t } = useI18n();
const siteStore = useSiteStore();
const list = ref<ContentFolder[]>([]);
const visibleEditDialog = ref(false);
const currentEdit = ref<ContentFolder>();
const route = useRoute();
const showHidden = ref(false);
const isContent = ref(false);

onMounted(() => {
  load();
});

async function load() {
  const records = await getList();
  list.value = records.sort((a, b) => (a.displayName < b.displayName ? -1 : 1));
}

const computedList = computed(() => {
  if (showHidden.value) return list.value;
  return list.value.filter((f) => !f.hidden);
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
</script>
