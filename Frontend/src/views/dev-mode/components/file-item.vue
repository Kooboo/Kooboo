<script lang="ts" setup>
import { useDevModeStore } from "@/store/dev-mode";
import type { Ref } from "vue";
import { computed, inject } from "vue";
import { useRoute } from "vue-router";

const devModeStore = useDevModeStore();
const route = useRoute();
const props = defineProps<{
  id: string;
  icon?: string;
  title: string;
  remove?: boolean;
  ignoreSearch?: boolean;
  permission: string;
  padding?: number;
  hasExpandRow?: boolean;
  name?: string; //用于搜索时支持带文件夹名的搜索
}>();

defineEmits<{
  (e: "remove"): void;
}>();

const active = computed(() => {
  if (route.query.activity === "code search") {
    return devModeStore.activeSearch === props.id;
  } else {
    return devModeStore.activeTab?.id === props.id;
  }
});
const keyword = inject<Ref<string>>("keyword");
const searchName = computed(() => {
  return props.name ?? props.title;
});
</script>

<template>
  <div
    v-if="
      ignoreSearch ||
      !keyword ||
      searchName.toLowerCase().indexOf(keyword.toLowerCase()) > -1
    "
  >
    <div
      class="group cursor-pointer h-26px flex items-center text-s space-x-4 group hover:bg-blue/30 pr-12"
      :class="active ? 'bg-blue/20 dark:text-fff/86' : 'dark:text-fff/60'"
      :title="title"
      :style="
        hasExpandRow
          ? {
              paddingLeft: padding + 'px',
            }
          : {
              paddingLeft: '18px',
            }
      "
    >
      <el-icon class="iconfont" :class="icon || 'icon-code3'" />
      <div class="ml-8px ellipsis flex-1" data-cy="file-name">
        {{ title }}
      </div>
      <div
        class="transition-all ease-in-out opacity-0 group-hover:opacity-100 space-x-8"
        :class="active ? 'opacity-100' : ''"
        @click.stop
      >
        <slot />
        <el-icon
          v-if="remove"
          v-hasPermission="{
            feature: permission,
            action: 'delete',
            effect: 'hiddenIcon',
          }"
          class="iconfont icon-delete hover:text-orange"
          data-cy="delete"
          @click="$emit('remove')"
        />
      </div>
    </div>
    <div class="pl-12 pr-4">
      <slot name="collapse" />
    </div>
  </div>
</template>
