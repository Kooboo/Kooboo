<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { marked } from "marked";
import { getMailModuleText } from "@/api/mail/mail-module";
import KFrame from "@/components/k-frame/index.vue";
import { computed } from "@vue/reactivity";

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

const props = defineProps<{ modelValue: boolean; id: string }>();

const { t } = useI18n();
const show = ref(true);
const html = ref<string>("");

getMailModuleText("root", props.id, "Readme.md").then((rsp) => {
  html.value = marked.parse(rsp);
});

const htmlWithBorder = computed(() => {
  return `<div style="padding:10px;">${html.value}<div>`;
});
</script>

<template>
  <el-dialog
    custom-class="el-dialog--zero-padding"
    :model-value="show"
    width="1000px"
    :close-on-click-modal="false"
    :title="t('common.description')"
    @closed="emit('update:modelValue', false)"
  >
    <div class="px-12">
      <KFrame class="h-400px" :content="htmlWithBorder" />
    </div>
  </el-dialog>
</template>
