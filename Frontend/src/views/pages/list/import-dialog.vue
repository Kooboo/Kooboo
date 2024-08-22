<script lang="ts" setup>
import { ref, watch } from "vue";
import { fromList, rules, createModel, accept } from "./import-dialog";
import { single } from "@/api/transfer";
import { convertFile } from "@/api/pages";
import { usePageStore } from "@/store/page";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

import { useI18n } from "vue-i18n";
import type { UploadFile } from "element-plus";
import { ElMessage } from "element-plus";
import { useAppStore } from "@/store/app";
const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

defineProps<{ modelValue: boolean }>();
const { t } = useI18n();

const show = ref(true);
const from = ref(fromList.value[0].key);
const form = ref();
const model = ref(createModel());
const pageStore = usePageStore();
const appStore = useAppStore();

watch(
  () => model.value.pageUrl,
  () => {
    if (!model.value.pageUrl) return;
    var path = /\w[^/]+:\/\/[\w\.]+:?\d*(.[^\?&]+)/.exec(
      model.value.pageUrl.trim()
    );
    var name = "";
    if (!path) name = "";
    else if (!path[1]) name = "";
    else if (path[1].startsWith("/")) name = path[1];
    else if (path[1].indexOf("/") > -1) name = "/";
    model.value.name = name;
  }
);

watch(
  () => from.value,
  () => {
    model.value = createModel();
  }
);

const onFileSelect = (value: UploadFile) => {
  if (!accept.includes(value.raw!.type)) {
    ElMessage.error(t("common.fileInvalid", { fileName: value.name }));
    return;
  }
  model.value.file = value;
};

const onSave = async () => {
  //from url
  if (from.value === fromList.value[0].key) {
    await form.value?.validate();
    const response = await single(model.value);
    if (response.success === false) {
      ElMessage.error(
        response.messages?.join(",") || t("common.pageDownloadFail")
      );
      return;
    }
  }

  //from file
  if (from.value === fromList.value[1].key) {
    await form.value?.validate();
    var formdata = new FormData();
    formdata.append("file", model.value.file.raw);
    await convertFile(formdata);
  }

  show.value = false;
  pageStore.load();
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
      :rules="rules"
      :model="model"
      @keydown.enter="onSave"
    >
      <el-form-item :label="t('common.from')">
        <el-radio-group v-model="from" class="el-radio-group--rounded h-38px">
          <el-radio-button
            v-for="item of fromList"
            :key="item.key"
            :label="item.key"
            :data-cy="`import-from-${item.value}`"
            >{{ item.value }}</el-radio-button
          >
        </el-radio-group>
      </el-form-item>
      <template v-if="from === fromList[0].key">
        <el-form-item label="URL " prop="pageUrl">
          <el-input v-model="model.pageUrl" data-cy="page-url" />
        </el-form-item>
        <el-form-item :label="t('common.pageURL')" prop="name">
          <el-input v-model="model.name" data-cy="url" />
        </el-form-item>
        <el-form-item>
          <el-checkbox
            v-if="!appStore.header?.isOnlineServer"
            v-model="model.headless"
          >
            <span>{{ t("common.useHeadlessMode") }}</span>
            <Tooltip
              :tip="t('common.useHeadlessModeTip')"
              custom-class="ml-4"
            />
          </el-checkbox>
        </el-form-item>
      </template>

      <template v-if="from === fromList[1].key">
        <el-form-item :label="t('common.file')" prop="file">
          <el-upload
            :show-file-list="false"
            action=""
            :accept="accept.join(',')"
            :auto-upload="false"
            :on-change="onFileSelect"
            data-cy="upload"
          >
            <el-button type="primary" data-cy="select" round>{{
              t("common.selectFile")
            }}</el-button>
          </el-upload>
          <div v-if="!model.file" class="el-upload__tip ml-8">
            {{ t("common.supportTypes") }}
          </div>
          <div v-if="model.file" class="mt-12 w-full">
            <el-alert
              :title="model.file.name"
              type="info"
              data-cy="uploaded-file-name"
              @close="model.file = null"
            />
          </div>
        </el-form-item>
      </template>
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

:deep(span.el-radio-button__inner) {
  width: 60px;
  margin: -1px;
}
</style>
