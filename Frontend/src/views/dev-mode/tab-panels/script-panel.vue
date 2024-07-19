<script lang="ts" setup>
import type { Script } from "@/api/script/types";
import { useScriptStore } from "@/store/script";
import { ref, watch } from "vue";
import MonacoEditor from "@/components/monaco-editor/index.vue";
import { useSaveTip } from "@/hooks/use-save-tip";
import type { Action } from "@/store/dev-mode";
import { Completer } from "@/utils/lang";
import { Uri } from "monaco-editor";
import { useDevModeStore } from "@/store/dev-mode";

const emit = defineEmits<{
  (e: "changed", value: boolean): void;
  (e: "setAction", name: string, params: Partial<Action>): void;
}>();

const props = defineProps<{ id: string }>();
const scriptStore = useScriptStore();
const devModeStore = useDevModeStore();
const model = ref<Script>();
const saveTip = useSaveTip();
const editor = ref();
const editorCompleter = new Completer();

const load = async () => {
  model.value = await scriptStore.getScript(props.id);
  editorCompleter.resolve(null);
  saveTip.init(model.value);
};

const save = async () => {
  if (!model.value) return;
  await scriptStore.updateScript(
    model.value,
    devModeStore.saveTabRecord(props.id)
  );
  saveTip.init(model.value);
  emit("changed", false);
  emit("setAction", "save", { visible: false });
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
defineExpose({ save, load, goToLine });
</script>

<template>
  <div v-if="model" class="h-full">
    <MonacoEditor
      ref="editor"
      v-model="model.body"
      language="javascript"
      :uri="
        Uri.from({
          scheme: 'memory',
          path: model.id,
        })
      "
      @monacoLoadComplete="
        (monaco) => devModeStore.addTabRecord(props.id, monaco, 'javascript')
      "
    />
  </div>
</template>
