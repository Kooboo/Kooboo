<script lang="ts" setup>
import KTable from "@/components/k-table";
import {
  getLogs,
  blame,
  restore,
  exportBatch,
  exportItems,
  getOptions,
  getLogVideo,
  cleanLogApi,
  cleanLogStatusApi,
  GetCleanLogRunningTask,
} from "@/api/site-log";
import { ref, watch, onMounted, onUnmounted } from "vue";
import type { KeyValue, PaginationResponse } from "@/global/types";
import type { Log, LogOptions } from "@/api/site-log/types";
import { useTime } from "@/hooks/use-date";
import ActionTag from "@/components/k-tag/action-tag.vue";
import ObjectTypeTag from "@/components/k-tag/object-type-tag.vue";
import CheckoutSiteDialog from "./checkout-site-dialog.vue";
import { useRouter } from "vue-router";
import { useRouteSiteId } from "@/hooks/use-site-id";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import type { DateModelType } from "element-plus";
import { ElMessage } from "element-plus";
import { searchDebounce } from "@/utils/url";
import { useI18n } from "vue-i18n";
import MonacoEventPlayer from "@/components/monaco-event-player/index.vue";
import { showConfirm } from "@/components/basic/confirm";

const { t } = useI18n();
const data = ref<PaginationResponse<Log>>();
const selectedId = ref(0);
const router = useRouter();
const showCheckoutDialog = ref(false);

const options = ref<LogOptions>();
const actionTypes = ref<KeyValue[]>();
const objectTypes = ref<KeyValue[]>();
const users = ref<KeyValue[]>();

const model = ref<{
  startDate: string;
  endDate: string;
  userId: string;
  name: string;
  storeName: string;
  actionType: string;
  pageNr: number;
  [key: string]: string | number;
}>({
  startDate: "",
  endDate: "",
  userId: "",
  name: "",
  storeName: "",
  actionType: "",
  pageNr: 1,
});

const dateValue = ref<[DateModelType, DateModelType]>([
  model.value.startDate,
  model.value.endDate,
]);

const getSiteLogs = async (pageNr?: number) => {
  if (pageNr && typeof pageNr === "number") {
    model.value.pageNr = pageNr;
  }
  model.value.name = model.value.name.trim();
  data.value = await getLogs(model.value);
};

const onBlame = async (selected: Log[]) => {
  await blame(selected.map((m) => m.id));
  await getSiteLogs();
  await updateActionTypes();
};

const onRestore = async (selected: Log) => {
  await restore(selected.id);
  await getSiteLogs();
  await updateActionTypes();
};

const onCheckout = async (selected: Log) => {
  selectedId.value = selected.id;
  showCheckoutDialog.value = true;
};

const onExportChanges = async (selected: Log) => {
  await exportBatch(selected.id);
};

const onExportItems = async (selected: Log[]) => {
  await exportItems(selected.map((m) => m.id));
};

const updateActionTypes = async () => {
  options.value = await getOptions();
  actionTypes.value = options.value["editType"];
};

const clean = () => {
  dateValue.value = ["", ""];
  Object.keys(model.value).forEach((key) => {
    if (key === "name") return;
    if (key !== "pageMr") {
      model.value[key] = "";
    } else {
      model.value[key] = 1;
    }
  });
  // 有name的时候调用watch里面的getSiteLogs，没name的时候手动更新
  if (!model.value.name) {
    getSiteLogs(1);
  } else {
    model.value.name = "";
  }
};

const cleanSiteLog = async () => {
  await showConfirm(t("common.cleanLogTips"));
  const taskId = await cleanLogApi();
  refreshTask(taskId);
};
const search = searchDebounce(() => getSiteLogs(1), 1000);
watch(
  () => model.value.name,
  () => {
    if (model.value.name) {
      search();
    } else {
      getSiteLogs(1);
    }
  }
);
const load = async () => {
  data.value = await getLogs(model.value);
  options.value = await getOptions();
  objectTypes.value = options.value["storeName"];
  actionTypes.value = options.value["editType"];
  users.value = options.value["user"];
};
load();
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
  getSiteLogs(1);
};

async function playVideo(versionId: number) {
  const video: any = await getLogVideo(versionId);
  // console.log(video); //replace with real play code video

  const events = JSON.parse(video);
  if (playerControl.value) {
    playerControl.value.load(versionId, events);
  }
}

const playerControl = ref<any>(null);

const logTask = ref({
  id: "",
  totalItem: 0,
  finishedItems: 0,
  isFinished: true,
});

async function refreshTask(taskId?: string) {
  if (taskId) {
    logTask.value.id = taskId;
    logTask.value.isFinished = false;
  }

  // 如果不存在任务
  if (!logTask.value.id) {
    logTask.value.totalItem = 0;
    logTask.value.finishedItems = 0;
    logTask.value.isFinished = true;
    return;
  }

  const { totalItem, finishedItems, isFinished } = await cleanLogStatusApi(
    logTask.value.id
  );

  // 完成提醒
  if (isFinished) {
    ElMessage.success(t("common.logCleaningInProgress"));
    logTask.value.id = "";
    logTask.value.totalItem = 0;
    logTask.value.finishedItems = 0;
    logTask.value.isFinished = true;
    await load();
  } else {
    logTask.value.totalItem = totalItem;
    logTask.value.finishedItems = finishedItems;
    logTask.value.isFinished = isFinished;
  }
}

const timer = ref();

// 每一秒扫描一次任务状态
timer.value = setInterval(async () => {
  if (!logTask.value.isFinished) {
    refreshTask();
  }
}, 3000);

