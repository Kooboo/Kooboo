<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import EditContent from "@/views/content/contents/components/edit-content.vue";
import { emptyGuid } from "@/utils/guid";
import { close, contentId } from ".";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

const { t } = useI18n();
const form = ref();
const success = ref(false);
const show = ref(true);

const tinymceAuxNewContainer = ref<HTMLDialogElement>();

// fixed element dialog and tinymce dialog conflict
setTimeout(() => {
  const tinymceContainer = document.querySelectorAll(".tox-tinymce-aux");
  tinymceContainer.forEach((f) => {
    tinymceAuxNewContainer.value?.appendChild(f);
  });
}, 500);

const save = async () => {
  await form.value.save();
  success.value = true;
  show.value = false;
};
</script>
<template>
  <el-dialog
    v-model="show"
    width="800px"
    :close-on-click-modal="false"
    :title="t('common.textContent')"
    custom-class="el-dialog--zero-padding"
    @closed="close(success)"
  >
    <Alert :content="t('common.immediatelyChange')" />
    <div class="px-32 py-24">
      <EditContent :id="contentId!" ref="form" :folder-id="emptyGuid" />
    </div>
    <div ref="tinymceAuxNewContainer" />
    <template #footer>
      <DialogFooterBar @confirm="save" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
