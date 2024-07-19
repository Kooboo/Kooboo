<script lang="ts" setup>
import { ref, watch } from "vue";
import Changes from "./changes.vue";
import Log from "./log.vue";
import IgnoreLog from "./ignore-log.vue";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useI18n } from "vue-i18n";
import { searchDebounce } from "@/utils/url";
import { ElLoading, type DateModelType } from "element-plus";
import { getQueryString } from "@/utils/url";
import type { LogOptions } from "@/api/publish/types";
import { getOptions, pull, push } from "@/api/publish";
import type { KeyValue } from "@/global/types";
import { showConfirm } from "@/components/basic/confirm";
import { errorMessage } from "@/components/basic/message";
import {
  show as showConflictDialog,
  pushFeedBack,
  showConflict,
} from "@/components/conflict-dialog/sync-conflict";

const { t } = useI18n();
const model = ref<{
  startDate: string;
  endDate: string;
  name: string;
  storeName: string;
  actionType: string;
  user: string;
  [key: string]: string;
}>({
  startDate: "",
  endDate: "",
  name: "",
  storeName: "",
  actionType: "",
  user: "",
});
const dateValue = ref<[DateModelType, DateModelType]>([
  model.value.startDate,
  model.value.endDate,
]);
const options = ref<LogOptions>();
const actionTypes = ref<KeyValue[]>();
const objectTypes = ref<KeyValue[]>();
const users = ref<KeyValue[]>();

type TypeObject = {
  change: string;
  pull: string;
  push: string;
  [key: string]: string;
};
const type: TypeObject = {
  change: "local",
  pull: "in",
  push: "out",
  ignore: "ignore",
};

const updateTypes = async () => {
  options.value = await getOptions(id, type[activeTab.value]);
  objectTypes.value = options.value["storeName"];
  actionTypes.value = options.value["editType"];
  users.value = options.value["user"];
};

const id = getQueryString("id")!;

const tabs = [
  {
    value: "change",
    displayName: t("common.localChanges"),
    component: Changes,
    conditions: model.value,
    tabInstance: null,
  },
  {
    value: "pull",
    displayName: t("common.pullLogs"),
    component: Log,
    type: "InItem",
    conditions: model.value,
    tabInstance: null,
  },
  {
    value: "push",
    displayName: t("common.pushLogs"),
    component: Log,
    type: "OutItem",
    conditions: model.value,
    tabInstance: null,
  },
  {
    value: "ignore",
    displayName: t("common.ignoreLogs"),
    component: IgnoreLog,
    conditions: model.value,
    tabInstance: null,
  },
];

const activeTab = ref(tabs[0].value);
let loadingInstance: { close: () => void } | undefined;
const goPullTab = () => {
  activeTab.value = "pull";
};

watch(
  () => model.value.name,
  () => {
    if (model.value.name) {
      search();
    } else {
      reload();
    }
  }
);
watch(
  () => activeTab.value,
  () => {
    clean();
  }
);

const reload = () => {
  const tabInstance: any = tabs.find(
    (i) => i.value === activeTab.value
  )?.tabInstance;
  if (tabInstance) {
    tabInstance.load();
  }
};
const search = searchDebounce(reload, 1000);
updateTypes();

const clean = () => {
  dateValue.value = ["", ""];
  Object.keys(model.value).forEach((key) => {
    if (key === "name") return;
    model.value[key] = "";
  });
  // 有name的时候调用watch里面的getSiteLogs，没name的时候手动更新
  if (!model.value.name) {
    reload();
  } else {
    model.value.name = "";
  }
};
const changeTime = () => {
  if (
    dateValue.value &&
    dateValue.value.some((item: DateModelType) => item !== null)
  ) {
    model.value.startDate = dateValue.value[0].toString();
    model.value.endDate = dateValue.value[1].toString();
  } else {
    model.value.startDate = "";
    model.value.endDate = "";
  }
  reload();
};

function showLoading() {
  if (!loadingInstance) {
    loadingInstance = ElLoading.service({ background: "rgba(0, 0, 0, 0.5)" });
  }
}

const realPull = async () => {
  await showConfirm(t("common.pullConfirm"));
  showLoading();
  await onPull();
};

