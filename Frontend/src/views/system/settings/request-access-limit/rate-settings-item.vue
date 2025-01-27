<script lang="ts" setup>
import { useI18n } from "vue-i18n";
import { ref } from "vue";
import type { ReteSettings } from "@/api/site/site";

const props = defineProps<{
  item: { key: string; value: ReteSettings };
  placeholder: string;
}>();
defineEmits<{
  (e: "remove"): void;
}>();
const { t } = useI18n();
const editing = ref(!!(props.item as any).editing);
</script>

<template>
  <div v-if="editing" class="space-y-8">
    <div class="flex items-center gap-4">
      <ElInput v-model="item.key" :placeholder="placeholder" />
      <IconButton
        circle
        class="hover:text-orange text-orange"
        icon="icon-delete "
        :tip="t('common.delete')"
        @click="$emit('remove')"
      />
    </div>
    <div class="flex items-end">
      <div class="grid grid-cols-2 w-full">
        <el-form-item :label="t('common.withinSeconds')">
          <el-input-number v-model="item.value.withinSeconds" :min="1" />
        </el-form-item>
        <el-form-item :label="t('common.permitLimit')">
          <el-input-number v-model="item.value.permitLimit" :min="1" />
        </el-form-item>
      </div>
      <ElButton round type="primary" @click="editing = false">{{
        t("common.done")
      }}</ElButton>
    </div>
  </div>
  <div v-else class="cursor-pointer" @click="editing = true">
    <span class="dark:text-[#cfd3dc]">{{
      t("common.rateLimitTip", {
        ip: item.key,
        seconds: item.value.withinSeconds,
        count: item.value.permitLimit,
      })
    }}</span>
  </div>
</template>
