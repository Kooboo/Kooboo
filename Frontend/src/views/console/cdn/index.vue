<script setup lang="ts">
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import EditCdnDialog from "./edit-cdn-dialog.vue";
import { getCDNList } from "@/api/console";
import type { CDN } from "@/api/console/types";

const { t } = useI18n();
const cdnList = ref<CDN[]>();
const editCdnDialog = ref(false);
const currentCDN = ref<CDN>();

const load = async () => {
  cdnList.value = await getCDNList();
};

const onEdit = (row: CDN) => {
  editCdnDialog.value = true;
  currentCDN.value = row;
};

load();
</script>

<template>
  <div class="p-24">
    <KTable :data="cdnList">
      <template #tableTopContent>
        <div class="h-60px flex items-center px-32 dark:text-999">
          {{ t("common.domainCDN") }}
        </div>
      </template>
      <el-table-column width="20" />
      <el-table-column :label="t('common.domain')">
        <template #default="{ row }">
          {{ row.domainName }}
        </template>
      </el-table-column>
      <el-table-column :label="t('common.status')" width="200" align="center">
        <template #default="{ row }">
          <el-icon
            class="iconfont"
            :class="
              row.siteCDN ? 'icon-yes2 text-green' : ' icon-Tips2 text-orange'
            "
          />
        </template>
      </el-table-column>
      <el-table-column label="icpCert" width="200">
        <template #default="{ row }">
          {{ row.icpCert }}
        </template>
      </el-table-column>
      <el-table-column width="100" align="right">
        <template #default="{ row }">
          <IconButton
            icon="icon-a-writein"
            :tip="t('common.edit')"
            @click="onEdit(row)"
          />
        </template>
      </el-table-column>
    </KTable>
  </div>
  <EditCdnDialog
    v-if="editCdnDialog"
    v-model="editCdnDialog"
    :cdn="currentCDN!"
    @edit-success="load"
  />
</template>
