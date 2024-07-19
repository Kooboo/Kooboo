<template>
  <span class="highlight">
    <template v-for="(item, index) in list" :key="index">
      <span>{{ item }}</span>
      <!-- 利用split做分割 数组中间显示高亮的keyword -->
      <span v-if="index + 1 !== list?.length" class="active">{{
        keywordList[index]
      }}</span>
    </template>
  </span>
</template>

<script lang="ts" setup>
import { ref, withDefaults, watch } from "vue";

export interface Props {
  light?: {
    color: string;
    background: string;
  };
  dark?: {
    color: string;
    background: string;
  };
  keyword: string;
  text: string;
  maxLength?: number; // 第一个关键词前面的最大字符限制; pattern: first 模式有效
  pattern?: "first" | "all"; // 模式：first 首个高亮 | all 全部高亮,
  start?: number;
  end?: number;
}

const props = withDefaults(defineProps<Props>(), {
  light: () => ({
    background: "#f0c1a3",
    color: "#616161",
  }),
  dark: () => ({
    background: "#595961",
    color: "#f8f8f2",
  }),
  maxLength: 16,
  pattern: "all",
});

const list = ref<string[]>([]);

// keyword不限制大小写，所有需要一组对应的值
const keywordList = ref<string[]>([]);

// 把搜索结果的字符串中的关键词字符串分割开来，用于关键词高亮
function splitResultAroundKeyword(text: string, keyword: string) {
  var pattern = new RegExp(keyword, "i");
  var index = text.search(pattern);
  var result = [];

  if (index !== -1) {
    result.push(text.substring(0, index));
    result.push(text.substring(index + keyword.length));
  } else {
    result.push(text);
  }

  return result;
}

// 高亮的字符串
function findHighlightText(text: string, keyword: string) {
  var pattern = new RegExp(keyword, "i");
  var match = text.match(pattern);

  if (match) {
    return [match[0]];
  } else {
    return [];
  }
}

watch(() => props, load, { immediate: true, deep: true });

function load() {
  if (!(props.text && props.keyword)) {
    return;
  }

  if (props.start !== undefined) {
    list.value = splitResultAroundKeyword(props.text, props.keyword);
    keywordList.value = findHighlightText(props.text, props.keyword);

    return;
  }

  if (props.pattern === "first") {
    // 找第一个
    const matchResult = props.text.match(new RegExp(props.keyword, "i"));
    // 没找到
    if (matchResult === null) {
      list.value = [props.text];
      return;
    }

    keywordList.value = [matchResult[0]];

    const front = props.text.slice(0, matchResult.index!);
    const back = props.text.slice(matchResult.index! + props.keyword.length);

    // 关键字开头
    if (matchResult.index === 0) {
      list.value = ["", back];
      return;
    }

    let usableLength = props.maxLength - props.keyword.length;
    if (usableLength < 0) {
      list.value = ["...", back];
      return;
    }

    // 前面文本大于剩余空间时
    if (matchResult.index! <= usableLength) {
      list.value = [front, back];
    } else {
      list.value = ["..." + front.slice(-usableLength).slice(3), back];
    }
  } else if (props.pattern === "all") {
    list.value = props.text.split(new RegExp(props.keyword, "i"));
    keywordList.value = props.text.match(
      new RegExp(props.keyword, "ig")
    ) as string[];
  }
}
</script>

<style>
.dark .highlight .active {
  background: v-bind("props.dark.background");
  color: v-bind("props.dark.color");
}

.highlight .active {
  background: v-bind("props.light.background");
  color: v-bind("props.light.color");
}
</style>
