<script lang="ts" setup>
import { getDashboardInfo } from "@/api/dashboard";
import type {
  Banner,
  News,
  Visitors,
  Resource,
  Doc,
  SiteInfo as SiteInfoType,
  EditLog,
  Info,
} from "@/api/dashboard/types";

import { ref, watch } from "vue";
import { useSiteStore } from "@/store/site";
import { getQueryString } from "@/utils/url";
import { getRecentlyVisits } from "@/global/recent-visits";
import { useI18n } from "vue-i18n";

import ChartJs from "@/components/basic/chart-js.vue";
import RecentVisit from "./components/recent-visit.vue";
import MyResource from "./components/my-resource.vue";
import RecentEdited from "./components/recent-edited.vue";
import SiteInfo from "./components/site-info.vue";
import DocumentEntry from "./components/document-entry.vue";
import BannerComponent from "./components/banner.vue";
import NewsComponent from "./components/news.vue";

const { t } = useI18n();

const resourceList = ref<Resource[]>([]);
const recentEditList = ref<EditLog[]>([]);
const recentlyVisits = getRecentlyVisits();
const recentOrderList = ref([
  {
    orderId: "1231231241412",
    amount: 100,
    status: "已支付",
    lastModified: "2023-09-25T05:39:52.3228284Z",
  },
  {
    orderId: "1231231241412",
    amount: 100,
    status: "已支付",
    lastModified: "2023-09-25T05:39:52.3228284Z",
  },
  {
    orderId: "1231231241412",
    amount: 100,
    status: "已支付",
    lastModified: "2023-09-25T05:39:52.3228284Z",
  },
]);
const newsList = ref<News[]>([]);
const helpDocumentList = ref<Doc[]>([]);
const infoBanner = ref<Banner>();
const visitors = ref<Visitors[]>([]);
const siteInfo = ref<SiteInfoType>();

const info = ref<Info>();

function getTextWidth(text: string, fontSize = "16px") {
  const canvas = document.createElement("canvas");
  const ctx = canvas.getContext("2d");
  ctx!.font = `${fontSize} Arial`;
  const width = ctx!.measureText(text)?.width;
  return width;
}

function cropTextByWidth({
  text,
  width,
  fontSize,
  isEllipsis = false,
}: {
  text: string;
  width: number;
  fontSize: string;
  isEllipsis?: boolean | undefined;
}): string {
  const textWidth =
    getTextWidth(text, fontSize) +
    (isEllipsis ? getTextWidth("…", fontSize) : 0);
  // console.log(text, textWidth)
  if (textWidth <= width) {
    return text + (isEllipsis ? "…" : "");
  } else {
    // 以字符…作为单字符宽度参考 来计算需要删除多少字符
    const CharacterCellWidth = getTextWidth("…", fontSize) as unknown as number;
    // 超出宽度裁剪文本加省略号
    // 距离指定宽度相差多少
    // 第一次计算（isEllipsis=false）不带省略号宽度，到这步已经是宽度超出 需要补上省略号宽度
    const difference =
      textWidth + (isEllipsis ? 0 : getTextWidth("…", fontSize)) - width;
    // 向上取整，宽度只能少不能多
    const len = text.length - Math.ceil(difference / CharacterCellWidth);
    // 裁剪后的文本再次计算
    return cropTextByWidth({
      text: text.slice(0, len),
      width,
      fontSize,
      isEllipsis: true,
    });
  }
}

