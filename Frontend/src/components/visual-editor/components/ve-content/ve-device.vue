<template>
  <el-button
    v-for="it in pageWidths"
    :key="it.name"
    :type="it.name === width.name ? 'primary' : 'default'"
    @click="switchWidth(it.name)"
  >
    {{ it.display }}
  </el-button>
</template>

<script lang="ts" setup>
import { pageWidths, switchWidth, width } from "./effects";
</script>
