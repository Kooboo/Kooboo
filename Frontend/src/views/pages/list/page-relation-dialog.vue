<script lang="ts" setup>
import { ref } from "vue";
import type { PageName } from "@/api/pages/types";
import { useI18n } from "vue-i18n";
import { usePreviewUrl } from "@/hooks/use-preview-url";

const { onPreview } = usePreviewUrl();

defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

defineProps<{
  modelValue: boolean;
  data: PageName[];
}>();
const { t } = useI18n();

const show = ref(true);
</script>

<template>
  <el-dialog
    v-model="show"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.relation')"
    @closed="$emit('update:modelValue', false)"
  >
    <el-scrollbar max-height="400px">
      <el-table :data="data" class="el-table--gray">
        <el-table-column :label="t('common.name')">
          <template #default="{ row }">
            <a
              class="cursor-pointer underline"
              data-cy="name"
              @click="onPreview(row.previewUrl)"
            >
              {{ row.name }}
            </a>
          </template>
        </el-table-column>
        <el-table-column :label="t('common.url')">
          <template #default="{ row }">
            {{ row.url }}
          </template>
        </el-table-column>
      </el-table>
    </el-scrollbar>
  </el-dialog>
</template>
