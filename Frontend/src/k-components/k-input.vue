<script lang="ts" setup>
import { computed } from "vue";
import { inject } from "vue";
import type { DataType } from "./data";
import type { PageState } from "./k-page";
import { PAGE_STATE_KEY } from "./k-page";

interface Props {
  type?: DataType;
  data?: any;
  options?: string;
}

const props = defineProps<Props>();
const pageState = inject<PageState>(PAGE_STATE_KEY);

const data = computed({
  get() {
    const state = pageState?.getState(props.data);
    return state?.value;
  },
  set(value) {
    const state = pageState?.getState(props.data);
    if (state) state.value = value;
  },
});

const innerOptions = computed(() => {
  const result = [];

  if (props.options) {
    const options = pageState?.getState<any>(
      props.options,
      "mappedResponse"
    ).value;

    if (options && Array.isArray(options)) {
      for (const option of options) {
        if (typeof option == "object") {
          result.push(option);
        } else {
          result.push({
            label: option,
            value: option,
          });
        }
      }
    }
  }

  return result;
});
</script>

<template>
  <template v-if="options">
    <el-select v-model="data" clearable>
      <el-option
        v-for="option in innerOptions"
        :key="option.value"
        :label="option.label"
        :value="option.value"
      />
    </el-select>
  </template>
  <template v-else-if="type == 'textarea'">
    <el-input v-model="data" type="textarea" />
  </template>
  <template v-else-if="type == 'date'">
    <el-date-picker
      v-model="data"
      type="date"
      value-format="YYYY-MM-DD HH:mm:ss"
    />
  </template>
  <template v-else-if="type == 'number'">
    <el-input-number v-model="data" />
  </template>
  <template v-else-if="type == 'datetime'">
    <el-date-picker
      v-model="data"
      type="datetime"
      value-format="YYYY-MM-DD HH:mm:ss"
    />
  </template>
  <template v-else-if="type == 'switch'">
    <el-switch v-model="data" />
  </template>
  <template v-else>
    <el-input v-model="data" :type="type" />
  </template>
</template>
