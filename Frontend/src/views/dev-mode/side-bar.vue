<script lang="ts" setup>
import { useDevModeStore } from "@/store/dev-mode";
import { useSiteStore } from "@/store/site";
import { getQueryString } from "@/utils/url";
import { computed, onMounted, provide, ref } from "vue";
import { useI18n } from "vue-i18n";
import FilterBar from "./components/filter-bar.vue";
import SettingPanel from "./tab-panels/setting-panel.vue";

const { t } = useI18n();
const devModeStore = useDevModeStore();
const filterBar = ref();
const keyword = ref("");
const panelFocus = ref(false);
const siteStore = useSiteStore();

const onSetting = () => {
  devModeStore.addTab({
    id: "setting",
    name: t("common.setting"),
    panel: SettingPanel,
    icon: "icon-a-setup",
  });
};

const isSearch = computed(() => {
  return devModeStore.activeActivity?.name == "code search";
});

onMounted(() => {
  const activity = getQueryString("activity");
  if (activity) {
    devModeStore.selectActivity(activity);
  } else {
    if (!devModeStore.activeActivity!.init) {
      devModeStore.selectActivity(devModeStore.activeActivity!.name);
    }
  }
});

const onActivityClick = (name: string) => {
  setTimeout(() => {
    devModeStore.selectActivity(name, true);
  }, 0);
};

provide("keyword", keyword);
</script>

<template>
  <div
    class="h-full bg-fff dark:(bg-[#252526] border-none) flex border-r border-solid border-line dark:border-[#f00]"
  >
    <div
      class="w-48px h-full dark:(bg-[#333333] border-none) border-r border-line border-solid flex flex-col"
    >
      <template v-for="item of devModeStore.activities" :key="item.name">
        <el-tooltip
          class="box-item"
          transition="bounce"
          :hide-after="0"
          :content="item.display"
          placement="right"
        >
          <div
            :class="[
              'side-icon',
              item.name === devModeStore.activeActivity?.name
                ? 'bg-blue/10 !text-blue/87'
                : '',
            ]"
            :data-cy="'tab-' + `${item.name}`"
            @click="onActivityClick(item.name)"
          >
            <el-icon class="iconfont text-18px" :class="item.icon" />
          </div>
        </el-tooltip>
      </template>
      <div class="flex-1" />
      <div
        v-if="siteStore.hasAccess('site', 'edit')"
        class="side-icon"
        data-cy="setting"
        @click="onSetting"
      >
        <el-icon class="iconfont icon-a-setup" />
      </div>
    </div>
    <div
      class="w-200px h-full relative"
      @click="!isSearch && filterBar && filterBar.focus()"
    >
      <FilterBar
        ref="filterBar"
        @active="panelFocus = $event"
        @search="keyword = $event"
      />
      <template v-for="item of devModeStore.activities" :key="item.name">
        <div
          v-show="item.name === devModeStore.activeActivity?.name"
          class="h-full flex flex-col"
        >
          <div
            class="ellipsis h-36px text-black flex items-center pl-12 pr-4 dark:text-fff/60"
          >
            <span class="ellipsis h-full leading-9" data-cy="tab-title">{{
              item.panelDisplay ?? item.display
            }}</span>
            <div class="flex-1" />
            <div v-if="item.actions" class="-space-x-4 flex items-center">
              <template v-for="i of item.actions" :key="i.name">
                <IconButton
                  v-if="i.visible"
                  :icon="i.icon"
                  :tip="i.display"
                  class=""
                  :data-cy="i.name"
                  @click="i.invoke()"
                />
              </template>
            </div>
          </div>
          <el-scrollbar
            class="flex-1"
            :style="{
              outline:
                panelFocus &&
                devModeStore.activeActivity?.name !== 'code search'
                  ? '1px solid rgba(34, 150, 243,.5)'
                  : '',
            }"
            @click.stop
          >
            <component
              :is="item.panel"
              :ref="(c:any) => item.panelInstance.resolve(c)"
            />
          </el-scrollbar>
        </div>
      </template>
    </div>
  </div>
</template>

<style scoped>
.side-icon {
  @apply h-48px flex items-center justify-center cursor-pointer dark:(text-fff/60 hover:text-blue/87) hover:text-blue;
}
</style>
