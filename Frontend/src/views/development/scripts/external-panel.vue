<script lang="ts" setup>
import type { UploadFile } from "element-plus";
import { ElMessage } from "element-plus";
import KTable from "@/components/k-table";
import { useScriptStore } from "@/store/script";
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
const scriptStore = useScriptStore();
const siteStore = useSiteStore();
const router = useRouter();

const searchDialog = ref<InstanceType<typeof SearchOnlineDialogType>>();
function handleSearchOnline() {
  (searchDialog.value as any)?.show();
}
const accepts = [
  "application/javascript",
  "application/x-javascript",
  "application/ecmascript",
  "text/javascript",
  "text/ecmascript",
];
const onUpload = async (file: UploadFile) => {
  if (!accepts.includes(file.raw!.type)) {
    ElMessage.error(t("common.fileInvalid", { fileName: file.name }));
    return;
  }

  if (scriptStore.external.some((s) => s.name === file.name)) {
    await showFileExistsConfirm();
  }

  scriptStore.uploadScript(file.raw!);
};

const onDelete = async (rows: ScriptItem[]) => {
  await showDeleteConfirm(rows.length);
  scriptStore.deleteScripts(rows.map((m) => m.id));
};

const onReload = async () => {
  await scriptStore.loadExternal();
};

onReload();
</script>

<template>
  <div>
    <div class="flex items-center mb-12 space-x-16">
      <el-button
        v-hasPermission="{ feature: 'script', action: 'edit' }"
        round
        data-cy="new-script"
        @click="router.push(useRouteSiteId({ name: 'script-edit' }))"
      >
        <div class="flex items-center">
          <el-icon class="iconfont icon-a-addto" />
          {{ t("common.newScript") }}
        </div>
      </el-button>
      <el-upload
        :show-file-list="false"
        :action="''"
        :accept="accepts.join(',')"
        :auto-upload="false"
        :on-change="onUpload"
        multiple
        :disabled="!siteStore.hasAccess('script', 'edit')"
      >
        <el-button
          v-hasPermission="{ feature: 'script', action: 'edit' }"
          round
        >
          {{ t("common.uploadScripts") }}
        </el-button>
      </el-upload>
      <el-button
        v-hasPermission="{
          feature: 'script',
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
      :data="scriptStore.external"
      show-check
      sort="lastModified"
      :permission="{ feature: 'script', action: 'delete' }"
      @delete="onDelete"
    >
      <el-table-column :label="t('common.name')">
        <template #default="{ row }">
          <router-link
            :to="
              useRouteSiteId({
                name: 'script-edit',
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
            class="text-blue cursor-pointer ellipsis"
            :title="row.fullUrl"
            data-cy="preview"
            @click="openInNewTab(row.fullUrl)"
            >{{ row.routeName }}</span
          >
        </template>
      </el-table-column>
      <el-table-column :label="t('common.usedBy')">
        <template #default="{ row }">
          <RelationsTag
            :id="row.id"
            :relations="row.references"
            type="script"
          />
        </template>
      </el-table-column>
      <el-table-column :label="t('common.lastModified')" prop="lastModified">
        <template #default="{ row }">{{ useTime(row.lastModified) }}</template>
      </el-table-column>
      <el-table-column width="60px" align="center">
        <template #default="{ row }">
          <IconButton
            :permission="{ feature: 'site', action: 'log' }"
            icon="icon-time"
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
    <SearchOnlineDialog ref="searchDialog" module="script" @reload="onReload" />
  </div>
</template>
