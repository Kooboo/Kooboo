<script lang="ts" setup>
import KTable from "@/components/k-table";
import RelationsTag from "@/components/relations/relations-tag.vue";
import type { PaginationResponse, KeyValue } from "@/global/types";
import { useTime } from "@/hooks/use-date";
import { ref } from "vue";
import type { UrlItem, GetInternalParams } from "@/api/url";
import { getInternal, deletes, getInternalOptions } from "@/api/url";
import BooleanTag from "@/components/k-tag/boolean-tag.vue";
import EditDialog from "./internal-edit-dialog.vue";
import IconButton from "@/components/basic/icon-button.vue";
import ObjectTypeTag from "@/components/k-tag/object-type-tag.vue";
import { useI18n } from "vue-i18n";
import { showDeleteConfirm } from "@/components/basic/confirm";
import { usePreviewUrl } from "@/hooks/use-preview-url";
import CreateRouteDialog from "./create-route-dialog.vue";

const { t } = useI18n();
const data = ref<PaginationResponse<UrlItem>>();
const showEditDialog = ref(false);
const showCreateAliasDialog = ref(false);
const selectedItem = ref<UrlItem>();

const model = ref<{
  type?: string;
  keyword?: string;
  hasObject?: boolean;
}>({
  type: "",
  keyword: "",
  hasObject: undefined,
});
const objectTypes = ref<KeyValue[]>();

const load = async (index?: number) => {
  const options = await getInternalOptions();
  objectTypes.value = options["resourceType"] || [];
  const params: GetInternalParams = {
    pageNr: index,
    type: model.value.type,
    keyword: model.value.keyword?.trim(),
    hasObject: model.value.hasObject,
    pageSize: 30,
  };
  data.value = await getInternal(params);
  data.value.list.forEach((item) => {
    if (item.hasObject && item.resourceType != "Route") {
      (item as any).$DisabledSelect = true;
    }
  });
};

const onDeletes = async (items: UrlItem[]) => {
  await showDeleteConfirm(items.length);
  await deletes(
    "internal",
    items.map((m) => m.id)
  );
  load(data.value?.pageNr);
};

const { onPreview } = usePreviewUrl();

const onEdit = (row: UrlItem) => {
  selectedItem.value = row;
  showEditDialog.value = true;
};

load();
</script>

<template>
  <div class="flex space-x-16">
    <el-button
      v-hasPermission="{ feature: 'link', action: 'edit' }"
      round
      @click="showCreateAliasDialog = true"
    >
      <div class="flex items-center">
        <el-icon class="iconfont icon-a-addto" />
        {{ t("common.makeAlias") }}
      </div>
    </el-button>
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
    <el-select
      v-model="model.hasObject"
      :placeholder="t('common.hasObject')"
      clearable
      class="w-140px"
      @change="load(1)"
    >
      <el-option :label="t('common.yes')" :value="true" />
      <el-option :label="t('common.no')" :value="false" />
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
    :permission="{ feature: 'link', action: 'edit' }"
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
    <el-table-column :label="t('common.resourceType')"
      ><template #default="{ row }">
        <ObjectTypeTag :type="row.resourceType" /> </template
    ></el-table-column>
    <el-table-column
      :label="t('common.hasObject')"
      width="100px"
      align="center"
    >
      <template #default="{ row }">
        <BooleanTag :value="row.hasObject" />
      </template>
    </el-table-column>
    <el-table-column :label="t('common.usedBy')">
      <template #default="{ row }">
        <RelationsTag :id="row.id" :relations="row.relations" type="route" />
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
          icon="icon-eyes"
          :tip="t('common.preview')"
          @click="onPreview(row.previewUrl)"
        />
      </template>
    </el-table-column>
  </KTable>
  <EditDialog
    v-if="showEditDialog"
    v-model="showEditDialog"
    :route="selectedItem!"
    @reload="load(data?.pageNr)"
  />
  <CreateRouteDialog
    v-if="showCreateAliasDialog"
    v-model="showCreateAliasDialog"
    :url="''"
    @reload="load(data?.pageNr)"
  />
</template>
