<script lang="ts" setup>
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import ImagePreview from "@/components/basic/image-preview.vue";
import { getChildElements, isTag, useAttribute, useElement } from "@/utils/dom";
import { image, close } from "./image-editor";
import { chooseImage } from "./image-dialog";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { tryGetExtension } from "@/utils/url";
import type { DomValueWrapper } from "@/global/types";

const { t } = useI18n();
const show = ref(true);
const form = ref();
const success = ref(false);
const elements = ref<HTMLElement[]>();

if (isTag(image.value!, "picture")) {
  elements.value = getChildElements(image.value!);
} else {
  elements.value = [image.value!];
}

const imageElement = computed(
  () => elements.value!.find((f) => isTag(f, "img")) as HTMLImageElement
);

const sourceElements = computed(() =>
  elements.value!.filter((f) => isTag(f, "source"))
);

const src = useAttribute(imageElement.value, "src");
const extension = tryGetExtension(src.value);
const sameSrcAttribute: DomValueWrapper[] = [];
if (extension) {
  try {
    for (const name of imageElement.value.getAttributeNames()) {
      if (["src", "title", "alt", "srcset", "id", "name"].includes(name))
        continue;
      if (imageElement.value.getAttribute(name)?.endsWith(extension)) {
        sameSrcAttribute.push(useAttribute(imageElement.value, name));
      }
    }
  } catch (error) {
    console.log(error);
  }
}
const srcset = useAttribute(
  sourceElements.value[0] ?? imageElement.value,
  "srcset"
);
const width = useAttribute(imageElement.value, "width");
const height = useAttribute(imageElement.value, "height");
const inheritWidth = useElement(imageElement.value, "width");
const inheritHeight = useElement(imageElement.value, "height");
const alt = useAttribute(imageElement.value, "alt");
const previewUrl = ref<string>(imageElement.value.src);

const onSave = async () => {
  success.value = true;
  show.value = false;
};

const replaceImage = async () => {
  const newImages = await chooseImage(false);
  const newImage = newImages[0];
  src.value = newImage.url;

  if (srcset.value) srcset.value = newImage.url;

  previewUrl.value = newImage.previewUrl;
};

watch(
  () => src.value,
  () => {
    previewUrl.value = src.value;
    for (const attribute of sameSrcAttribute) {
      attribute.value = src.value;
    }
    if (sourceElements.value) {
      for (const i of sourceElements.value) {
        (i as HTMLSourceElement).srcset = src.value;
      }
    }
  }
);

const closed = () => {
  close(success.value, [src, srcset, ...sameSrcAttribute, width, height, alt]);
};
</script>

<template>
  <el-dialog
    :model-value="show"
    width="800px"
    :close-on-click-modal="false"
    :title="t('common.image')"
    draggable
    @closed="closed"
  >
    <div class="flex w-full space-x-16">
      <div
        class="flex-1 items-center justify-center h-280px overflow-hidden rounded-normal relative"
      >
        <ImagePreview :src="previewUrl" :on-select="replaceImage" />
      </div>
      <div class="flex-1">
        <el-form ref="form" label-position="top">
          <el-form-item>
            <template #label>
              <div class="el-form-item__label">
                {{ t("common.size") }}
                <span class="text-s text-999"
                  >({{ t("common.imgSizeTip") }})</span
                >
              </div>
            </template>
            <div class="flex space-x-8 w-full">
              <el-input
                v-model.number="width"
                class="flex-1"
                :placeholder="inheritWidth?.toString()"
              >
                <template #append
                  ><span class="px-12">{{ t("common.width") }}</span></template
                >
              </el-input>
              <el-input
                v-model.number="height"
                class="flex-1"
                :placeholder="inheritHeight?.toString()"
              >
                <template #append
                  ><span class="px-12">{{ t("common.height") }}</span></template
                >
              </el-input>
            </div>
          </el-form-item>
          <el-form-item label="src">
            <el-input v-model="src" />
          </el-form-item>
          <el-form-item label="srcset">
            <el-input v-model="srcset" />
          </el-form-item>
          <el-form-item label="alt">
            <el-input v-model="alt" />
          </el-form-item>
        </el-form>
      </div>
    </div>

    <template #footer>
      <DialogFooterBar @confirm="onSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
