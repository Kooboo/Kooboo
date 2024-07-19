<template>
  <GroupPanel
    v-model="site!.enableTinymceToolbarSettings"
    :label="t('common.richTextEditorConfigurationForContentType')"
  >
    <el-form-item class="w-800px mb-0">
      <TinymceEditor
        v-if="site!.enableTinymceToolbarSettings"
        v-model="site!.tinymceSettings"
        @reset="onTinymceSettingReset"
      />
    </el-form-item>
  </GroupPanel>
</template>

<script lang="ts" setup>
import { site } from "./settings";
import { useI18n } from "vue-i18n";
import TinymceEditor from "@/components/tinymce-editor-config/index.vue";
import GroupPanel from "./group-panel.vue";

const { t } = useI18n();

function onTinymceSettingReset(key: string) {
  site.value!.tinymceSettings[key] = undefined;
}
</script>
