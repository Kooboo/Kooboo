<script lang="ts" setup>
import KTable from "@/components/k-table";
import RelationsTag from "@/components/relations/relations-tag.vue";
import type { PaginationResponse, KeyValue } from "@/global/types";
import { useTime } from "@/hooks/use-date";
import { ref } from "vue";
import type { UrlItem, GetExternalParams } from "@/api/url";
import {
  getExternal,
  deletes,
  internalizeResource as internalizeResourceApi,
  getExternalOptions,
} from "@/api/url";
import EditDialog from "./external-edit-dialog.vue";
import IconButton from "@/components/basic/icon-button.vue";
import ObjectTypeTag from "@/components/k-tag/object-type-tag.vue";
import { ElMessage } from "element-plus";
import { useI18n } from "vue-i18n";
import { showDeleteConfirm } from "@/components/basic/confirm";
import { usePreviewUrl } from "@/hooks/use-preview-url";

const { t } = useI18n();
const data = ref<PaginationResponse<UrlItem>>();
const showEditDialog = ref(false);
const selectedItem = ref<UrlItem>();
const model = ref<{
  type?: string;
  keyword?: string;
}>({
  type: "",
  keyword: "",
});
const objectTypes = ref<KeyValue[]>();

const init = async () => {
  const options = await getExternalOptions();
  objectTypes.value = options["resourceType"] || [];
  await load();
};

const load = async (index?: number) => {
  const params: GetExternalParams = {
    pageNr: index,
    pageSize: 30,
    type: model.value.type,
    keyword: model.value.keyword?.trim(),
  };
  data.value = await getExternal(params);
};

const internalizeResource = async (id: string) => {
  await internalizeResourceApi(id);
  ElMessage.success(t("common.convertedResourceTip"));
  load();
};

const onDeletes = async (items: UrlItem[]) => {
  await showDeleteConfirm(items.length);
  await deletes(
    "external",
    items.map((m) => m.id)
  );
  load(data.value?.pageNr);
};

const { onPreview } = usePreviewUrl();

const onEdit = (row: UrlItem) => {
  selectedItem.value = row;
  showEditDialog.value = true;
};
init();
</script>

<template>
  <div class="flex space-x-16">
    <div class="flex-1" />
    <el-select
      v-model="model.type"
      :placeholder="t('common.resourceType')"
      clearable
      class="w-180px"
      @change="load(1)"
    >
      <el-option
        v-for="item in objectTypes"
        :key="item.key"
        :label="item.value"
        :value="item.key"
      />
    </el-select>
    <SearchInput
      v-model="model.keyword"
      placeholder="URL"
      class="w-280px h-10"
      @search="load(1)"
    />
  </div>
  <KTable
    v-if="data"
    class="mt-24"
    :data="data.list"
    :pagination="{
      currentPage: data.pageNr,
      pageCount: data.totalPages,
      pageSize: data.pageSize,
    }"
    show-check
    :permission="{ feature: 'link', action: 'delete' }"
    @change="load"
    @delete="onDeletes"
  >
    <el-table-column label="URL">
      <template #default="{ row }">
        <span :title="row.name" class="ellipsis" data-cy="url">{{
          row.name
        }}</span>
      </template>
    </el-table-column>
    <el-table-column :label="t('common.resourceType')">
      <template #default="{ row }">
        <ObjectTypeTag :type="row.resourceType" />
      </template>
    </el-table-column>
    <el-table-column :label="t('common.usedBy')">
      <template #default="{ row }">
        <RelationsTag
          :id="row.id"
          :relations="row.relations"
          type="externalResource"
        />
      </template>
    </el-table-column>
    <el-table-column :label="t('common.lastModified')">
      <template #default="{ row }">{{ useTime(row.lastModified) }}</template>
    </el-table-column>
    <el-table-column width="120px" align="right">
      <template #default="{ row }">
        <IconButton
          icon="icon-a-writein"
          :tip="t('common.edit')"
          @click="onEdit(row)"
        />
        <IconButton
          v-if="['Script', 'Style', 'Image'].includes(row.resourceType)"
          icon="icon-xiazai-wenjianxiazai-05"
          :tip="t('common.internalizeResource')"
          data-cy="internalizeResource"
          @click="internalizeResource(row.id)"
        />
        <IconButton
          icon="icon-eyes"
          :tip="t('common.preview')"
          data-cy="preview"
          @click="onPreview(row.previewUrl)"
        />
      </template>
    </el-table-column>
  </KTable>
  <EditDialog
    v-if="showEditDialog"
    :id="selectedItem!.id"
    v-model="showEditDialog"
    type="external"
    :value="selectedItem!.name"
    @reload="load(data?.pageNr)"
  />
</template>
