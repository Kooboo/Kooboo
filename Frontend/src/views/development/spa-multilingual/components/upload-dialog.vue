<template>
  <el-dialog
    v-model="modelValue"
    width="650px"
    :close-on-click-modal="false"
    :title="t('common.import')"
    @close="handleClose"
  >
    <el-form label-position="top" @submit.prevent>
      <el-form-item :label="t('common.submitter')">
        <el-radio-group
          v-model="model.submitter"
          class="el-radio-group--rounded"
        >
          <el-radio-button size="large" label="replace" data-cy="replace">{{
            t("common.replace")
          }}</el-radio-button>
          <el-radio-button size="large" label="combine" data-cy="combine">{{
            t("common.combine")
          }}</el-radio-button>
        </el-radio-group>
      </el-form-item>
      <el-form-item :label="t('common.contentType')">
        <el-radio-group v-model="model.type" class="el-radio-group--rounded">
          <el-radio-button size="large" label="file" data-cy="file">{{
            t("common.files")
          }}</el-radio-button>
          <el-radio-button size="large" label="code" data-cy="code">{{
            t("common.code")
          }}</el-radio-button>
        </el-radio-group>
      </el-form-item>
      <el-form-item v-if="model.type === 'file'" :label="t('common.files')">
        <el-button type="primary" class="rounded-full relative"
          >{{ t("common.selectFile") }}
          <label v-if="modelValue" class="absolute left-0 top-0 w-full h-full">
            <input
              id="attachments"
              accept=".json"
              class="hidden"
              type="file"
              @change="selectFile"
              @click="resetFile"
            />
          </label>
        </el-button>
        <span class="ml-8">{{
          model.filename || t("common.supportFile") + ": .json"
        }}</span>
        <span class="ml-8 text-blue cursor-pointer" @click="downloadExample">{{
          t("common.downloadTheSample")
        }}</span>
      </el-form-item>
      <el-form-item v-if="model.type === 'code'" :label="t('common.code')">
        <MonacoEditor
          ref="editor"
          v-model="model.text"
          class="border border-line border-solid dark:border-opacity-50 !min-h-200px h-auto"
          language="json"
        />
      </el-form-item>
    </el-form>
    <template #footer>
      <DialogFooterBar
        :confirm-label="t('common.start')"
        @confirm="handleSave"
        @cancel="handleClose"
      />
    </template>
  </el-dialog>
</template>
<script setup lang="ts">
import { upload } from "@/api/development/spa-multilingual";
import { ref, watch } from "vue";
import { ElMessage } from "element-plus";
import { useI18n } from "vue-i18n";
import MonacoEditor from "@/components/monaco-editor/index.vue";
import { showConfirm } from "@/components/basic/confirm";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

interface PropsType {
  modelValue: boolean;
  isNull: boolean;
}
interface EmitsType {
  (e: "close"): void;
  (e: "save-success"): void;
}

const props = defineProps<PropsType>();
const emits = defineEmits<EmitsType>();
const { t } = useI18n();

const model = ref({
  submitter: "replace",
  type: "file",
  filename: "",
  text: "",
  file: null as Blob | null,
});

const example = {
  example: {
    en: "example",
    "zh-cn": "示例",
  },
  key: {
    en: "",
    "zh-cn": "",
  },
};

watch(
  () => props.modelValue,
  (n) => {
    // 初始化
    if (n) {
      model.value = {
        submitter: "replace",
        type: "file",
        filename: "",
        text: JSON.stringify(example, null, 2),
        file: null,
      };
    }
  }
);

const handleClose = () => {
  emits("close");
};

const resetFile = (e: any) => {
  e.target.value = "";
};

const selectFile = (e: any) => {
  const file = e.target?.files[0];
  if (file && file.type === "application/json") {
    model.value.filename = file.name;
    model.value.file = file;
  } else {
    ElMessage.warning(t("common.fileFormatIsIncorrect"));
    e.target.files = null;
    model.value.filename = "";
    model.value.file = null;
  }
};

async function handleSave() {
  if (model.value.type === "file") {
    if (!model.value.file) {
      ElMessage.warning(t("common.selectFileTips"));
      return;
    }
    const formData = new FormData();
    formData.append("file", model.value.file);
    formData.append("importType", model.value.submitter);
    if (model.value.submitter === "replace" && !props.isNull) {
      await showConfirm(t("common.replaceWarning"));
    }
    await upload(formData);
  } else {
    let json: string;
    try {
      json = JSON.parse(model.value.text);
    } catch (error) {
      ElMessage.warning(t("common.codeFormatIsIncorrect"));
      return;
    }

    if (Object.keys(json).length <= 0) {
      ElMessage.warning(t("common.inputValue"));
      return;
    }

    const formData = new FormData();
    formData.append("file", new Blob([JSON.stringify(json)]));
    formData.append("importType", model.value.submitter);
    if (model.value.submitter === "replace" && !props.isNull) {
      await showConfirm(t("common.replaceWarning"));
    }
    await upload(formData);
  }

  emits("save-success");
  handleClose();
}

function downloadExample() {
  const str = JSON.stringify(example, null, 2);
  const blob = new Blob([str]);
  const url = window.URL.createObjectURL(blob);
  const a = document.createElement("a");
  a.href = url;
  a.download = `example.json`;
  a.click();
}
</script>
