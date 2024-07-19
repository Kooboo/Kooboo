<script lang="ts" setup>
import MonacoEditor from "@/components/monaco-editor/index.vue";
import { ref, computed, watch } from "vue";
import { emptyGuid } from "@/utils/guid";
import type { Rules } from "async-validator";
import { onBeforeRouteLeave, useRouter } from "vue-router";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { getQueryString } from "@/utils/url";
import { useShortcut } from "@/hooks/use-shortcuts";
import KBottomBar from "@/components/k-bottom-bar/index.vue";
import { useSaveTip } from "@/hooks/use-save-tip";
import { isUniqueNameRule, rangeRule, requiredRule } from "@/utils/validate";
import { useI18n } from "vue-i18n";
import type { PostPage } from "@/api/pages/types";
import { getLayout } from "@/api/layout";
import { usePageStore } from "@/store/page";
import { mergeAddon } from "./layout-model";
import { addonToXml, layoutToAddon, pageToAddon } from "@/utils/page";
import type { AddonSource } from "./source";
import { usePreviewUrl } from "@/hooks/use-preview-url";
import { isUniqueName, pageUrlIsUniqueName } from "@/api/pages";
import { Uri } from "monaco-editor";
import { htmlFormat } from "monaco-editor-ex";
import { ElMessage } from "element-plus";
import { createRecorder } from "@/utils/monacoRecorder";

const { t } = useI18n();
const router = useRouter();
const saveTip = useSaveTip();
const model = ref<PostPage>();
const pageStore = usePageStore();
const sources = ref<AddonSource[]>([]);
const oldUrlPath = ref();
const { onPreview } = usePreviewUrl();
const record = createRecorder(getQueryString("id") || "");

(async () => {
  const layout = await getLayout(getQueryString("layoutId")!);
  model.value = await pageStore.getPage(getQueryString("id")!);
  oldUrlPath.value = model.value.urlPath;
  const layoutAddon = layoutToAddon(layout.name, layout.body);
  const pageAddon = pageToAddon(model.value.body);
  await mergeAddon(layoutAddon, pageAddon, sources.value);
  model.value.body = addonToXml(layoutAddon);
  model.value.body = (await htmlFormat(model.value.body))!;
  saveTip.init(model.value);
})();

const rules = computed(() => {
  return {
    name:
      model.value?.id !== emptyGuid
        ? []
        : [
            rangeRule(1, 50),
            requiredRule(t("common.nameRequiredTips")),
            isUniqueNameRule(
              isUniqueName,
              t("common.thePageNameAlreadyExists")
            ),
          ],
    urlPath: [
      requiredRule(t("common.urlRequiredTips")),
      oldUrlPath.value === model.value?.urlPath
        ? ""
        : isUniqueNameRule(
            (name: string) => pageUrlIsUniqueName(name, oldUrlPath.value),
            t("common.urlOccupied")
          ),
    ],
  } as Rules;
});

const form = ref();
const editor = ref();

const nameChanged = (value: string) => {
  if (!model.value) return;
  if (!model.value.urlPath) {
    model.value.urlPath = `/${value}`;
  }
};

const onBack = () => {
  router.goBackOrTo(useRouteSiteId({ name: "pages" }));
};

const onSave = async () => {
  if (!model.value) return;
  model.value.type = "Layout";
  await pageStore.updatePage(model.value, record.save());
  ElMessage.success(t("common.saveSuccess"));
  oldUrlPath.value = model.value.urlPath;
  saveTip.init(model.value);
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
watch(
  () => model.value,
  () => {
    saveTip.changed(model.value);
  },
  {
    deep: true,
  }
);
useShortcut("save", onSave);
const saveAndReturn = async () => {
  await onSave();
  onBack();
};
</script>

<template>
  <div v-if="model" class="h-full w-full flex flex-col overflow-hidden">
    <el-form
      ref="form"
      :model="model"
      :rules="rules"
      inline
      class="pt-24 pl-50px bg-fff dark:bg-[#333] border-b border-solid border-line dark:border-transparent"
    >
      <div class="flex items-center space-x-16">
        <el-form-item :label="t('common.pageName')" prop="name">
          <el-input
            v-model="model.name"
            class="w-302px"
            :title="model.name"
            :disabled="model.id !== emptyGuid"
            data-cy="page-name"
            @input="model!.name = model?.name.replace(/\s+/g, '') as string"
            @change="nameChanged"
          />
        </el-form-item>
        <el-form-item label="URL" prop="urlPath">
          <el-input
            v-model="model.urlPath"
            class="w-302px"
            @input="
            model!.urlPath = model?.urlPath.replace(/\s+/g, '') as string
            "
          />
        </el-form-item>
        <div class="flex-1" />
        <el-form-item class="pr-16">
          <el-button
            v-if="model.previewUrl"
            round
            class="lineButton"
            @click="onPreview(model!.previewUrl!)"
          >
            {{ t("common.preview") }}
          </el-button>
          <el-button round class="lineButton" @click="editor.format()">{{
            t("common.format")
          }}</el-button>
        </el-form-item>
      </div>
    </el-form>
    <div class="flex-1">
      <MonacoEditor
        ref="editor"
        v-model="model.body"
        language="html"
        :uri="Uri.file(model.id)"
        k-script
        @monaco-load-complete="(monaco) => record.bindMonaco(monaco, 'html')"
      />
    </div>
    <KBottomBar
      back
      :permission="{
        feature: 'pages',
        action: 'edit',
      }"
      @cancel="onBack"
      @save="onSave"
    >
      <template #extra-buttons>
        <el-button
          v-hasPermission="{
            feature: 'pages',
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
