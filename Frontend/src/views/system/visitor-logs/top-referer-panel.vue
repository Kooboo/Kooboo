<script lang="ts" setup>
import KTable from "@/components/k-table";
import { ref, watch } from "vue";
import { getTopReferer } from "@/api/visitor-log";
import type { TopPage } from "@/api/visitor-log/types";

import { useI18n } from "vue-i18n";
const props = defineProps<{ week: string }>();
const { t } = useI18n();
const data = ref<TopPage[]>([]);

const load = async () => {
  data.value = await getTopReferer(props.week);
};

watch(
  () => props.week,
  (val) => {
    val && load();
  },
  { immediate: true }
);
</script>

<template>
  <KTable :data="data">
    <el-table-column label="URL" prop="name" />
    <el-table-column :label="t('visitorLog.views')" prop="count" />
  </KTable>
</template>
