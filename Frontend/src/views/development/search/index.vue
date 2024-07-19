<template>
  <div class="p-24">
    <Breadcrumb :name="t('common.search')" />
  </div>

  <div class="px-24 w-full">
    <el-card shadow="never">
      <template #header>
        <div class="flex items-center">
          <span>{{ t("common.indexData") }}</span>
          <div class="flex items-center ml-12">
            <el-tooltip
              class="box-item"
              :content="t('common.addToIndexAutomaticallyWhenDocumentChanged')"
              placement="left"
            >
              <el-icon class="iconfont icon-problem mr-4" />
            </el-tooltip>
            <el-switch
              v-model="enableSearch"
              size="large"
              :disabled="!siteStore.hasAccess('search', 'edit')"
              :title="
                !siteStore.hasAccess('search', 'edit')
                  ? t('common.noPermission')
                  : ''
              "
              data-cy="auto-add-index"
              @change="handleEnableChange"
            />
          </div>
        </div>
      </template>
      <div class="flex items-center">
        <div class="flex-1">
          <div class="text-666">{{ wordCount }}</div>
          <div class="text-444 text-s">{{ t("common.keyword") }}</div>
        </div>
        <div class="flex-1">
          <div class="text-666">{{ docCount }}</div>
          <div class="text-444 text-s">{{ t("common.document") }}</div>
        </div>
        <div class="flex-1">
          <div class="text-666">{{ bytesToSize(diskSize) }}</div>
          <div class="text-444 text-s">{{ t("common.diskSize") }}</div>
        </div>
        <div class="flex-1 whitespace-nowrap text-center">
          <el-button
            v-hasPermission="{ feature: 'search', action: 'edit' }"
            class="bg-blue mr-4 rounded-full"
            type="primary"
            data-cy="rebuild"
            @click="rebuild"
            >{{ t("common.rebuild") }}</el-button
          >
          <el-popconfirm
            :title="t('common.cleanSearchIndexTips')"
            @confirm="clean"
          >
            <template #reference>
              <el-button
                v-hasPermission="{ feature: 'search', action: 'delete' }"
                class="bg-orange rounded-full"
                type="danger"
                data-cy="clean"
                >{{ t("common.clean") }}</el-button
              >
            </template>
          </el-popconfirm>
        </div>
      </div>
    </el-card>
  </div>

  <div class="mb-32 p-24 w-full flex">
    <div class="w-6/12">
      <el-card shadow="never" class="mr-12" :body-style="{ padding: 0 }">
        <template #header>
          <div>
            <span>{{ t("common.lastestSearch") }}</span>
          </div>
        </template>
        <el-table :data="model" class="w-full">
          <el-table-column width="10" />
          <el-table-column prop="ip" label="IP" width="120" />
          <el-table-column prop="keywords" :label="t('common.keyword')" />
          <el-table-column prop="docFound" :label="t('common.documentFound')" />
          <el-table-column :label="t('common.date')">
            <template #default="{ row }">
              <span>{{ useTime(row.time) }}</span>
            </template>
          </el-table-column>
        </el-table>
      </el-card>
    </div>

    <div class="w-6/12 search-statistic">
      <el-card shadow="never" class="ml-12" :body-style="{ padding: 0 }">
        <template #header>
          <div class="flex justify-between items-center">
            <span>{{ t("common.searchStatistic") }}</span>
            <el-select
              v-model="week"
              class="w-120px"
              :placeholder="t('common.select')"
            >
              <el-option
                v-for="item in weeks"
                :key="item"
                :label="item"
                :value="item"
              />
            </el-select>
          </div>
        </template>
        <div class="p-24">
          <ChartJs height="180" :options="options" />
        </div>
      </el-card>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { bytesToSize } from "@/utils/common";
import { ElMessageBox } from "element-plus";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import ChartJs from "@/components/basic/chart-js.vue";
import { ref, watch } from "vue";
import type { Ref } from "vue";
import {
  getWeekNames,
  getIndexStat,
  getLastest,
  SearchStat,
  Rebuild,
  Disable,
  Enable,
  Clean,
} from "@/api/development/search";
import type { Lastest } from "@/api/development/search";
import { useTime } from "@/hooks/use-date";

import { useI18n } from "vue-i18n";
import { useSiteStore } from "@/store/site";
const { t } = useI18n();
const siteStore = useSiteStore();
const enableSearch = ref(false);
const wordCount = ref(0);
const docCount = ref(0);
const diskSize = ref(0);
const model: Ref<Lastest[]> = ref([]);
const week = ref("");
const weeks = ref([] as string[]);
const options = ref({
  type: "bar",
  data: {
    labels: [] as string[],
    datasets: [
      {
        label: "PV",
        data: [] as number[],
      },
    ],
  },
  options: {
    scales: {
      y: {
        beginAtZero: true,
      },
    },
  },
});

watch(week, (n) => {
  if (n) {
    SearchStat({ weekname: n }).then((res) => {
      options.value.data.labels = Object.keys(res);
      options.value.data.datasets[0].data = Object.values(res);
    });
  }
});

function rebuild() {
  Rebuild().then((res) => {
    enableSearch.value = res.enableFullTextSearch;
    wordCount.value = res.wordCount;
    docCount.value = res.docCount;
    diskSize.value = res.diskSize;
  });
}

function clean() {
  Clean().then((res) => {
    enableSearch.value = res.enableFullTextSearch;
    wordCount.value = res.wordCount;
    docCount.value = res.docCount;
    diskSize.value = res.diskSize;
  });
}

async function handleEnableChange(enable: string | number | boolean) {
  if (!siteStore.hasAccess("search", "edit")) return;
  if (enable) {
    const EnableApi = (rebuild: boolean) => {
      Enable({ rebuild }).then((res) => {
        enableSearch.value = res.enableFullTextSearch;
        wordCount.value = res.wordCount;
        docCount.value = res.docCount;
        diskSize.value = res.diskSize;
      });
    };
    ElMessageBox.confirm(
      t("common.rebuildSiteIndexTips"),
      t("common.warning"),
      {
        confirmButtonText: t("common.ok"),
        cancelButtonText: t("common.cancel"),
        type: "warning",
      }
    )
      .then(() => {
        EnableApi(true);
      })
      .catch(() => {
        EnableApi(false);
      });
  } else {
    await Disable();
  }
}

function load() {
  getWeekNames().then((res) => {
    if (res.length) {
      weeks.value = res.reverse();
      week.value = weeks.value[0];
    }
  });

  getIndexStat().then((res) => {
    enableSearch.value = res.enableFullTextSearch;
    wordCount.value = res.wordCount;
    docCount.value = res.docCount;
    diskSize.value = res.diskSize;
  });

  getLastest().then((res) => {
    model.value = res;
  });
}

load();
</script>

<style lang="scss" scoped>
.search-statistic :deep(.el-card__header) {
  padding-top: 10px;
  padding-bottom: 10px;
}
</style>
