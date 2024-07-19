<script lang="ts" setup>
import MonacoEditor from "@/components/monaco-editor/index.vue";
import { nextTick, ref, watch } from "vue";
import { emptyGuid } from "@/utils/guid";
import { onBeforeRouteLeave, useRouter } from "vue-router";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { getQueryString } from "@/utils/url";
import { useShortcut } from "@/hooks/use-shortcuts";
import KBottomBar from "@/components/k-bottom-bar/index.vue";
import { useSaveTip } from "@/hooks/use-save-tip";
import type { Style } from "@/api/style/types";
import { useStyleStore } from "@/store/style";
import EditForm from "./edit-form.vue";
import { useI18n } from "vue-i18n";
import { Uri } from "monaco-editor";
import { ElMessage } from "element-plus";
import { css_beautify } from "js-beautify";
import { createRecorder } from "@/utils/monacoRecorder";

const { t } = useI18n();
const router = useRouter();
const form = ref();
const editor = ref();
const saveTip = useSaveTip();
const styleStore = useStyleStore();
const record = createRecorder(getQueryString("id") || "");

const model = ref<Style>({
  id: getQueryString("id") || emptyGuid,
  name: "",
  body: "",
  extension: "css",
  isEmbedded: false,
  ownerObjectId: undefined as any,
});

const load = async () => {
  if (model.value.id !== emptyGuid) {
    model.value = await styleStore.getStyle(model.value.id);
  }

  saveTip.init(model.value);
  await nextTick();
};

const onBack = () => {
  router.goBackOrTo(useRouteSiteId({ name: "styles" }));
};

const onSave = async () => {
  await form.value?.validate();
  const isNewPage = model.value.id === emptyGuid;
  await styleStore.updateStyle(model.value, record.save());
  ElMessage.success(t("common.saveSuccess"));
  saveTip.init(model.value);
  if (isNewPage) {
    router.replace(
      useRouteSiteId({ name: "style-edit", query: { id: model.value.id } })
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

const format = () => {
  model.value.body = css_beautify(model.value.body);
};

useShortcut("format", format);
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
      class="py-16 px-32 bg-fff dark:bg-[#222] dark:border-transparent border-b border-solid border-line flex"
    >
      <EditForm
        v-if="!model.isEmbedded"
        ref="form"
        :model="model"
        inline
        class="h-40px"
      />
      <span v-else class="h-40px leading-10 dark:text-[#fffa]">{{
        t("common.embeddedStyle")
      }}</span>

      <div class="flex-1" />
      <el-button round class="lineButton" @click="editor.format()">{{
        t("common.format")
      }}</el-button>
    </div>
    <div class="flex-1">
      <MonacoEditor
        ref="editor"
        v-model="model.body"
        language="css"
        k-script
        :uri="Uri.file(model.id)"
        @monaco-load-complete="(monaco) => record.bindMonaco(monaco, 'css')"
      />
    </div>
    <KBottomBar
      back
      :permission="{ feature: 'style', action: 'edit' }"
      @cancel="onBack"
      @save="onSave"
    >
      <template #extra-buttons>
        <el-button
          v-hasPermission="{ feature: 'style', action: 'edit' }"
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
