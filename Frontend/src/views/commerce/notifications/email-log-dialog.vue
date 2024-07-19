<script setup lang="ts">
import { getEmailLogs } from "@/api/commerce/settings";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { useTime } from "@/hooks/use-date";

interface PropsType {
  modelValue: boolean;
}
interface EmitsType {
  (e: "update:model-value", value: boolean): void;
}

const props = defineProps<PropsType>();
const emits = defineEmits<EmitsType>();
const { t } = useI18n();
const show = ref(true);
const data = ref();
const showPreviewDialog = ref(false);
const preview = ref();

const load = async (index?: number) => {
  data.value = await getEmailLogs({
    pagenr: index,
  });
};

load();
</script>
<template>
  <el-dialog
    v-model="show"
    width="1000px"
    :close-on-click-modal="false"
    :title="t('common.log')"
    @closed="emits('update:model-value', false)"
  >
    <KTable
      v-if="data"
      :data="data?.dataList!"
      :pagination="{
        currentPage: data?.pageNr,
        pageCount: data?.totalPages,
        pageSize: data?.pageSize,
      }"
      @change="load"
    >
      <el-table-column :label="t('common.email')">
        <template #default="{ row }">
          <span>{{ row.address }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.event')" width="180">
        <template #default="{ row }">
          <span>{{ row.event }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.content')">
        <template #default="{ row }">
          <span
            class="text-blue cursor-pointer"
            @click="
              preview = row;
              showPreviewDialog = true;
            "
            >{{ row.subject }}</span
          >
        </template>
      </el-table-column>
      <el-table-column :label="t('common.dateTime')" width="180">
        <template #default="{ row }">
          <span>{{ useTime(new Date(row.timestamp)) }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.status')" width="80" align="center">
        <template #default="{ row }">
          <el-tooltip
            v-if="!row.sent && row.errorCount"
            placement="top-start"
            :content="row.error"
          >
            <el-icon class="iconfont icon-fasongshibai text-orange" />
          </el-tooltip>
          <el-tooltip
            v-if="row.sent"
            placement="top-start"
            :content="t('common.deliverySuccessful')"
          >
            <el-icon class="iconfont icon-fasongchenggong text-green" />
          </el-tooltip>
        </template>
      </el-table-column>
    </KTable>
  </el-dialog>
  <el-dialog
    v-if="preview"
    v-model="showPreviewDialog"
    :title="preview?.subject"
    width="1200px"
  >
    <KFrame class="h-600px" :content="preview?.content" />
  </el-dialog>
</template>
