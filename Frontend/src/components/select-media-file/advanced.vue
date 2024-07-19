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
              :url="element.value.src"
              :advanced="meta && meta.length > 0"
              @remove="removeFile(index)"
              @edit="onEditMetadata(element.value, index)"
            />
          </template>
        </VueDraggable>
      </template>
      <PreviewImage
        v-else
        :url="(urls as AdvancedMediaFileType).src"
        :advanced="meta && meta.length > 0"
        @remove="removeFile()"
        @edit="onEditMetadata(urls as AdvancedMediaFileType, -1)"
      />
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
  <teleport to="body">
    <KMediaDialog
      v-if="visibleMediaDialog"
      v-model="visibleMediaDialog"
      :multiple="multiple"
      @choose="handleChooseFile"
    />
    <EditMetadataDialog
      ref="editMetadataDialog"
      v-model="visibleEditMetadataDialog"
      :meta="meta ?? []"
      @save="onSaveMetadata"
    />
  </teleport>
</template>
<script lang="ts" setup>
import KMediaDialog from "@/components/k-media-dialog";
import { ref, computed, watch } from "vue";
import PreviewImage from "./preview-image.vue";
import EditMetadataDialog from "./edit-metadata.vue";
import type EditMetadataDialogType from "./edit-metadata.vue";
import type { MediaFileItem } from "@/components/k-media-dialog";
import VueDraggable from "vuedraggable";

import { useI18n } from "vue-i18n";
import type { KeyValue } from "@/global/types";
import { useSortable } from "@/hooks/use-sortable";
import { cloneDeep, template } from "lodash-es";

type AdvancedMediaFileType = {
  src: string;
  [key: string]: any;
};
type MediaFileValueTypes = AdvancedMediaFileType[] | AdvancedMediaFileType;

type KeyMediaFileValueType = {
  key: string;
  value: AdvancedMediaFileType;
};

interface PropsType {
  multiple?: boolean;
  modelValue: string[] | string;
  placeholder?: string;
  meta?: KeyValue[];
}

const props = defineProps<PropsType>();
const emits = defineEmits<{
  (e: "update:modelValue", value: string[] | string): void;
}>();
const { t } = useI18n();
const innerUrls = ref<MediaFileValueTypes>(getInitUrls());
function getInitUrls(): MediaFileValueTypes {
  if (props.multiple) {
    if (Array.isArray(props.modelValue)) {
      return props.modelValue.map((it) => {
        return it ? JSON.parse(it) : { src: "" };
      });
    } else {
      return [props.modelValue ? JSON.parse(props.modelValue) : { src: "" }];
    }
  } else {
    let jsonValue = props.modelValue;
    if (Array.isArray(props.modelValue)) {
      jsonValue = props.modelValue[0];
    }
    return jsonValue ? JSON.parse(jsonValue as string) : { src: "" };
  }
}

const urls = computed({
  get() {
    return innerUrls.value;
  },
  set(value: MediaFileValueTypes) {
    innerUrls.value = value;
    emitChange(value);
  },
});

const emitChange = (value: MediaFileValueTypes) => {
  if (Array.isArray(value)) {
    emits(
      "update:modelValue",
      value.map((it) => JSON.stringify(it))
    );
  } else {
    emits("update:modelValue", JSON.stringify(value));
  }
};

watch(
  () => [props.modelValue, props.multiple],
  () => {
    innerUrls.value = getInitUrls();
  },
  {
    deep: true,
  }
);

const fileList = computed<KeyMediaFileValueType[]>(() => {
  if (!props.multiple) {
    return [];
  }

  return (urls.value as AdvancedMediaFileType[]).map(
    (it: AdvancedMediaFileType) => {
      return {
        key: it.src,
        value: it,
      };
    }
  );
});
const { onChange } = useSortable(fileList, (changedList) => {
  urls.value = cloneDeep(changedList.map((it) => it.value));
});
const visibleMediaDialog = ref(false);
const visibleEditMetadataDialog = ref(false);
const editMetadataDialog = ref<InstanceType<typeof EditMetadataDialogType>>();
const editingIndex = ref<number>(-1);

function getAdvancedValue(file: MediaFileItem) {
  const result: AdvancedMediaFileType = { src: file.url };
  if (props.meta) {
    props.meta.forEach((it) => {
      if (it.key === "src") {
        return;
      }
      const compiled = template(it.value, {
        interpolate: /{{([\s\S]+?)}}/g,
      });
      result[it.key] = compiled(file);
    });
  }
  return result;
}

function handleChooseFile(files: MediaFileItem[]) {
  if (props.multiple) {
    urls.value = ((urls.value ?? []) as AdvancedMediaFileType[]).concat(
      files.map(getAdvancedValue)
    );
  } else {
    urls.value = getAdvancedValue(files[0]);
  }
}

function removeFile(index?: number) {
  if (index === undefined) {
    urls.value = { src: "" };
  } else {
    (urls.value as AdvancedMediaFileType[]).splice(index, 1);
    emitChange(urls.value);
  }
}

function onEditMetadata(file: AdvancedMediaFileType, index: number) {
  editingIndex.value = index;
  editMetadataDialog.value?.showDialog(file as Record<string, string>);
}

function onSaveMetadata(model: Record<string, string>) {
  if (Array.isArray(urls.value) && props.multiple) {
    urls.value = urls.value.map((it, i) => {
      if (i === editingIndex.value) {
        return {
          ...it,
          ...model,
        };
      }
      return it;
    });
  } else {
    urls.value = {
      ...urls.value,
      ...model,
    };
  }
}
</script>
