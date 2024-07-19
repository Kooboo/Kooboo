<script lang="ts" setup>
import MonacoEditor from "@/components/monaco-editor/index.vue";
import { ref, watch } from "vue";
import { emptyGuid } from "@/utils/guid";
import { onBeforeRouteLeave, useRouter } from "vue-router";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { getQueryString } from "@/utils/url";
import { useShortcut } from "@/hooks/use-shortcuts";
import KBottomBar from "@/components/k-bottom-bar/index.vue";
import type { PostView } from "@/api/view/types";
import { useSaveTip } from "@/hooks/use-save-tip";
import { useViewStore } from "@/store/view";
import EditForm from "./edit-form.vue";
import { Uri } from "monaco-editor";
import { useI18n } from "vue-i18n";
import { ElMessage } from "element-plus";
import FieldsEditor from "@/components/fields-editor/index.vue";
import type {
  PropertyJsonString,
  PropertyOptions,
} from "@/global/control-type";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { cloneDeep } from "lodash-es";
import { createRecorder } from "@/utils/monacoRecorder";

const { t } = useI18n();
const viewStore = useViewStore();
const router = useRouter();
const form = ref();
const editor = ref();
const saveTip = useSaveTip();
const model = ref<PostView>();
const record = createRecorder(getQueryString("id") || "");

const editorOptions: PropertyOptions = {
  hideDisplayInSearchResult: true,
  hideMultipleLanguage: true,
  hideSummaryField: true,
};

const showEditParameters = ref(false);
const paramDefines = ref<PropertyJsonString[]>([]);

const load = async () => {
  model.value = await viewStore.getView(getQueryString("id"));
  saveTip.init(model.value);
};

const onBack = () => {
  router.goBackOrTo(useRouteSiteId({ name: "views" }));
};

const onSave = async () => {
  if (!model.value) return;
  await form.value.validate();
  const isNewPage = model.value.id === emptyGuid;
  await viewStore.updateView(model.value, record.save());
  ElMessage.success(t("common.saveSuccess"));
  saveTip.init(model.value);
  if (isNewPage) {
    router.replace(
      useRouteSiteId({ name: "view-edit", query: { id: model.value.id } })
    );
  }
};

const onEditPropDefines = () => {
  showEditParameters.value = true;
  paramDefines.value = cloneDeep(model.value?.propDefines ?? []);
};
const onSavePropDefines = () => {
  if (!model.value) return;
  model.value.propDefines = cloneDeep(paramDefines.value);
  showEditParameters.value = false;
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
      class="pt-16 px-32 bg-fff dark:bg-[#222] dark:border-transparent border-b border-solid border-line flex items-start"
    >
      <EditForm v-if="model" ref="form" :model="model" inline />
      <div class="flex-1" />
      <el-button round class="lineButton" @click="onEditPropDefines">
        {{ t("common.parameters") }}
      </el-button>
      <el-button round class="lineButton" @click="editor.format()">
        {{ t("common.format") }}
      </el-button>
    </div>

    <div class="flex-1">
      <MonacoEditor
        v-if="model"
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
      :permission="{ feature: 'view', action: 'edit' }"
      @cancel="onBack"
      @save="onSave"
    >
      <template #extra-buttons>
        <el-button
          v-hasPermission="{ feature: 'view', action: 'edit' }"
          round
          type="primary"
          data-cy="save-and-return"
          @click="saveAndReturn"
          >{{ t("common.saveAndReturn") }}</el-button
        >
      </template>
    </KBottomBar>
  </div>
  <el-dialog
    v-model="showEditParameters"
    :close-on-click-modal="false"
    :title="t('common.parameters')"
  >
    <FieldsEditor
      v-model="paramDefines"
      :property-options="editorOptions"
      :show-default-value="true"
      :hide-preview-field="true"
    />
    <template #footer>
      <DialogFooterBar
        @confirm="onSavePropDefines"
        @cancel="showEditParameters = false"
      />
    </template>
  </el-dialog>
</template>
