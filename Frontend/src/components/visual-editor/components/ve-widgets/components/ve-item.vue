<template>
  <div
    class="h-[128px] mt-3 ml-12 dark:bg-444 bg-fff border border-999 py-5 flex flex-col justify-between items-center text-[#6b7280] text-[12px] font-bold hover:shadow-444 cursor-move"
    style="width: 28%"
    :title="tooltip"
  >
    <div class="w-10 h-10">
      <el-icon
        v-if="icon"
        class="iconfont w-full h-full text-44px text-[#505659]"
        :class="icon"
      />
    </div>
    <div
      class="w-full truncate text-center text-bold text-xs px-2 leading-[1.2em]"
    >
      {{ name }}
    </div>
  </div>
</template>

<script lang="ts" setup>
defineProps<{
  name: string;
  tooltip?: string;
  icon?: string;
}>();
</script>
