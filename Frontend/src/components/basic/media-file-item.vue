<template>
  <div
    class="file-item relative rounded-4px w-full h-161px px-32 pt-32 mb-12 hover:bg-[#E9EAF0] dark:hover:bg-444 flex flex-col"
    :class="{ 'bg-[#E9EAF0] dark:bg-444': selected }"
  >
    <slot name="checkbox" />
    <div class="hidden absolute top-8 right-8">
      <div
        class="h-20px flex items-center space-x-8 dark:text-fff/50 text-14px text-666"
      >
        <slot name="actions" />
      </div>
    </div>

    <div class="w-full flex-1 rounded-4px overflow-hidden">
      <slot name="thumbnail">
        <img
          v-if="src"
          class="select-none w-full h-full object-contain dark:text-fff/60"
          :src="src"
          data-cy="thumbnail"
          :alt="alt"
          :title="title"
        />
      </slot>
    </div>

    <div
      class="ellipsis text-s text-444 dark:text-fff/60 h-32 leading-32px text-center"
      data-cy="file-name"
    >
      <slot name="file-name">
        {{ fileName }}
      </slot>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { computed } from "vue";

const props = defineProps<{
  src: string;
  name?: string;
  selected?: boolean;
  alt?: string;
  title?: string;
}>();

function getFileName(url: string) {
  const arr = url.split("/");
  return arr[arr.length - 1];
}

const fileName = computed(() => {
  return props.name || getFileName(props.src);
});
</script>

<style lang="scss" scoped>
.file-item {
  &:hover {
    .hidden {
      display: block;
      &.inline-flex {
        display: inline-flex;
      }
    }
  }
}
</style>
