<script lang="ts" setup>
import { ref } from "vue";
import { getMonthly } from "@/api/visitor-log";
import type { TopPage } from "@/api/visitor-log/types";
import ChartJs from "@/components/basic/chart-js.vue";

const data = ref<TopPage[]>([]);
const show = ref(false);

const options = ref({
  type: "bar",
  data: {
    labels: [] as string[],
    datasets: [
      {
        label: "PV",
        data: [] as number[],
        backgroundColor: ["rgb(54, 162, 235,0.6)"],
      },
    ],
  },
  options: {
    scales: {
      y: {
        beginAtZero: true,
      },
    },
  },
});

const load = async () => {
  data.value = await getMonthly();
  if (data.value.length < 16) {
    for (let i = 0; i < 16 - data.value.length; i++) {
      options.value.data.labels.push("");
      options.value.data.datasets[0].data.push(0);
    }
  }
  options.value.data.labels.push(...data.value.map((m) => m.name));
  options.value.data.datasets[0].data.push(...data.value.map((m) => m.count));
  show.value = true;
};

load();
</script>

<template>
  <div class="p-16 rounded-normal bg-fff dark:bg-444">
    <div>
      <ChartJs height="180" :options="options" />
    </div>
  </div>
</template>
