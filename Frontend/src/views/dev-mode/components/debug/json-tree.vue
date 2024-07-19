<template>
  <span class="json-tree" :class="{ 'json-tree-root': parsed.depth === 0 }">
    <span v-if="parsed.primitive" class="json-tree-row">
      <span v-for="n in parsed.depth * 2 + 3" :key="n" class="json-tree-indent"
        >&nbsp;</span
      >
      <span v-if="parsed.key" class="json-tree-key">{{ parsed.key }}</span>
      <span v-if="parsed.key" class="json-tree-colon">:&nbsp;</span>
      <span
        class="json-tree-value"
        :class="'json-tree-value-' + parsed.type"
        :title="`${parsed.value}`"
        >{{ `${parsed.value}` }}</span
      >
      <span v-if="!parsed.last" class="json-tree-comma">,</span>
      <span class="json-tree-indent">&nbsp;</span>
    </span>
    <span v-if="!parsed.primitive" class="json-tree-deep">
      <span
        class="json-tree-row json-tree-expando"
        @click="expanded = !expanded"
        @mouseover="hovered = true"
        @mouseout="hovered = false"
      >
        <span class="json-tree-indent">&nbsp;</span>
        <span class="json-tree-sign">{{ expanded ? "-" : "+" }}</span>
        <span
          v-for="n in parsed.depth * 2 + 1"
          :key="n"
          class="json-tree-indent"
          >&nbsp;</span
        >
        <span v-if="parsed.key" class="json-tree-key">{{ parsed.key }}</span>
        <span v-if="parsed.key" class="json-tree-colon">:&nbsp;</span>
        <span class="json-tree-open">{{
          parsed.type === "array" ? "[" : "{"
        }}</span>
        <span v-show="!expanded" class="json-tree-collapsed"
          >&nbsp;/*&nbsp;{{ format(parsed.value.length) }}&nbsp;*/&nbsp;</span
        >
        <span v-show="!expanded" class="json-tree-close">{{
          parsed.type === "array" ? "]" : "}"
        }}</span>
        <span v-show="!expanded && !parsed.last" class="json-tree-comma"
          >,</span
        >
        <span class="json-tree-indent">&nbsp;</span>
      </span>
      <span v-show="expanded" class="json-tree-deeper">
        <JsonTree
          v-for="(item, index) in parsed.value"
          :key="index"
          :kv="item"
          :level="level"
        />
      </span>
      <span v-show="expanded" class="json-tree-row">
        <span class="json-tree-ending" :class="{ 'json-tree-paired': hovered }">
          <span
            v-for="n in parsed.depth * 2 + 3"
            :key="n"
            class="json-tree-indent"
            >&nbsp;</span
          >
          <span class="json-tree-close">{{
            parsed.type === "array" ? "]" : "}"
          }}</span>
          <span v-if="!parsed.last" class="json-tree-comma">,</span>
          <span class="json-tree-indent">&nbsp;</span>
        </span>
      </span>
    </span>
  </span>
</template>

<script lang="ts" setup>
import { computed, ref } from "vue";

const parse = (data: any, depth = 0, last = true, key = undefined) => {
  let kv = { depth, last, primitive: true, key: JSON.stringify(key) };
  if (typeof data !== "object") {
    return Object.assign(kv, {
      type: typeof data,
      value: JSON.stringify(data),
    });
  } else if (data === null) {
    return Object.assign(kv, { type: "null", value: "null" });
  } else if (Array.isArray(data)) {
    let value: any = data.map((item, index) => {
      return parse(item, depth + 1, index === data.length - 1);
    });
    return Object.assign(kv, { primitive: false, type: "array", value });
  } else {
    let keys = Object.keys(data);
    let value: any = keys.map((key, index) => {
      return parse(data[key], depth + 1, index === keys.length - 1, key as any);
    });
    return Object.assign(kv, { primitive: false, type: "object", value });
  }
};

const props = defineProps<{
  level?: number;
  kv?: any;
  raw?: string;
  data?: any;
}>();

const expanded = ref(true);
const hovered = ref(false);

const parsed = computed(() => {
  if (props.kv) {
    return props.kv;
  }
  let result;
  try {
    if (props.raw) {
      result = JSON.parse(props.raw);
    } else if (typeof props.data !== "undefined") {
      result = props.data;
    } else {
      result = "[Vue JSON Tree] No data passed.";
      console.warn(result);
    }
  } catch (e) {
    result = "[Vue JSON Tree] Invalid raw JSON.";
    console.warn(result);
  }
  return parse(result);
});

const format = (n: number) => {
  if (n > 1) return `${n} items`;
  return n ? "1 item" : "no items";
};
</script>

<style>
.json-tree {
  color: #394359;
  display: flex;
  flex-direction: column;
  font-family: Menlo, Monaco, Consolas, monospace;
  font-size: 12px;
  line-height: 20px;
}

.json-tree-root {
  border-radius: 3px;
  margin: 2px 0;
  min-width: 560px;
}

.json-tree-ending,
.json-tree-row {
  display: flex;
}

.json-tree-paired,
.json-tree-row:hover {
  background-color: #bce2ff;
}

.json-tree-expando {
  cursor: pointer;
}

.json-tree-sign {
  font-weight: 700;
}

.json-tree-collapsed {
  color: gray;
  font-style: italic;
}

.json-tree-value {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.json-tree-value-string {
  color: #9aab3a;
}

.json-tree-value-boolean {
  color: #ff0080;
}

.json-tree-value-number {
  color: #4f7096;
}

.json-tree-value-null {
  color: #c7444a;
}
</style>
