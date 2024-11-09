<script lang="ts" setup>
import { useSiteStore } from "@/store/site";
import { combineUrl } from "@/utils/url";
import { computed, ref } from "vue";
import defaultImage from "@/assets/images/commerce_default_image.svg";
import type { MediaFileItem } from "@/components/k-media-dialog";
import KMediaDialog from "@/components/k-media-dialog";
import { useI18n } from "vue-i18n";
import MediaUpload from "../k-media-dialog/media-upload.vue";
import { ElButton } from "element-plus";

const props = defineProps<{
  modelValue?: string;
  editable?: boolean;
  size?: "mini" | "small" | "large";
  description?: string;
  folder?: string;
  prefix?: string;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: string): void;
  (e: "delete"): void;
}>();

const siteStore = useSiteStore();
const visibleMediaDialog = ref(false);
const { t } = useI18n();

const imgUrl = computed(() => {
  const url = props.modelValue
    ? combineUrl(siteStore.site.prUrl, props.modelValue)
    : defaultImage;
  return `url("${url}")`;
});

function handleChooseFile(files: MediaFileItem[]) {
  emit("update:model-value", files[0].url);
}

function onDelete() {
  emit("update:model-value", "");
  emit("delete");
}
</script>

<template>
  <div class="flex items-center justify-center">
    <el-popover
      placement="top"
      trigger="click"
      :disabled="!editable || size == 'large'"
    >
      <template #reference>
        <div
          :style="{ backgroundImage: imgUrl }"
          class="w-48px h-48px bg-contain bg-center bg-no-repeat rounded-normal overflow-hidden inline-block relative group bg-gray"
          :class="{
            'cursor-pointer': editable,
            'w-128px': size == 'large',
            'h-128px': size == 'large',
            'w-24px': size == 'mini',
            'h-24px': size == 'mini',
            '!rounded-4px': size == 'mini',
          }"
          @click.stop=""
        >
          <div
            v-if="editable && size == 'large'"
            class="text-fff absolute inset-0 bg-444/60 flex flex-col items-center justify-center transition-all opacity-0 group-hover:opacity-100 space-y-4"
          >
            <MediaUpload
              :folder="folder || '/'"
              :prefix="prefix"
              :multiple="false"
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
              @click.stop="visibleMediaDialog = true"
            >
              <el-icon class="iconfont icon-folder" />
              <span>{{ t("common.select") }}</span>
            </ElButton>
            <el-icon
              v-if="props.modelValue"
              class="iconfont icon-delete text-orange absolute top-8 right-8"
              :class="{ 'text-l': size == 'large' }"
              @click.stop="onDelete"
            />
          </div>
          <div
            v-if="description"
            class="bg-444/60 text-s h-16px text-fff absolute left-0 right-0 bottom-0 flex items-center justify-center"
            :class="{
              'h-24px': size == 'large',
            }"
          >
            {{ description }}
          </div>
        </div>
      </template>

      <div class="flex flex-col gap-4 items-center">
        <MediaUpload
          :folder="folder || '/'"
          :prefix="prefix"
          :multiple="false"
          @after-upload="handleChooseFile"
        >
          <ElButton round size="small" type="primary" class="w-100px">
            <el-icon class="iconfont icon-a-Cloudupload" />
            <span>{{ t("common.upload") }}</span>
          </ElButton>
        </MediaUpload>

        <ElButton
          round
          size="small"
          type="primary"
          class="!ml-0 w-100px"
          @click.stop="visibleMediaDialog = true"
        >
          <el-icon class="iconfont icon-folder" />
          <span>{{ t("common.select") }}</span>
        </ElButton>
        <ElButton
          v-if="props.modelValue"
          round
          size="small"
          type="danger"
          class="!ml-0 w-100px"
          @click.stop="onDelete"
        >
          <el-icon class="iconfont icon-delete" />
          <span>{{ t("common.delete") }}</span>
        </ElButton>
      </div>
    </el-popover>

    <KMediaDialog
      v-if="visibleMediaDialog"
      v-model="visibleMediaDialog"
      @choose="handleChooseFile"
    />
  </div>
</template>
