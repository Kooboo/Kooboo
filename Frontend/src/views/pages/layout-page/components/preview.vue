<script lang="ts" setup>
import type { PostPage } from "@/api/pages/types";
import KFrame from "@/components/k-frame/index.vue";
import { useSiteStore } from "@/store/site";
import { domParse, domStringify, getElements } from "@/utils/dom";
import { ref, watch } from "vue";
import type { Addon, Placeholder } from "@/global/types";
import type { AddonSource } from "../source";

const props = defineProps<{
  page: PostPage;
  design: Addon;
  sources: AddonSource[];
}>();

const siteStore = useSiteStore();
const previewContent = ref("");

const buildHtml = (addon: Addon, isRoot: boolean) => {
  const source = props.sources.find(
    (f) => f.tag === addon.type && f.id === addon.id
  );

  if (!source) return "";

  if (addon.type !== "layout") {
    return source.source.body;
  }

  let dom = domParse(source.source.body);

  if (isRoot) {
    if (props.page.styles) {
      for (const i of props.page.styles) {
        const style = dom.createElement("link");
        style.rel = "stylesheet";
        style.href = i;
        dom.head.appendChild(style);
      }
    }

    if (props.page.scripts) {
      for (const i of props.page.scripts) {
        const script = dom.createElement("script");
        script.src = i;
        dom.head.appendChild(script);
      }
    }
  }

  const placeholders = addon.content as Placeholder[];

  for (const i of getElements(dom)) {
    const name = i.getAttribute("k-placeholder");
    const placeholder = placeholders.find((f) => f.name === name);

    if (placeholder && placeholder.addons) {
      i.innerHTML = placeholder.addons.map((m) => buildHtml(m, false)).join("");
    }
  }

  return domStringify(dom);
};

watch(
  [() => props.sources, () => props.design, () => props.page],
  () => {
    previewContent.value = buildHtml(props.design, true);
  },
  { deep: true, immediate: true }
);
</script>

<template>
  <KFrame :content="previewContent" :base-url="siteStore.site.prUrl" />
</template>
