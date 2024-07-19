<script lang="ts" setup>
import { onBeforeUnmount, onMounted, onUpdated, ref, watch } from "vue";
import { useRoute } from "vue-router";

defineEmits<{
  (e: "onAdd"): void;
}>();

const route = useRoute();
const props = defineProps<{
  title: string;
  add?: boolean;
  expandDefault?: boolean;
  permission?: string;
}>();

const expand = ref(props.expandDefault || false);
const bodyHeight = ref<string>();
const body = ref<HTMLElement>();
let observer: MutationObserver;

const updateBodyHeight = async () => {
  const value = body.value ? `${body.value?.scrollHeight}px` : undefined;
  if (value === bodyHeight.value) return;
  bodyHeight.value = value;
  expand.value = true;
};

onUpdated(updateBodyHeight);

onMounted(() => {
  observer = new MutationObserver(updateBodyHeight);
  observer.observe(body.value!, {
    childList: true,
    subtree: true,
  });
});

onBeforeUnmount(() => {
  observer?.disconnect();
});

watch(
  () => route.query.activity,
  () => {
    updateBodyHeight();
  }
);
</script>

<template>
  <div class="group-collapse">
    <div
      class="h-26px flex items-center px-12 text-s bg-gray/60 dark:(bg-[#333] text-fff/86) space-x-8 not-last:(border-b border-solid border-gray/50 dark:border-opacity-20) cursor-pointer"
      @click.stop="expand = !expand"
    >
      <span class="flex-1" data-cy="folder">{{ title }}</span>
      <div class="flex items-center space-x-8 collapse-bar" @click.stop>
        <div class="transition-all opacity-0 flex items-center space-x-8">
          <slot name="bar" />
          <el-icon
            v-if="add"
            v-hasPermission="{
              feature: permission,
              action: 'edit',
              effect: 'hiddenIcon',
            }"
            class="iconfont icon-a-addto hover:text-blue"
            data-cy="add"
            @click="$emit('onAdd')"
          />
        </div>

        <el-icon
          class="iconfont icon-pull-down origin-center transform transition duration-200 origin-center"
          :class="expand ? 'rotate-180 ' : 'rotate-0'"
          data-cy="expand"
          @click="expand = !expand"
        />
      </div>
    </div>
    <div
      :style="{ height: expand ? bodyHeight : '0px' }"
      class="overflow-hidden transition-all duration-200"
    >
      <div ref="body">
        <slot />
      </div>
    </div>
  </div>
</template>
<style lang="scss">
.group-collapse:hover,
.group-collapse:focus {
  .collapse-bar * {
    opacity: 1;
  }
}
</style>
