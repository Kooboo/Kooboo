<script lang="ts" setup>
import { useSiteStore } from "@/store/site";
import { combineUrl } from "@/utils/url";
import { ref } from "vue";
import defaultImage from "@/assets/images/commerce_default_image.svg";
import type { MediaFileItem } from "@/components/k-media-dialog";
import KMediaDialog from "@/components/k-media-dialog";
import { useI18n } from "vue-i18n";
import VueDraggable from "vuedraggable";
import type { SortChangeEvent } from "@/global/types";
import { cloneDeep } from "lodash";

const props = defineProps<{
  modelValue: string[];
  main: string;
  mainLabel?: string;
  prefix?: string;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: string[]): void;
  (e: "update:main", value: string): void;
}>();

const siteStore = useSiteStore();
const visibleMediaDialog = ref(false);
const { t } = useI18n();
const isChange = ref(false);
const changeIndex = ref(0);

const displayImage = (url: string) => {
  url = url ? combineUrl(siteStore.site.prUrl, url) : defaultImage;
  return `url("${url}")`;
};

function handleChooseFile(files: MediaFileItem[]) {
  const images = [...props.modelValue, ...files.map((m) => m.url)];
  if (!props.main) {
    emit("update:main", images[0]);
  }
  emit("update:model-value", images);
}

function changeImage(files: MediaFileItem[], index: number) {
  const images = [...props.modelValue];
  const replaced = images.splice(index, 1, files[0].url);
  if (props.main == replaced[0]) {
    emit("update:main", files[0].url);
  }
  emit("update:model-value", images);
}

function onDelete(index: number) {
  const list = [...props.modelValue];
  list.splice(index, 1);
  emit("update:model-value", list);
  if (!list.includes(props.main)) {
    emit("update:main", list[0]);
  }
}

function onChange(e: SortChangeEvent) {
  const clonedList = cloneDeep(props.modelValue);
  if (e.moved) {
    clonedList.splice(e.moved.oldIndex, 1);
    clonedList.splice(e.moved.newIndex, 0, e.moved.element);
  } else if (e.added) {
    clonedList.splice(e.added.newIndex, 0, e.added.element);
  } else if (e.removed) {
    clonedList.splice(e.removed.oldIndex, 1);
  }

  emit("update:model-value", clonedList);
}
</script>

<template>
  <div class="flex flex-wrap items-center space-x-8">
    <VueDraggable
      :model-value="modelValue"
      :disabled="modelValue.length < 2"
      class="space-x-8 leading-none"
      item-key=""
      @change="onChange($event)"
    >
      <template #item="{ element, index }">
        <div
          :key="index"
          :style="{ backgroundImage: displayImage(element) }"
          class="w-128px h-128px bg-contain bg-no-repeat bg-center rounded-normal overflow-hidden inline-block relative group bg-gray outline-line outline outline-1"
        >
          <div
            class="text-fff absolute inset-0 bg-444/60 flex flex-col items-center justify-center transition-all opacity-0 group-hover:opacity-100 space-y-4"
          >
            <MediaUpload
              :folder="`/commerce/product`"
              :prefix="prefix"
              :multiple="false"
              @after-upload="changeImage($event, index)"
            >
              <ElButton round size="small" type="primary" class="w-80px">
                <el-icon class="iconfont icon-a-Cloudupload" />
                <span>{{ t("common.upload") }}</span>
              </ElButton>
            </MediaUpload>

            <ElButton
              round
              size="small"
              type="primary"
              class="!ml-0 w-80px"
              @click.stop="
                changeIndex = index;
                isChange = true;
                visibleMediaDialog = true;
              "
              >{{ t("common.select") }}</ElButton
            >
            <el-icon
              v-if="props.modelValue"
              class="iconfont icon-delete text-orange absolute top-8 right-8 cursor-pointer"
              @click.stop="onDelete(index)"
            />
          </div>
          <div
            v-if="element == main"
            class="bg-444/60 text-s h-24px text-fff absolute left-0 right-0 bottom-0 flex items-center justify-center"
          >
            {{ mainLabel || t("common.mainImage") }}
          </div>
          <div
            v-else
            class="bg-444/60 text-s h-24px text-fff absolute left-0 right-0 bottom-0 flex items-center justify-center opacity-0 group-hover:opacity-100 cursor-pointer"
            @click="$emit('update:main', element)"
          >
            {{ t("commerce.setAsMain") }}
          </div>
        </div>
      </template>
    </VueDraggable>

    <div
      class="w-128px h-128px rounded-normal overflow-hidden inline-block relative group bg-gray cursor-pointer border-dashed flex gap-12 justify-center items-center text-blue"
    >
      <el-icon class="iconfont icon-a-addto text-3l group-hover:opacity-50" />
      <div
        class="text-fff absolute inset-0 bg-444/60 flex flex-col items-center justify-center transition-all opacity-0 group-hover:opacity-100 space-y-4"
      >
        <MediaUpload
          :folder="`/commerce/product`"
          :prefix="prefix"
          :multiple="true"
          @after-upload="handleChooseFile"
        >
          <ElButton round size="small" type="primary" class="w-80px">
            <el-icon class="iconfont icon-a-Cloudupload" />
            <span>{{ t("common.upload") }}</span>
          </ElButton>
        </MediaUpload>

        <ElButton
          round
          size="small"
          type="primary"
          class="!ml-0 w-80px"
          @click.stop="
            isChange = false;
            visibleMediaDialog = true;
          "
        >
          <el-icon class="iconfont icon-folder" />
          <span>{{ t("common.select") }}</span>
        </ElButton>
      </div>
    </div>

    <KMediaDialog
      v-if="visibleMediaDialog"
      v-model="visibleMediaDialog"
      :multiple="!isChange"
      @choose="
        isChange ? changeImage($event, changeIndex) : handleChooseFile($event)
      "
    />
  </div>
</template>
