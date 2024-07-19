<script lang="ts" setup>
import { ImageEditor } from "@/components/k-media-dialog";
import type { MediaItem } from "../../../api/content/media";
import { getMedia } from "../../../api/content/media";
import { computed, onMounted, ref, reactive } from "vue";
import { getQueryString, combineUrl } from "../../../utils/url";
import KBottomBar from "@/components/k-bottom-bar/index.vue";
import { imageUpdate } from "../../../api/content/media";
import { useRoute, useRouter } from "vue-router";
import SvgEditor from "@/components/svg-editor/index.vue";
import { api } from "@/utils/request";
import { useSiteStore } from "@/store/site";
import { useSaveTip } from "@/hooks/use-save-tip";

const route = useRoute();
const router = useRouter();
const media = ref<MediaItem>();
const id = getQueryString("id") as string;
const provider = getQueryString("provider") as string;
const sonMedia = reactive({
  id,
  alt: "",
  base64: "",
  provider: provider,
  name: "",
});
const originBase64 = ref("");
const ext = ref("");
const fullName = computed(() => `${sonMedia.name}${ext.value}`);

const isSvg = ref(false);
const svgData = ref();
const siteStore = useSiteStore();
const imageEditor = ref();
const saveTip = useSaveTip(undefined, {
  defaultActiveMenu: "editImage",
  modelGetter: () => sonMedia,
});

async function fetchImage() {
  media.value = await getMedia({ id, provider });
  let name = media.value.name ?? "";
  const extIndex = name.lastIndexOf(".");
  if (extIndex > 0) {
    ext.value = name.substring(extIndex);
    name = name.substring(0, extIndex);
  }
  sonMedia.name = name;
  media.value.alt = media.value.alt || "";
  sonMedia.alt = media.value.alt;
  sonMedia.base64 = media.value.base64 ?? "";

  originBase64.value = media.value.base64 ?? "";
  sonMedia.provider = provider;
  isSvg.value = media.value.url.endsWith(".svg");
  if (isSvg.value) {
    if (media.value.svg) {
      svgData.value = media.value.svg;
    } else {
      const svgUrl = combineUrl(
        siteStore.site.prUrl,
        media.value.siteUrl || media.value.url
      );
      const result = await api.get(svgUrl);
      svgData.value = result.data;
    }
  }

  saveTip.init(sonMedia);
}
onMounted(() => {
  fetchImage();
});

const editable = computed(() => {
  if (!media.value) return false;
  return (
    sonMedia.base64 !== originBase64.value ||
    sonMedia.alt !== media.value.alt ||
    fullName.value !== media.value.name ||
    isSvg.value
  );
});

function goBack() {
  router.push({ name: "media", query: { ...route.query, id: undefined } });
}

async function submitEdit() {
  if (isSvg.value) {
    const svg = imageEditor.value.getSvgString();
    sonMedia.base64 = svg ? window.btoa(svg) : "";
  } else if (sonMedia.base64 === originBase64.value) {
    sonMedia.base64 = "";
  }
  imageEditor.value?.validate(async (isValid: boolean) => {
    if (!isValid) {
      return;
    }
    await imageUpdate({ ...sonMedia, name: fullName.value });
    saveTip.init(sonMedia);
    goBack();
  });
}

async function cancel() {
  if (!media.value) return false;
  goBack();
}
</script>
<template>
  <template v-if="media">
    <SvgEditor
      v-if="isSvg"
      ref="imageEditor"
      v-model:alt="sonMedia.alt"
      v-model:name="sonMedia.name"
      :svg="svgData"
      :ext="ext"
      :url="media.url"
      :site-url="media.siteUrl"
    />
    <ImageEditor
      v-else
      ref="imageEditor"
      v-model:alt="sonMedia.alt"
      v-model:base64="sonMedia.base64"
      v-model:name="sonMedia.name"
      :ext="ext"
      :url="media.url"
      :site-url="media.siteUrl"
    />
  </template>

  <KBottomBar
    v-if="media"
    :disabled="!editable"
    @save="submitEdit"
    @cancel="cancel"
  />
</template>
