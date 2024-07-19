<script lang="ts" setup>
import { ref } from "vue";
import {
  getOuterHtml,
  getBlockNames,
  toHtmlBlock,
  getHtmlBlockBody,
} from "@/api/pages";
import type { NodeGroup } from "@/api/pages/types";
import MonacoEditor from "@/components/monaco-editor/index.vue";
import type { Rules } from "async-validator";
import {
  isUniqueNameRule,
  rangeRule,
  requiredRule,
  simpleNameRule,
} from "@/utils/validate";
import { isUniqueName } from "@/api/view";

import { useI18n } from "vue-i18n";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { ElMessage } from "element-plus";
const props = defineProps<{
  modelValue: boolean;
  data: NodeGroup;
}>();

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "addTask", value: string): void;
}>();

const { t } = useI18n();
const rules = {
  name: [
    simpleNameRule(),
    rangeRule(1, 50),
    requiredRule(t("common.nameRequiredTips")),
    isUniqueNameRule(isUniqueName, t("common.viewNameExistsTips")),
  ],
} as Rules;

const show = ref(true);
const form = ref();

const blockNames = ref<string[]>([]);

const model = ref({
  isCreate: true,
  name: "",
  code: "",
});

const onLoad = async () => {
  model.value.name = "";
  model.value.code = "";

  if (model.value.isCreate) {
    model.value.code = await getOuterHtml(props.data.pages);
  } else {
    blockNames.value = await getBlockNames();
  }
};

const loadBlockCode = async () => {
  if (model.value.name) {
    model.value.code = "";
    model.value.code = await getHtmlBlockBody(model.value.name);
  }
};

const onSave = async () => {
  const taskId = await toHtmlBlock(
    props.data.pages,
    model.value.name,
    model.value.code,
    model.value.isCreate ? false : true
  );

  emit("addTask", taskId);

  show.value = false;
  ElMessage.warning(t("common.jobStartRunning"));
};

onLoad();
</script>

<template>
  <el-dialog
    :model-value="show"
    :width="800"
    :close-on-click-modal="false"
    :title="t('common.convertToHtmlBlock')"
    @closed="emit('update:modelValue', false)"
  >
    <el-form
      ref="form"
      label-position="top"
      :model="model"
      :rules="rules"
      @keydown.enter="onSave"
      @submit.prevent
    >
      <el-form-item prop="name">
        <el-radio-group
          v-model="model.isCreate"
          class="el-radio-group--rounded"
          @change="onLoad"
        >
          <el-radio-button :label="true" data-cy="pageTree">{{
            t("page.createNewHTMLBlock")
          }}</el-radio-button>
          <el-radio-button :label="false" data-cy="page">{{
            t("page.convertToExistingHTMLBlock")
          }}</el-radio-button>
        </el-radio-group>
      </el-form-item>

      <el-form-item :label="t('common.name')" prop="name">
        <el-input v-if="model.isCreate" v-model="model.name" data-cy="name" />
        <el-select
          v-else
          v-model="model.name"
          data-cy="name"
          class="w-full"
          @change="loadBlockCode"
        >
          <el-option
            v-for="item in blockNames"
            :key="item"
            :value="item"
            :label="item"
          />
        </el-select>
      </el-form-item>

      <el-form-item :label="t('common.code')" prop="code">
        <div
          class="px-8px rounded-normal h-300px w-full border border-solid border-[#cccccc] focus-within:border-[#2296f3] overflow-hidden"
        >
          <MonacoEditor
            ref="editor"
            v-model="model.code"
            language="html"
            k-script
          />
        </div>
      </el-form-item>
    </el-form>
    <template #footer>
      <DialogFooterBar @confirm="onSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
