<script lang="ts" setup>
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import type { UploadFile } from "element-plus";
import { requiredRule } from "@/utils/validate";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { importLabel } from "@/api/content/label";

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

defineProps<{ modelValue: boolean }>();
const { t } = useI18n();

const rules = {
  file: requiredRule(t("common.selectFileTips")),
};
const show = ref(true);
const form = ref();
const model = ref({
  file: null as any,
});

const onFileSelect = (value: UploadFile) => {
  model.value.file = value;
};

const onSave = async () => {
  await form.value.validate();
  var formData = new FormData();
  formData.append("file", model.value.file.raw);
  await importLabel(formData);
  show.value = false;
  emit("reload");
};

watch(
  () => model.value.file,
  () => {
    if (model.value.file) {
      form.value.clearValidate("file");
    }
  }
);
</script>

<template>
  <el-dialog
    v-model="show"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.import')"
    @closed="emit('update:modelValue', false)"
  >
    <el-form
      ref="form"
      label-position="top"
      :model="model"
      :rules="rules"
      @submit.prevent
      @keydown.enter="onSave"
    >
      <el-form-item :label="t('common.file')" prop="file">
        <el-upload
          :show-file-list="false"
          action=""
          :auto-upload="false"
          :on-change="onFileSelect"
          accept=".json"
          data-cy="upload"
        >
          <el-button type="primary" round plain>{{
            t("common.selectFile")
          }}</el-button>
        </el-upload>
        <div v-if="!model.file" class="el-upload__tip ml-8">
          {{ t("common.supportFile") + ": .json" }}
        </div>
        <div v-if="model.file" class="mt-12 w-full">
          <el-alert
            :title="model.file.name"
            type="info"
            @close="model.file = null"
          />
        </div>
      </el-form-item>
    </el-form>

    <template #footer>
      <DialogFooterBar
        :confirm-label="t('common.start')"
        @confirm="onSave"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>

<style lang="scss" scoped>
.el-alert {
  line-height: normal;
}
</style>
