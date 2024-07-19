<script lang="ts" setup>
import { getCurrentInstance, withDefaults } from "vue";
import { useSlots, nextTick } from "vue";
import type { Format } from "../types";
import { getSlotContent } from "../utils";

interface Props {
  type?: Format;
}

const slots = useSlots();
const props = withDefaults(defineProps<Props>(), {
  type: "json",
});
const defaultValue = getSlotContent(slots);
const response = getCurrentInstance()?.parent?.exposed?.rawResponse;

if (response && defaultValue) {
  nextTick(() => {
    switch (props.type) {
      case "json":
        response.value = JSON.parse(defaultValue);
        break;
      default:
        response.value = defaultValue;
    }
  });
}
</script>

<template>
  <div class="hidden">
    <slot />
  </div>
</template>
