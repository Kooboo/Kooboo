<script lang="ts" setup>
import { useI18n } from "vue-i18n";
import { computed } from "vue";
import { show, close, widget } from ".";
import PropsEditor from "./props-editor.vue";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { findWidget } from "@/components/visual-editor/utils/widget";
import { preview } from "@/components/visual-editor/render";
import { usePageDesigner } from "@/components/visual-editor/main";
import { isClassic } from "@/components/visual-editor/utils";

const { page, rootMeta, onSave: savePage } = usePageDesigner();

const { t } = useI18n();

const baseUrl = computed(() => {
  const url = page.value?.previewUrl;
  if (!url) {
    return "/";
  }
  return new URL(url).origin;
});

const onSave = async () => {
  const item = widget.value;
  if (!item || !page.value) {
    return;
  }
  const [toEdit] = findWidget(rootMeta.value!, (it) => it.id === item.id);
  if (!toEdit) {
    console.log(["error"]);
    return;
  }
  toEdit.props = item.props;
  toEdit.htmlStr = await preview(toEdit, {
    baseUrl: baseUrl.value,
    classic: isClassic(),
    rootMeta: rootMeta.value,
  });
  widget.value = toEdit;

  await savePage();
  close(true);
};
</script>
<template>
  <el-dialog
    v-model="show"
    width="800px"
    :close-on-click-modal="false"
    :title="t('ve.editWidget', { name: widget?.name })"
    custom-class="el-dialog--zero-padding"
    @closed="close(false)"
  >
    <Alert :content="t('common.immediatelyChange')" />
    <div class="mt-8">
      <PropsEditor v-if="widget" v-model="widget" />
    </div>
    <template #footer>
      <DialogFooterBar @confirm="onSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
