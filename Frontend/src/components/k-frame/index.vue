<script lang="ts" setup>
import { toUniversalSchema } from "@/utils/url";
import { onMounted, ref, watch } from "vue";

const props = defineProps<{
  content: string;
  baseUrl?: string;
  agentEvents?: (el: HTMLElement) => void;
}>();
const element = ref<HTMLIFrameElement>();

const appendBaseUrl = (html: string) => {
  var url = toUniversalSchema(props.baseUrl);
  var head = `<!DOCTYPE html><base href='${url}' />`;
  return head + html;
};

const setContent = () => {
  if (!element.value?.contentDocument) return;
  element.value.contentDocument.open();
  const content = props.baseUrl ? appendBaseUrl(props.content) : props.content;
  element.value.contentDocument.write(content);
  element.value.contentDocument.close();

  if (props.agentEvents) {
    const win = element.value?.contentWindow;
    if (!win) return;
    const elPrototype = (win as any).HTMLElement.prototype;
    props.agentEvents(elPrototype);
  } else {
    element.value.contentDocument.onclick = (e) => {
      e.preventDefault();
    };

    element.value.contentDocument.onkeydown = (e) => {
      e.preventDefault();
      const event = new KeyboardEvent("keydown", e);
      element.value?.dispatchEvent(event);
    };
  }
};

watch(() => props.content, setContent);

onMounted(setContent);

defineExpose({ element });
</script>

<template>
  <iframe
    ref="element"
    frameborder="0"
    src="about:blank"
    class="w-full h-full"
    sandbox="allow-same-origin allow-scripts"
  />
</template>
