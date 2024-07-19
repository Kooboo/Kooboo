<template>
  <div
    v-if="show"
    ref="richText"
    class="dark:text-[#fffa] overflow-x-auto"
    data-cy="email-content"
  />
</template>
<script setup lang="ts">
import { ref, nextTick, watch } from "vue";

const show = ref(true);
const richText = ref<HTMLElement>();
const props = defineProps<{
  html: string;
  hideReplyCalendarButton: boolean;
}>();

const model = ref<string>("");

const load = () => {
  if (props.html) {
    model.value = props.html;
  }
  // 把所有a标签加上属性target="_blank"
  model.value = model.value?.replace(
    new RegExp("\<a(.*?)\>", "ig"),
    (match: any, p1: string) => {
      if (/target=['"].*?['"]/.exec(p1)) {
        p1 = p1.replace(/target=(['"])(.*?['"])/, 'target="_blank"');
        return "<a" + p1 + ">";
      } else {
        return `<a target="_blank" ` + p1 + ">";
      }
    }
  );

  model.value = model.value?.replace(
    new RegExp("\<img(.*?)>", "ig"),
    (match: any, p1: string) => {
      // 是否有style标签
      if (/style=['"].*?['"]/.exec(p1)) {
        p1 = p1.replace(/style=(['"])(.*?['"])/, "style=$1max-width:100%;$2");
        return "<img" + p1 + ">";
      } else {
        return `<img style="max-width: 100%;" ` + p1 + ">";
      }
    }
  );

  // 对日程邀请里的按钮进行隐藏
  if (props.hideReplyCalendarButton)
    model.value = model.value?.replace(
      /<tr class="hide-reply-calendar-button">/g,
      `<tr class="hide-reply-calendar-button" style="display: none;">`
    );

  nextTick(() => {
    var richShadow =
      richText.value?.shadowRoot ??
      richText.value?.attachShadow({ mode: "open" });
    richShadow!.innerHTML = model.value;
  });
};

load();

watch(
  () => props.html,
  () => {
    load();
  }
);
</script>
