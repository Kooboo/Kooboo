<template>
  <div>
    <div @click.stop="flag = !flag">
      <slot name="name" :flag="flag" />
    </div>
    <div v-show="flag">
      <slot />
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref } from "vue";

const flag = ref(true);
</script>
