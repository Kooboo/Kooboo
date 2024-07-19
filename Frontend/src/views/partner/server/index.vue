<template>
  <div class="p-24">
    <KTable :data="serverList">
      <el-table-column label="IP">
        <template #default="{ row }">
          <span>
            {{ row.ip }}
          </span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.region')">
        <template #default="{ row }">
          <span>
            {{ row.region }}
          </span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.domain')" align="center">
        <template #default="{ row }">
          <span>
            {{ row.domain }}
          </span>
        </template>
      </el-table-column>
      <!--
      <el-table-column :label="t('common.userCount')" align="center">
        <template #default="{ row }">
          <span>
            {{ row.userCount }}
          </span>
        </template>
      </el-table-column>-->
      <el-table-column :label="t('common.requireICP')" align="center">
        <template #default="{ row }">
          <span>
            {{ row.requireICP ? t("common.yes") : t("common.no") }}
          </span>
        </template>
      </el-table-column>
    </KTable>
  </div>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import KTable from "@/components/k-table";
import { useI18n } from "vue-i18n";
import { getPartnerServer } from "@/api/partner";
import type { partnerServer } from "@/api/partner/type";

const { t } = useI18n();

const serverList = ref<partnerServer[]>([]);
const load = async () => {
  serverList.value = await getPartnerServer();
};

load();
</script>
