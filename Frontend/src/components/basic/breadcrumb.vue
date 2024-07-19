<script lang="ts" setup>
import type { CrumbPathItem } from "@/api/content/media";
import { ref, computed, watch, nextTick } from "vue";
import type { ElScrollbar } from "element-plus";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { router } from "@/modules/router";
import { ArrowRight } from "@element-plus/icons-vue";

import { useI18n } from "vue-i18n";
export interface Props {
  name?: string; // 当前路径名 与固定面包屑拼接
  crumbPath?: CrumbPathItem[]; // 多个路径 [{name, fullPath, route}], 通过 @handleClickPath 控制跳转
  hideHeader?: boolean; // 隐藏固定头
  size?: "small" | "default" | "large";
}

export interface Emits {
  (evt: "handleClickPath", item: CrumbPathItem): void;
}

const props = defineProps<Props>();
const emits = defineEmits<Emits>();
const { t } = useI18n();

// 大小样式控制
const fontSize = computed(() => {
  return (
    {
      small: 14,
      default: 16,
      large: 18,
    }[props.size || "default"] + "px"
  );
});
const space = computed(() => {
  return (
    {
      small: 4,
      default: 6,
      large: 10,
    }[props.size || "default"] + "px"
  );
});

const scrollbar = ref<InstanceType<typeof ElScrollbar>>();

watch(
  () => props.crumbPath,
  () => {
    nextTick(() => {
      // 更新滚动条
      scrollbar.value?.update();
      scrollbar.value?.setScrollLeft(9999);
    });
  }
);

const handleClickPath = (item: CrumbPathItem) => {
  emits("handleClickPath", item);
  if (item.route) {
    router.push(useRouteSiteId(item.route));
  }
};
</script>

<template>
  <el-scrollbar ref="scrollbar">
    <el-breadcrumb :separator-icon="ArrowRight">
      <template v-if="!hideHeader">
        <el-breadcrumb-item
          :to="{ name: 'home' }"
          class="text-black dark:text-blue-10"
          data-cy="nav-sites"
        >
          {{ t("common.sites") }}
        </el-breadcrumb-item>
        <el-breadcrumb-item
          :to="useRouteSiteId({ name: 'dashboard' })"
          class="text-black dark:text-blue-10"
          data-cy="nav-dashboard"
        >
          {{ t("common.dashboard") }}
        </el-breadcrumb-item>
      </template>

      <el-breadcrumb-item
        v-if="name"
        class="cursor-default"
        data-cy="nav-item"
        >{{ name }}</el-breadcrumb-item
      >

      <!-- crumbPath -->
      <el-breadcrumb-item
        v-for="(item, index) in crumbPath"
        :key="item.fullPath"
        class="max-w-15rem"
        data-cy="nav-item"
      >
        <span
          :class="
            index < crumbPath!.length - 1
              ? 'cursor-pointer hover:text-blue font-bold text-black dark:text-fff/86'
              : 'text-666 dark:text-fff/60 cursor-default'
          "
          @click="handleClickPath(item)"
        >
          {{ item.name }}
        </span>
      </el-breadcrumb-item>
    </el-breadcrumb>
  </el-scrollbar>
</template>

<style lang="scss" scoped>
:deep(.el-breadcrumb) {
  @apply flex w-full;
}
:deep(.el-breadcrumb__item .el-breadcrumb__inner) {
  @apply truncate;
}
:deep(.el-scrollbar__bar.is-horizontal) {
  @apply h-3px bottom-0;
}
:deep(.el-scrollbar__wrap) {
  @apply mb-4;
}
:deep(.el-breadcrumb) {
  font-size: v-bind(fontSize);
}

:deep(.el-breadcrumb__separator),
:deep(.el-breadcrumb__item:not(:first-child)) {
  margin-left: v-bind(space) !important;
}
</style>
