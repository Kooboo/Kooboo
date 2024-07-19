<script lang="ts" setup>
import { computed, inject, withDefaults } from "vue";
import type { PageState } from "./k-page";
import { PAGE_STATE_KEY } from "./k-page";
import type { Size } from "./types";
interface Props {
  data?: string;
  size?: number | Size;
  color?: string;
}
const props = withDefaults(defineProps<Props>(), {
  size: 16,
  color: "#303133",
  data: undefined,
});

const pageState = inject<PageState>(PAGE_STATE_KEY);

const data = computed(() => pageState?.getState(props.data).value);

const innerSize = computed(() => {
  if (typeof props.size == "number") return props.size;

  switch (props.size.toLowerCase()) {
    case "large":
      return 24;
    case "small":
      return 12;
    default:
      return 16;
  }
});
</script>

<template>
  <p
    class="whitespace-nowrap"
    v-bind="$attrs"
    :style="{
      color: color,
      fontSize: innerSize + 'px',
    }"
  >
    <slot>
      {{ data }}
    </slot>
  </p>
</template>
