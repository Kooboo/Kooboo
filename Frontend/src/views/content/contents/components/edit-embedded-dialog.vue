<template>
  <el-dialog
    v-model="visible"
    width="800px"
    :close-on-click-modal="false"
    custom-class="el-dialog--fixed-footer editEmbeddedDialog"
    destroy-on-close
    @close="handleClose"
  >
    <template #header>
      <div class="flex items-center gap-8 max-w-700px truncate">
        <template v-for="(item, i) of titlePaths" :key="i">
          <div class="el-dialog__title truncate" :title="item">{{ item }}</div>
          <div v-if="i < titlePaths.length - 1" class="el-dialog__title">></div>
        </template>
      </div>
    </template>
    <EditContentDefault
      :id="id"
      ref="editContent"
      :folder-id="folderId"
      :paths="paths"
    />
    <template #footer>
      <DialogFooterBar @confirm="handleSave" @cancel="handleClose" />
    </template>
  </el-dialog>
</template>
<script setup lang="ts">
import {
  computed,
  nextTick,
  onBeforeUnmount,
  onMounted,
  ref,
  watch,
} from "vue";
import useOperationDialog from "@/hooks/use-operation-dialog";
import type { UPDATE_MODEL_EVENT } from "@/constants/constants";
import type {
  ContentEmbedded,
  TextContentItem,
} from "@/api/content/textContent";
// use import * for fixing ts build error
import * as EditContent from "./edit-content.vue";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

import { useI18n } from "vue-i18n"; // eslint-disable-next-line @typescript-eslint/no-explicit-any
const EditContentDefault = (EditContent as any).default;

interface PropsType {
  modelValue: boolean;
  current: ContentEmbedded;
  currentContent?: TextContentItem;
  paths?: string[];
}
interface EmitsType {
  (e: typeof UPDATE_MODEL_EVENT, value: boolean): void;
  (e: "save-success", value: TextContentItem): void;
}
const props = defineProps<PropsType>();
const emits = defineEmits<EmitsType>();
const { t } = useI18n();
const { visible, handleClose } = useOperationDialog(props, emits);
const folderId = computed(() => props.current.embeddedFolder.id);
const id = computed(() => props.currentContent?.id);
const editContent = ref<any>();

async function handleSave() {
  const content = await editContent.value?.save();
  if (content) {
    emits("save-success", {
      id: content.id,
      textValues: content.values[Object.keys(content.values)[0]],
    } as TextContentItem);
    handleClose();
  }
}

const titlePaths = computed(() => {
  let result = props.paths;
  if (!result) result = [];

  if (result.length && !result[result.length - 1]) {
    result.pop();
    result.push(t("common.createEmbedded"));
  }

  if (!result.length) {
    result.push(t("common.createEmbedded"));
  }

  return result;
});

//给tinymce编辑器的菜单弹框设置z-index样式
const index = ref();
watch(
  () => visible.value,
  () => {
    nextTick(() => {
      if (visible.value) {
        index.value = Array.from<HTMLElement>(
          document.querySelectorAll(".el-overlay")
        ).at(-1)?.style.zIndex;

        document.body.style.setProperty("--tox-tinymce-aux-index", index.value);
        document.body.classList.add("editEmbeddedDialog");
      } else {
        document.body.classList.remove("editEmbeddedDialog");
      }
    });
  }
);

function onIntercept(e: Event) {
  e.stopImmediatePropagation();
}

onMounted(() => {
  window.addEventListener("focusout", onIntercept, true);
});

onBeforeUnmount(() => {
  window.removeEventListener("focusout", onIntercept);
});
</script>

<style>
.editEmbeddedDialog .tox-tinymce-aux {
  z-index: var(--tox-tinymce-aux-index);
}
</style>
