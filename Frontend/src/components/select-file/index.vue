<template>
  <div class="select-media-file">
    <div v-if="urls">
      <template v-if="multiple">
        <VueDraggable
          :model-value="fileList"
          item-key="key"
          :disabled="fileList.length < 2"
          class="flex items-center flex-wrap"
          @change="onChange"
        >
          <template #item="{ element, index }">
            <PreviewFile
              :key="index"
              :tooltip="
                fileList.length < 2 ? undefined : t('common.dragToSort')
              "
              :url="element.value"
              @remove="removeFile(index)"
            />
          </template>
        </VueDraggable>
      </template>
      <PreviewFile v-else :url="(urls as string)" @remove="removeFile()" />
    </div>
    <div class="flex items-center">
      <el-button
        v-hasPermission="{ feature: 'content', action: 'edit' }"
        type="primary"
        round
        @click="visibleMediaDialog = true"
        >{{ t("common.selectFile") }}</el-button
      >
      <div
        v-if="placeholder"
        class="ml-8px text-s text-666"
        data-cy="tooltip-right"
      >
        {{ placeholder }}
      </div>
    </div>
  </div>
  <KFileDialog
    v-model="visibleMediaDialog"
    :multiple="multiple"
    @choose="handleChooseFile"
  />
</template>
<script lang="ts" setup>
import { useSync } from "@/hooks/use-sync";
import { ref, computed } from "vue";
import PreviewFile from "./preview-file.vue";
import type { MediaFileItem } from "@/components/k-media-dialog";
import KFileDialog from "../k-file-dialog/index.vue";
import VueDraggable from "vuedraggable";

import { useI18n } from "vue-i18n";
import type { KeyValue } from "@/global/types";
import { useSortable } from "@/hooks/use-sortable";

interface PropsType {
  multiple?: boolean;
  modelValue: string[] | string;
  placeholder?: string;
}
const props = defineProps<PropsType>();
const emits = defineEmits<{
  (e: "update:modelValue"): void;
}>();
const { t } = useI18n();
const urls = useSync(props, "modelValue", emits);
const fileList = computed<KeyValue[]>(() => {
  return (urls.value as string[]).map((it: string) => {
    return {
      key: it,
      value: it,
    };
  });
});
const { onChange } = useSortable(fileList, (changedList) => {
  urls.value = changedList.map((it) => it.value);
});
const visibleMediaDialog = ref(false);

function handleChooseFile(files: MediaFileItem[]) {
  if (props.multiple) {
    urls.value = ((urls.value ?? []) as string[]).concat(
      files.map((x) => x.url)
    );
  } else {
    urls.value = files[0].url;
  }
}

function removeFile(index?: number) {
  if (index === undefined) {
    urls.value = "";
  } else {
    (urls.value as string[]).splice(index, 1);
  }
}
</script>
