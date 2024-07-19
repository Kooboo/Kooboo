<script lang="ts" setup>
import { useI18n } from "vue-i18n";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { ref } from "vue";
import type { Meta } from "@/components/visual-editor/types";
import { usePageStyles } from "@/components/visual-editor/page-styles";
import PropsEditor from "@/components/visual-editor/components/ve-props-edit/ve-props-editor.vue";
import type { PostPage } from "@/api/pages/types";
import { cloneDeep } from "lodash-es";

const currentTab = ref<string>("general");
const currentPage = ref<PostPage>();

const { rootProps, linkFields, generalFields, initPageStyles } =
  usePageStyles();
const rootMeta = ref<Meta>();
const showStylesDialog = ref(false);
const { t } = useI18n();

function closeStylesDialog() {
  showStylesDialog.value = false;
}

function init(page: PostPage) {
  const meta = JSON.parse(page.designConfig);
  initPageStyles(meta);
  currentPage.value = page;
  rootMeta.value = meta;
  showStylesDialog.value = true;
}

const onSave = async () => {
  const newProps = cloneDeep(rootProps.value);
  rootMeta.value!.props = newProps;
  console.log(["newProps", newProps]);
  currentPage.value!.designConfig = JSON.stringify(rootMeta.value);
  emit("save", currentPage.value!);
  closeStylesDialog();
  location.reload();
};
const emit = defineEmits<{
  (e: "save", value: PostPage): void;
}>();
defineExpose({
  init,
});
</script>
<template>
  <el-dialog
    v-if="showStylesDialog"
    v-model="showStylesDialog"
    width="800px"
    :close-on-click-modal="false"
    :title="t('common.styles')"
    custom-class="el-dialog--zero-padding"
    @closed="closeStylesDialog"
  >
    <Alert :content="t('common.immediatelyChange')" />
    <div class="mt-8">
      <el-tabs v-model="currentTab">
        <el-tab-pane
          v-if="generalFields.length"
          :label="t('common.general')"
          name="general"
        >
          <PropsEditor
            class="props-editor-form"
            :model="rootProps"
            :fields="generalFields"
          />
        </el-tab-pane>
        <el-tab-pane
          v-if="linkFields.length"
          :label="t('common.link')"
          name="link"
        >
          <PropsEditor
            class="props-editor-form"
            :model="rootProps"
            :fields="linkFields"
          />
        </el-tab-pane>
      </el-tabs>
    </div>
    <template #footer>
      <DialogFooterBar @confirm="onSave" @cancel="closeStylesDialog" />
    </template>
  </el-dialog>
</template>

<style lang="scss">
.props-editor-form .el-form {
  display: flex;
  flex-wrap: wrap;
  justify-content: flex-start;
  align-items: flex-start;
  .el-form-item {
    box-sizing: border-box;
    padding-right: 20px;
    width: 50%;
    &.w-full {
      width: 100%;
    }
    &.ve-rich-editor {
      width: 100%;
    }
    .el-form-item__content {
      > * {
        width: 100%;
      }
    }
  }
}
</style>
