<script lang="ts" setup>
import { computed, inject } from "vue";
import type { PageState } from "./k-page";
import { PAGE_STATE_KEY } from "./k-page";
import KInline from "./layout/k-inline.vue";
import KFill from "./layout/k-fill.vue";

interface Props {
  total?: string | number;
  pageSize?: string | number;
  currentPage?: string | number;
}

const props = defineProps<Props>();

const pageState = inject<PageState>(PAGE_STATE_KEY);

function getNumberValue(
  prop: string | number | undefined,
  defaultValue: number
) {
  if (!prop) return defaultValue;
  if (typeof prop == "number") return prop;
  let state = pageState?.getState(prop).value;
  if (!state) return defaultValue;
  state = parseInt(state as any);
  if (Number.isNaN(state)) return defaultValue;
  return state as number;
}

const innerTotal = computed(() => getNumberValue(props.total, 1));
const innerPageSize = computed(() =>
  getNumberValue(props.pageSize, 5)
) as unknown as number | undefined;

const innerCurrentPage = computed<number>({
  get() {
    return getNumberValue(props.currentPage, 1);
  },
  set(value) {
    if (typeof props.currentPage == "string") {
      var state = pageState?.getState<any>(props.currentPage);
      if (state) state.value = value;
    }
  },
});
</script>

<template>
  <KInline>
    <KFill />
    <el-pagination
      v-model:current-page="innerCurrentPage"
      background
      layout="prev, pager, next"
      :total="innerTotal"
      :page-size="innerPageSize"
    />
    <KFill />
  </KInline>
</template>
