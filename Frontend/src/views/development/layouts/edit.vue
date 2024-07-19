<script lang="ts" setup>
import MonacoEditor from "@/components/monaco-editor/index.vue";
import { computed, ref, watch } from "vue";
import { emptyGuid } from "@/utils/guid";
import { onBeforeRouteLeave, useRouter } from "vue-router";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { getQueryString } from "@/utils/url";
import { useShortcut } from "@/hooks/use-shortcuts";
import KBottomBar from "@/components/k-bottom-bar/index.vue";
import type { PostLayout } from "@/api/layout/types";
import { useSaveTip } from "@/hooks/use-save-tip";
import { useLayoutStore } from "@/store/layout";
import EditForm from "./edit-form.vue";
import { useI18n } from "vue-i18n";
import { Uri } from "monaco-editor";
import { ElMessage } from "element-plus";
import { createRecorder } from "@/utils/monacoRecorder";

const { t } = useI18n();
const router = useRouter();
const form = ref();
const editor = ref();
const saveTip = useSaveTip();
const layoutStore = useLayoutStore();
const model = ref<PostLayout>();
const editMode = computed(() => model.value?.id !== emptyGuid);
const record = createRecorder(getQueryString("id") || "");

const load = async () => {
  model.value = await layoutStore.getLayout(getQueryString("id"));
  saveTip.init(model.value);
};

const onBack = () => {
  router.goBackOrTo(useRouteSiteId({ name: "layouts" }));
};

const onSave = async () => {
  if (!model.value) return;
  await form.value.validate();
  const isNewPage = model.value.id === emptyGuid;
  await layoutStore.updateLayout(model.value, record.save());
  ElMessage.success(t("common.saveSuccess"));
  saveTip.init(model.value);
  if (isNewPage) {
    router.replace(
      useRouteSiteId({ name: "layout-edit", query: { id: model.value.id } })
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
  <div class="h-full w-full flex flex-col overflow-hidden">
    <div
      class="pt-16 px-32 bg-fff dark:bg-[#222] dark:border-transparent border-b border-solid border-line flex"
    >
      <EditForm
        v-if="model"
        ref="form"
        :model="model"
        :edit-mode="editMode"
        inline
      />
      <div class="flex-1" />

      <el-button round class="lineButton" @click="editor.format()">{{
        t("common.format")
      }}</el-button>
    </div>

    <div v-if="model" class="flex-1">
      <MonacoEditor
        ref="editor"
        v-model="model.body"
        language="html"
        k-script
        :uri="Uri.file(model.id)"
        @monaco-load-complete="(monaco) => record.bindMonaco(monaco, 'html')"
      />
    </div>
    <KBottomBar
      back
      :permission="{
        feature: 'layout',
        action: 'edit',
      }"
      @cancel="onBack"
      @save="onSave"
    >
      <template #extra-buttons>
        <el-button
          v-hasPermission="{
            feature: 'layout',
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
