<script lang="ts" setup>
import { nextTick, onMounted, ref, watch, useAttrs } from "vue";
import { useI18n } from "vue-i18n";
import { api } from "@/utils/request";
import { useRoute, useRouter } from "vue-router";

const route = useRoute();
const router = useRouter();
const { t } = useI18n();
const props = defineProps<{ context?: any; messageId: number; type: string }>();
const content = ref<[{ name: string; content: string }]>();
const attrs = useAttrs();
(async () => {
  content.value = (
    await api(`/MailExtension/${props.type}?messageId=${props.messageId}`)
  ).data;
})();

const open = ref(false);
const frame = ref();
const isShowFrame = ref(true);
const showMailModule = (show: boolean) => {
  open.value = show;
  localStorage.setItem("isOpenMailModule", show ? "true" : "false");
};

watch(
  () => frame.value,
  (value) => {
    for (const i of value) {
      if (!i || !i.element.contentWindow) return;
      if (!i.element.contentWindow.k) {
        i.element.contentWindow.k = {};
      }
      i.element.contentWindow.k.currentMail = props.context;
    }
  }
);

watch(
  () => route,
  () => {
    if (route.name === "compose") {
      reloadKframe();
    }
  },
  { deep: true }
);

onMounted(() => {
  if (localStorage.getItem("isOpenMailModule") === "true") {
    open.value = true;
  } else {
    open.value = false;
  }
});

const reloadKframe = () => {
  isShowFrame.value = false;
  nextTick(() => {
    isShowFrame.value = true;
  });
};
defineExpose({
  open,
});
</script>

<template>
  <div
    class="group w-[22%] h-[calc(100vh-160px)] fixed top-72px right-[calc(-22%-15px)] bg-fff dark:bg-[#333] p-24 shadow-s-20 rounded-normal transition-all duration-500"
    :class="[
      open ? ' !right-0' : '!ml-30px',
      attrs.height ?? 'h-[calc(100vh-160px)]',
    ]"
  >
    <div class="h-full w-full flex flex-col">
      <div class="py-8 dark:text-[#fffffa]">{{ t("common.mailModule") }}</div>
      <el-tabs
        v-if="content?.length"
        class="moduleTabs flex-1"
        tab-position="right"
      >
        <el-tab-pane
          v-for="(item, index) in content"
          :key="index"
          :title="item.name"
          class="h-full"
        >
          <KFrame
            v-if="isShowFrame"
            ref="frame"
            :key="item.name"
            class="h-full w-full"
            :content="item.content"
          />
          <template #label>
            <span :title="item.name">{{ item.name }}</span>
          </template>
        </el-tab-pane>
      </el-tabs>
      <div v-else class="w-full h-full text-m mt-12 dark:text-[#fffa]">
        <div class="flex flex-wrap">
          {{ t("common.haveNotMailModuleTips") }}
          <div
            class="cursor-pointer text-blue"
            @click="
              router.push({
                name: 'mail-module',
                query: {
                  ...router.currentRoute.value.query,
                  oldFolder: route.name?.toString(),
                },
              })
            "
          >
            {{ t("common.goToCreate") }}
          </div>
        </div>
      </div>
    </div>

    <div
      class="group-hover:flex hidden rounded-full bg-fff shadow-s-10 w-26px flex items-center h-26px cursor-pointer dark:bg-666 absolute top-24px -left-12px flex items-center justify-center"
      @click="showMailModule(false)"
    >
      <el-icon
        class="iconfont icon-a-nextstep2 cursor-pointer text-blue text-s"
      />
    </div>
  </div>
  <div
    class="fixed top-90px -right-42px bg-fff dark:bg-999 p-12 shadow-s-20 rounded-l-full cursor-pointer transition-all duration-500 flex items-center justify-center text-444 text-opacity-60 hover:text-blue"
    :class="!open ? ' !right-0' : ''"
    :title="t('common.mailModule')"
    @click="showMailModule(true)"
  >
    <el-icon class="iconfont icon-a-Expansionmodule text-18px" />
  </div>
</template>
<style>
.moduleTabs .el-tabs__content {
  height: 100%;
  overflow: auto;
}
.moduleTabs .el-tabs__item.is-right {
  writing-mode: vertical-lr;
  height: auto;
  padding: 12px 0;
  overflow: hidden;
  max-height: 150px;
  white-space: nowrap;
  text-overflow: ellipsis;
}
.moduleTabs.el-tabs--right.el-tabs--card .el-tabs__item.is-right.is-active {
  border-left-color: transparent !important;
}
.moduleTabs span.el-tabs__nav-prev.is-disabled,
.moduleTabs span.el-tabs__nav-next.is-disabled {
  cursor: not-allowed !important;
  opacity: 0.4 !important;
}
</style>
