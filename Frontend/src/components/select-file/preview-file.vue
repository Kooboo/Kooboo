<template>
  <div class="mb-8 mr-32">
    <a
      :title="tooltip"
      :href="previewUrl"
      target="_blank"
      class="break-all inline-flex leading-normal"
      data-cy="preview-url"
      >{{ url }}
    </a>
    <a class="cursor-pointer ml-8" @click="emits('remove')">
      <el-icon class="text-orange iconfont icon-delete" />
    </a>
  </div>
</template>
<script setup lang="ts">
import { useSiteStore } from "@/store/site";
import { combineUrl } from "@/utils/url";
import { computed } from "vue";

const props = defineProps<{
  url: string;
  tooltip?: string;
}>();
const emits = defineEmits<{
  (e: "remove"): void;
}>();

const siteStore = useSiteStore();
const previewUrl = computed(() =>
  combineUrl(siteStore.site.baseUrl, props.url)
);
</script>
