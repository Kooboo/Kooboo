<script setup lang="ts">
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import {
  disableResourceCDN,
  enableResourceCDN,
  getResourceCDNList,
} from "@/api/console";
import type { ResourceCDN } from "@/api/console/types";
import { showConfirm } from "@/components/basic/confirm";

const { t } = useI18n();
const resourceCDNList = ref<ResourceCDN[]>([]);

const load = async () => {
  resourceCDNList.value = await getResourceCDNList();
};

const changeStatus = async (selectResourceCDN: ResourceCDN) => {
  if (selectResourceCDN.enable) {
    await showConfirm(t("common.areYouSureYouWantToDisableTheResourceCDN"));
    await disableResourceCDN(selectResourceCDN.id);
  } else {
    await showConfirm(t("common.areYouSureYouWantToEnableTheResourceCDN"));
    await enableResourceCDN(selectResourceCDN.id);
  }
  load();
};
load();
</script>

<template>
  <div class="p-24">
    <KTable :data="resourceCDNList" show-check :is-radio="true" hide-delete>
      <template #leftBar="{ selected }">
        <div class="h-60px flex items-center px-44px">
          <div>
            <el-button
              round
              :disabled="selected[0]?.enable || !selected.length"
              @click="changeStatus(selected[0])"
            >
              <el-icon
                class="iconfont icon-yes2 text-green"
                :class="
                  selected[0]?.enable || !selected.length
                    ? 'text-opacity-50'
                    : ''
                "
              />
              <div class="flex items-center">
                {{ t("common.enable") }}
              </div>
            </el-button>

            <el-button
              round
              :disabled="!selected[0]?.enable || !selected.length"
              @click="changeStatus(selected[0])"
            >
              <el-icon
                class="iconfont icon-Tips2 text-orange"
                :class="
                  !selected[0]?.enable || !selected.length
                    ? 'text-opacity-50'
                    : ''
                "
              />
              <div class="flex items-center">
                {{ t("common.disable") }}
              </div>
            </el-button>
          </div>
        </div>
      </template>
      <el-table-column :label="t('common.site')">
        <template #default="{ row }">
          {{ row.name }}
        </template>
      </el-table-column>
      <el-table-column :label="t('common.status')" width="200" align="center">
        <template #default="{ row }">
          <el-icon
            class="iconfont"
            :class="
              row.enable ? 'icon-yes2 text-green' : ' icon-Tips2 text-orange'
            "
          />
        </template>
      </el-table-column>
    </KTable>
  </div>
</template>
