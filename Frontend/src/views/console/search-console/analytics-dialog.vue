<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { getSearchAnalytics } from "@/api/search-console";
import { useTime } from "@/hooks/use-date";

const endDate = new Date();
const startDate = new Date();
const aggregationType = ref("auto");
startDate.setTime(startDate.getTime() - 3600 * 1000 * 24 * 30);

const dateRange = ref<any>([startDate, endDate]);
const list = ref<any>([]);
const props = defineProps<{ modelValue: boolean; siteUrl: string }>();
const { t } = useI18n();
const show = ref(true);

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

async function load() {
  list.value = (
    await getSearchAnalytics({
      siteUrl: props.siteUrl,
      startDate: dateRange.value[0],
      endDate: dateRange.value[1],
      aggregationType: aggregationType.value,
    })
  ).rows;
}

load();

const aggregationTypes = [
  {
    key: "auto",
    value: t("common.auto"),
  },
  {
    key: "byPage",
    value: t("console.byPage"),
  },
  {
    key: "byProperty",
    value: t("common.byProperty"),
  },
];
</script>

<template>
  <el-dialog
    :model-value="show"
    width="1000px"
    :close-on-click-modal="false"
    :title="t('common.analytics')"
    @closed="emit('update:modelValue', false)"
  >
    <div>
      <div class="flex items-center pb-24 space-x-16 flex">
        <ElSelect v-model="aggregationType">
          <ElOption
            v-for="i in aggregationTypes"
            :key="i.key"
            :value="i.key"
            :label="i.value"
          />
        </ElSelect>
        <el-date-picker
          v-model="dateRange"
          type="daterange"
          unlink-panels
          :range-separator="t('common.to')"
          :start-placeholder="t('common.startDate')"
          :end-placeholder="t('common.endDate')"
        />
        <ElButton type="primary" round @click="load">{{
          t("common.search")
        }}</ElButton>
      </div>
      <KTable :data="list">
        <el-table-column :label="t('common.clicks')" prop="clicks" />
        <el-table-column :label="t('common.ctr')" prop="ctr" />
        <el-table-column :label="t('common.impressions')" prop="impressions" />
        <el-table-column :label="t('console.keys')" prop="keys">
          <template #default="{ row }">
            {{ row.keys?.join(" ") }}
          </template>
        </el-table-column>
      </KTable>
    </div>
  </el-dialog>
</template>
