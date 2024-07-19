<template>
  <div class="p-24">
    <KTable :data="recentList">
      <template #tableTopContent>
        <div
          class="h-60px flex items-center justify-between px-32 dark:text-999"
        >
          <span>
            {{ t("common.currentBalance") }}:
            <span class="ml-8 text-blue text-l">{{
              currentBalance?.amount
            }}</span>
            <span class="ml-8">{{ currentBalance?.currency }}</span>
          </span>
          <span class="text-blue cursor-pointer" @click="openRechargeDialog">{{
            t("common.recharge")
          }}</span>
        </div>
      </template>
      <el-table-column width="15" />
      <el-table-column :label="t('common.name')">
        <template #default="{ row }">
          <span>{{ row.name }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.date')">
        <template #default="{ row }">
          <span>{{ useTime(row.date) }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.Direction')">
        <template #default="{ row }">
          <el-icon
            class="iconfont text-m ml-8"
            :class="
              row.isUp ? ' icon-a-addto text-green' : ' icon-reduce text-orange'
            "
          />
        </template>
      </el-table-column>
      <el-table-column :label="t('common.amount')" width="120" align="center">
        <template #default="{ row }">
          <span>{{ row.amount }}</span>
        </template>
      </el-table-column>
    </KTable>
  </div>
  <RechargeDialog
    v-if="showRechargeDialog"
    v-model="showRechargeDialog"
    :balance="currentBalance"
  />
</template>

<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { useTime } from "@/hooks/use-date";
import RechargeDialog from "./recharge-dialog.vue";
import { getCurrentBalance, getRecentList } from "@/api/console";
import type { Balance, Recent } from "@/api/console/types";

const { t } = useI18n();
const showRechargeDialog = ref(false);
const recentList = ref<Recent[]>();
const currentBalance = ref<Balance>();

const openRechargeDialog = () => {
  showRechargeDialog.value = true;
};

const load = async () => {
  recentList.value = await getRecentList();
  currentBalance.value = await getCurrentBalance();
};

load();
</script>
