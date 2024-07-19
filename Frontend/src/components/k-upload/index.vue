<template>
  <el-upload
    :show-file-list="false"
    action=""
    :on-change="uploadChange"
    :disabled="!siteStore.hasAccess(permission.feature, permission.action)"
  >
    <slot>
      <el-button
        v-hasPermission="permission"
        type="primary"
        round
        class="shadow-s-10 !py-8px"
        data-cy="upload"
        :disabled="disabled"
        ><el-icon class="iconfont icon-a-Cloudupload !text-20px" />{{
          t("common.uploadFiles")
        }}</el-button
      >
    </slot>
  </el-upload>
</template>

<script lang="ts" setup>
import { useFileUpload } from "@/hooks/use-file-upload";
import { useSiteStore } from "@/store/site";

import { useI18n } from "vue-i18n";
const { t } = useI18n();
const siteStore = useSiteStore();
defineProps<{
  permission: {
    feature: string;
    action: string;
  };
  disabled?: boolean;
}>();
const { uploadChange } = useFileUpload();
</script>
