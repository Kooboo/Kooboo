<script lang="ts" setup>
import { onBeforeRouteLeave, useRouter } from "vue-router";
import { useRouteSiteId } from "@/hooks/use-site-id";
import KBottomBar from "@/components/k-bottom-bar/index.vue";
import { ref, computed, watch } from "vue";
import { useShortcut } from "@/hooks/use-shortcuts";
import { getQueryString } from "@/utils/url";
import { options } from "../components/types";
import NavBar from "../components/nav-bar.vue";
import Settings from "./components/settings.vue";
import Design from "./components/design.vue";
import type { Addon } from "@/global/types";
import { mergeAddon } from "./layout-model";
import { layoutToAddon, addonToXml, pageToAddon } from "@/utils/page";
import Preview from "./components/preview.vue";
import { emptyGuid } from "@/utils/guid";
import { useSaveTip } from "@/hooks/use-save-tip";
import { isUniqueNameRule, rangeRule, requiredRule } from "@/utils/validate";
import { useStyleStore } from "@/store/style";
import { useScriptStore } from "@/store/script";
import { useI18n } from "vue-i18n";
import type { Rules } from "async-validator";
import type { Layout } from "@/api/layout/types";
import { getLayout } from "@/api/layout";
import type { PostPage } from "@/api/pages/types";
import { usePageStore } from "@/store/page";
import type { AddonSource } from "./source";
import type { Source } from "@/api/component/types";
import { isUniqueName } from "@/api/pages";
import { ElMessage } from "element-plus";

const { t } = useI18n();
const layout = ref<Layout>();
const designModel = ref<Addon>();
const model = ref<PostPage>();
const pageStore = usePageStore();
const sources = ref<AddonSource[]>([]);
const router = useRouter();
const currentTab = ref(options.design);
const form = ref();
const saveTip = useSaveTip();
const settingsForm = ref();
const oldUrlPath = ref();

const rules = computed(
  () =>
    ({
      name:
        model.value?.id && model.value.id !== emptyGuid
          ? []
          : [
              rangeRule(1, 50),
              requiredRule(t("common.nameRequiredTips")),
              isUniqueNameRule(
                isUniqueName,
                t("common.thePageNameAlreadyExists")
              ),
            ],
    } as Rules)
);

(async () => {
  layout.value = await getLayout(getQueryString("layoutId")!);
  model.value = await pageStore.getPage(getQueryString("id")!);
  const layoutAddon = layoutToAddon(layout.value.name, layout.value.body);
  const pageAddon = pageToAddon(model.value.body);
  oldUrlPath.value = model.value.urlPath;
  sources.value.push({
    id: layout.value.name,
    tag: "layout",
    source: { body: layout.value.body } as Source,
  });
  await mergeAddon(layoutAddon, pageAddon, sources.value);
  designModel.value = layoutAddon;
  saveTip.init(model.value);
})();

const nameChanged = (value: string) => {
  if (!model.value) return;
  if (!model.value.urlPath) {
    model.value.urlPath = `/${value}`;
  }
};

const onBack = () => {
  if (model.value) {
    model.value!.body = addonToXml(designModel.value!);
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
  await Promise.all([
    form.value.validate(),
    settingsForm.value?.validate(),
  ]).then(async () => {
    const isNewPage = model.value!.id === emptyGuid;
    await pageStore.updatePage(model.value!);
    ElMessage.success(t("common.saveSuccess"));
    oldUrlPath.value = model.value!.urlPath ?? "";
    saveTip.init(model.value);
    if (isNewPage) {
      router.replace(
        useRouteSiteId({
          name: "layout-page-setting",
          query: {
            id: model.value!.id,
            layoutId: getQueryString("layoutId")!,
          },
        })
      );
    }
  });
};
const saveAndReturn = async () => {
  await onSave();
  onBack();
};

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

watch(
  () => designModel.value,
  () => {
    if (designModel.value) {
      model.value!.body = addonToXml(designModel.value);
    }
    saveTip.changed(model.value);
  },
  {
    deep: true,
  }
);
</script>

<template>
  <div v-if="designModel" class="flex w-full h-full">
    <div class="flex-1 pr-32 pl-80px flex flex-col min-h-400px min-w-600px">
      <div class="pt-16 flex items-center">
        <el-form v-if="model" ref="form" :model="model" :rules="rules">
          <el-form-item :label="t('common.pageName')" prop="name">
            <el-input
              v-model="model.name"
              class="w-300px"
              :title="model.name"
              :disabled="model.id !== emptyGuid"
              data-cy="page-name"
              @change="nameChanged"
              @input="model!.name = model?.name.replace(/\s+/g, '') as string"
            />
          </el-form-item>
        </el-form>
      </div>
      <div
        class="flex-1 rounded-normal overflow-hidden shadow-s-10 mb-100px bg-fff min-h-400px"
      >
        <Preview
          v-if="model && layout"
          :design="designModel"
          :page="model"
          :sources="sources"
        />
      </div>
    </div>
    <div class="bg-card dark:bg-[#333] shadow-s-10 w-400px h-full">
      <el-scrollbar>
        <div class="pb-150px">
          <NavBar v-model="currentTab" />
          <Design
            v-if="currentTab === options.design && designModel"
            :model="designModel"
            :sources="sources"
          />
          <Settings
            v-if="currentTab === options.settings && model && layout"
            ref="settingsForm"
            :model="model"
            :old-url-path="oldUrlPath"
            :layout="layout.body"
            :sources="sources"
          />
        </div>
      </el-scrollbar>
    </div>
  </div>
  <KBottomBar
    back
    :permission="{
      feature: 'pages',
      action: 'edit',
    }"
    @cancel="onBack"
    @save="onSave"
  >
    <template #extra-buttons>
      <el-button
        v-hasPermission="{
          feature: 'pages',
          action: 'edit',
        }"
        round
        type="primary"
        data-cy="save-and-return"
        @click="saveAndReturn"
        >{{ t("common.saveAndReturn") }}</el-button
      >
    </template>
  </KBottomBar>
</template>
