<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { close, dataId, dbType, dataTable } from ".";
import EditDataForm from "@/views/database/table/edit-data-form.vue";

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
    :title="t('common.database')"
    custom-class="el-dialog--zero-padding"
    @closed="close(success)"
  >
    <Alert :content="t('common.immediatelyChange')" />
    <div class="px-32 py-24">
      <EditDataForm
        :id="dataId!"
        ref="form"
        :db-type="dbType!"
        :table="dataTable!"
      />
    </div>
    <div ref="tinymceAuxNewContainer" />
    <template #footer>
      <el-button round @click="show = false">{{
        t("common.cancel")
      }}</el-button>
      <el-button type="primary" round @click="save">{{
        t("common.save")
      }}</el-button>
    </template>
  </el-dialog>
</template>
