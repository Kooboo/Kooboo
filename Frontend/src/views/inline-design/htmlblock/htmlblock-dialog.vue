<script lang="ts" setup>
import HtmlblockForm from "@/views/content/html-blocks/htmlblock-form.vue";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { close, htmlblockId } from ".";
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
    width="1000px"
    :close-on-click-modal="false"
    :title="t('common.htmlBlock')"
    custom-class="el-dialog--zero-padding"
    append-to-body
    @closed="close(success, form.model)"
  >
    <Alert :content="t('common.immediatelyChange')" />
    <div class="px-32 py-24">
      <HtmlblockForm :id="htmlblockId!" ref="form" />
    </div>
    <div ref="tinymceAuxNewContainer" />
    <template #footer>
      <DialogFooterBar @confirm="save" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
