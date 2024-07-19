<script lang="ts" setup>
import { site } from "./settings";
import GroupPanel from "./group-panel.vue";
import { useI18n } from "vue-i18n";
import { ref } from "vue";
import MonacoEditor from "@/components/monaco-editor/index.vue";
import { Uri } from "monaco-editor";

const { t } = useI18n();
const showCodeDialog = ref(false);
const code = ref("");

function onSaveCode() {
  if (!site.value) return;
  site.value.sitemapSettings.code = code.value;
  showCodeDialog.value = false;
}
function onEditCode() {
  if (!site.value) return;
  showCodeDialog.value = true;
  code.value = site.value.sitemapSettings.code;
}
</script>

<template>
  <template v-if="site">
    <GroupPanel
      v-model="site.sitemapSettings.enable"
      :label="t('common.sitemap')"
      :tooltip-label="t('common.sitemapTips')"
    >
      <el-form-item :label="t('common.path')">
        <el-input v-model="site.sitemapSettings.path" />
      </el-form-item>
      <el-form-item :label="t('common.generateType')">
        <div class="flex items-center justify-between w-full">
          <ElRadioGroup v-model="site.sitemapSettings.autoGenerate">
            <ElRadio :label="true">{{ t("common.auto") }}</ElRadio>
            <ElRadio :label="false">{{ t("common.manual") }}</ElRadio>
          </ElRadioGroup>
          <ElButton
            v-if="!site.sitemapSettings.autoGenerate"
            type="primary"
            size="small"
            @click="onEditCode"
            >{{ t("common.editCode") }}</ElButton
          >
        </div>
      </el-form-item>
      <Teleport to="body">
        <el-dialog
          :model-value="showCodeDialog"
          custom-class="el-dialog--zero-padding"
          width="800px"
          :close-on-click-modal="false"
          :title="t('common.editCode')"
          @closed="showCodeDialog = false"
        >
          <div class="h-500px" @keydown.stop="">
            <MonacoEditor
              v-model="code"
              language="typescript"
              k-script
              :uri="Uri.file(site.id + 'sitemap')"
            />
          </div>
          <template #footer>
            <DialogFooterBar
              @confirm="onSaveCode"
              @cancel="showCodeDialog = false"
            />
          </template>
        </el-dialog>
      </Teleport>
    </GroupPanel>
  </template>
</template>
