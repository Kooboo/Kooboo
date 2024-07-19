<script lang="ts" setup>
import { useRouteSiteId } from "@/hooks/use-site-id";
import { useModuleStore } from "@/store/module";
import { ClickOutside as vClickOutside } from "element-plus";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute } from "vue-router";
import { useDevModeStore } from "@/store/dev-mode";
import { useSiteStore } from "@/store/site";

const { t } = useI18n();
const route = useRoute();

const devModeStore = useDevModeStore();
const siteStore = useSiteStore();

const isDevMode = computed(() => {
  return route.name === "dev-mode";
});

const visible = ref(false);
const popoverRef = ref();
const onClickOutside = () => {
  visible.value = false;
};
const list = [
  t("common.devModeList1"),
  t("common.devModeList2"),
  t("common.devModeList3"),
];
const openDevModePopover = () => {
  if (!devModeStore.activities.length && !siteStore.isAdmin) return;
  visible.value = true;
};
</script>

<template>
  <el-popover
    ref="popoverRef"
    :visible="visible"
    placement="bottom-start"
    :width="400"
    trigger="click"
    content="K"
  >
    <template #reference>
      <div
        v-popover="popoverRef"
        class="h-full cursor-pointer border-l border-solid border-line dark:border-opacity-50 flex items-center justify-center pr-16 pl-16 text-14px dark:bg-[#333]"
        :class="[
          isDevMode ? 'bg-bg-[#EFF6FF]' : '',
          !devModeStore.activities.length && !siteStore.isAdmin
            ? 'hover:cursor-not-allowed text-999'
            : 'hover:bg-[#EFF6FF] dark:hover:bg-444',
        ]"
        data-cy="dev-mode"
        @click="openDevModePopover"
      >
        <div v-if="visible" class="absolute inset-0" @click.stop="" />
        {{ t("common.devMode") }}
      </div>
    </template>
    <div v-click-outside="onClickOutside" class="ml-12 inline-block">
      <p class="text-000 dark:text-fff/86 font-bold mt-8 ml-8 mb-8">
        {{ t("common.KoobooDevMode") }}
      </p>
      <ul class="ml-8 justify-between mb-12 h-auto mr-12">
        <li
          v-for="(item, index) in list"
          :key="index"
          class="mt-8 flex items-start"
        >
          <el-icon class="iconfont icon-yes2 text-green mr-8 pt-4 mt-8" />
          <span class="text-666 dark:text-fff/60 leading-8">{{ item }} </span>
        </li>
      </ul>
    </div>
    <div class="mb-16 ml-12">
      <router-link
        v-if="route.name === 'dev-mode'"
        :to="useRouteSiteId({ name: 'dashboard' })"
      >
        <el-button type="primary" round data-cy="close-dev-mode">
          {{ t("common.closeDevMode") }}
        </el-button>
      </router-link>

      <router-link
        v-else
        :to="
            useRouteSiteId({
              name: 'dev-mode',
              query: { activity: devModeStore.activities.map((i) => i.name).includes(route.name as string)? route.name as string:devModeStore.activities[0]?.name},
            })
          "
        @click="visible = false"
      >
        <el-button type="primary" round data-cy="open-dev-mode">
          {{ t("common.openDevMode") }}
        </el-button>
      </router-link>
    </div>
  </el-popover>
</template>
<style scoped>
button.el-button.el-button--default.el-tooltip__trigger {
  margin-left: 0;
}
.el-button--default {
  box-shadow: none;
}
.cursor-default.border-r.border-solid.border-line.flex.items-center.justify-center.pr-16.text-14px.el-tooltip__trigger.el-tooltip__trigger {
  margin-left: 0px;
}
</style>
