<script lang="ts" setup>
import { useI18n } from "vue-i18n";
import { openInNewTab } from "@/utils/url";

const { t } = useI18n();
const props = defineProps<{
  id: string;
  name: string;
  params: { url: string };
}>();
const onDownload = () => {
  openInNewTab(props.params.url + "?__content_type=application/octet-stream");
};
</script>

<template>
  <div class="h-full">
    <el-result icon="info" :title="t('common.canNotOpenFile')">
      <template #sub-title>
        <p>{{ t("common.canNotOpenFileTip") }}</p>
      </template>
      <template #extra>
        <el-button
          v-if="params?.url"
          round
          plain
          type="primary"
          @click="onDownload"
          ><span class="ellipsis w-120px h-24 leading-6">{{
            t("common.downloadThisFile")
          }}</span></el-button
        >
      </template>
    </el-result>
  </div>
</template>
