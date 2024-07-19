<script lang="ts" setup>
import BasicPanel from "./basic.vue";
import { load, site } from "./settings";
import Custom from "./custom.vue";
import Advance from "./advance.vue";
import ExportSiteDialog from "@/components/export-site-dialog/index.vue";
import { onUnmounted, ref, watch } from "vue";
import { useSiteStore } from "@/store/site";
import { useSaveTip } from "@/hooks/use-save-tip";
import { onBeforeRouteLeave } from "vue-router";

const showExportSiteDialog = ref(false);
const saveTip = useSaveTip();
const siteStore = useSiteStore();
const updateKey = ref(+new Date());

async function loadSite() {
  site.value = undefined;
  await useSiteStore().loadSite();
  load();
  saveTip.init(site.value);
}

const initSite = () => {
  saveTip.init(site.value);
  updateKey.value = +new Date();
};

// 组件销毁时重置firstActiveMenu的值，防止影响到activeName外面的行为
onUnmounted(() => {
  siteStore.firstActiveMenu = "";
});

loadSite();
onBeforeRouteLeave(async (to, from, next) => {
  if (to.name === "login") {
    next();
  } else {
    siteStore.firstActiveMenu = to.meta.activeMenu ?? to.name;
    await saveTip
      .check(site.value)
      .then(() => {
        next();
      })
      .catch(() => {
        siteStore.firstActiveMenu = "settings";
        next(false);
      });
  }
});
watch(
  () => site.value,
  () => {
    saveTip.changed(site.value);
  },
  {
    deep: true,
  }
);

defineExpose({
  initSite,
});
</script>

<template>
  <div v-if="site">
    <el-form :key="updateKey" label-position="top">
      <BasicPanel @export="showExportSiteDialog = true" @init-site="initSite" />
      <Advance />
      <Custom />
    </el-form>
    <ExportSiteDialog
      v-if="showExportSiteDialog"
      v-model="showExportSiteDialog"
      :site-id="site.id"
    />
  </div>
</template>
