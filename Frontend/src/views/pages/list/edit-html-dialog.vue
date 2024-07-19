<script lang="ts" setup>
import { ref } from "vue";
import {
  getOuterHtml,
  editOuterHtml,
  getInnerHtml,
  editInnerHtml,
} from "@/api/pages";
import type { NodeGroup } from "@/api/pages/types";
import MonacoEditor from "@/components/monaco-editor/index.vue";
import type { Rules } from "async-validator";
import { requiredRule } from "@/utils/validate";

import { useI18n } from "vue-i18n";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { ElMessage } from "element-plus";
const props = defineProps<{
  modelValue: boolean;
  data: NodeGroup;
  editType: "innerHtml" | "outerHtml";
}>();

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "addTask", value: string): void;
}>();

const { t } = useI18n();
const rules = {
  code: [requiredRule(t("common.code"))],
} as Rules;

const show = ref(true);
const form = ref();

const model = ref({
  code: "",
});

async function onLoad() {
  if (props.editType === "innerHtml") {
    model.value.code = await getInnerHtml(props.data.pages);
  } else {
    model.value.code = await getOuterHtml(props.data.pages);
  }
}

onLoad();

const onSave = async () => {
  if (props.editType === "innerHtml") {
    const taskId = await editInnerHtml(props.data.pages, model.value.code);
    emit("addTask", taskId);
  } else {
    const taskId = await editOuterHtml(props.data.pages, model.value.code);
    emit("addTask", taskId);
  }
  show.value = false;
  ElMessage.warning(t("common.jobStartRunning"));
};
</script>

<template>
  <el-dialog
    :model-value="show"
    width="800px"
    :close-on-click-modal="false"
    :title="t('common.edit')"
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
      <el-form-item :label="t('common.code')" prop="code">
        <div
          class="px-8px rounded-normal h-400px w-full border border-solid border-[#cccccc] focus-within:border-[#2296f3] overflow-hidden"
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
