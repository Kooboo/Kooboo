<template>
  <div class="w-full">
    <el-collapse
      v-model="activeNames"
      class="richEditorConfig w-full bg-[#fafafa] dark:bg-[#333]"
    >
      <el-collapse-item name="toolbar">
        <template #title>
          <TitleItem
            :title="t('common.richEditorToolbarConfiguration')"
            @reset="emit('reset', 'toolbar')"
          />
        </template>
        <TinymceToolbarSettings v-model="modelValue.toolbar" />
      </el-collapse-item>
      <el-collapse-item name="font-size">
        <template #title>
          <TitleItem
            :title="t('common.richEditorFontSizeConfiguration')"
            @reset="emit('reset', 'font_size_formats')"
          />
        </template>
        <TinymceFontSizeSettings v-model="modelValue.font_size_formats" />
      </el-collapse-item>
      <el-collapse-item name="font-family">
        <template #title>
          <TitleItem
            :title="t('common.richEditorFontFamilyConfiguration')"
            @reset="emit('reset', 'font_formats')"
          />
        </template>
        <TinymceFontFamilySettings v-model="modelValue.font_formats" />
      </el-collapse-item>
    </el-collapse>
  </div>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import type { TinymceSettings } from "@/api/site/site";
import TinymceToolbarSettings from "./toolbar.vue";
import TinymceFontSizeSettings from "./font-size.vue";
import TinymceFontFamilySettings from "./font-family.vue";
import TitleItem from "./header.vue";

const { t } = useI18n();
interface PropsType {
  modelValue: TinymceSettings;
}

defineProps<PropsType>();
const activeNames = ref(["toolbar", "font-size", "font-family"]);

interface EmitType {
  (e: "update:modelValue", data: TinymceSettings): void;
  (e: "reset", value: string): void;
}

const emit = defineEmits<EmitType>();
</script>
<style>
.richEditorConfig .el-collapse-item .el-collapse-item__header {
  font-size: 14px;
  height: 36px;
  padding-left: 16px;
}

.richEditorConfig .el-collapse-item__content {
  padding-bottom: 0px;
}
</style>
<style scoped>
:deep(.el-collapse
    .el-collapse-item
    .el-collapse-item__header
    .el-collapse-item__arrow) {
  left: 0px;
}

:deep(.el-collapse .el-collapse-item .el-collapse-item__header.is-active) {
  box-shadow: none;
}

:deep(.el-checkbox__label) {
  height: 28px;
  line-height: 28px;
}

/* 系统设置页面取消边框样式 */
:deep(.systemSettingStyle .el-collapse-item__wrap) {
  border: 0;
}
</style>
