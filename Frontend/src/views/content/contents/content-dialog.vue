<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import EditContent from "@/views/content/contents/components/edit-content.vue";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

const { t } = useI18n();
const form = ref();
const show = ref(true);
defineProps<{ folder: string; id: string }>();
const emit = defineEmits<{
  (e: "reload", value: string): void;
  (e: "update:model-value", value: boolean): void;
}>();

const tinymceAuxNewContainer = ref<HTMLDialogElement>();

// fixed element dialog and tinymce dialog conflict
setTimeout(() => {
  const tinymceContainer = document.querySelectorAll(".tox-tinymce-aux");
  tinymceContainer.forEach((f) => {
    tinymceAuxNewContainer.value?.appendChild(f);
  });
}, 500);

const save = async () => {
  const { id } = await form.value.save();
  show.value = false;
  emit("reload", id);
};
</script>
<template>
  <div>
    <el-dialog
      v-model="show"
      width="800px"
      :close-on-click-modal="false"
      :title="t('common.textContent')"
      custom-class="el-dialog--zero-padding"
      @closed="emit('update:model-value', false)"
    >
      <div class="px-32 py-24">
        <EditContent :id="id!" ref="form" :folder-id="folder" />
      </div>
      <div ref="tinymceAuxNewContainer" />
      <template #footer>
        <DialogFooterBar @confirm="save" @cancel="show = false" />
      </template>
    </el-dialog>
  </div>
</template>
