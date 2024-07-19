<script lang="ts" setup>
import DiffView from "@/components/monaco-editor/diff.vue";
import type { Compare } from "@/api/site-log/types";
import { VersionDataType } from "@/api/site-log/types";
import { getCompare } from "@/api/site-log";
import { ref, computed } from "vue";
import { getQueryString } from "@/utils/url";
import DiffImage from "./diff-image.vue";

import { useI18n } from "vue-i18n";
const { t } = useI18n();

const data = ref<Compare>();

const diffComponent = computed(() => {
  if (data.value?.dataType === VersionDataType.Image) {
    return DiffImage;
  }

  return DiffView;
});
getCompare(getQueryString("left")!, getQueryString("right")!).then((rsp) => {
  data.value = rsp;
});
</script>

<template>
  <div class="flex flex-col h-full">
    <div
      class="py-16 px-32 bg-fff dark:bg-[#333] dark:text-[#fffa] border-b border-solid border-line dark:border-[#555] flex items-center"
    >
      <span class="text-2l pr-16">
        {{ t("common.compare") }}
      </span>
      <div class="flex-1 flex items-center overflow-hidden leading-7">
        <span class="truncate mr-2">
          {{ data?.title1 }}
        </span>
        <span>
          {{ data?.id1 }}
          <span class="text-2l m-4">:</span>{{ data?.id2 }}
        </span>
      </div>
    </div>
    <div class="flex-1 overflow-hidden">
      <component
        :is="diffComponent"
        v-if="data"
        class="w-full"
        :original="data.source1"
        :modified="data.source2"
      />
    </div>
  </div>
</template>
