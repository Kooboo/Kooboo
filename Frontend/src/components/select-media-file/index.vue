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
            <PreviewImage
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
      <PreviewImage v-else :url="(urls as string)" @remove="removeFile()" />
    </div>
    <div class="flex items-center">
      <el-button
        v-hasPermission="{ feature: 'content', action: 'edit' }"
        type="primary"
        round
        @click="visibleMediaDialog = true"
      >
        {{ t("common.selectFile") }}
      </el-button>
      <div
        v-if="placeholder"
        class="ml-8px text-s text-666"
        data-cy="tooltip-right"
      >
        {{ placeholder }}
      </div>
    </div>
  </div>
  <KMediaDialog
    v-if="visibleMediaDialog"
    v-model="visibleMediaDialog"
    :multiple="multiple"
    @choose="handleChooseFile"
  />
</template>
<script lang="ts" setup>
import KMediaDialog from "@/components/k-media-dialog";
import { useSync } from "@/hooks/use-sync";
import { ref, computed } from "vue";
import PreviewImage from "./preview-image.vue";
import type { MediaFileItem } from "@/components/k-media-dialog";
import VueDraggable from "vuedraggable";

import { useI18n } from "vue-i18n";
import type { KeyValue } from "@/global/types";
import { useSortable } from "@/hooks/use-sortable";
import type { FieldValidation } from "@/global/control-type";
import { errorMessage } from "../basic/message";

interface PropsType {
  multiple?: boolean;
  modelValue: string[] | string;
  placeholder?: string;
  validations?: FieldValidation[];
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
const fileSizeLimitRule = computed(() => {
  return props.validations?.find((it) => it.type === "fileSize") ?? undefined;
});
const fileSizeLimit = computed(() => {
  const item = fileSizeLimitRule.value ?? {
    value: 0,
    pattern: "kb",
  };
  const { value, pattern } = item;
  if (!value || !["kb", "mb"].includes(pattern ?? "")) {
    return 0;
  }

  if (pattern === "kb") {
    return value * 1024;
  }

  if (pattern === "mb") {
    return value * 1024 * 1024;
  }

  return 0;
});
const fileTypesRule = computed(() => {
  return props.validations?.find((it) => it.type === "fileTypes") ?? undefined;
});
const fileTypes = computed(() => {
  const { pattern } = fileTypesRule.value ?? { pattern: "" };
  if (!pattern) {
    return [];
  }

  const items = Array.isArray(pattern) ? pattern : pattern.split(",");
  return items.map((it) => {
    const ext = it.trim().toLowerCase();
    if (!ext.startsWith(".")) {
      return `.${ext}`;
    }
    return ext;
  });
});

type ValidationErrorContext = {
  fileSizeTitle?: string;
  fileTypesTitle?: string;
  fileSize?: string[];
  fileTypes?: string[];
};

function validateFile(file: MediaFileItem, errors: ValidationErrorContext) {
  if (
    fileTypes.value.length > 0 &&
    !fileTypes.value.some((t) =>
      file.url.toLowerCase().endsWith(t.toLowerCase())
    )
  ) {
    if (!errors.fileTypesTitle) {
      const { msg } = fileTypesRule.value ?? { msg: "" };
      if (msg) {
        errors.fileTypesTitle = msg.replace(
          "{types}",
          fileTypes.value.join(", ")
        );
      } else {
        errors.fileTypesTitle = t("common.fileTypesNotAllowed", {
          types: fileTypes.value.join(", "),
        });
      }
    }
    if (!errors.fileTypes) {
      errors.fileTypes = [];
    }
    errors.fileTypes.push(file.name);
    return false;
  }

  if (fileSizeLimit.value && file.size > fileSizeLimit.value) {
    if (!errors.fileSizeTitle) {
      const { value, pattern, msg } = fileSizeLimitRule.value ?? {
        value: 0,
        pattern: "kb",
      };
      if (msg) {
        errors.fileSizeTitle = msg.replace(
          "{size}",
          `${value}${pattern}`.toUpperCase()
        );
      } else {
        errors.fileSizeTitle = t("common.fileSizeExceedLimitTips", {
          size: `${value}${pattern}`.toUpperCase(),
        });
      }
    }
    if (!errors.fileSize) {
      errors.fileSize = [];
    }
    errors.fileSize.push(file.name);
    return false;
  }

  return true;
}

function handleChooseFile(files: MediaFileItem[]) {
  const errors: ValidationErrorContext = {};
  const errorMessages = [];
  if (props.multiple) {
    const validUrls: string[] = [];
    for (const file of files) {
      const valid = validateFile(file, errors);
      if (valid) {
        validUrls.push(file.url);
      }
    }
    urls.value = ((urls.value ?? []) as string[]).concat(validUrls);
  } else {
    const valid = validateFile(files[0], errors);
    if (valid) {
      urls.value = files[0].url;
    }
  }

  if (errors.fileTypes?.length) {
    errorMessages.push(errors.fileTypesTitle);
    errorMessages.push(
      `<ul class="max-w-504px">${errors.fileTypes
        .map((name) => `<li><p class="ellipsis">${name}</p></li>`)
        .join("")}</ul>`
    );
  }

  if (errors.fileSize?.length) {
    errorMessages.push(errors.fileSizeTitle);
    errorMessages.push(
      `<ul class="max-w-504px">${errors.fileSize
        .map((name) => `<li><p class="ellipsis">${name}</p></li>`)
        .join("")}</ul>`
    );
  }
  if (errorMessages.length > 0) {
    const message = errorMessages.join("<br/>");
    errorMessage(message, true, {
      dangerouslyUseHTMLString: true,
    });
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
