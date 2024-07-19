<script setup lang="ts">
import { getWebhookLogs } from "@/api/commerce/settings";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { useTime } from "@/hooks/use-date";
import JsonTree from "@/views/dev-mode/components/debug/json-tree.vue";

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
  data.value = await getWebhookLogs({
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
      <el-table-column label="URL">
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
            class="text-blue cursor-pointer truncate"
            @click="
              preview = row;
              showPreviewDialog = true;
            "
            >{{ row.content }}</span
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
            :content="t('common.success')"
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
    :title="t('common.content')"
    width="1200px"
  >
    <div class="max-h-600px overflow-auto">
      <JsonTree :data="JSON.parse(preview.content)" />
    </div>
  </el-dialog>
</template>
