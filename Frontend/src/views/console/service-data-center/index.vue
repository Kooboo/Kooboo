<script setup lang="ts">
import { ref } from "vue";
import KTable from "@/components/k-table";

import { useI18n } from "vue-i18n";
import {
  enableServer,
  getDataCenterList,
  disableServer,
  makeDefault,
} from "@/api/organization";
import { showConfirm } from "@/components/basic/confirm";
import { openInNewTab } from "@/utils/url";
import type { DataCenter } from "@/api/organization/types";
import Cookies from "universal-cookie";

const { t } = useI18n();
const cookies = new Cookies();

const serverDataList = ref(
  [] as (DataCenter & { $DisabledSelect?: boolean; id?: string })[]
);

const load = async () => {
  serverDataList.value = await getDataCenterList();
  serverDataList.value.forEach((item) => {
    item.$DisabledSelect = item.default;
    item.id = item.name;
  });
};

const goTo = (row: any) => {
  if (!row.enable) return;
  const token = cookies.get("jwt_token") || localStorage.getItem("TOKEN");
  openInNewTab(row.navUrl + "?auto_login=true&access_token=" + token);
};

const changeStatus = async (selectDataCenter: DataCenter) => {
  if (selectDataCenter.enable) {
    await showConfirm(t("common.areYouSureYouWantToDisableTheDataCenter"));
    await disableServer(selectDataCenter.name);
  } else {
    await showConfirm(t("common.areYouSureYouWantToEnableTheDataCenter"));
    await enableServer(selectDataCenter.name);
  }
  load();
};

const makeDefaultServer = async (selectDataCenter: DataCenter) => {
  await showConfirm(t("common.confirmDefaultDataCenter"));
  await makeDefault(selectDataCenter.name);
  load();
};

load();
</script>

<template>
  <div class="p-24">
    <KTable :data="serverDataList" show-check :is-radio="true" hide-delete>
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
            <el-button
              round
              :disabled="!selected[0]?.enable || !selected.length"
              @click="makeDefaultServer(selected[0])"
            >
              <el-icon
                class="iconfont icon-yes2 text-green"
                :class="
                  !selected[0]?.enable || !selected.length
                    ? 'text-opacity-50'
                    : ''
                "
              />
              <div class="flex items-center">
                {{ t("common.setAsDefault") }}
              </div>
            </el-button>
          </div>
        </div>
      </template>

      <el-table-column :label="t('common.name')" width="200">
        <template #default="{ row }">
          {{ row.name }}
        </template>
      </el-table-column>
      <el-table-column :label="t('common.description')">
        <template #default="{ row }">
          {{ row.description }}
        </template>
      </el-table-column>
      <el-table-column
        :label="t('common.enableStatus')"
        width="200"
        align="center"
      >
        <template #default="{ row }">
          <el-icon
            class="iconfont"
            :class="
              row.enable ? 'icon-yes2 text-green' : 'icon-Tips2 text-orange'
            "
          />
        </template>
      </el-table-column>

      <el-table-column :label="t('common.default')" width="200" align="center">
        <template #default="{ row }">
          <el-icon v-if="row.default" class="iconfont icon-yes2 text-green" />
        </template>
      </el-table-column>

      <el-table-column width="80" :label="t('common.goTo')" align="center">
        <template #default="{ row }">
          <el-tooltip placement="top" :content="t('common.goToDataCenter')">
            <el-icon
              class="iconfont icon-next cursor-pointer text-l"
              :class="!row.enable ? 'cursor-not-allowed text-999' : 'text-blue'"
              @click="goTo(row)"
            />
          </el-tooltip>
        </template>
      </el-table-column>
    </KTable>
  </div>
</template>
