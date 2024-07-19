<script lang="ts" setup>
import { useI18n } from "vue-i18n";
defineProps<{
  enable: boolean;
  enableVersion: boolean;
  minutes: number;
  queryKeys?: string;
}>();

defineEmits<{
  (e: "update:enable", value: boolean): void;
  (e: "update:enableVersion", value: boolean): void;
  (e: "update:minutes", value?: number): void;
  (e: "update:queryKeys", value: string): void;
}>();
const { t } = useI18n();
</script>

<template>
  <div class="px-24 py-16">
    <el-form label-position="top">
      <el-form-item :label="t('common.enable')">
        <el-switch
          :model-value="enable"
          data-cy="enable-cache"
          @update:model-value="$emit('update:enable', !!$event)"
        />
      </el-form-item>
      <template v-if="enable">
        <el-form-item :label="t('common.cacheByVersion')">
          <el-switch
            :model-value="enableVersion"
            data-cy="enable-cache-by-version"
            @update:model-value="$emit('update:enableVersion', !!$event)"
          />
        </el-form-item>
        <el-form-item v-if="!enableVersion" :label="t('common.timeMinutes')">
          <el-input-number
            :model-value="minutes"
            data-cy="cache-time"
            @update:model-value="$emit('update:minutes', $event)"
          />
        </el-form-item>
        <el-form-item :label="t('common.cacheQueryKeys')">
          <el-input
            :model-value="queryKeys"
            data-cy="cache-query-keys"
            @update:model-value="$emit('update:queryKeys', $event)"
          />
        </el-form-item>
      </template>
    </el-form>
  </div>
</template>
