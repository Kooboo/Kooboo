<script lang="ts" setup>
import Basic from "@/components/html-setting/basic.vue";
import Styles from "@/components/html-setting/styles.vue";
import type { PostPage } from "@/api/pages/types";
import Scripts from "@/components/html-setting/scripts.vue";
import Cache from "@/components/html-setting/cache.vue";
import HtmlMeta from "@/components/html-setting/html-meta.vue";
import { useSettings } from "./use-settings";
import { useI18n } from "vue-i18n";
import { onBeforeMount, ref } from "vue";
import type { Source } from "@/api/component/types";
import { useSiteStore } from "@/store/site";

const props = defineProps<{
  model: PostPage;
  oldUrlPath?: string;
  layout: string;
  sources?: {
    id: string;
    tag: string;
    source: Source;
  }[];
  hideStyles?: boolean;
  hideScripts?: boolean;
}>();
const { t } = useI18n();

const {
  metas,
  addOrUpdateMeta,
  deleteScript,
  deleteStyle,
  insertScript,
  insertStyle,
  scripts,
  sortScript,
  sortStyle,
  styles,
} = useSettings(props.model);

const basicForm = ref();
const metaBindings = ref<string[]>([]);
const urlParamsBindings = ref<string[]>([]);
const siteStore = useSiteStore();

defineExpose({ validate: () => basicForm.value?.validate() });
onBeforeMount(() => {
  props.sources?.forEach((f) => {
    if (!f.source.metaBindings || !f.source.urlParamsBindings) return;
    if (f.source.metaBindings.length) {
      metaBindings.value = metaBindings.value.concat(f.source?.metaBindings);
    }
    if (f.source.urlParamsBindings.length) {
      urlParamsBindings.value = urlParamsBindings.value.concat(
        f.source?.urlParamsBindings
      );
    }
  });
});
</script>

<template>
  <el-collapse :model-value="'basic'">
    <el-collapse-item name="basic" :title="t('common.basic')">
      <Basic
        ref="basicForm"
        v-model:url="model.urlPath"
        v-model:titles="model.contentTitle"
        v-model:published="model.published"
        :old-url-path="oldUrlPath"
        :meta-bindings="metaBindings"
        :url-params-bindings="urlParamsBindings"
      />
    </el-collapse-item>
    <el-collapse-item :title="t('common.HTMLMeta')" data-cy="meta">
      <HtmlMeta
        :list="metas"
        :meta-bindings="metaBindings"
        @delete="model.metas = metas.filter((f) => f !== $event)"
        @add-or-update="addOrUpdateMeta"
      />
    </el-collapse-item>
    <!-- <el-collapse-item title="Parameters">
      <Parameters v-model="model.parameters" />
    </el-collapse-item> -->
    <el-collapse-item
      v-if="!hideStyles"
      :title="t('common.style')"
      data-cy="style"
    >
      <Styles
        :list="styles"
        @sort="sortStyle"
        @insert="insertStyle"
        @delete="deleteStyle"
      />
      <el-form
        v-if="siteStore?.site?.unocssSettings?.enable"
        label-position="top"
        class="px-24"
      >
        <el-form-item :label="t('common.disableUnocss')">
          <ElSwitch v-model="model.disableUnocss" />
        </el-form-item>
      </el-form>
    </el-collapse-item>
    <el-collapse-item
      v-if="!hideScripts"
      :title="t('common.scripts')"
      data-cy="script"
    >
      <Scripts
        :list="scripts"
        hidde-body
        @sort="sortScript"
        @delete="deleteScript"
        @insert="insertScript"
      />
    </el-collapse-item>
    <el-collapse-item :title="t('common.cache')" data-cy="cache">
      <Cache
        v-model:enable="model.enableCache"
        v-model:enableVersion="model.cacheByVersion"
        v-model:minutes="model.cacheMinutes"
        v-model:queryKeys="model.cacheQueryKeys"
      />
    </el-collapse-item>
  </el-collapse>
</template>
<style scoped>
:deep(.el-collapse-item__content) {
  padding-bottom: 0px;
}
</style>
