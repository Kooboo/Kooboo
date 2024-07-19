<script lang="ts" setup>
import MonacoEditor from "@/components/monaco-editor/index.vue";
import { ref, watch } from "vue";
import type { PostPage } from "@/api/pages/types";
import { emptyGuid } from "@/utils/guid";
import { onBeforeRouteLeave, useRouter } from "vue-router";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { getQueryString } from "@/utils/url";
import { useShortcut } from "@/hooks/use-shortcuts";
import { useSaveTip } from "@/hooks/use-save-tip";
import KBottomBar from "@/components/k-bottom-bar/index.vue";
import { usePageStore } from "@/store/page";
import EditForm from "./edit-form.vue";
import { usePreviewUrl } from "@/hooks/use-preview-url";
import { Uri } from "monaco-editor";
import { useI18n } from "vue-i18n";
import { ElMessage } from "element-plus";
import { createRecorder } from "@/utils/monacoRecorder";
import {
  provideDefaultFileNotFound,
  provideDefaultFileOpen,
} from "@/components/monaco-editor/config";
provideDefaultFileNotFound();
provideDefaultFileOpen();

const { t } = useI18n();
const router = useRouter();
const model = ref<PostPage>();
const form = ref();
const editor = ref();
const saveTip = useSaveTip();
const pageStore = usePageStore();
const oldUrlPath = ref();
const { onPreview } = usePreviewUrl();
const record = createRecorder(getQueryString("id") || "");

const load = async () => {
  model.value = await pageStore.getPage(getQueryString("id"));
  model.value.published = true;
  saveTip.init(model.value);
  oldUrlPath.value = model.value.urlPath;
};

const onBack = async () => {
  router.goBackOrTo(useRouteSiteId({ name: "pages" }));
};

const onSave = async () => {
  if (!model.value) return;
  await form.value.validate();
  const isNewPage = model.value.id === emptyGuid;
  await pageStore.updatePage(model.value, record.save());
  ElMessage.success(t("common.saveSuccess"));
  oldUrlPath.value = model.value.urlPath;
  saveTip.init(model.value);
  if (isNewPage) {
    router.replace(
      useRouteSiteId({ name: "page-edit", query: { id: model.value.id } })
    );
  }
};

const saveAndReturn = async () => {
  await onSave();
  onBack();
};
onBeforeRouteLeave(async (to, from, next) => {
  if (to.name === "login") {
    next();
  } else {
    await saveTip
      .check(model.value)
      .then(() => {
        next();
      })
      .catch(() => {
        next(false);
      });
  }
});

useShortcut("save", onSave);
watch(
  () => model.value,
  () => {
    saveTip.changed(model.value);
  },
  {
    deep: true,
  }
);
load();
</script>

<template>
  <div v-if="model" class="h-full w-full flex flex-col overflow-hidden">
    <div
      class="pt-16 px-32 bg-fff dark:bg-[#222] border-b border-solid border-line dark:border-transparent flex"
    >
      <EditForm
        v-if="model"
        ref="form"
        :model="model"
        :old-url-path="oldUrlPath"
        inline
      />
      <div class="flex-1" />
      <el-button
        v-if="model.previewUrl"
        round
        class="lineButton"
        data-cy="preview"
        @click="onPreview(model!.previewUrl!)"
      >
        {{ t("common.preview") }}
      </el-button>
      <el-button
        round
        class="lineButton"
        data-cy="format"
        @click="editor.format()"
      >
        {{ t("common.format") }}
      </el-button>
    </div>
    <div class="flex-1">
      <MonacoEditor
        ref="editor"
        v-model="model.body"
        class="flex-1"
        language="html"
        :uri="Uri.file(model.id)"
        k-script
        data-cy="code-editor"
        @monaco-load-complete="(monaco) => record.bindMonaco(monaco, 'html')"
      />
    </div>
    <KBottomBar
      back
      :permission="{ feature: 'pages', action: 'edit' }"
      @cancel="onBack"
      @save="onSave"
    >
      <template #extra-buttons>
        <el-button
          v-hasPermission="{ feature: 'pages', action: 'edit' }"
          round
          type="primary"
          data-cy="save-and-return"
          @click="saveAndReturn"
          >{{ t("common.saveAndReturn") }}</el-button
        >
      </template>
    </KBottomBar>
  </div>
</template>
