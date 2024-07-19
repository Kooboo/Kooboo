<script lang="ts" setup>
import type { PostForm } from "@/api/form/types";
import { useFormStore } from "@/store/form";
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
const formStore = useFormStore();
const devModeStore = useDevModeStore();
const model = ref<PostForm>();
const saveTip = useSaveTip();
const editor = ref();
const editorCompleter = new Completer();

const load = async () => {
  model.value = await formStore.getForm(props.id);
  editorCompleter.resolve(null);
  saveTip.init(model.value);
};

const save = async () => {
  if (!model.value) return;
  await formStore.updateForm(model.value, devModeStore.saveTabRecord(props.id));
  saveTip.init(model.value);
  emit("changed", false);
  emit("setAction", "save", { visible: false });
};

watch(
  () => model.value,
  () => {
    emit("changed", saveTip.changed(model.value));
    emit("setAction", "save", { visible: saveTip.changed(model.value) });
  },
  { deep: true }
);

const goToLine = async (line: number, searchText?: string) => {
  await editorCompleter.promise;
  if (searchText) {
    editor.value.search(searchText, line);
  } else {
    editor.value.goToLine(line);
  }
};

load();
defineExpose({ save, load, goToLine });
</script>

<template>
  <div v-if="model" class="h-full">
    <MonacoEditor
      ref="editor"
      v-model="model.body"
      language="html"
      k-script
      :uri="Uri.file(model.id)"
      @monacoLoadComplete="
        (monaco) => devModeStore.addTabRecord(props.id, monaco, 'html')
      "
    />
  </div>
</template>
