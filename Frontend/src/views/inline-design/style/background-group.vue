<script lang="ts" setup>
import { useStyle } from "@/utils/dom";
import EnumProperty from "./enum-property.vue";
import {
  backgroundSizes,
  backgroundRepeats,
  backgroundPositions,
} from "@/global/style";
import { inject } from "vue";
import type { DomValueWrapper } from "@/global/types";
import { element } from ".";
import { chooseImage } from "../image/image-dialog";
import { useI18n } from "vue-i18n";

const { t } = useI18n();
const image = useStyle(element.value!, "background-image");
const size = useStyle(element.value!, "background-size");
const repeat = useStyle(element.value!, "background-repeat");
const position = useStyle(element.value!, "background-position");
const color = useStyle(element.value!, "background-color");
const style = getComputedStyle(element.value!);

const selectImage = async () => {
  const newImages = await chooseImage(false);
  const newImage = newImages[0];
  image.value = `url(${newImage.url})`;
};

const values = inject<DomValueWrapper[]>("values");
values?.push(image, size, repeat, position, color);
</script>

<template>
  <div class="h-180px flex w-full space-x-12">
    <div class="flex-1 w-full relative group">
      <ImagePreview
        :src="image || style.backgroundImage"
        class="rounded-normal"
        :has-url-wrap="true"
        :on-select="selectImage"
      />
      <IconButton
        v-if="style.backgroundImage != 'none'"
        class="opacity-0 group-hover:opacity-100 absolute top-12 right-12 transform scale-75 origin-top-right"
        circle
        icon="icon-delete"
        :tip="t('common.remove')"
        @click="image = 'none'"
      />
    </div>
    <div class="flex-1 space-y-4">
      <EnumProperty
        v-model="size"
        :placeholder="style.backgroundSize"
        :options="backgroundSizes"
        title="background-size"
      />
      <EnumProperty
        v-model="repeat"
        :placeholder="style.backgroundRepeat"
        :options="backgroundRepeats"
        title="background-repeat"
      />
      <EnumProperty
        v-model="position"
        :placeholder="style.backgroundPosition"
        :options="backgroundPositions"
        title="background-position"
      />
      <ColorInput v-model="color" :display-color="style.backgroundColor" />
    </div>
  </div>
</template>

<style lang="scss" scoped>
:deep(.el-button) {
  &:hover {
    @apply !bg-card-disabled;
  }
  &:focus {
    @apply !bg-card-disabled;
  }
}
</style>
