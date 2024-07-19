<script lang="ts" setup>
import { onBeforeRouteLeave, useRouter } from "vue-router";
import { useRouteSiteId } from "@/hooks/use-site-id";
import KBottomBar from "@/components/k-bottom-bar/index.vue";
import { ref, watch, computed } from "vue";
import { useShortcut } from "@/hooks/use-shortcuts";
import { getQueryString } from "@/utils/url";
import { useSaveTip } from "@/hooks/use-save-tip";
import PageDesigner from "@/components/visual-editor/index.vue";
import PageSidebar from "./sidebar.vue";
import PageHeader from "../components/designer-header.vue";

import { useI18n } from "vue-i18n";
import { ElMessage } from "element-plus";
import { getDesignTemplate } from "@/api/pages/index";
import { useSiteStore } from "@/store/site";
import { useSetting } from "./use-setting";
import { useCustomWidgetEffects } from "@/components/visual-editor/custom-widgets";
import { emptyGuid } from "@/utils/guid";
import type { Meta } from "@/components/visual-editor/types";
import PropsEditor from "@/components/visual-editor/components/ve-props-edit/ve-props-editor.vue";
import { debounce } from "lodash-es";
import {
  usePageStyles,
  getRootStyles,
} from "@/components/visual-editor/page-styles";
import { usePageStore } from "@/store/page";
import type { KeyValue } from "@/global/types";

const { initCustomWidgets, customWidgets } = useCustomWidgetEffects();
const editor = ref();
const pageHeader = ref();
const siteStore = useSiteStore();
const pageStore = usePageStore();
const { t } = useI18n();
const router = useRouter();
const id = ref(getQueryString("id"));
const classic = ref(getQueryString("classic") === "true");
const oldUrlPath = ref();
const template = ref("");
const { init, model, setBody, save: doSave, setPageStyle } = useSetting();
const isReady = ref(false);

const pages = computed<KeyValue[]>(() => {
  return pageStore.list.map((it) => {
    return {
      key: it.name,
      value: it.path,
    };
  });
});

const pageStyles = ref<string>();

//判断是否为邮件编辑器
const list = computed(() => {
  return classic.value
    ? ["pageStyle"]
    : ["pageStyle", "basic", "HTMLMeta", "parameters", "cache"];
});

const { rootProps, linkFields, generalFields, initPageStyles } = usePageStyles(
  classic.value
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

const isEdit = computed(() => !!id.value && id.value !== emptyGuid);

const saveTip = useSaveTip();

const sidebarTabs = [
  {
    key: "settings",
    value: t("common.settings"),
  },
];

const initDesigner = async () => {
  await pageStore.load();
  await init(id.value!, { type: "Designer" });
  if (!model.value) {
    return;
  }
  rootMeta.value = JSON.parse(model.value.designConfig);
  const html = await getDesignTemplate();
  template.value = html;
  await initCustomWidgets(classic.value);
  await editor.value!.loadDesign();
  oldUrlPath.value = model.value?.urlPath;
};
initDesigner();

function onEditorReady(initMeta: Meta) {
  initPageStyles(initMeta);
  model.value!.designConfig = JSON.stringify(initMeta);
  pageStyles.value = getRootStyles(initMeta.props, classic.value);
  rootMeta.value = initMeta;
  saveTip.init([model.value]);
  isReady.value = true;
}

const save = async () => {
  await pageHeader.value?.validate();
  await editor.value?.validate();
  if (!model.value) return;
  const html: Record<string, string> = await editor.value.getHtml();
  setPageStyle(pageStyles.value);
  setBody(html[""], classic.value, rootProps.value);
  rootMeta.value.props = rootProps.value;
  model.value.designConfig = JSON.stringify(rootMeta.value);
  const page = await doSave();
  id.value = page?.id;
};
const onSave = async () => {
  if (!model.value) return;
  await save();
  ElMessage.success(t("common.saveSuccess"));
  saveTip.init([model.value]);
  oldUrlPath.value = model.value?.urlPath;
};

const onBack = () => {
  rootMeta.value.props = rootProps.value;
  model.value!.designConfig = JSON.stringify(rootMeta.value);
  router.goBackOrTo(useRouteSiteId({ name: "pages" }));
};

const saveAndReturn = async () => {
  await onSave();
  onBack();
};

onBeforeRouteLeave(async (to, from, next) => {
  if (to.name === "login") {
    next();
  } else {
    await saveTip
      .check([model.value])
      .then(() => {
        next();
      })
      .catch(() => {
        next(false);
      });
  }
});
watch(
  () => [model.value],
  () => {
    saveTip.changed([model.value]);
  },
  {
    deep: true,
  }
);

useShortcut("save", onSave);

watch(
  () => rootProps.value,
  debounce(function (m: Record<string, any>) {
    rootMeta.value.props = m;
    pageStyles.value = getRootStyles(m, classic.value);
  }, 300),
  {
    deep: true,
  }
);
</script>

<template>
  <PageDesigner
    ref="editor"
    v-model="rootMeta"
    :html="template"
    :base-url="siteStore.site.prUrl"
    :custom-tabs="sidebarTabs"
    :custom-widgets="customWidgets"
    :page-styles="pageStyles"
    :pages="pages"
    :classic="classic"
    @ready="onEditorReady"
  >
    <template #header>
      <PageHeader
        v-if="model"
        ref="pageHeader"
        :key="id"
        v-model="model"
        :is-edit="isEdit"
      />
    </template>
    <template #settings>
      <!-- :items="['pageStyle', 'basic', 'HTMLMeta', 'parameters', 'cache']" -->
      <!-- 移除Settings下的以下选项 -->
      <PageSidebar :old-url-path="oldUrlPath" :items="list">
        <template #pageStyle>
          <el-collapse-item
            v-if="rootMeta"
            name="pageStyle"
            :title="classic ? t('ve.globalStyles') : t('ve.pageStyle')"
          >
            <PropsEditor :model="rootProps" :fields="generalFields" />
            <hr />
            <PropsEditor :model="rootProps" :fields="linkFields" />
          </el-collapse-item>
        </template>
      </PageSidebar>
    </template>
  </PageDesigner>
  <KBottomBar
    back
    :permission="{ feature: 'pages', action: 'edit' }"
    :disabled="!isReady"
    :cancel-disabled="!isReady"
    @cancel="onBack"
    @save="onSave"
  >
    <template #extra-buttons>
      <el-button
        v-hasPermission="{ feature: 'pages', action: 'edit' }"
        round
        type="primary"
        :disabled="!isReady"
        data-cy="save-and-return"
        @click="saveAndReturn"
        >{{ t("common.saveAndReturn") }}</el-button
      >
    </template>
  </KBottomBar>
</template>
<style scoped>
:deep(.el-collapse-item__content) {
  padding-bottom: 0px;
}
</style>
