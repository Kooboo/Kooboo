<template>
  <el-dialog
    :model-value="show"
    :title="t('common.scanResult')"
    width="800px"
    custom-class=" el-dialog--zero-padding"
    :close-on-click-modal="false"
    @closed="closeDialog"
  >
    <el-scrollbar max-height="50vh" class="py-8">
      <div v-if="!result.length && showTaskCompleted" class="px-32px py-12">
        {{ t("common.noIssueFound") }}
      </div>
      <div
        v-for="item of result"
        :key="item.name"
        class="hover:bg-[#EFF6FF] dark:hover:bg-666 px-32px space-y-4 pt-8"
      >
        <div class="text-l font-bold">{{ item.name }}</div>
        <div class="text-999 p-4" v-html="item.description" />
        <el-divider class="m-0" />
      </div>
    </el-scrollbar>
    <div class="px-32px py-12 text-center">
      <el-icon
        v-if="showTaskCompleted"
        class="iconfont icon-yes2 text-green mr-8"
      />
      {{
        showTaskCompleted
          ? t("common.taskExecutionCompleted")
          : t("common.taskIsExecuting")
      }}
    </div>
  </el-dialog>
</template>

<script lang="ts" setup>
import { ref, onUnmounted } from "vue";
import { useI18n } from "vue-i18n";
import {
  ExecuteTask as ExecuteTaskAPI,
  getStatus,
} from "@/api/development/automation";
const { t } = useI18n();
const result = ref<
  {
    name: string;
    description: string;
  }[]
>([]);
const show = ref(true);
const showTaskCompleted = ref(false);

const props = defineProps<{
  modelValue: boolean;
  id: string;
}>();
const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();
let timer: any;

const load = async () => {
  executeTask();
};

const closeDialog = () => {
  emit("update:modelValue", false);
  clearInterval(timer);
};

const executeTask = async () => {
  ExecuteTaskAPI(props.id).then(async (res: any) => {
    timer = setInterval(async () => {
      let status: any = await getStatus(res);
      if (!status.isEnd) {
        result.value.push(...status.results);
      } else {
        result.value.push(...status.results);
        showTaskCompleted.value = true;
        clearInterval(timer);
      }
    }, 1000);
  });
};

onUnmounted(() => clearInterval(timer));
load();
</script>
<style scoped>
:deep(a) {
  text-decoration: underline;
}
</style>
