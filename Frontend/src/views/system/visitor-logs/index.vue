<script lang="ts" setup>
import { getWeeks } from "@/api/visitor-log";
import { ref, watch } from "vue";
import AllPanel from "./all-panel.vue";
import TopPagesPanel from "./top-pages-panel.vue";
import TopRefererPanel from "./top-referer-panel.vue";
import TopImagesPanel from "./top-images-panel.vue";
import Chart from "./chart.vue";
import ErrorList from "./error-list.vue";
import BotList from "./bot-panel.vue";
import TopBots from "./top-bot-panel.vue";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { weekToDates } from "@/utils/date";

import { useI18n } from "vue-i18n";
const { t } = useI18n();
const weeks = ref<string[]>([]);
const currentWeek = ref<string>();
const showWeek = ref(true);

getWeeks().then((r) => {
  weeks.value = r.sort((left: string, right: string) => {
    const leftNumbers = left.split("-").map((m) => parseInt(m));
    const rightNumbers = right.split("-").map((m) => parseInt(m));
    if (leftNumbers[0] === rightNumbers[0]) {
      return rightNumbers[1] - leftNumbers[1];
    } else {
      return rightNumbers[0] - leftNumbers[0];
    }
  });
  currentWeek.value = r[0];
});

const tabs = [
  {
    displayName: t("common.visitors"),
    value: "All",
    component: AllPanel,
    showWeek: true,
  },
  {
    displayName: t("common.topPages"),
    value: "topPages",
    component: TopPagesPanel,
    showWeek: true,
  },
  {
    displayName: t("common.topReferer"),
    value: "topReferer",
    component: TopRefererPanel,
    showWeek: true,
  },
  {
    displayName: t("common.topImages"),
    value: "topImages",
    component: TopImagesPanel,
    showWeek: true,
  },
  {
    displayName: t("common.chart"),
    value: "chart",
    component: Chart,
    showWeek: false,
  },
  {
    displayName: t("common.errorList"),
    value: "errorList",
    component: ErrorList,
    showWeek: true,
  },
  {
    displayName: t("common.bots"),
    value: "botList",
    component: BotList,
    showWeek: true,
  },
  {
    displayName: t("common.topBots"),
    value: "topBots",
    component: TopBots,
    showWeek: true,
  },
];

const activeTab = ref(tabs[0].value);

watch(
  () => activeTab.value,
  () => {
    showWeek.value = tabs.find((f) => f.value === activeTab.value)!.showWeek;
  }
);
</script>

<template>
  <div>
    <div class="p-24">
      <Breadcrumb :name="t('common.visitorLogs')" />
      <div v-show="showWeek" class="absolute top-16 right-24">
        <span class="text-black dark:text-fff/86 mr-12">
          {{ t("common.week") }}
        </span>
        <el-select v-model="currentWeek" class="w-240px">
          <el-option
            v-for="item of weeks"
            :key="item"
            :value="item"
            :label="weekToDates(item)"
            data-cy="week-opt"
          />
        </el-select>
      </div>
    </div>
    <el-tabs v-model="activeTab">
      <el-tab-pane
        v-for="tab in tabs"
        :key="tab.value"
        :label="tab.displayName"
        :name="tab.value"
      >
        <component
          :is="tab.component"
          v-if="activeTab === tab.value && currentWeek"
          :week="currentWeek"
        />
      </el-tab-pane>
    </el-tabs>
  </div>
</template>