const realPush = async () => {
  await showConfirm(t("common.pushAllConfirm"));
  showLoading();
  await onPush();
};

const onPush = async () => {
  try {
    var pushFeedBack;
    do {
      pushFeedBack = await push(id);
      if (pushFeedBack.hasConflict) await showConflict(pushFeedBack, true, id);
    } while (!pushFeedBack.isFinish);
    if (activeTab.value == "push") {
      reload();
    } else {
      activeTab.value = "push";
    }
  } catch (error: any) {
    if (error) errorMessage(error.message, true);
  }

  loadingInstance?.close();
  loadingInstance = undefined;
};

const onPull = async () => {
  try {
    let pushFeedBack;
    do {
      pushFeedBack = await pull(id, pushFeedBack?.currentSenderVersion);
      if (pushFeedBack.hasConflict) await showConflict(pushFeedBack, false, id);
    } while (!pushFeedBack.isFinish);
    if (activeTab.value == "pull") {
      reload();
    } else {
      activeTab.value = "pull";
    }
  } catch (error: any) {
    if (error) errorMessage(error.message, true);
  }

  loadingInstance?.close();
  loadingInstance = undefined;
};
</script>

<template>
  <div>
    <Breadcrumb
      class="p-24"
      :crumb-path="[
        { name: t('common.sync'), route: { name: 'sync' } },
        { name: t('common.syncLogs') },
      ]"
    />
    <div class="flex space-x-16 mb-16 px-24">
      <el-select
        v-model="model.storeName"
        :placeholder="t('common.objectType')"
        class="w-180px"
        clearable
        @change="reload"
      >
        <el-option
          v-for="item in objectTypes"
          :key="item.key"
          :label="item.value"
          :value="item.key"
        />
      </el-select>

      <el-select
        v-model="model.actionType"
        :placeholder="t('common.action')"
        class="w-140px"
        clearable
        @change="reload"
      >
        <el-option
          v-for="item in actionTypes"
          :key="item.key"
          :label="item.value"
          :value="item.key"
        />
      </el-select>

      <el-select
        v-model="model.userId"
        :placeholder="t('common.user')"
        clearable
        class="w-160px"
        @change="reload"
      >
        <el-option
          v-for="item in users"
          :key="item.key"
          :label="item.value"
          :value="item.key"
        />
      </el-select>

      <el-date-picker
        v-model="dateValue"
        type="datetimerange"
        :unlink-panels="true"
        format="YYYY-MM-DD HH:mm"
        value-format="YYYY-MM-DD HH:mm"
        :range-separator="t('common.to')"
        :start-placeholder="t('common.startTime')"
        :end-placeholder="t('common.endTime')"
        class="h-40px max-w-460px"
        :editable="false"
        @change="changeTime"
      />
      <SearchInput
        v-model="model.name"
        :placeholder="t('common.logItem')"
        class="w-300px h-10"
      />
      <el-button class="rounded-full" data-cy="clean" @click="clean">{{
        t("common.reset")
      }}</el-button>
    </div>
    <div class="relative">
      <div class="absolute top-6px right-24px z-50">
        <el-button
          v-hasPermission="{ feature: 'sync', action: 'edit' }"
          round
          class="dark:bg-666"
          data-cy="pushAll"
          @click="realPush()"
          >{{ t("common.push") }}</el-button
        >
        <el-button
          v-hasPermission="{ feature: 'sync', action: 'edit' }"
          round
          class="dark:bg-666"
          data-cy="pull"
          @click="realPull()"
          >{{ t("common.pull") }}</el-button
        >
      </div>
      <el-tabs v-model="activeTab" @tab-change="updateTypes">
        <el-tab-pane
          v-for="tab in tabs"
          :key="tab.value"
          :label="tab.displayName"
          :name="tab.value"
        >
          <component
            :is="tab.component"
            v-if="activeTab === tab.value"
            :ref="(r: any) => (tab.tabInstance = r)"
            :type="tab.type"
            :conditions="tab.conditions"
            @go-pull="goPullTab"
          />
        </el-tab-pane>
      </el-tabs>
    </div>
    <SyncConflict
      v-if="showConflictDialog"
      v-model="showConflictDialog"
      :data="pushFeedBack!"
    />
  </div>
</template>
