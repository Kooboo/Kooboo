<script lang="ts" setup>
import EditActionBar from "./edit-action-bar.vue";
import EditCover from "./edit-cover.vue";
import { editing } from "@/views/inline-design/inline-editor";
</script>

<template>
  <div v-if="editing">
    <EditCover />
    <EditActionBar />
  </div>
</template>
