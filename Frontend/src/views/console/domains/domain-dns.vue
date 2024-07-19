<template>
  <div class="p-24">
    <div class="flex items-center pb-24 space-x-16">
      <el-button round @click="resolutionSettingDialog = true">
        <div class="flex items-center">
          {{ t("common.add") }}
        </div>
      </el-button>
    </div>
    <KTable :data="dnsRecordList" show-check @delete="onDelete">
      <el-table-column :label="t('common.host')" width="200">
        <template #default="{ row }">
          <span class="text-blue ellipsis">
            {{ row.host }}
          </span>
        </template>
      </el-table-column>
      <el-table-column
        :label="t('common.recordType')"
        width="200"
        align="center"
      >
        <template #default="{ row }">
          {{ row.type }}
        </template>
      </el-table-column>
      <el-table-column :label="t('common.recordValue')" align="center">
        <template #default="{ row }">
          {{ row.value }}
        </template>
      </el-table-column>

      <el-table-column label="TTL" width="100" align="center">
        <template #default="{ row }">
          <span class="text-blue cursor-pointer">{{ row.ttl }}</span>
        </template>
      </el-table-column>
    </KTable>
    <ResolutionSettingDialog
      v-if="resolutionSettingDialog"
      v-model="resolutionSettingDialog"
      @create-success="load"
    />
    <KBottomBar @cancel="router.back()" />
  </div>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import { useRouter, useRoute } from "vue-router";
import { useI18n } from "vue-i18n";
import KTable from "@/components/k-table";
import ResolutionSettingDialog from "./resolution-setting-dialog.vue";
import { deleteDns, getDnsRecords } from "@/api/console";
import KBottomBar from "@/components/k-bottom-bar/index.vue";
import type { DNS } from "@/api/console/types";

const { t } = useI18n();
const route = useRoute();
const router = useRouter();
const resolutionSettingDialog = ref(false);
const dnsRecordList = ref<DNS[]>();

const load = async () => {
  dnsRecordList.value = await getDnsRecords(route.query.domainName as string);
};
const onDelete = async (rows: any) => {
  const ids = rows.map((item: any) => item.id);
  await deleteDns(ids);
  load();
};
load();
</script>
