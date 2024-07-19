<script lang="ts" setup>
import type { UploadFile } from "element-plus";
import KTable from "@/components/k-table";
import { useStyleStore } from "@/store/style";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { openInNewTab } from "@/utils/url";
import RelationsTag from "@/components/relations/relations-tag.vue";
import { useTime } from "@/hooks/use-date";
import IconButton from "@/components/basic/icon-button.vue";
import { useRouter } from "vue-router";
import type { ScriptItem } from "@/api/script/types";
import {
  showDeleteConfirm,
  showFileExistsConfirm,
} from "@/components/basic/confirm";

import { useI18n } from "vue-i18n";
import { useSiteStore } from "@/store/site";
import { ref } from "vue";
import SearchOnlineDialog from "@/components/search-dialog/package-search.vue";
import type SearchOnlineDialogType from "@/components/search-dialog/package-search.vue";

const { t } = useI18n();
const styleStore = useStyleStore();
const siteStore = useSiteStore();
const router = useRouter();
const searchDialog = ref<InstanceType<typeof SearchOnlineDialogType>>();
function handleSearchOnline() {
  (searchDialog.value as any)?.show();
}

const onUpload = async (file: UploadFile) => {
  if (styleStore.external.some((s) => s.name === file.name)) {
    await showFileExistsConfirm();
  }

  await styleStore.uploadStyle(file.raw!);
};

const onDelete = async (rows: ScriptItem[]) => {
  await showDeleteConfirm(rows.length);
  await styleStore.deleteStyles(rows.map((m) => m.id));
};

const onReload = async () => {
  await styleStore.loadExternal();
};

onReload();
</script>

<template>
  <div>
    <div class="flex items-center mb-12 space-x-16">
      <el-button
        v-hasPermission="{
          feature: 'style',
          action: 'edit',
        }"
        round
        data-cy="new-style"
        @click="router.push(useRouteSiteId({ name: 'style-edit' }))"
      >
        <div class="flex items-center">
          <el-icon class="iconfont icon-a-addto" />
          {{ t("common.newStyle") }}
        </div>
      </el-button>
      <el-upload
        :show-file-list="false"
        :action="''"
        :accept="['text/css'].join(',')"
        :auto-upload="false"
        :on-change="onUpload"
        multiple
        :disabled="!siteStore.hasAccess('style', 'edit')"
      >
        <el-button
          v-hasPermission="{
            feature: 'style',
            action: 'edit',
          }"
          round
          data-cy="upload-styles"
        >
          {{ t("common.uploadStyles") }}
        </el-button>
      </el-upload>
      <el-button
        v-hasPermission="{
          feature: 'style',
          action: 'edit',
        }"
        round
        data-cy="search"
        @click="handleSearchOnline"
      >
        <el-icon class="iconfont icon-search" />
        {{ t("common.search") }}
      </el-button>
    </div>
    <KTable
      :data="styleStore.external"
      show-check
      :permission="{
        feature: 'style',
        action: 'delete',
      }"
      @delete="onDelete"
    >
      <el-table-column :label="t('common.name')">
        <template #default="{ row }">
          <router-link
            :to="
              useRouteSiteId({
                name: 'style-edit',
                query: {
                  id: row.id,
                },
              })
            "
            data-cy="name"
          >
            <span :title="row.name" class="text-blue cursor-pointer ellipsis">{{
              row.name
            }}</span>
          </router-link>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.preview')">
        <template #default="{ row }">
          <span
            class="text-blue cursor-pointer"
            :title="row.fullUrl"
            data-cy="preview"
            @click="openInNewTab(row.fullUrl)"
            >{{ row.routeName }}</span
          >
        </template>
      </el-table-column>
      <el-table-column :label="t('common.usedBy')">
        <template #default="{ row }">
          <RelationsTag :id="row.id" :relations="row.references" type="style" />
        </template>
      </el-table-column>
      <el-table-column :label="t('common.lastModified')">
        <template #default="{ row }">{{ useTime(row.lastModified) }}</template>
      </el-table-column>
      <el-table-column width="60px" align="right">
        <template #default="{ row }">
          <IconButton
            icon="icon-time"
            :permission="{ feature: 'site', action: 'log' }"
            :tip="t('common.version')"
            data-cy="versions"
            @click="
              $router.goLogVersions(
                row.keyHash,
                row.storeNameHash,
                row.tableNameHash
              )
            "
          />
        </template>
      </el-table-column>
    </KTable>
    <SearchOnlineDialog ref="searchDialog" module="style" @reload="onReload" />
  </div>
</template>
