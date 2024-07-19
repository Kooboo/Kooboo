<template>
  <div class="p-24">
    <KTable :data="dnsList">
      <el-table-column :label="t('common.name')">
        <template #default="{ row }">
          <span>
            {{ row.name }}
          </span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.region')" align="center">
        <template #default="{ row }">
          <span>
            {{ row.region }}
          </span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.domainCount')" align="center">
        <template #default="{ row }">
          <span>
            {{ row.domainCount }}
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
import { getDNS } from "@/api/partner";
import type { partnerDNS } from "@/api/partner/type";

const { t } = useI18n();

const dnsList = ref<partnerDNS[]>([]);
const load = async () => {
  dnsList.value = await getDNS();
};

load();
</script>
