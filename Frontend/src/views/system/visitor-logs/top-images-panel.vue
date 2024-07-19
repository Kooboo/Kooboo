<script lang="ts" setup>
import KTable from "@/components/k-table";
import { ref, watch } from "vue";
import { getTopImages } from "@/api/visitor-log";
import { bytesToSize } from "@/utils/common";
import type { TopImage } from "@/api/visitor-log/types";
import { useSiteStore } from "@/store/site";
import { combineUrl } from "@/utils/url";

import { useI18n } from "vue-i18n";
const props = defineProps<{ week: string }>();
const { t } = useI18n();
const data = ref<TopImage[]>([]);
const siteStore = useSiteStore();

const load = async () => {
  data.value = await getTopImages(props.week);
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
    <el-table-column label="URL">
      <template #default="{ row }">
        <div class="flex items-center">
          <div class="w-80px h-44px mr-24 flex items-center justify-center">
            <img :src="combineUrl(siteStore.site.baseUrl, row.thumbNail)" />
          </div>
          <span class="text-l font-bold flex-1 ellipsis" :title="row.name">{{
            row.name
          }}</span>
        </div>
      </template>
    </el-table-column>
    <el-table-column :label="t('common.size')">
      <template #default="{ row }"> {{ bytesToSize(row.size) }} </template>
    </el-table-column>
    <el-table-column
      :label="t('visitorLog.views')"
      prop="count"
      width="100px"
      align="center"
    />
  </KTable>
</template>
