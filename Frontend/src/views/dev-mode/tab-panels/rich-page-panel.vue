<script lang="ts" setup>
import type { Page, PostRichPage } from "@/api/pages/types";
import { toRichPost } from "@/api/pages/types";
import { usePageStore } from "@/store/page";
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
const pageStore = usePageStore();
const devModeStore = useDevModeStore();
const model = ref<PostRichPage>();
const saveTip = useSaveTip((key, value) => {
  if (["url", "published", "title"].includes(key)) {
    return undefined;
  } else {
    return value;
  }
});
const editor = ref();
const editorCompleter = new Completer();

const load = async () => {
  const rsp = await pageStore.getPage(props.id);
  editorCompleter.resolve(null);
  model.value = toRichPost(rsp);
  saveTip.init(model.value);
};
const updateModel = async (page: Page) => {
  if (!model.value) return;
  model.value.url = page.path;
  model.value.title = page.title;
  model.value.published = page.online;
};
const save = async () => {
  if (!model.value) return;
  await pageStore.updateRichPage(
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
defineExpose({ save, load, goToLine, updateModel });
</script>

<template>
  <div v-if="model" class="h-full">
    <MonacoEditor
      ref="editor"
      v-model="model.body"
      language="html"
      :uri="Uri.file(model.id)"
      @monacoLoadComplete="
        (monaco) => devModeStore.addTabRecord(props.id, monaco, 'html')
      "
    />
  </div>
</template>
