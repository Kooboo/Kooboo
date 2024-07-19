<script lang="ts" setup>
import KTable from "@/components/k-table";
import type { PushItem } from "@/api/publish/types";
import { getPushItems, push, ignore } from "@/api/publish";
import { ref } from "vue";
import { getQueryString } from "@/utils/url";
import { useTime } from "@/hooks/use-date";
import ActionTag from "@/components/k-tag/action-tag.vue";
import { useI18n } from "vue-i18n";
import { showConfirm } from "@/components/basic/confirm";
import { ElLoading } from "element-plus";
import { showConflict } from "@/components/conflict-dialog/sync-conflict";

const { t } = useI18n();
const props = defineProps<{
  conditions: {
    startDate: string;
    endDate: string;
    name: string;
    storeName: string;
    actionType: string;
    user: string;
    id?: string;
  };
}>();

const data = ref<any>();
const id = getQueryString("id")!;
let loadingInstance: { close: () => void } | undefined;

const load = async (isCloseLoading?: boolean, index?: number) => {
  let obj: any = props.conditions;
  obj.id = id;
  obj.name = obj.name.trim();
  obj.pageNr = index ?? 1;
  obj.pageSize = 50;
  data.value = await getPushItems(obj);

  if (isCloseLoading) {
    loadingInstance?.close();
    loadingInstance = undefined;
  }
};

const onPush = async (selected: PushItem[]) => {
  await showConfirm(t("common.pushConfirm"));
  showLoading();
  selected = selected.sort((a, b) => a.id - b.id);
  try {
    for (const i of selected) {
      var pushFeedBack = await push(id, i.logId);
      if (pushFeedBack.hasConflict) await showConflict(pushFeedBack, true, id);
      //$t("common.pushSuccess")
    }
  } catch (error) {
    console.log(error);
  }
  await load(true);
};

const onIgnore = async (selected: PushItem[]) => {
  await showConfirm(t("common.ignoreSiteLogConfirm"));
  await ignore(
    id,
    selected.map((m) => m.logId)
  );
  await load();
};

function showLoading() {
  if (!loadingInstance) {
    loadingInstance = ElLoading.service({ background: "rgba(0, 0, 0, 0.5)" });
  }
}

load();
defineExpose({
  load,
});
</script>
<template>
  <KTable
    v-if="data"
    :data="data.list"
    show-check
    hide-delete
    :pagination="{
      currentPage: data.pageNr,
      pageCount: data.totalPages,
      pageSize: data.pageSize,
    }"
    @change="load(true, $event)"
  >
    <template #bar="{ selected }">
      <el-button
        v-if="selected.length > 0"
        v-hasPermission="{ feature: 'sync', action: 'edit' }"
        class="dark:bg-666"
        round
        data-cy="ignore"
        @click="onIgnore(selected)"
        >{{ t("common.ignoreSelected") }}</el-button
      >
      <el-button
        v-if="selected.length > 0"
        v-hasPermission="{ feature: 'sync', action: 'edit' }"
        class="dark:bg-666"
        round
        data-cy="push"
        @click="onPush(selected)"
        >{{ t("common.pushSelected") }}</el-button
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
    <el-table-column :label="t('common.lastModified')" align="center">
      <template #default="{ row }">
        <span data-cy="last-modified">{{ useTime(row.lastModified) }}</span>
      </template>
    </el-table-column>
    <el-table-column :label="t('common.user')" width="150px">
      <template #default="{ row }">
        <span data-cy="username">{{ row.user }}</span>
      </template>
    </el-table-column>
    <el-table-column align="right" width="80px">
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
