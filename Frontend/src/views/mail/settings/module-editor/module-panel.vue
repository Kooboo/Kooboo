<script lang="ts" setup>
import {
  updateMailModuleFile,
  getMailModuleText,
} from "@/api/mail/mail-module";
import { ref, watch } from "vue";
import MonacoEditor from "@/components/monaco-editor/index.vue";
import { useSaveTip } from "@/hooks/use-save-tip";
import type { ModuleFileInfo, ResourceType } from "@/api/module/types";
import { computed } from "@vue/reactivity";
import type { Action } from "@/store/dev-mode";
import { newGuid } from "@/utils/guid";
import { useI18n } from "vue-i18n";
import { Uri } from "monaco-editor";
const { t } = useI18n();

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

const load = async () => {
  model.value = await getMailModuleText(
    props.params.file.objectType,
    props.params.moduleId,
    props.params.file.name
  );
  saveTip.init(model.value);
};

const save = async () => {
  await updateMailModuleFile(
    props.params.file.objectType,
    props.params.moduleId,
    props.params.file.name,
    model.value ? model.value : " ",
    t("common.saveSuccess")
  );
  saveTip.init(model.value);
  emit("changed", false);
  emit("setAction", "save", { visible: false });
};

const isConfig = computed(() => props.params.file.name === "Module.config");

const modelUri = computed(() => {
  if (isConfig.value)
    return `memory:///mail_module:${props.params.moduleId}/${props.params.file.name}`;
  return newGuid();
});

const lang = computed(() => {
  if (props.params.file.objectType === "css") return "css";
  if (props.params.file.objectType === "js") return "typescript";
  if (props.params.file.objectType === "view") return "html";
  if (props.params.file.objectType === "read") return "html";
  if (props.params.file.objectType === "compose") return "html";
  if (props.params.file.objectType === "backend") return "html";
  if (props.params.file.objectType === "api") return "typescript";
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
      :model-uri="
        Uri.from({
          scheme: 'memory',
          path: modelUri,
        })
      "
      :language="lang as any"
      :kmail="lang == 'html' || lang == 'typescript'"
      k-script
    />
  </div>
</template>
