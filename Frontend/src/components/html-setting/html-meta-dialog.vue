<script lang="ts" setup>
import type { Meta } from "@/api/pages/types";
import { onBeforeMount, ref, watch } from "vue";
import { charsets, metaNames, httpEquivs } from "./html-meta";
import MultilingualSelector from "@/components/multilingual-selector/index.vue";
import { useMultilingualStore } from "@/store/multilingual";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

import { useI18n } from "vue-i18n";
const props = defineProps<{
  modelValue: boolean;
  meta: Meta;
  metaBindings?: string[];
}>();

const multilingualStore = useMultilingualStore();

const getTitleLabel = (item: string) => {
  if (item === multilingualStore.default) {
    return multilingualStore.selected.length > 1
      ? t("common.content") + " - " + item + " (" + t("common.default") + ")"
      : t("common.content");
  } else {
    return t("common.content") + " - " + item;
  }
};

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "selected", value: string): void;
  (e: "addOrUpdate", value: Meta): void;
  (e: "update:content", value: string): void;
}>();
const { t } = useI18n();

const show = ref(true);
const model = ref<Meta>({
  charset: "",
  content: { zh: "", en: "" },
  httpequiv: "",
  name: "",
});

onBeforeMount(() => {
  if (props.meta) {
    model.value = { ...props.meta };
  }
});

watch(
  () => model.value.charset,
  (value) => {
    if (value) {
      model.value.content = {};
      model.value.httpequiv = "";
      model.value.name = "";
      model.value.property = "";
    }
  }
);

watch(
  () => model.value.name,
  (value) => {
    if (value) {
      model.value.httpequiv = "";
      model.value.charset = "";
      model.value.property = "";
    }
  }
);

watch(
  () => model.value.httpequiv,
  (value) => {
    if (value) {
      model.value.name = "";
      model.value.charset = "";
      model.value.property = "";
    }
  }
);

watch(
  () => model.value.property,
  (value) => {
    if (value) {
      model.value.name = "";
      model.value.charset = "";
      model.value.httpequiv = "";
    }
  }
);

const onSave = async () => {
  emit("addOrUpdate", model.value);
  show.value = false;
};

const changeUrl = (item: string, content: string) => {
  if (!model.value.content![item]) {
    model.value.content![item] = "";
  }
  model.value.content![item] += content;
};
</script>

<template>
  <el-dialog
    :model-value="show"
    width="700px"
    :close-on-click-modal="false"
    :title="t('common.meta')"
    @closed="emit('update:modelValue', false)"
  >
    <div class="flex justify-end">
      <MultilingualSelector />
    </div>
    <el-form label-position="top" @keydown.enter="onSave">
      <el-form-item :label="t('common.name')">
        <el-select
          v-model="model.name"
          class="w-full"
          filterable
          allow-create
          default-first-option
          :disabled="!!model.charset || !!model.httpequiv || !!model.property"
          data-cy="name-dropdown"
        >
          <el-option
            v-for="item in metaNames"
            :key="item"
            :label="item"
            :value="item"
            data-cy="name-opt"
          />
        </el-select>
      </el-form-item>
      <ElFormItem label="Property">
        <ElInput
          v-model="model.property"
          :disabled="!!model.httpequiv || !!model.name || !!model.charset"
        />
      </ElFormItem>
      <el-form-item :label="t('common.charset')">
        <el-select
          v-model="model.charset"
          class="w-full"
          filterable
          allow-create
          default-first-option
          :disabled="!!model.httpequiv || !!model.name || !!model.property"
          data-cy="charset-dropdown"
        >
          <el-option
            v-for="item in charsets"
            :key="item"
            :label="item"
            :value="item"
            data-cy="charset-opt"
          />
        </el-select>
      </el-form-item>
      <el-form-item label="HTTP-equiv">
        <el-select
          v-model="model.httpequiv"
          class="w-full"
          filterable
          allow-create
          default-first-option
          :disabled="!!model.charset || !!model.name || !!model.property"
          data-cy="http-equiv-dropdown"
        >
          <el-option
            v-for="item in httpEquivs"
            :key="item"
            :label="item"
            :value="item"
            data-cy="http-equiv-opt"
          />
        </el-select>
      </el-form-item>
      <el-form-item
        v-for="item of multilingualStore.selected"
        :key="item"
        :label="getTitleLabel(item)"
      >
        <el-input
          :disabled="!!model.charset"
          :model-value="model.content![item]"
          data-cy="content"
          @update:model-value="model.content![item]=$event"
        />
        <div v-if="!model.charset" class="flex-row mb-8 mt-4">
          <el-tag
            v-for="content in metaBindings"
            :key="content"
            type="info"
            class="m-2px cursor-pointer"
            data-cy="meta-content-dynamic-field"
            @click="changeUrl(item, content)"
            >{{ content }}</el-tag
          >
        </div>
      </el-form-item>
    </el-form>

    <template #footer>
      <DialogFooterBar @confirm="onSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
