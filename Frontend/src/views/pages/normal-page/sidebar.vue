<script lang="ts" setup>
import HtmlMeta from "@/components/html-setting/html-meta.vue";
import Parameters from "@/components/html-setting/parameters.vue";
import Styles from "@/components/html-setting/styles.vue";
import Scripts from "@/components/html-setting/scripts.vue";
import Cache from "@/components/html-setting/cache.vue";
import Basic from "@/components/html-setting/basic.vue";
import { useI18n } from "vue-i18n";
import { useSetting } from "./use-setting";
import { withDefaults, ref } from "vue";
import { useSiteStore } from "@/store/site";

interface Props {
  oldUrlPath: string;
  items?: SidebarItems[];
}

export type SidebarItems =
  | "basic"
  | "HTMLMeta"
  | "parameters"
  | "style"
  | "scripts"
  | "cache"
  | string;

const props = withDefaults(defineProps<Props>(), {
  items() {
    return ["basic", "HTMLMeta", "parameters", "style", "scripts", "cache"];
  },
});

const {
  model,
  updateTitle,
  deleteElement,
  styles,
  sortStyle,
  insertStyle,
  scripts,
  sortScript,
  insertScript,
  metas,
  addOrUpdateMeta,
  deleteMeta,
} = useSetting();

const { t } = useI18n();
const activeTab = ref("basic");
const siteStore = useSiteStore();

(function init() {
  const tabs = props.items ?? [];
  activeTab.value = tabs[0] ?? "basic";
})();
</script>

<template>
  <el-collapse v-if="model" v-model="activeTab">
    <template v-for="item in items" :key="item">
      <el-collapse-item
        v-if="item === 'basic'"
        name="basic"
        :title="t('common.basic')"
      >
        <Basic
          ref="basicForm"
          v-model:url="model.urlPath"
          v-model:titles="model.contentTitle"
          v-model:published="model.published"
          :old-url-path="oldUrlPath"
          @update:default-title="updateTitle"
        />
      </el-collapse-item>
      <el-collapse-item
        v-else-if="item === 'HTMLMeta'"
        name="HTMLMeta"
        :title="t('common.HTMLMeta')"
      >
        <HtmlMeta
          :list="metas"
          @delete="deleteMeta"
          @add-or-update="addOrUpdateMeta"
        />
      </el-collapse-item>
      <el-collapse-item
        v-else-if="item === 'parameters'"
        name="parameters"
        :title="t('common.parameters')"
      >
        <Parameters v-model="model.parameters" />
      </el-collapse-item>
      <el-collapse-item
        v-else-if="item === 'style'"
        name="style"
        :title="t('common.style')"
      >
        <Styles
          :list="styles"
          @sort="sortStyle"
          @insert="insertStyle"
          @delete="deleteElement($event.el!)"
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
        v-else-if="item === 'scripts'"
        name="scripts"
        :title="t('common.scripts')"
      >
        <Scripts
          :list="scripts"
          @sort="sortScript"
          @delete="deleteElement($event.el!)"
          @insert="insertScript"
        />
      </el-collapse-item>
      <el-collapse-item
        v-else-if="item === 'cache'"
        name="cache"
        :title="t('common.cache')"
      >
        <Cache
          v-model:enable="model.enableCache"
          v-model:enableVersion="model.cacheByVersion"
          v-model:minutes="model.cacheMinutes"
          v-model:queryKeys="model.cacheQueryKeys"
        />
      </el-collapse-item>
      <slot v-else :name="item" />
    </template>
  </el-collapse>
</template>
