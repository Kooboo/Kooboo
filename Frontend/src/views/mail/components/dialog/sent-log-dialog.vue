<template>
  <el-dialog
    :model-value="show"
    :title="t('common.deliveryLogs')"
    width="60%"
    @closed="emits('update:modelValue', false)"
  >
    <div class="mb-12">
      {{ t("common.deliveryStatus") }}:<span
        class="ml-4"
        :class="viewSendState(currentLog, 'style')"
        >{{ viewSendState(currentLog, "view") }}</span
      >
    </div>
    <KTable :max-height="400" :data="currentLog?.items || []">
      <el-table-column :label="t('mail.to')">
        <template #default="{ row }">
          <div class="flex items-center">
            <span class="break-normal ellipsis" :title="row.to">{{
              row.to
            }}</span>
          </div>
        </template>
      </el-table-column>
      <el-table-column
        :label="t('common.deliveryStatus')"
        width="240"
        align="center"
      >
        <template #default="{ row }">
          <span
            class="mr-4"
            :class="{
              'text-green': row.isSuccess && !row.isSending,
              'text-999': row.isSending,
              'text-orange': !row.isSuccess && !row.isSending,
            }"
            >{{
              row.isSending
                ? t("common.Delivering")
                : row.isSuccess && !row.isSending
                ? t("common.deliverySuccessful")
                : t("common.deliveryFailed")
            }}
          </span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.dateTime')" align="center" width="180">
        <template #default="{ row }">
          <span v-if="!row.isSending" class="font-bold">{{
            useTime(row.deliveryTime)
          }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.log')" align="center" width="120">
        <template #default="{ row }">
          <el-popover
            placement="left-start"
            trigger="hover"
            popper-class="!w-auto max-w-800px !p-0"
          >
            <template #reference>
              <IconButton
                v-if="!row.isSending"
                icon="icon-eyes"
                :tip="t('common.details')"
                data-cy="preview"
              />
            </template>
            <el-scrollbar max-height="50vh" class="p-12 !break-normal">
              <pre>{{ row.log }}</pre>
            </el-scrollbar>
          </el-popover>
        </template>
      </el-table-column>
    </KTable>
  </el-dialog>
</template>
<script setup lang="ts">
import type { DeliveryLog } from "@/api/mail/types";
import { useTime } from "@/hooks/use-date";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import KTable from "@/components/k-table";

const show = ref(true);
const { t } = useI18n();

defineProps<{
  modelValue: boolean;
  currentLog: { isSuccess: boolean; items: DeliveryLog[] } | null;
  viewSendState: any;
}>();
const emits = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();
</script>
