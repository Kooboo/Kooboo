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
    <div>
      <div>
        <ElSwitch v-model="model.isDigital" />
      </div>
      <div>
        <div class="h-24">
          <el-checkbox
            v-if="model.isDigital"
            :label="t('common.limitDownloadCount')"
            :model-value="!!model.maxDownloadCount"
            @click.stop.prevent="
              model.maxDownloadCount = !!model.maxDownloadCount ? undefined : 3
            "
          />
        </div>
        <div v-if="model.maxDownloadCount" class="flex items-center gap-4">
          <ElInputNumber v-model="model.maxDownloadCount" :min="1" />
          <span>{{ t("common.times") }}</span>
        </div>
        <div class="h-24">
          <el-checkbox
            v-if="model.isDigital"
            :label="t('common.limitDownloadTime')"
            :model-value="!!model.maxDownloadDay"
            @click.stop.prevent="
              model.maxDownloadDay = !!model.maxDownloadDay ? undefined : 3
            "
          />
        </div>
        <div v-if="model.maxDownloadDay" class="flex items-center gap-4">
          <ElInputNumber v-model="model.maxDownloadDay" :min="1" />
          <span>{{ t("common.days") }}</span>
        </div>
      </div>
    </div>
  </ElFormItem>
</template>
