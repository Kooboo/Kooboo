<script lang="ts" setup>
defineProps<{ icon: string; display: string; shortcut: string }>();
</script>

<template>
  <div
    class="transition-all px-8 py-4 flex items-center space-x-4 cursor-pointer rounded-normal dark:text-fff/86 hover:(!text-blue)"
  >
    <el-icon class="iconfont" :class="icon" />
    <span>
      {{ display }}
    </span>
    <span class="text-s">
      {{ `(${shortcut})` }}
    </span>
  </div>
</template>
