<script lang="ts" setup>
import { useMailModuleEditorStore } from "@/store/mail-module-editor";
import { computed } from "vue";

const mailModuleEditorStore = useMailModuleEditorStore();
const props = defineProps<{
  id: string;
  icon?: string;
  title: string;
  remove?: boolean;
  permission?: string;
}>();

defineEmits<{
  (e: "remove"): void;
}>();

const active = computed(() => {
  return mailModuleEditorStore.activeTab?.id === props.id;
});
</script>

<template>
  <div>
    <div
      class="group px-12 cursor-pointer h-26px flex items-center text-s space-x-4 group hover:bg-blue/30"
      :class="active ? 'bg-blue/20 dark:text-fff/86' : 'dark:text-fff/60'"
      :title="title"
    >
      <el-icon class="iconfont" :class="icon || 'icon-code3'" />
      <div class="ml-8px ellipsis flex-1" data-cy="file-name">{{ title }}</div>
      <div
        class="transition-all ease-in-out opacity-0 group-hover:opacity-100 space-x-8"
        :class="active ? 'opacity-100' : ''"
        @click.stop
      >
        <slot />
        <el-icon
          v-if="remove"
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
