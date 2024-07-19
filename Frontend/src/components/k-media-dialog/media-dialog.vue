<template>
  <el-dialog
    v-model="show"
    append-to-body
    width="850px"
    :close-on-click-modal="false"
    custom-class="el-dialog--fixed-footer k-media-dialog"
    :title="isFile ? t('common.file') : t('common.mediaLibrary')"
    @closed="$emit('update:modelValue', false)"
  >
    <template #header>
      <div
        class="flex items-center overflow-hidden"
        :class="{
          'w-500px': providers.length <= 1,
          'w-330px': providers.length > 1,
        }"
      >
        <div class="flex items-center mr-8 h-28px font-bold dark:text-[#fffa]">
          {{ isFile ? t("common.file") : t("common.mediaLibrary") }}
        </div>
        <div class="flex-1 mt-4 overflow-hidden">
          <Breadcrumb
            size="small"
            :hide-header="!!dialogInfo"
            :crumb-path="($refs['media'] as any)?.getCrumbPath || []"
            @handle-click-path="($refs['media'] as any)?.handleClickBreadcrumb"
          />
        </div>
      </div>
    </template>

    <File v-if="isFile" ref="media" class="media-dialog" @loaded="onLoaded" />
    <Media
      v-else
      ref="media"
      class="media-dialog"
      :grid-col="4"
      @loaded="onLoaded"
    />
    <template #footer>
      <div class="flex justify-between items-center">
        <el-button
          v-hasPermission="{
            feature: isFile ? 'file' : 'mediaLibrary',
            action: 'edit',
          }"
          type="primary"
          :disabled="!!keyword"
          @click="handleUpload"
        >
          {{ t("common.uploadFiles") }}
        </el-button>
        <div>
          <DialogFooterBar
            :disabled="selectedFiles?.length === 0"
            :confirm-label="t('common.choose')"
            @confirm="handleChoose"
            @cancel="show = false"
          />
        </div>
      </div>
    </template>
  </el-dialog>
</template>
<script setup lang="ts">
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import Media from "./media.vue";
import File from "@/components/k-file-dialog/file.vue";
import type { ComputedRef } from "vue";
import { computed, provide, ref, nextTick } from "vue";
import type { DialogInfo, MediaFileItem } from "./";
import { useI18n } from "vue-i18n";

interface PropsType {
  modelValue: boolean;
  multiple?: boolean;
  folderType?: "File";
  permission?: { feature: string; action: string };
  infinite?: boolean;
}

interface EmitsType {
  (e: "update:modelValue", value: boolean): void;
  (e: "choose", value: MediaFileItem[]): void;
}

const props = defineProps<PropsType>();
const emits = defineEmits<EmitsType>();
const { t } = useI18n();
const show = ref(true);
const isFile = computed(() => props.folderType === "File");
const media = ref();

const dialogInfo = computed(() => ({
  multiple: !!props.multiple,
}));

const keyword = ref("");
const providers = ref<string[]>([]);

provide<ComputedRef<DialogInfo>>("dialogInfo", dialogInfo);

const selectedFiles = computed(() => {
  if (media.value) return media.value.selectedFiles;
  return [] as MediaFileItem[];
});

function handleChoose() {
  emits("choose", selectedFiles.value as MediaFileItem[]);
  show.value = false;
}

function handleUpload() {
  const uploadBtn = document.querySelector(
    ".media-dialog .el-upload button"
  ) as HTMLElement;

  uploadBtn?.click();
}

function onLoaded(option: {
  keyword: string;
  providers?: string[];
  infinite?: boolean;
}) {
  keyword.value = option.keyword;
  providers.value = option.providers ?? [];
  if (!option.infinite) {
    nextTick(() => {
      const el = document.querySelector(".k-media-dialog .el-dialog__body");
      if (el) {
        el.scrollTop = 0;
      }
    });
  }
}
</script>
<style lang="scss">
.k-media-dialog {
  .el-dialog__headerbtn {
    top: 22px;
  }
}
</style>
<style lang="scss" scoped>
.media-dialog {
  :deep(.media__body) {
    padding: 0;
    .folder-item:nth-child(2n),
    .file-item:nth-child(4n) {
      margin-right: 0;
    }
    .media__breadcrumb {
      padding-left: 0;
    }
    .media__head {
      position: absolute;
      top: 12px;
      right: 80px;
      .el-upload {
        display: none;
      }
    }
  }
}
</style>
