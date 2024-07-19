<script lang="ts" setup>
import { onMounted, ref } from "vue";
import { watch } from "vue";
import { Chart, registerables } from "chart.js";
import { dark } from "@/composables/dark";
import { cloneDeep } from "lodash-es";

Chart.register(...registerables);
const container = ref<HTMLCanvasElement>();
const props = defineProps<{ options: any }>();
let chart: Chart;

onMounted(init);

function init() {
  chart = new Chart(container.value!, cloneDeep(props.options));
}

watch(
  dark,
  async (dark) => {
    Chart.defaults.color = dark ? "rgb(168,168,168)" : "rgb(25,40,69)";
    Chart.defaults.borderColor = dark ? "#333" : "#ddd";
    if (!container.value) return;
    chart?.destroy();
    init();
  },
  {
    immediate: true,
  }
);

watch(
  () => props.options,
  () => {
    if (!chart) return;
    chart.data = props.options.data;
    chart.update("none");
  },
  { deep: true }
);
</script>
<template>
  <canvas ref="container" width="400" height="400" />
</template>