// 访客日志
const visitorLogConfig = ref({
  type: "bar",
  data: {
    labels: [],
    datasets: [
      {
        label: t("common.pageView"),
        data: [],
        backgroundColor: "#2296f3",
        borderWidth: 1,
        fill: false,
        barPercentage:
          visitors.value.map((m) => m.name).length < 10 ? 0.4 : 0.7,
        categoryPercentage:
          visitors.value.map((m) => m.name).length < 10 ? 0.4 : 0.7,
      },
    ],
  },
  options: {
    maintainAspectRatio: false,
    scales: {
      y: {
        // max: 1250,
        // ticks: {
        //   stepSize: 250, // 设置纵坐标刻度的步长
        // },
      },
      x: {
        offset: true,
        grid: {
          display: false, // 不显示纵坐标轴网格线
        },
        ticks: {
          // stepSize: 3, // 设置横坐标的步长，即每隔3个显示一个 label
          // maxTicksLimit: 10,
          callback: function (value: number, index: number, ticks: any) {
            return visitors.value.map((m) => m.name)[index]?.substring(5);
          },
        },
      },
    },

    plugins: {
      legend: {
        display: false, // 设置为 false 隐藏图例
      },
    },
  },
});
// top Page
const topPagesConfig = ref({
  type: "bar",
  data: {
    labels: [],
    datasets: [
      {
        label: t("dashboard.count"),
        data: [],
        backgroundColor: "#2296f3",
        borderWidth: 1,
        fill: false,
        barPercentage: 0.7,
        categoryPercentage: 0.7,
      },
    ],
  },
  options: {
    maintainAspectRatio: false,
    indexAxis: "y",
    scales: {
      y: {
        // max: 1250,
        ticks: {
          stepSize: 250, // 设置纵坐标刻度的步长
          callback: function (value: number, index: number, ticks: any) {
            return cropTextByWidth({
              text: Object.keys(info.value?.top.topPages)[index] ?? "",
              fontSize: "12px",
              width: 100,
            });
          },
        },
      },
      x: {
        grid: {
          display: false, // 不显示纵坐标轴网格线
        },
        ticks: {
          stepSize: 3, // 设置横坐标的步长，即每隔3个显示一个 label
        },
      },
    },
    plugins: {
      legend: {
        display: false, // 设置为 false 隐藏图例
      },
    },
  },
});

// Top reference
const topReferenceConfig = ref({
  type: "bar",
  data: {
    labels: [],
    datasets: [
      {
        label: t("dashboard.count"),
        data: [],
        borderWidth: 1,
        backgroundColor: "#2296f3",
        barPercentage: 0.7,
        categoryPercentage: 0.7,
      },
    ],
  },
  options: {
    maintainAspectRatio: false,
    indexAxis: "y",
    scales: {
      x: {
        ticks: {
          stepSize: 400, // 设置纵坐标刻度的步长
        },
      },
      y: {
        ticks: {
          callback: function (value: number, index: number, ticks: any) {
            return cropTextByWidth({
              text: Object.keys(info.value?.top.topReferrer)[index] ?? "",
              fontSize: "12px",
              width: 100,
            });
          },
        },
      },
    },
    plugins: {
      legend: {
        display: false, // 设置为 false 隐藏图例
      },
    },
  },
});
const siteStore = useSiteStore();

function updateChartData(
  config: any,
  data: {
    labels: any[];
    values: any[];
  }
) {
  config.value.data.labels = data.labels;
  if (config.value.data.labels.length < 5) {
    for (let i = config.value.data.labels.length; i < 5; i++) {
      config.value.data.labels.push("");
    }
  }

  config.value.data.datasets[0].data = data.values;
  if (config.value.data.datasets[0].data.length < 5) {
    for (let i = config.value.data.datasets[0].data.length; i < 5; i++) {
      config.value.data.datasets[0].data.push(0);
    }
  }
}

const load = async () => {
  if (siteStore.hasAccess("dashboard")) {
    info.value = await getDashboardInfo();

    recentEditList.value = info.value.editLog;
    helpDocumentList.value = info.value.info.doc;
    resourceList.value = info.value.resource;
    newsList.value = info.value.info.news;
    infoBanner.value = info.value.info.banner;
    visitors.value = info.value.visitors;
    siteInfo.value = info.value.site;

    updateChartData(visitorLogConfig, {
      labels: visitors.value.map((m) => m.name),
      values: visitors.value.map((m) => m.count),
    });

    updateChartData(topPagesConfig, {
      labels: Object.keys(info.value.top.topPages),
      values: Object.values(info.value.top.topPages),
    });

    updateChartData(topReferenceConfig, {
      labels: Object.keys(info.value.top.topReferrer),
      values: Object.values(info.value.top.topReferrer),
    });
  }
};

