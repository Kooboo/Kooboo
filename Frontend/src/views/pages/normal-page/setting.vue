<script lang="ts" setup>
import { onBeforeRouteLeave, useRouter } from "vue-router";
import { useRouteSiteId } from "@/hooks/use-site-id";
import KBottomBar from "@/components/k-bottom-bar/index.vue";
import { ref, watch, nextTick } from "vue";
import { useShortcut } from "@/hooks/use-shortcuts";
import { getQueryString } from "@/utils/url";
import KFrame from "@/components/k-frame/index.vue";
import { useSetting } from "./use-setting";
import MultilingualSelector from "@/components/multilingual-selector/index.vue";

import PageSidebar from "./sidebar.vue";

import { useSiteStore } from "@/store/site";
import { useSaveTip } from "@/hooks/use-save-tip";
import { useStyleStore } from "@/store/style";
import { useScriptStore } from "@/store/script";

import { useI18n } from "vue-i18n";
import { ElMessage } from "element-plus";
const { t } = useI18n();
const router = useRouter();
const id = getQueryString("id");
const siteStore = useSiteStore();
const oldUrlPath = ref();
const saveTip = useSaveTip((key, value) => {
  if (key === "el" && value) {
    return undefined;
  } else {
    return value;
  }
});

const { init, model, save, preview, styles, scripts } = useSetting();

init(id!);

watch(
  () => model.value,
  () => {
    nextTick(() => {
      saveTip.init([model.value, styles.value, scripts.value]);
    });
    oldUrlPath.value = model.value?.urlPath;
  }
);
const basicForm = ref();
const onSave = async () => {
  await basicForm.value?.validate();
  await save();
  ElMessage.success(t("common.saveSuccess"));
  saveTip.init([model.value, styles.value, scripts.value]);
  oldUrlPath.value = model.value?.urlPath;
};

const onBack = () => {
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
      .check([model.value, styles.value, scripts.value])
      .then(() => {
        next();
      })
      .catch(() => {
        next(false);
      });
  }
});
watch(
  () => [model.value, styles.value, scripts.value],
  () => {
    saveTip.changed([model.value, styles.value, scripts.value]);
  },
  {
    deep: true,
  }
);

useShortcut("save", onSave);
useStyleStore().loadAll();
useScriptStore().loadAll();
</script>

<template>
  <div v-if="model" class="flex w-full h-full">
    <div class="flex-1 pr-32 pl-80px flex flex-col min-h-400px min-w-600px">
      <div class="pt-16">
        <el-form v-if="model">
          <el-form-item :label="t('common.pageName')">
            <el-input
              v-model="model.name"
              :title="model.name"
              class="w-300px"
              disabled
              @input="model!.name = model?.name.replace(/\s+/g, '') as string"
            />
          </el-form-item>
        </el-form>
      </div>
      <div
        class="flex-1 rounded-normal overflow-hidden shadow-s-10 mb-100px min-h-400px bg-fff"
      >
        <KFrame :content="preview" :base-url="siteStore.site.prUrl!" />
      </div>
    </div>
    <div
      class="bg-card dark:bg-[#333] shadow-s-10 w-400px h-full dark:text-fff/86"
    >
      <el-scrollbar>
        <div class="pb-150px">
          <div class="flex items-center px-24 py-12 leading-none">
            <p class="flex-1 text-m">{{ t("common.setting") }}</p>
            <MultilingualSelector />
          </div>
          <PageSidebar :old-url-path="oldUrlPath" />
        </div>
      </el-scrollbar>
    </div>
  </div>
  <KBottomBar
    back
    :permission="{ feature: 'pages', action: 'edit' }"
    @cancel="onBack"
    @save="onSave"
  >
    <template #extra-buttons>
      <el-button
        v-hasPermission="{ feature: 'pages', action: 'edit' }"
        round
        type="primary"
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
