<template>
  <el-select
    ref="inputRef"
    :model-value="inputValue"
    placeholder=" "
    class="w-480px ellipsis el-hover-title selectInput"
    multiple
    filterable
    allow-create
    remote
    popper-class="el-select-popper"
    default-first-option
    :reserve-keyword="false"
    :remote-method="remoteMethod"
    fit-input-width
    @update:model-value="emit('update:modelValue', $event.map(formatInput))"
    @keydown="onKeyDown"
    @change="inputOnBlur(inputRef)"
    @input="setInputQuery(inputRef)"
    @paste="onPaste"
    @blur="onBlur"
  >
    <el-option
      v-for="item in completeAddress"
      :key="item"
      :label="item"
      :value="item"
      :title="item"
    />
  </el-select>
</template>
<script lang="ts" setup>
import { onMounted, ref, watch } from "vue";
import { getCompleteAddressList } from "@/api/mail/index";
import { uniq } from "lodash-es";

export interface Props {
  inputValue: string[];
  disableQuery?: boolean;
}
const props = defineProps<Props>();
const emit = defineEmits<{
  (e: "update:modelValue", value: string[]): void;
}>();

// 对input进行处理，去掉 '<' 后面的空格并且如果 '<' 前面没有空格，则加一个空格。
// 去掉 '>' 的前后空格以及input的前后空格
const formatInput = (value: string) => {
  return value
    .trim()
    .replace(/<\s+/g, "<")
    .replace(/\s+>/g, ">")
    .replace(/([^<])</g, "$1 <");
};
const inputRef = ref();
const tempValue = ref("");
const completeAddress = ref<string[]>();
const inputQuery = ref("");
const focusInput = ref(true);
const inputOnBlur = (ref: { input: { focus: () => void } }) => {
  if (focusInput.value) {
    ref.input.focus();
  } else {
    focusInput.value = true;
  }
  inputRef.value.query = "";
  inputQuery.value = "";
};

onMounted(() => {
  inputRef.value.input.onclick = (e: MouseEvent) => e.stopPropagation();
});

const setInputQuery = (currentRef: any) => {
  inputQuery.value = currentRef.query;
};

const onKeyDown = (e: KeyboardEvent) => {
  if ([";", "Tab"].includes(e.key)) {
    e.preventDefault();
    tabEnter(e);
    inputRef.value?.input?.focus();
  }
};

const onPaste = (e: ClipboardEvent) => {
  const text = e.clipboardData?.getData("text");
  e.preventDefault();
  if (text) {
    const addresses = text.split(";");
    updateModelValue(addresses);
  }
};

const tabEnter = function (e: KeyboardEvent | FocusEvent) {
  if (inputQuery.value) {
    e.preventDefault();
    //如果输入输入框中已经存在的值，去掉已经存在的值（为了Tab键行为与ElementPlus多选框回车行为一致）
    if (!props.inputValue.includes(inputQuery.value)) {
      updateModelValue([tempValue.value]);
    } else {
      let index = props.inputValue.indexOf(inputQuery.value);
      props.inputValue.splice(index, 1);
    }
    (e.target as HTMLInputElement)?.focus();
  } else {
    focusInput.value = false;
  }
  inputQuery.value = "";
  inputRef.value.query = "";
};

watch(
  () => inputRef.value?.query,
  (n) => {
    if (n) {
      tempValue.value = n;
    } else {
      completeAddress.value = [];
    }
  }
);

const remoteMethod = async (query: string) => {
  if (query && !props.disableQuery) {
    getCompleteAddressList(query)
      .then((res) => (completeAddress.value = res))
      .catch(() => (completeAddress.value = []));
  } else {
    completeAddress.value = [];
  }
};

const updateModelValue = (values: string[]) => {
  const addresses = [...props.inputValue, ...values]
    .map((it) => it.trim())
    .filter((it) => it)
    .map(formatInput);
  emit("update:modelValue", uniq(addresses));
};

function onBlur(e: any) {
  if (!e?.target?.value) return;
  var currentValue = e.target.value;
  const currentLength = props.inputValue.length;
  setTimeout(() => {
    if (currentLength == props.inputValue.length) {
      e.target.value = currentValue;
      tabEnter(e);
    }
  }, 200);
}
</script>
<style>
.selectInput span.el-tag__content {
  height: 16px;
  line-height: 16px;
}
.selectInput span.el-input__suffix {
  display: none;
}
</style>
<style>
.el-select-popper
  .el-scrollbar
  .el-select-dropdown__list
  .el-select-dropdown__item.hover {
  height: auto;
  white-space: normal;
}
</style>
