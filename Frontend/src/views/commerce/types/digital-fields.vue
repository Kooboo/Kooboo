<script lang="ts" setup>
import type { ProductType } from "@/api/commerce/type";
import { useI18n } from "vue-i18n";

const { t } = useI18n();
defineProps<{ model: ProductType }>();
</script>

<template>
  <ElFormItem>
    <template #label>
      <span>{{ t("common.isDigitalProduct") }}</span>
      <Tooltip :tip="t('common.digitalProductTip')" custom-class="ml-4" />
    </template>
    <div class="flex items-center gap-12">
      <ElSwitch v-model="model.isDigital" />
      <el-checkbox
        v-if="model.isDigital"
        :label="t('common.limitDownloadCount')"
        :model-value="!!model.maxDownloadCount"
        @click.stop.prevent="
          model.maxDownloadCount = !!model.maxDownloadCount ? undefined : 3
        "
      />
      <ElInputNumber
        v-if="model.maxDownloadCount"
        v-model="model.maxDownloadCount"
        :min="1"
        size="small"
      />
    </div>
  </ElFormItem>
</template>
