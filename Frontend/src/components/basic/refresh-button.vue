<script lang="ts" setup>
import { onBeforeUnmount, onMounted, ref } from "vue";

const props = defineProps<{ auto?: boolean; intervalSeconds?: number }>();
const rotate = ref(0);
const time = ref(0);
let cancellationToken: any;

const emit = defineEmits<{
  (e: "reload"): void;
}>();

onMounted(() => {
  if (props.auto && props.intervalSeconds) {
    let interval = props.intervalSeconds;
    if (interval < 3) interval = 3;
    time.value = interval;
    cancellationToken = setInterval(async () => {
      if (time.value > 1) {
        time.value--;
      } else {
        reload();
        time.value = interval;
      }
    }, 1000);
  }
});

onBeforeUnmount(() => {
  clearInterval(cancellationToken);
});

function reload() {
  rotate.value += 360;
  emit("reload");
}
</script>

<template>
  <div class="text-999 text-m space-x-8 flex items-center">
    <span v-if="auto" class="text-s">{{ time }}s</span>
    <ElIcon
      class="iconfont icon-Refresh hover:text-blue transition-all duration-500"
      :style="{ transform: `rotate(${rotate}deg)` }"
      @click="reload"
    />
  </div>
</template>
