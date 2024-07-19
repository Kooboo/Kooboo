<template>
  <el-dialog
    v-model="visible"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.editImage')"
    @close="handleClose"
  >
    <el-form
      v-if="file"
      ref="form"
      class="el-form--label-normal"
      :model="file"
      :rules="rules"
      label-position="top"
      @submit.prevent
    >
      <el-form-item prop="name" :label="t('common.name')">
        <el-input v-model="file.name" disabled data-cy="folder-name-input" />
      </el-form-item>
      <el-form-item prop="alt" :label="t('common.altText')">
        <el-input
          v-model="file.alt"
          data-cy="folder-alt-input"
          @keydown.enter="handleSave"
        />
      </el-form-item>
      <el-form-item :label="t('common.image')">
        <div class="flex items-center justify-center h-90px w-90px">
          <el-image
            style="max-height: 100%; max-width: 100%"
            :src="file.thumbnail"
            :zoom-rate="1.2"
            :max-scale="7"
            :min-scale="0.2"
            :preview-src-list="[getPreviewUrl(file.url)]"
            :initial-index="0"
            fit="contain"
          />
        </div>
      </el-form-item>
    </el-form>
    <template #footer>
      <DialogFooterBar
        :confirm-label="t('common.save')"
        @confirm="handleSave"
        @cancel="handleClose"
      />
    </template>
  </el-dialog>
</template>
<script lang="ts" setup>
import type { MediaFileItem } from ".";
import { useI18n } from "vue-i18n";
import useOperationDialog from "@/hooks/use-operation-dialog";
import type { UPDATE_MODEL_EVENT } from "@/constants/constants";
import { ref } from "vue";
import type { ElForm } from "element-plus";
import type { Rules } from "async-validator";
import { useSiteStore } from "@/store/site";
import {
  combineUrl,
  getQueryString,
  isAbsoluteUrl,
  updateQueryString,
} from "@/utils/url";
import { imageUpdate } from "@/api/content/media";

export type EditingMediaFile = Pick<
  MediaFileItem,
  "key" | "alt" | "name" | "url" | "thumbnail"
>;

interface Props {
  modelValue: boolean;
  file: EditingMediaFile;
  provider: string;
}
interface EmitsType {
  (e: typeof UPDATE_MODEL_EVENT, value: boolean): void;
  (e: "saved"): void;
}

const props = defineProps<Props>();
const emits = defineEmits<EmitsType>();
const form = ref<InstanceType<typeof ElForm>>();
const rules = ref<Rules>({});

const { t } = useI18n();
const { visible, handleClose } = useOperationDialog(props, emits);

const siteStore = useSiteStore();

function getPreviewUrl(url: string) {
  if (url) {
    if (isAbsoluteUrl(url)) {
      return url;
    }
    url = updateQueryString(url, {
      siteId: getQueryString("siteId"),
    });
    return combineUrl(siteStore.site.prUrl!, url);
  }
  return url;
}
function handleSave() {
  form.value?.validate(async (valid) => {
    if (valid) {
      await imageUpdate({
        id: props.file.key,
        alt: props.file.alt ?? "",
        name: props.file.name,
        provider: props.provider,
      });
      handleClose();
      emits("saved");
    }
  });
}
</script>
