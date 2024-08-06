<script lang="ts" setup>
import { useI18n } from "vue-i18n";
defineProps<{
  top: any;
}>();

defineEmits<{
  (e: "selected", value: string): void;
}>();

const { t } = useI18n();
</script>

<template>
  <div class="py-16 px-24">
    <div v-if="top?.topRecommends">
      <div class="text-999 py-8">{{ t("common.recommends") }}</div>
      <div class="flex gap-4 flex-wrap">
        <el-tag
          v-for="item of top.topRecommends"
          :key="item.id"
          :title="item.description"
          class="cursor-pointer"
          round
          @click="$emit('selected', item.title)"
          >{{ item.title }}</el-tag
        >
      </div>
    </div>
    <div v-if="top?.topDownloads">
      <div class="text-999 py-8 mt-8">{{ t("common.mostDownloads") }}</div>
      <div class="flex gap-4 flex-wrap">
        <el-tag
          v-for="item of top.topDownloads"
          :key="item.id"
          :title="item.description"
          class="cursor-pointer"
          round
          @click="$emit('selected', item.title)"
          >{{ item.title }}</el-tag
        >
      </div>
    </div>
  </div>
</template>
