<script lang="ts" setup>
import { useDevModeStore } from "@/store/dev-mode";
import { onMounted, onUnmounted, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute } from "vue-router";

const { t } = useI18n();
const keyword = ref("");
const input = ref();
const route = useRoute();
const devModeStore = useDevModeStore();

const emit = defineEmits<{
  (e: "search", value: string): void;
  (e: "active", value: boolean): void;
}>();

const focus = () => {
  input.value.focus();
};

const clear = () => {
  keyword.value = "";
};

defineExpose({ focus, clear });

watch(keyword, () => {
  emit("search", keyword.value.trim());
});

const showSearchInput = ref(false);
const handleKeyDown = async (e: {
  keyCode: number;
  ctrlKey: any;
  preventDefault: () => void;
}) => {
  if (e.keyCode === 70 && e.ctrlKey) {
    if (devModeStore.activeActivity?.name === "code search") return;
    showSearchInput.value = true;
    input.value.focus();
    e.preventDefault();
  }
};
onMounted(() => {
  document.addEventListener("keydown", handleKeyDown);
});
onUnmounted(() => {
  document.removeEventListener("keydown", handleKeyDown);
});
const closeSearch = () => {
  showSearchInput.value = false;
  keyword.value = "";
};
watch(
  () => showSearchInput.value,
  () => {
    emit("active", showSearchInput.value);
  }
);
watch(
  () => [devModeStore.activeActivity?.name, route.query.moduleId],
  () => {
    showSearchInput.value = false;
    keyword.value = "";
  }
);
</script>
<template>
  <div
    v-show="
      showSearchInput && devModeStore.activeActivity?.name !== 'code search'
    "
    class="absolute top-4 right-2px z-50 bg-fff dark:bg-[#252526]"
  >
    <SearchInput
      ref="input"
      v-model="keyword"
      :placeholder="t('common.searchFileName')"
      class="h-28px w-180px mr-2px text-12px"
    />

    <el-icon
      class="iconfont icon-delete5 text-blue text-opacity-80 cursor-pointer text-12px"
      :title="t('common.close')"
      @click="closeSearch"
    />
  </div>
</template>
