<script lang="ts" setup>
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

const props = defineProps<{
  src: string;
  hasUrlWrap?: boolean;
  onSelect?: () => void;
}>();

const container = ref<HTMLDivElement>();
const { t } = useI18n();
const imgUrl = computed(() => {
  if (props.hasUrlWrap) {
    return props.src;
  } else {
    return `url("${props.src}")`;
  }
});
</script>

<template>
  <div
    ref="container"
    class="h-full w-full overflow-hidden relative bg-fff group cursor-pointer"
    @click="onSelect"
  >
    <TransparentBackground />
    <div
      class="absolute inset-0 bg-no-repeat bg-center bg-contain"
      :style="{ backgroundImage: imgUrl }"
    />
    <div
      v-if="onSelect"
      class="transition-all duration-300 absolute inset-0 top-auto opacity-0 bg-000/40 flex items-center justify-center group-hover:opacity-100"
    >
      <span class="text-fff text-2l font-bold p-12">
        {{ t("common.clickReplace") }}
      </span>
    </div>
  </div>
</template>
