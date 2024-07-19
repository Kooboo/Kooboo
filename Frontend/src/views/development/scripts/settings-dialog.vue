<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { useSiteStore } from "@/store/site";
import type { Site } from "@/api/site/site";
import { saveSite } from "@/api/site";

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
  await saveSite(site.value);
  siteStore.loadSite();
  siteStore.loadSites();
  show.value = false;
};
</script>

<template>
  <el-dialog
    :model-value="show"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.script')"
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
    </el-form>
    <template #footer>
      <DialogFooterBar @confirm="onSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
