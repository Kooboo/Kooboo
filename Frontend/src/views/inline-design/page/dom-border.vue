<script lang="ts" setup>
import LineRect from "./line-rect.vue";
import { computed } from "vue";
import { getRect } from "../utils/dom";
import { currentElement, hoverElement } from "@/views/inline-design/page";
import { editing } from "@/views/inline-design/inline-editor";

const rect = computed(() => getRect(currentElement.value));
const hoverRect = computed(() => getRect(hoverElement.value));
</script>

<template>
  <div v-if="!editing">
    <LineRect
      v-if="hoverRect"
      :position="hoverRect"
      color="rgba(34, 150, 243,.8)"
      :width="2"
    />
    <LineRect v-if="rect" :position="rect" :width="1" />
  </div>
</template>
