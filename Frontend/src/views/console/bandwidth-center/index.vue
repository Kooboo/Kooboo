<script setup lang="ts">
import { ref } from "vue";
import { useRouter } from "vue-router";
import { useI18n } from "vue-i18n";

const { t } = useI18n();
const router = useRouter();
const bandwidthList = ref();

const load = async () => {
  // flowList.value = await getDataCenterList();
};

load();
</script>

<template>
  <div class="p-24">
    <div class="dark:text-gray">
      {{ t("common.remainingBandwidth") }}:
      <span class="mx-16 text-orange">120</span>
      <el-button round @click="router.push({ name: 'topupBandwidth' })">
        <div class="flex items-center">
          {{ t("common.topupBandwidth") }}
        </div>
      </el-button>
    </div>
    <!-- <KTable :data="flowList">
      <template #tableTopContent>
        <div class="h-60px flex items-center px-32">
          {{ t("common.bandwidthUsageInRecentSevenDays") }}
        </div>
      </template>
      <el-table-column :label="t('common.date')" width="200">
        <template #default="{ row }">
          {{ row.name }}
        </template>
      </el-table-column>
      <el-table-column :label="t('common.usage')">
        <template #default="{ row }">
          {{ row.description }}
        </template>
      </el-table-column>
    </KTable> -->
  </div>
</template>
