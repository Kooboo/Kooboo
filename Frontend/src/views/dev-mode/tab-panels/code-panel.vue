<script lang="ts" setup>
import type { PostCode } from "@/api/code/types";
import { useCodeStore } from "@/store/code";
import { computed, onUnmounted, ref, watch } from "vue";
import MonacoEditor from "@/components/monaco-editor/index.vue";
import { useSaveTip } from "@/hooks/use-save-tip";
import type { Action } from "@/store/dev-mode";
import { Completer } from "@/utils/lang";
import { monaco } from "@/components/monaco-editor/userWorker";
import { useDevModeStore } from "@/store/dev-mode";
import { useI18n } from "vue-i18n";

const emit = defineEmits<{
  (e: "changed", value: boolean): void;
  (e: "setAction", name: string, params: Partial<Action>): void;
}>();

const { t } = useI18n();
const props = defineProps<{ id: string }>();
const codeStore = useCodeStore();
const devModeStore = useDevModeStore();
const model = ref<PostCode>();
const saveTip = useSaveTip((key, value) => {
  if ((key === "url" || key === "scriptType") && value) {
    return undefined;
  } else {
    return value;
  }
});
const config = ref(false);
const editor = ref();
const editorCompleter = new Completer();

const load = async () => {
  model.value = await codeStore.getCode(props.id);
  editorCompleter.resolve(null);
  saveTip.init(model.value);
};

const save = async () => {
  if (!model.value || model.value.isDecrypted) return;
  await codeStore.updateCode(model.value, devModeStore.saveTabRecord(props.id));
  saveTip.init(model.value);
  emit("changed", false);
  emit("setAction", "save", { visible: false });
};
const updateModel = async (code: PostCode) => {
  if (!model.value) return;
  model.value.url = code.url;
  model.value.scriptType = code.scriptType;
};
const goToLine = async (line: number, searchText?: string) => {
  await editorCompleter.promise;
  if (searchText) {
    editor.value.search(searchText, line);
  } else {
    editor.value.goToLine(line);
  }
};

watch(
  () => model.value,
  () => {
    emit("changed", saveTip.changed(model.value));
    emit("setAction", "save", { visible: saveTip.changed(model.value) });
  },
  { deep: true }
);

load();
defineExpose({ save, load, config, goToLine, updateModel });

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
</script>

<template>
  <div v-if="model" class="h-full relative">
    <div
      v-if="model.isDecrypted"
      class="absolute inset-0 flex items-center justify-center"
    >
      <el-result icon="error" :title="t('common.codeEncryptedTip')" />
    </div>
    <MonacoEditor
      v-else
      ref="editor"
      v-model="model.body"
      language="typescript"
      :uri="monaco.Uri.file(modelUri!)"
      k-script
      @monacoLoadComplete="
        (monaco) => devModeStore.addTabRecord(props.id, monaco, 'typescript')
      "
    />
  </div>
</template>
