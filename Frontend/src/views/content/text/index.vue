<template>
  <div class="p-24">
    <Breadcrumb :name="t('common.KoobooConfig')" />
    <KTable
      :data="list"
      show-check
      class="mt-24"
      :permission="{
        feature: 'text',
        action: 'delete',
      }"
      @delete="onDelete"
    >
      <el-table-column :label="t('common.name')">
        <template #default="{ row }">
          <div>
            <p class="ellipsis" :title="row.name" data-cy="name">
              {{ row.name }}
            </p>
            <p class="ellipsis" data-cy="inner-html">
              {{ row.description }}
            </p>
          </div>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.preview')">
        <template #default="{ row }">
          <a v-if="row.preview" target="_blank" :href="row.preview">
            <img
              class="max-w-84px max-h-44px"
              :src="row.preview"
              data-cy="preview"
            />
          </a>
          <div v-else>-</div>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.tagName')" width="150">
        <template #default="{ row }">
          <el-tooltip placement="top">
            <template #content
              ><div
                class="max-w-400px"
                style="
                  overflow: hidden;
                  text-overflow: ellipsis;
                  display: -webkit-box;
                  -webkit-line-clamp: 10;
                  -webkit-box-orient: vertical;
                "
              >
                {{ row.tagHtml }}
              </div></template
            >
            <el-tag
              size="small"
              class="rounded-full cursor-pointer"
              data-cy="tag-name"
              >&lt;{{ row.tagName }}&gt;</el-tag
            >
          </el-tooltip>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.usedBy')" width="200">
        <template #default="{ row }">
          <RelationsTag
            :id="row.id"
            :relations="row.relations"
            type="KConfig"
          />
        </template>
      </el-table-column>
      <el-table-column :label="t('common.lastModified')" width="180">
        <template #default="{ row }">{{ useTime(row.lastModified) }}</template>
      </el-table-column>
      <el-table-column width="90" align="right">
        <template #default="{ row }">
          <IconButton
            class="inline-flex"
            icon="icon-a-writein"
            :tip="t('common.edit')"
            data-cy="edit"
            @click="edit(row)"
          />
          <IconButton
            class="inline-flex"
            icon="icon-time"
            :tip="t('common.version')"
            :permission="{
              feature: 'site',
              action: 'log',
            }"
            data-cy="versions"
            @click="$router.goLogVersions(row.keyHash, row.storeNameHash)"
          />
        </template>
      </el-table-column>
    </KTable>
    <EditDialog
      v-if="visibleEditDialog"
      v-model="visibleEditDialog"
      :current="currentEdit"
      @success="$event ? load() : null"
    />
    <KMediaDialog
      v-if="visibleMediaDialog"
      v-model="visibleMediaDialog"
      @choose="handleChooseFile"
    />
  </div>
</template>

<script lang="ts" setup>
import KTable from "@/components/k-table";
import { onMounted, ref } from "vue";
import type { KConfig } from "@/api/content/kconfig";
import { getList, deletes, getKConfig, update } from "@/api/content/kconfig";
import { useTime } from "@/hooks/use-date";
import EditDialog from "./edit-dialog.vue";
import KMediaDialog from "@/components/k-media-dialog";
import type { MediaFileItem } from "@/components/k-media-dialog";
import RelationsTag from "@/components/relations/relations-tag.vue";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import IconButton from "@/components/basic/icon-button.vue";
import { combineUrl } from "@/utils/url";
import { useSiteStore } from "@/store/site";
import { showDeleteConfirm } from "@/components/basic/confirm";

import { useI18n } from "vue-i18n";
const { t } = useI18n();
type KConfigVM = KConfig & {
  description: string;
  preview: string;
};
const list = ref<KConfigVM[]>([]);
const visibleEditDialog = ref(false);
const visibleMediaDialog = ref(false);
const currentEdit = ref<KConfig>({} as KConfig);
const siteStore = useSiteStore();
onMounted(() => {
  load();
});

async function load() {
  const records = await getList();
  list.value = records
    .sort((a, b) => (a.lastModified > b.lastModified ? -1 : 1))
    .map((item) => {
      const itemVM = item as KConfigVM;
      const firstKey = Object.keys(item.binding)[0];
      const firstValue = item.binding[firstKey];
      itemVM.description = `${firstKey}: ${firstValue || ""}`;
      if (item.tagName === "img" && firstKey === "src") {
        itemVM.preview = firstValue?.startsWith("/")
          ? combineUrl(siteStore.site.baseUrl, firstValue)
          : firstValue;
      }
      return itemVM;
    });
}
async function onDelete(rows: KConfigVM[]) {
  await showDeleteConfirm(rows.length);
  const ids = rows.map((item) => item.id);
  await deletes({ ids });
  load();
}
async function edit(row: KConfigVM) {
  currentEdit.value = await getKConfig({ id: row.id });
  if (currentEdit.value.controlType?.toLowerCase() === "mediafile") {
    visibleMediaDialog.value = true;
  } else {
    visibleEditDialog.value = true;
  }
}

async function handleChooseFile(files: MediaFileItem[]) {
  currentEdit.value.binding.src = files[0].url;
  await update(currentEdit.value);
  load();
}
</script>
