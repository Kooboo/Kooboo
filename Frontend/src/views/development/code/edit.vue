<script lang="ts" setup>
import MonacoEditor from "@/components/monaco-editor/index.vue";
import { computed, onUnmounted, ref, watch } from "vue";
import { emptyGuid } from "@/utils/guid";
import { onBeforeRouteLeave, useRouter } from "vue-router";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { getQueryString } from "@/utils/url";
import { useShortcut } from "@/hooks/use-shortcuts";
import KBottomBar from "@/components/k-bottom-bar/index.vue";
import { useSaveTip } from "@/hooks/use-save-tip";
import type { PostCode } from "@/api/code/types";
import { useCodeStore } from "@/store/code";
import EditForm from "./edit-form.vue";
import { useI18n } from "vue-i18n";
import { Uri } from "monaco-editor";
import { monaco } from "@/components/monaco-editor/userWorker";
import { ElMessage } from "element-plus";
import { createRecorder } from "@/utils/monacoRecorder";

const { t } = useI18n();
const router = useRouter();
const form = ref();
const editor = ref();
const saveTip = useSaveTip();
const codeStore = useCodeStore();
const record = createRecorder(getQueryString("id") || "");

const model = ref<PostCode>();

const editMode = computed(() => model.value?.id !== emptyGuid);

const load = async () => {
  model.value = await codeStore.getCode(
    getQueryString("id"),
    getQueryString("type"),
    getQueryString("eventType")
  );
  saveTip.init(model.value);
};

const onBack = () => {
  router.goBackOrTo(useRouteSiteId({ name: "code" }));
};

const onSave = async () => {
  if (!model.value || model.value.isDecrypted) return;
  await form.value?.validate();
  const isNewPage = model.value.id === emptyGuid;
  await codeStore.updateCode(model.value, record.save());
  ElMessage.success(t("common.saveSuccess"));
  saveTip.init(model.value);
  if (isNewPage) {
    router.replace(
      useRouteSiteId({ name: "code-edit", query: { id: model.value.id } })
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
load();

const modelUri = computed(() => {
  if (!model.value) return;
  return `${model.value.name || model.value.id}.ts`;
});

onUnmounted(() => {
  if (!modelUri.value) return;
  const module = monaco.editor.getModel(monaco.Uri.file(modelUri.value));
  const currentModel = saveTip.getValue();
  if (module && currentModel) {
    module.setValue(JSON.parse(currentModel).body);
  }
});
watch(
  () => model.value,
  () => {
    saveTip.changed(model.value);
  },
  {
    deep: true,
  }
);
</script>

<template>
  <div
    v-if="model"
    class="h-full w-full flex flex-col overflow-hidden relative"
  >
    <div
      class="pt-16 px-32 bg-fff dark:bg-[#222] dark:border-transparent border-b border-solid border-line flex items-start"
    >
      <EditForm ref="form" :model="model" :edit-mode="editMode" inline />
      <div class="flex-1" />
      <div class="flex items-center space-x-12">
        <el-button
          round
          class="lineButton"
          data-cy="format"
          @click="editor.format()"
          >{{ t("common.format") }}</el-button
        >
      </div>
    </div>
    <div class="flex-1">
      <MonacoEditor
        v-if="model"
        ref="editor"
        v-model="model.body"
        language="typescript"
        :uri="Uri.file(modelUri!)"
        k-script
        @monaco-load-complete="
          (monaco) => record.bindMonaco(monaco, 'typescript')
        "
      />
    </div>
    <div
      v-if="model.isDecrypted"
      class="absolute inset-0 flex items-center justify-center"
    >
      <el-result icon="error" :title="t('common.codeEncryptedTip')" />
    </div>
    <KBottomBar
      back
      :permission="{
        feature: 'code',
        action: 'edit',
      }"
      :hidden-confirm="model.isDecrypted"
      :hidden-cancel="model.isDecrypted"
      @cancel="onBack"
      @save="onSave"
    >
      <template #extra-buttons>
        <el-button
          v-if="model.isDecrypted"
          round
          type="primary"
          @click="onBack"
          >{{ t("common.back") }}</el-button
        >
        <el-button
          v-else
          v-hasPermission="{
            feature: 'code',
            action: 'edit',
          }"
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
