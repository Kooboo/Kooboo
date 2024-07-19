<script lang="ts" setup>
import { onBeforeRouteLeave, useRouter, useRoute } from "vue-router";
import { useRouteSiteId } from "@/hooks/use-site-id";
import KBottomBar from "@/components/k-bottom-bar/index.vue";
import { ref, watch, computed } from "vue";
import { useShortcut } from "@/hooks/use-shortcuts";
import { getQueryString } from "@/utils/url";
import type { Placeholder } from "@/global/types";
import { addonToXml, layoutToAddon, elementToAddon } from "@/utils/page";
import { emptyGuid } from "@/utils/guid";
import { useSaveTip } from "@/hooks/use-save-tip";
import { useStyleStore } from "@/store/style";
import { useScriptStore } from "@/store/script";
import { useI18n } from "vue-i18n";
import type { Layout } from "@/api/layout/types";
import { getLayout } from "@/api/layout";
import type { PostPage } from "@/api/pages/types";
import { usePageStore } from "@/store/page";

import { ElMessage } from "element-plus";
import PageDesigner from "@/components/visual-editor/index.vue";
import PageSidebar from "./components/settings.vue";
import PageHeader from "../components/designer-header.vue";

import { useSiteStore } from "@/store/site";
import { useCustomWidgetEffects } from "@/components/visual-editor/custom-widgets";
import type { Meta } from "@/components/visual-editor/types";
import type { KeyValue } from "@/global/types";

const { initCustomWidgets, customWidgets } = useCustomWidgetEffects();

const { t } = useI18n();
const siteStore = useSiteStore();
const layout = ref<Layout>();
const html = computed(() => layout.value?.body ?? "");
const id = getQueryString("id");
const classic = ref(getQueryString("classic") === "true");
const isReady = ref(false);

const pages = computed<KeyValue[]>(() => {
  return pageStore.list.map((it) => {
    return {
      key: it.name,
      value: it.path,
    };
  });
});

const sidebarTabs = [
  {
    key: "settings",
    value: t("common.settings"),
  },
];
const model = ref<PostPage>();
const pageStore = usePageStore();
const router = useRouter();
const route = useRoute();
const editor = ref();
const saveTip = useSaveTip((key, value) => {
  if (key === "body" && value) {
    return undefined;
  }
  return value;
});
const settingsForm = ref();
const oldUrlPath = ref("");
const isEdit = computed(
  () => !!model.value?.id && model.value.id !== emptyGuid
);
const rootMeta = ref<Meta>({
  children: [],
  htmlStr: "",
  id: emptyGuid,
  name: "page",
  props: {},
  propDefines: [],
  type: "page",
});

const designToXml = async () => {
  if (!layout.value) {
    return "";
  }
  const sectionHtmls: Record<string, string> = await editor.value.getHtml();
  const layoutAddon = layoutToAddon(layout.value.name, layout.value.body);
  for (const section of layoutAddon.content as Placeholder[]) {
    const sectionHtml = sectionHtmls[section.name];
    if (!sectionHtml) {
      continue;
    }

    const sectionEl = document.createElement("div");
    sectionEl.innerHTML = `<div class="ve-global">${sectionHtml}</div>`;
    for (let index = 0; index < sectionEl.children.length; index++) {
      const child = sectionEl.children[index];
      const addon = elementToAddon(child);
      section.addons.push(addon);
    }
  }

  return addonToXml(layoutAddon);
};

const onBack = async () => {
  if (model.value) {
    model.value.body = await designToXml();
    model.value.designConfig = JSON.stringify(rootMeta.value);
  }

  router.goBackOrTo(useRouteSiteId({ name: "pages" }));
};

onBeforeRouteLeave(async (to, from, next) => {
  if (to.name === "login") {
    next();
  } else {
    await saveTip
      .check(model.value)
      .then(() => {
        next();
      })
      .catch(() => {
        next(false);
      });
  }
});

const onSave = async () => {
  if (!model.value) return;
  model.value.body = await designToXml();
  if (layout.value) {
    model.value.layoutId = layout.value.id;
    model.value.layoutName = layout.value.name;
  }
  model.value.type = "Designer";
  model.value.designConfig = JSON.stringify(rootMeta.value);

  await Promise.all([
    editor.value.validate(),
    settingsForm.value?.validate(),
  ]).then(async () => {
    const isNewPage = model.value!.id === emptyGuid;

    await pageStore.updatePage(model.value!);
    ElMessage.success(t("common.saveSuccess"));
    oldUrlPath.value = model.value!.urlPath;
    saveTip.init(model.value);
    if (isNewPage) {
      router.replace({
        query: {
          ...route.query,
          id: model.value!.id,
          layoutId: getQueryString("layoutId")!,
        },
      });
    }
  });
};
const saveAndReturn = async () => {
  await onSave();
  onBack();
};

const init = async () => {
  model.value = await pageStore.getPage(id!, {
    type: "Designer",
  });
  rootMeta.value = JSON.parse(model.value.designConfig);
  oldUrlPath.value = model.value.urlPath ?? "";
  layout.value = await getLayout(getQueryString("layoutId")!, {
    design: "true",
    env: import.meta.env.MODE,
  });
  await initCustomWidgets();
  await editor.value!.loadDesign();
};
async function onEditorReady(initMeta: Meta) {
  model.value!.designConfig = JSON.stringify(initMeta);
  saveTip.init(model.value);
  isReady.value = true;
}

init();

useStyleStore().loadAll();
useScriptStore().loadAll();

useShortcut("save", onSave);

watch(
  () => model.value,
  () => {
    saveTip.changed(model.value);
  },
  {
    deep: true,
  }
);
</script>

<template>
  <PageDesigner
    v-if="model"
    ref="editor"
    v-model="rootMeta"
    :html="html"
    :base-url="siteStore.site.prUrl"
    :custom-tabs="sidebarTabs"
    :custom-widgets="customWidgets"
    :classic="classic"
    :pages="pages"
    @ready="onEditorReady"
  >
    <template #header>
      <PageHeader ref="settingsForm" v-model="model" :is-edit="isEdit" />
    </template>
    <template #settings>
      <PageSidebar
        :model="model"
        :layout="layout!.body"
        :old-url-path="oldUrlPath"
        :hide-scripts="true"
        :hide-styles="true"
      />
    </template>
  </PageDesigner>
  <KBottomBar
    back
    :permission="{
      feature: 'pages',
      action: 'edit',
    }"
    :disabled="!isReady"
    :cancel-disabled="!isReady"
    @cancel="onBack"
    @save="onSave"
  >
    <template #extra-buttons>
      <el-button
        v-hasPermission="{
          feature: 'pages',
          action: 'edit',
        }"
        :disabled="!isReady"
        round
        type="primary"
        data-cy="save-and-return"
        @click="saveAndReturn"
      >
        {{ t("common.saveAndReturn") }}
      </el-button>
    </template>
  </KBottomBar>
</template>
