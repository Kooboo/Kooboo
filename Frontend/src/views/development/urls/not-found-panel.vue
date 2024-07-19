<script lang="ts" setup>
import KTable from "@/components/k-table";
import { useTime } from "@/hooks/use-date";
import { ref } from "vue";
import { getNotFound } from "@/api/url";
import CreateRouteDialog from "./create-route-dialog.vue";
import IconButton from "@/components/basic/icon-button.vue";
import { useI18n } from "vue-i18n";
import type { PaginationResponse } from "@/global/types";

const { t } = useI18n();
const data = ref<PaginationResponse<any>>();
const showMakeDialog = ref(false);
const selectedItem = ref();

const load = async (index?: number) => {
  data.value = await getNotFound({
    pageNr: index,
    pageSize: 30,
  });
};

const onMakeAlias = (item: any) => {
  selectedItem.value = item;
  showMakeDialog.value = true;
};

load();
</script>

<template>
  <KTable
    v-if="data"
    class="mt-24"
    :data="data.list"
    :pagination="{
      currentPage: data.pageNr,
      pageCount: data.totalPages,
      pageSize: data.pageSize,
    }"
    @change="load"
  >
    <el-table-column label="URL">
      <template #default="{ row }">
        <span :title="row.url" class="ellipsis" data-cy="url">{{
          row.url
        }}</span>
      </template>
    </el-table-column>
    <el-table-column :label="t('common.dateTime')">
      <template #default="{ row }">{{ useTime(row.startTime + "Z") }}</template>
    </el-table-column>
    <el-table-column width="120px" align="right">
      <template #default="{ row }">
        <IconButton
          :permission="{ feature: 'link', action: 'edit' }"
          icon="icon-copy"
          :tip="t('common.makeAlias')"
          @click="onMakeAlias(row)"
        />
      </template>
    </el-table-column>
  </KTable>
  <CreateRouteDialog
    v-if="showMakeDialog"
    v-model="showMakeDialog"
    :url="selectedItem!.url"
    @reload="load()"
  />
</template>
