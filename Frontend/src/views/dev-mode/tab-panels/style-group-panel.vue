<script lang="ts" setup>
import { ref } from "vue";
import EditGroup from "@/views/development/styles/edit-group.vue";

defineProps<{ id: string }>();
const inner = ref();

const save = async () => {
  inner.value.save();
};

defineExpose({ save });
</script>

<template>
  <div class="h-full bg-fff dark:bg-[#141414]">
    <EditGroup :id="id" ref="inner" hidden-name />
  </div>
</template>