watch(
  () => siteStore.site,
  async () => {
    if (getQueryString("siteId") != siteStore.site?.id) return;
    if (siteStore.site) {
      load();
    }
  },
  { immediate: true }
);
</script>
<template>
  <div class="p-24 text-14px">
    <div class="flex space-x-16">
      <div class="w-2/3 flex flex-col gap-16">
        <!-- 最近访问 -->
        <RecentVisit :list="recentlyVisits" />

        <!-- 我的资源 -->
        <MyResource :list="resourceList" />

        <!-- 访客日志 -->
        <el-card shadow="always">
          <div>
            <div class="mb-20px text-18px">
              {{ t("common.visitorLog") }}
            </div>
            <div>
              <ChartJs class="h-300px" :options="visitorLogConfig" />
            </div>
          </div>
        </el-card>
        <!-- 页面数据 -->
        <el-card shadow="always">
          <div class="mb-20px text-18px">{{ t("common.topPage") }}</div>
          <div>
            <ChartJs class="h-300px" :options="topPagesConfig" />
          </div>
        </el-card>

        <el-card shadow="always">
          <div>
            <div class="mb-20px text-18px">{{ t("common.topReference") }}</div>
            <div>
              <ChartJs class="h-300px" :options="topReferenceConfig" />
            </div>
          </div>
        </el-card>

        <!-- 最近编辑 -->
        <RecentEdited :list="recentEditList" />

        <!-- 帮助文档 -->
        <!-- <el-card shadow="always">
          <div class="my-16 text-18px flex items-center justify-between">
            <span>帮助文档</span
            ><span class="text-blue text-14px cursor-pointer">{{
              t("common.viewMore")
            }}</span>
          </div>
          <ul class="flex flex-wrap flex-row">
            <li
              class="py-8 rounded-normal w-1/2 flex items-center hover:text-blue"
              v-for="item in helpDocumentList"
            >
              <a :href="item.url">
                {{ item.name }}
              </a>
              <img :src="item.ico" class="ml-4 w-24 h-24" />
            </li>
          </ul>

          <div class="my-16 text-18px">更多帮助</div>
          <ul class="flex gap-12">
            <li
              class="px-12 py-8 rounded-normal w-1/4 bg-[#e9eaf0] dark:bg-[#555] dark:bg-999"
              v-for="item in 4"
            >
              <el-icon class="iconfont icon-Datacenter text-16px" />
              kooboo服务器
            </li>
          </ul>
        </el-card> -->

        <!-- 费用信息 -->
        <!-- <el-card shadow="always">
          <div class="my-16 text-18px">费用信息</div>

          <ul class="flex flex-wrap flex-row text-16px grid grid-cols-3 gap-8">
            <li class="p-16 bg-[#e9eaf0] dark:bg-[#555] dark:bg-999">
              <span class="">账户余额</span>
              <div class="flex items-center justify-between mt-8">
                <span>0.00</span>
                <button
                  class="bg-orange px-16 py-8 text-[#fff] rounded-5px text-14px"
                >
                  重置
                </button>
              </div>
            </li>

            <li class="p-16 bg-[#e9eaf0] dark:bg-[#555] dark:bg-999">
              <span class="">优惠券</span>
              <div
                class="flex items-center justify-between mt-8 h-30px leading-30px"
              >
                0
              </div>
            </li>

            <li class="p-16 bg-[#e9eaf0] dark:bg-[#555] dark:bg-999">
              <span class="">账户余额</span>
              <div class="flex items-center justify-between mt-8">
                <span>0.00</span>
                <button
                  class="border-1 border-blue bg-[#fff] px-16 py-8 text-blue rounded-5px text-14px"
                >
                  开票
                </button>
              </div>
            </li>
          </ul>

          <div class="my-16 text-18px flex items-center justify-between">
            <span>最新订单</span
            ><span class="text-blue text-14px cursor-pointer">查看全部</span>
          </div>

          <KTable :options="recentOrderList">
            <el-table-column :label="t('common.orderId')">
              <template #default="{ row }">{{ row.orderId }}</template>
            </el-table-column>

            <el-table-column :label="t('common.name')">
              <template #default="{ row }">{{ row.name }}</template>
            </el-table-column>

            <el-table-column :label="t('common.amount')">
              <template #default="{ row }">{{ row.amount }}</template>
            </el-table-column>
            <el-table-column :label="t('common.status')">
              <template #default="{ row }">{{ row.status }}</template>
            </el-table-column>
            <el-table-column :label="t('common.lastModified')">
              <template #default="{ row }">{{
                useTime(row.lastModified)
              }}</template>
            </el-table-column>
          </KTable>
        </el-card> -->
      </div>

      <div class="flex-1 flex flex-col gap-16 overflow-hidden">
        <!-- 站点信息 -->
        <SiteInfo :info="siteInfo!" />

        <!-- 文档入口 -->
        <DocumentEntry :list="helpDocumentList" />

        <!-- banner -->
        <BannerComponent v-if="infoBanner" :info="infoBanner" />

        <!-- News -->
        <NewsComponent :list="newsList" />
      </div>
    </div>
  </div>
</template>
