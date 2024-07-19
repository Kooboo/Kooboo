<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { useSiteStore } from "@/store/site";
import type { Site } from "@/api/site/site";
import { saveSite } from "@/api/site";
import { ElMessage } from "element-plus";

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

defineProps<{ modelValue: boolean }>();

const siteStore = useSiteStore();
const site = ref<Site>(JSON.parse(JSON.stringify(siteStore.site)));

const { t } = useI18n();
const show = ref(true);
const form = ref();

const onSave = async () => {
  if (site.value.unocssSettings.enable) {
    try {
      // 解析 JSON 字符串，如果没有抛出错误，表示 JSON 格式正确
      JSON.parse(site.value.unocssSettings.config);
      saveAction();
    } catch (error) {
      // 如果抛出了错误，表示 JSON 格式不正确
      ElMessage.error(t("common.codeFormatIsIncorrect"));
      return;
    }
  } else {
    saveAction();
  }
};

const saveAction = async () => {
  await saveSite(site.value);
  siteStore.loadSite();
  siteStore.loadSites();
  show.value = false;
};
</script>

<template>
  <el-dialog
    :model-value="show"
    width="800px"
    :close-on-click-modal="false"
    :title="t('common.style')"
    @closed="emit('update:modelValue', false)"
  >
    <el-form ref="form" label-position="top" @keydown.enter="onSave">
      <el-form-item>
        <span class="font-bold dark:text-fff/86">{{
          t("common.JsCssCompression")
        }}</span>
        <div class="flex-1" />
        <el-switch
          v-model="site.enableJsCssCompress"
          data-cy="js-css-compress"
        />
      </el-form-item>
      <el-form-item>
        <span class="font-bold dark:text-fff/86">{{
          t("common.JsCssBrowserCache")
        }}</span>
        <div class="flex-1" />
        <el-switch
          v-model="site.enableJsCssBrowerCache"
          data-cy="js-css-cache"
        />
      </el-form-item>
      <el-form-item>
        <div class="flex items-center w-full">
          <span class="font-bold text-666 flex-1">{{
            t("settings.atomicCss")
          }}</span>
          <el-switch v-model="site.unocssSettings.enable" />
        </div>
      </el-form-item>
      <div
        v-if="site.unocssSettings.enable"
        class="border-l-2 border-blue/50 pl-8"
      >
        <el-form-item>
          <div class="flex items-center w-full">
            <span class="text-666 flex-1">{{ t("settings.disableSsr") }}</span>
            <el-switch v-model="site.unocssSettings.disableSsr" />
          </div>
        </el-form-item>
        <el-form-item>
          <div class="flex items-center w-full">
            <span class="text-666 flex-1">{{
              t("settings.resetDefaultStyle")
            }}</span>
            <el-switch v-model="site.unocssSettings.resetStyle" />
          </div>
        </el-form-item>
        <el-form-item>
          <div class="h-300px w-full" @keydown.stop="">
            <MonacoEditor
              v-model="site.unocssSettings.config"
              language="json"
            />
          </div>
        </el-form-item>
      </div>
      <el-form-item>
        <div class="flex items-center w-full">
          <span class="font-bold text-666 flex-1">{{
            t("common.enableCssSplitByMedia")
          }}</span>
          <el-switch v-model="site.enableCssSplitByMedia" />
        </div>
      </el-form-item>
      <div
        v-if="site.enableCssSplitByMedia"
        class="border-l-2 border-blue/50 pl-8"
      >
        <el-form-item class="w-800px">
          <div>
            <p class="text-666">{{ t("common.maxWidthForMobileBrowser") }}</p>
            <p class="text-s">
              ({{ t("common.maxWidthForMobileBrowserDetail") }})
            </p>
          </div>

          <el-input
            v-model="site.mobileMaxWidth"
            class="w-500px"
            data-cy="max-width"
          />
        </el-form-item>
        <el-form-item class="w-900px">
          <div>
            <p class="text-666">{{ t("common.minWidthForDesktopBrowser") }}</p>
            <p class="text-s">
              ({{ t("common.minWidthForDesktopBrowserDetail") }})
            </p>
          </div>
          <el-input
            v-model="site.desktopMinWidth"
            class="w-500px"
            data-cy="min-width"
          />
        </el-form-item>
      </div>
    </el-form>
    <template #footer>
      <DialogFooterBar @confirm="onSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
