<template>
  <div class="p-24 space-y-24">
    <div
      class="w-full bg-[#E8F4FD] dark:bg-666 dark:text-fff p-16 border-1 border-solid border-blue dark:border-none text-s tracking-[1px] break-all"
    >
      <ul>
        <li>{{ t("order.Note") }}</li>
        <li>
          {{ t("order.NonRefundNotice") }}
        </li>
      </ul>
    </div>

    <KTable :data="list">
      <el-table-column width="10" />
      <el-table-column prop="title" :label="t('order.LineTitle')" />
      <el-table-column prop="totalAmount" :label="t('order.TotalAmount')">
        <template #default="{ row }">
          {{ row.totalAmount?.toFixed(2) }} CNY
        </template>
      </el-table-column>
      <el-table-column :label="t('common.date')">
        <template #default="{ row }">
          {{ useTime(row.creationDate) }}
        </template>
      </el-table-column>
      <el-table-column :label="t('order.IsPaid')">
        <template #default="{ row }">
          <BooleanTag :value="row.isPaid" />
        </template>
      </el-table-column>
      <el-table-column :label="t('order.IsDelivered')">
        <template #default="{ row }">
          <BooleanTag :value="row.isDelivered" />
        </template>
      </el-table-column>
      <el-table-column align="right">
        <template #default="{ row }">
          <ElButton
            v-if="row.canPay"
            round
            plain
            type="primary"
            @click="pay(row.id)"
          >
            {{ t("common.pay") }}
          </ElButton>
        </template>
      </el-table-column>
    </KTable>
  </div>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { getDomainOrder } from "@/api/market-order";
import type { DomainOrder } from "@/api/console/types";
import { useRouter } from "vue-router";
import { useTime } from "@/hooks/use-date";

const { t } = useI18n();

const list = ref<DomainOrder[]>([]);
const router = useRouter();

const load = async () => {
  list.value = await getDomainOrder();
};
load();

const pay = (id: string) => {
  router.push({
    name: "checkOrder",
    query: {
      orderId: id,
    },
  });
};
</script>
