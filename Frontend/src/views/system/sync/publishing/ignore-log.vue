<script lang="ts" setup>
import KTable from "@/components/k-table";
import type { PushItem } from "@/api/publish/types";
import { getIgnoreList, unIgnore } from "@/api/publish";
import { ref } from "vue";
import { getQueryString } from "@/utils/url";
import type { PaginationResponse } from "@/global/types";
import ActionTag from "@/components/k-tag/action-tag.vue";
import { useTime } from "@/hooks/use-date";

import { useI18n } from "vue-i18n";
const data = ref<PaginationResponse<PushItem>>();
const props = defineProps<{
  conditions: {
    startDate: string;
    endDate: string;
    name: string;
    storeName: string;
    actionType: string;
    SyncSettingId?: string;
    pageNr?: number;
  };
}>();

const { t } = useI18n();
const id = getQueryString("id")!;

const load = async (index?: number) => {
  let obj: any = props.conditions;
  obj.SettingId = id;
  obj.pageNr = index ?? 1;
  obj.pageSize = 30;
  data.value = await getIgnoreList(obj);
};

async function onDeletes(rows: any[]) {
  await unIgnore(
    id,
    rows.map((r) => r.objectId)
  );
  load();
}

load();
defineExpose({
  load,
});
</script>
<template>
  <KTable
    v-if="data"
    show-check
    :data="data?.list"
    :pagination="{
      currentPage: data.pageNr,
      pageCount: data.totalPages,
      pageSize: data.pageSize,
    }"
    hide-delete
    @change="load"
  >
    <template #bar="{ selected }">
      <el-button
        v-if="selected.length > 0"
        v-hasPermission="{ feature: 'sync', action: 'edit' }"
        class="dark:bg-666"
        round
        @click="onDeletes(selected)"
        >{{ t("common.undo") }}</el-button
      >
    </template>
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
