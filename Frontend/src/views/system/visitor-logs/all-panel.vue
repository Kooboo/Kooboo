<script lang="ts" setup>
import KTable from "@/components/k-table";
import { ref, watch } from "vue";
import type { VisitorLog } from "@/api/visitor-log/types";
import { getList } from "@/api/visitor-log";
import type { PaginationResponse } from "@/global/types";
import { useSiteStore } from "@/store/site";
import { useAppStore } from "@/store/app";
import { toUniversalSchema, combineUrl } from "@/utils/url";
import { useTime } from "@/hooks/use-date";
import { bytesToSize } from "@/utils/common";

import { useI18n } from "vue-i18n";
const props = defineProps<{ week: string }>();
const { t } = useI18n();
const data = ref<PaginationResponse<VisitorLog>>();
const siteStore = useSiteStore();
const appStore = useAppStore();

const load = async (index?: number) => {
  data.value = await getList(props.week, index);
};

const previewUrl = (value: string) => {
  return toUniversalSchema(combineUrl(siteStore.site.baseUrl, value));
};

function getClientInfo(row: any) {
  return `${row.clientInfo.os || ""} ${row.clientInfo?.platform || ""} ${
    row.clientInfo?.application?.name || ""
  } ${row.clientInfo?.application?.version || ""} ${row?.userAgent || ""}`;
}

watch(
  () => props.week,
  (val) => {
    val && load(1);
  },
  { immediate: true }
);
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
    <el-table-column label="IP" width="140">
      <template #default="{ row }">
        <span data-cy="ip">{{ row.clientIP }}</span>
      </template>
    </el-table-column>
    <template v-if="appStore.header?.isOnlineServer">
      <el-table-column :label="t('common.state')">
        <template #default="{ row }">
          <el-tag
            v-if="row.state"
            type="success"
            size="small"
            class="rounded-full"
            data-cy="state"
          >
            {{ row.state }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.country')">
        <template #default="{ row }">
          <el-tag
            v-if="row.country"
            type="success"
            size="small"
            class="rounded-full"
            data-cy="country"
          >
            {{ row.country }}
          </el-tag>
        </template>
      </el-table-column>
    </template>

    <el-table-column :label="t('common.page')">
      <template #default="{ row }">
        <a
          :title="row.url"
          class="text-blue ellipsis"
          :href="previewUrl(row.url)"
          target="_blank"
          data-cy="page-url"
          >{{ row.url }}</a
        >
      </template>
    </el-table-column>

    <el-table-column :label="t('common.startTime')" width="180">
      <template #default="{ row }">
        <span data-cy="start-time">{{ useTime(row.begin) }} </span></template
      >
    </el-table-column>
    <el-table-column :label="t('common.timeElapsed')">
      <template #default="{ row }">
        <el-popover
          width="auto"
          trigger="hover"
          placement="right"
          popper-style="padding: 0;"
        >
          <template #reference>
            <el-tag
              v-if="row.timeSpan"
              type="success"
              size="small"
              class="rounded-full cursor-pointer"
              data-cy="time-elapsed"
            >
              {{ row.timeSpan }} ms
            </el-tag>
          </template>
          <el-scrollbar max-height="98vh" view-style="padding: 12px;">
            <div class="space-y-4">
              <table v-if="row.entries?.length">
                <tbody>
                  <tr v-for="(item, index) of row.entries" :key="index">
                    <td>
                      <span class="mr-4">{{ item.name }}</span>
                    </td>
                    <td>
                      <span class="mr-4">{{ item.value }}</span>
                    </td>
                    <td>
                      <el-tag
                        type="success"
                        size="small"
                        class="rounded-full cursor-pointer"
                        data-cy="time-elapsed"
                      >
                        {{
                          new Date(item.endTime).getTime() -
                          new Date(item.startTime).getTime()
                        }}
                        ms
                      </el-tag>
                    </td>
                  </tr>
                </tbody>
              </table>
              <div v-else>{{ t("common.empty") }}</div>
            </div>
          </el-scrollbar>
        </el-popover>
      </template>
    </el-table-column>
    <el-table-column :label="t('common.referer')">
      <template #default="{ row }">
        <div class="ellipsis" :title="row.referer">{{ row.refererHost }}</div>
      </template>
    </el-table-column>
    <el-table-column :label="t('common.clientInfo')">
      <template #default="{ row }">
        <div v-if="row.clientInfo" class="text-s flex space-x-4 items-center">
          <ElTag
            v-if="row.clientInfo?.application?.isWebBrowser"
            round
            size="small"
            >{{ t("common.webBrowser") }}</ElTag
          >
          <div class="ellipsis" :title="getClientInfo(row)">
            {{ getClientInfo(row) }}
          </div>
        </div>
      </template>
    </el-table-column>
    <el-table-column :label="t('common.size')" width="120">
      <template #default="{ row }">
        <span>{{ bytesToSize(row.size) }} </span></template
      >
    </el-table-column>
  </KTable>
</template>
