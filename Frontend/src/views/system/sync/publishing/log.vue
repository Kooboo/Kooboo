<script lang="ts" setup>
import KTable from "@/components/k-table";
import type { PushItem } from "@/api/publish/types";
import { getLogItems } from "@/api/publish";
import { ref } from "vue";
import { getQueryString } from "@/utils/url";
import type { PaginationResponse } from "@/global/types";
import ActionTag from "@/components/k-tag/action-tag.vue";
import { useTime } from "@/hooks/use-date";

import { useI18n } from "vue-i18n";
const data = ref<PaginationResponse<PushItem>>();
const props = defineProps<{
  type: "InItem" | "OutItem";
  conditions: {
    startDate: string;
    endDate: string;
    name: string;
    storeName: string;
    actionType: string;
    SyncSettingId?: string;
    pageNr?: number;
    pageSize?: number;
  };
}>();

const { t } = useI18n();
const id = getQueryString("id")!;

const load = async (index?: number) => {
  let obj = props.conditions;
  obj.pageSize = 30;
  obj.SyncSettingId = id;
  obj.pageNr = index;
  obj.name = obj.name.trim();
  data.value = await getLogItems(props.type, obj);
};

load();
defineExpose({
  load,
});
</script>
<template>
  <KTable
    v-if="data"
    :data="data?.list"
    :pagination="{
      currentPage: data.pageNr,
      pageCount: data.totalPages,
      pageSize: data.pageSize,
    }"
    @change="load"
  >
    <el-table-column :label="t('common.logItem')">
      <template #default="{ row }">
        <span data-cy="name">{{ row.name }}</span>
      </template>
    </el-table-column>
    <el-table-column :label="t('common.objectType')" width="200px">
      <template #default="{ row }">
        <ObjectTypeTag :type="row.objectType" />
      </template>
    </el-table-column>
    <el-table-column :label="t('common.action')" width="120px">
      <template #default="{ row }">
        <ActionTag :type="row.changeType" />
      </template>
    </el-table-column>
    <el-table-column :label="t('common.size')" width="120px">
      <template #default="{ row }">
        <span data-cy="size">{{ row.size }}</span>
      </template>
    </el-table-column>
    <el-table-column :label="t('common.lastModified')" width="200px">
      <template #default="{ row }">
        <span data-cy="last-modified">
          {{ useTime(row.lastModified) }}
        </span>
      </template>
    </el-table-column>
    <el-table-column :label="t('common.user')" width="150px">
      <template #default="{ row }">
        <span data-cy="user">{{ row.user }}</span>
      </template>
    </el-table-column>
    <el-table-column align="right" width="60px">
      <template #default="{ row }">
        <IconButton
          icon="icon-time"
          :tip="t('common.versions')"
          :permission="{ feature: 'site', action: 'log' }"
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
</template>