// 进入后立刻查询是否有任务在执行
GetCleanLogRunningTask().then((task) => {
  if (task && task.id && !task.isFinished) {
    refreshTask(task.id);
  }
});

onUnmounted(() => {
  clearInterval(timer.value);
});
</script>
<template>
  <div class="p-24">
    <Breadcrumb :name="t('common.siteLogs')" />
    <div class="flex space-x-16 mt-16">
      <el-select
        v-model="model.storeName"
        :placeholder="t('common.objectType')"
        clearable
        class="w-180px"
        @change="getSiteLogs(1)"
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
        clearable
        class="w-140px"
        @change="getSiteLogs(1)"
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
        @change="getSiteLogs(1)"
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
        class="w-250px h-10"
      />
      <el-button class="rounded-full" data-cy="reset" @click="clean">{{
        t("common.reset")
      }}</el-button>
      <el-button
        class="rounded-full"
        data-cy="cleanLog"
        :disabled="!data?.list?.length"
        @click="cleanSiteLog"
        >{{ t("common.cleanLog") }}</el-button
      >
    </div>

    <div v-if="logTask.isFinished">
      <KTable
        v-if="data"
        :data="data?.list"
        show-check
        hide-delete
        :pagination="{
          currentPage: data.pageNr,
          pageCount: data.totalPages,
          pageSize: data.pageSize,
        }"
        class="mt-16"
        @change="getSiteLogs"
      >
        <template #bar="{ selected }">
          <el-tooltip
            v-if="selected.length > 0"
            placement="top"
            :content="t('common.undoChangeTip')"
          >
            <el-button round data-cy="undo-changes" @click="onBlame(selected)">
              {{
                selected.length === 1
                  ? t("common.undoChange")
                  : t("common.undoChanges")
              }}
            </el-button>
          </el-tooltip>

          <el-tooltip
            v-if="selected.length === 1"
            placement="top"
            :content="t('common.restoreTip')"
          >
            <el-button
              round
              data-cy="restore-to-point"
              @click="onRestore(selected[0])"
            >
              {{ t("common.restore") }}
            </el-button>
          </el-tooltip>

          <el-tooltip
            v-if="selected.length === 1"
            placement="top"
            :content="t('common.checkoutTip')"
          >
            <el-button
              round
              data-cy="checkout"
              @click="onCheckout(selected[0])"
            >
              {{ t("common.checkout") }}
            </el-button>
          </el-tooltip>

          <el-tooltip
            v-if="selected.length === 1"
            placement="top"
            :content="t('common.exportChangesTip')"
          >
            <el-button
              round
              data-cy="export-changes"
              @click="onExportChanges(selected[0])"
            >
              {{ t("common.exportChanges") }}
            </el-button>
          </el-tooltip>

          <el-tooltip
            v-if="selected.length > 0"
            placement="top"
            :content="t('common.exportItemTip')"
          >
            <el-button
              round
              data-cy="export-item"
              @click="onExportItems(selected)"
            >
              {{
                selected.length === 1
                  ? t("common.exportItem")
                  : t("common.exportItems")
              }}
            </el-button>
          </el-tooltip>
        </template>
        <el-table-column label="ID" prop="id" width="80px" />
        <el-table-column :label="t('common.logItem')">
          <template #default="{ row }">
            <span
              :title="row.itemName"
              class="ellipsis text-blue cursor-pointer"
              data-cy="log-item"
              @click="
                router.push(
                  useRouteSiteId({
                    name: 'log-versions',
                    query: {
                      keyHash: row.keyHash,
                      storeNameHash: row.storeNameHash,
                      tableNameHash: row.tableNameHash,
                    },
                  })
                )
              "
              >{{ row.itemName }}</span
            >
          </template>
        </el-table-column>
        <el-table-column :label="t('common.objectType')" width="200px">
          <template #default="{ row }">
            <ObjectTypeTag :type="row.storeName" />
          </template>
        </el-table-column>
        <el-table-column :label="t('common.action')" width="120px">
          <template #default="{ row }">
            <ActionTag :type="row.actionType" />
          </template>
        </el-table-column>
        <el-table-column :label="t('common.user')" width="150px">
          <template #default="{ row }">
            <span data-cy="username">{{ row.userName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="t('common.lastModified')" width="180px">
          <template #default="{ row }">
            <span data-cy="last-modified">
              {{ useTime(row.lastModified) }}
            </span>
          </template>
        </el-table-column>
        <el-table-column align="right" width="120px">
          <template #default="{ row }">
            <IconButton
              v-if="row.hasVideo"
              icon="icon-ve-video"
              :tip="t('common.playVideo')"
              :permission="{ feature: 'site', action: 'log' }"
              @click="playVideo(row.id)"
            />
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
    </div>
    <div
      v-else
      class="shadow-s-10 rounded-normal overflow-hidden flex flex-col bg-fff dark:bg-[#303030] mt-16"
    >
      <div class="flex justify-center items-center h-full">
        <el-result
          :title="t('common.taskIsExecuting')"
          :sub-title="
            t('common.cleanLogRemaining', {
              total: logTask.totalItem,
              count: logTask.finishedItems,
            })
          "
        >
          <template #icon>
            <el-icon
              class="iconfont icon-bufenchenggong text-56px text-green"
            />
          </template>
        </el-result>
      </div>
    </div>
  </div>

  <MonacoEventPlayer @ready="(e:any) => playerControl=e" />
  <CheckoutSiteDialog
    v-if="showCheckoutDialog"
    :id="selectedId"
    v-model="showCheckoutDialog"
  />
</template>
