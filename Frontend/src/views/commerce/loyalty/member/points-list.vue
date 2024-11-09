<script lang="ts" setup>
import type { PagingResult } from "@/api/commerce/common";
import {
  redeemPoints,
  pointList,
  earnPoints,
  type CustomerPoint,
} from "@/api/commerce/loyalty";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { useTime } from "@/hooks/use-date";

const { t } = useI18n();
const props = defineProps<{
  memberId: string;
  maxPoints: number;
}>();
const emit = defineEmits<{
  (e: "reload"): void;
}>();
const pagingResult = ref<PagingResult<CustomerPoint>>();
const showTopUpDialog = ref(false);
const showConsumeDialog = ref(false);
const points = ref(10);
const description = ref("");

const changeTypes = [
  { key: "ManualRedeem", value: t("common.manualConsume") },
  { key: "ManualEarn", value: t("common.manualTopUp") },
  { key: "OrderEarn", value: t("common.orderReward") },
  { key: "OrderRedeem", value: t("common.orderConsume") },
  { key: "LoginEarn", value: t("common.loginReward") },
];

const params = {
  pageIndex: 1,
  pageSize: 5,
  id: props.memberId,
};

async function load(index: number) {
  params.pageIndex = index;
  pagingResult.value = await pointList(params);
}

function showTopUp() {
  points.value = 10;
  description.value = "";
  showTopUpDialog.value = true;
}

function showConsume() {
  points.value = 10;
  description.value = "";
  showConsumeDialog.value = true;
}

async function topUp() {
  await earnPoints(props.memberId, points.value, description.value);
  showTopUpDialog.value = false;
  load(1);
  emit("reload");
}

async function consume() {
  await redeemPoints(props.memberId, points.value, description.value);
  showConsumeDialog.value = false;
  load(1);
  emit("reload");
}

load(1);
</script>

<template>
  <div>
    <div class="flex items-center pb-12 space-x-16">
      <el-button
        v-hasPermission="{ feature: 'loyalty', action: 'edit' }"
        round
        type="primary"
        @click="showTopUp"
      >
        <div class="flex items-center">
          <el-icon class="mr-16 iconfont icon-a-addto" />
          {{ t("common.topUp") }}
        </div>
      </el-button>

      <el-button
        v-hasPermission="{ feature: 'loyalty', action: 'edit' }"
        round
        @click="showConsume"
      >
        <div class="flex items-center">
          <el-icon class="mr-16 iconfont icon-zhifu" />
          {{ t("common.consume") }}
        </div>
      </el-button>
      <div class="flex-1" />
    </div>
    <ElTable
      v-if="pagingResult"
      :data="pagingResult.list"
      class="el-table--gray"
      :row-class-name="({ row }:any) => (row.disabled ? 'opacity-50' : '')"
    >
      <el-table-column :label="t('common.type')">
        <template #default="{ row }">
          {{ changeTypes.find((f) => f.key == row.type)?.value }}
        </template>
      </el-table-column>
      <el-table-column :label="t('commerce.points')">
        <template #default="{ row }">
          {{ row.points }}
        </template>
      </el-table-column>
      <el-table-column :label="t('common.date')">
        <template #default="{ row }">
          {{ useTime(row.createdAt + "Z") }}
        </template>
      </el-table-column>

      <el-table-column :label="t('common.description')">
        <template #default="{ row }">
          {{ row.description }}
        </template>
      </el-table-column>
    </ElTable>
    <div class="text-center">
      <el-pagination
        class="py-8"
        layout="prev, pager, next"
        hide-on-single-page
        :page-size="pagingResult?.pageSize"
        :current-page="pagingResult?.pageIndex"
        :total="pagingResult?.count"
        @current-change="load"
      />
    </div>

    <el-dialog v-model="showTopUpDialog" :title="t('common.topUp')">
      <ElForm label-position="top">
        <ElFormItem :label="t('commerce.points')">
          <ElInputNumber v-model="points" :min="1" />
        </ElFormItem>
        <ElFormItem :label="t('common.description')">
          <ElInput v-model="description" type="textarea" />
        </ElFormItem>
      </ElForm>
      <template #footer>
        <DialogFooterBar @confirm="topUp" @cancel="showTopUpDialog = false" />
      </template>
    </el-dialog>
    <el-dialog v-model="showConsumeDialog" :title="t('common.consume')">
      <ElForm label-position="top">
        <ElFormItem :label="t('commerce.points')">
          <ElInputNumber v-model="points" :min="1" :max="maxPoints" />
        </ElFormItem>
        <ElFormItem :label="t('common.description')">
          <ElInput v-model="description" type="textarea" />
        </ElFormItem>
      </ElForm>
      <template #footer>
        <DialogFooterBar
          @confirm="consume"
          @cancel="showConsumeDialog = false"
        />
      </template>
    </el-dialog>
  </div>
</template>
