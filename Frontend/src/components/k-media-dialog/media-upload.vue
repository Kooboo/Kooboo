<script lang="ts" setup>
import { isUniqueKey } from "@/api/content/media";
import { useFileUpload } from "@/hooks/use-file-upload";
import { combineUrl } from "@/utils/url";
import { showFileExistsConfirm } from "../basic/confirm";
import { ElLoading } from "element-plus";
import type { MediaFileItem } from "@/api/content/media";
import { getUploadActionUrl } from "@/api/content/media";
import { uploadMessage } from "../basic/message";
import { computed } from "vue";

const props = defineProps<{
  folder: string;
  provider?: string;
  multiple?: boolean;
  prefix?: string;
}>();

const emit = defineEmits<{
  (e: "afterUpload", value: MediaFileItem[]): void;
}>();

let loadingInstance: { close: () => void } | undefined;
const { getAccepts, checkFile } = useFileUpload();
const imageAccepts = getAccepts("image");
const uploadUrl = computed(() =>
  getUploadActionUrl(props.provider ?? "default")
);

function showLoading() {
  if (!loadingInstance) {
    loadingInstance = ElLoading.service({ background: "rgba(0, 0, 0, 0.5)" });
  }
}

async function handleBeforeUpload(file: { type: string; name: string }) {
  if (!checkFile(imageAccepts, file)) {
    return false;
  }
  let fileName = file.name;

  if (props.prefix) fileName = `${props.prefix}_${fileName}`;
  const fileUrl = combineUrl(props.folder, fileName);
  const isValidName = await isUniqueKey(props.provider ?? "default", fileUrl)
    .then(() => true)
    .catch(() => false);
  if (!isValidName) {
    await showFileExistsConfirm();
  }

  showLoading();
}

function hideLoading() {
  loadingInstance?.close();
  loadingInstance = undefined;
}

function uploadFinish(rsp: MediaFileItem[]) {
  uploadMessage();
  hideLoading();
  emit("afterUpload", rsp);
}
</script>

<template>
  <KUpload
    :permission="{
      feature: 'mediaLibrary',
      action: 'edit',
    }"
    :multiple="multiple"
    :action="uploadUrl"
    :accept="imageAccepts.join(',')"
    :before-upload="handleBeforeUpload"
    :on-success="uploadFinish"
    :data="{ folder: folder, prefix: prefix || '' }"
    data-cy="upload"
  >
    <slot />
  </KUpload>
</template>
