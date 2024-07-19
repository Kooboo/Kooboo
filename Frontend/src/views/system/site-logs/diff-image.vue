<template>
  <el-scrollbar max-height="100%" view-class="h-full">
    <div
      class="image-compare flex justify-center space-x-16 p-32 bg-[#fff] dark:bg-[#1e1e1e] overflow-auto h-full"
    >
      <div class="flex justify-center flex-1">
        <img :src="original" :alt="original" />
      </div>
      <div class="w-1px bg-[#ebeef5] dark:bg-[#555] h-auto" />
      <div class="flex justify-center flex-1">
        <img :src="modified" :alt="modified" />
      </div>
    </div>
  </el-scrollbar>
</template>

<script lang="ts" setup>
defineProps<{
  original: string;
  modified: string;
}>();
</script>

<style lang="scss" scoped>
.image-compare {
  img {
    max-width: 100%;
    height: fit-content;
  }
}
</style>
