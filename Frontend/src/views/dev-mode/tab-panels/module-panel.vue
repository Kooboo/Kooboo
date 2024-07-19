<script lang="ts" setup>
import { updateFile, getText } from "@/api/module/files";
import { ref, watch } from "vue";
import MonacoEditor from "@/components/monaco-editor/index.vue";
import { useSaveTip } from "@/hooks/use-save-tip";
import type { ModuleFileInfo, ResourceType } from "@/api/module/types";
import { computed } from "@vue/reactivity";
import type { Action } from "@/store/dev-mode";
import { newGuid } from "@/utils/guid";
import { useModuleStore } from "@/store/module";

const emit = defineEmits<{
  (e: "changed", value: boolean): void;
  (e: "setAction", name: string, params: Partial<Action>): void;
}>();

const props = defineProps<{
  id: string;
  params: { type: ResourceType; file: ModuleFileInfo; moduleId: string };
}>();

const model = ref<string>("");
const saveTip = useSaveTip();
const moduleStore = useModuleStore();

const load = async () => {
  model.value = await getText(
    props.params.file.objectType,
    props.params.moduleId,
    props.params.file.name
  );
  saveTip.init(model.value);
};

const save = async () => {
  if (!model.value) return;
  await updateFile(
    props.params.file.objectType,
    props.params.moduleId,
    props.params.file.name,
    model.value
  );
  saveTip.init(model.value);
  emit("changed", false);
  emit("setAction", "save", { visible: false });
};

const isConfig = computed(() => props.params.file.name === "Module.config");

const modelUri = computed(() => {
  if (props.params.file.objectType == "code") {
    const name = moduleStore.list.find(
      (f) => f.id == props.params.moduleId
    )?.name;
    let file = props.params.file.name;
    return `file:///node_modules/@types/module:${name}/${file}.ts`;
  }
  if (isConfig.value)
    return `memory:///module:${props.params.moduleId}/${props.params.file.name}`;
  return `memory:///${newGuid()}`;
});

const lang = computed(() => {
  if (props.params.file.objectType === "css") return "css";
  if (props.params.file.objectType === "js") return "javascript";
  if (props.params.file.objectType === "view") return "html";
  if (props.params.file.objectType === "backend") return "html";
  if (props.params.file.objectType === "api") return "typescript";
  if (props.params.file.objectType === "code") return "typescript";
  if (props.params.file.name === "Module.config") return "json";
  if (props.params.file.name === "Dashboard.js") return "typescript";
  if (props.params.file.name === "Event.js") return "typescript";
  return "";
});

watch(
  () => model.value,
  () => {
    emit("changed", saveTip.changed(model.value));
    emit("setAction", "save", { visible: saveTip.changed(model.value) });
  },
  { deep: true }
);

load();
defineExpose({ save, load });
</script>

<template>
  <div class="h-full">
    <MonacoEditor
      v-model="model"
      :uri="modelUri"
      :language="lang as any"
      k-script
    />
  </div>
</template>
