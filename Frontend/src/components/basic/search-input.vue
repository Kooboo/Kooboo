<template>
  <el-input
    ref="input"
    class="rounded-full shadow-s-10 overflow-hidden"
    clearable
    @clear="clear"
    @keypress.enter="onSearch"
    @input="delaySearch"
  >
    <template #prefix>
      <div class="h-full flex items-center">
        <span class="iconfont icon-search text-blue ml-8" />
      </div>
    </template>
    <template #suffix>
      <slot />
    </template>
  </el-input>
</template>
<script setup lang="ts">
import { ref } from "vue";
import { searchDebounce } from "@/utils/url";

const input = ref();
const emit = defineEmits<{
  (e: "clear"): void;
  (e: "focus"): void;
  (e: "search"): void;
}>();

const clear = () => {
  emit("clear");
};
const focus = () => {
  input.value.focus();
};

const onSearch = () => {
  emit("search");
};

const delaySearch = searchDebounce(onSearch, 1000);

defineExpose({ focus });
</script>
<style lang="scss" scoped>
:deep(.el-input__wrapper) {
  @apply rounded-full;
}
</style>
