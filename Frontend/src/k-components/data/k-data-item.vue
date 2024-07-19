<script lang="ts" setup>
import {
  inject,
  useSlots,
  computed,
  ref,
  shallowRef,
  getCurrentInstance,
} from "vue";
import { getDefaultValue } from ".";
import type { PageState } from "../k-page";
import { PAGE_STATE_KEY } from "../k-page";
import type { DataSchema, DataType, DataSource } from ".";
import { getSlotContent } from "../utils";
import Cookie from "universal-cookie";

interface Props {
  name: string;
  type?: DataType;
  id?: string;
  hidden?: boolean;
  label?: string;
  from?: string;
  options?: string;
  width?: string;
  source?: DataSource;
  disabled?: boolean;
}

const cookie = new Cookie();
const props = defineProps<Props>();
const slots = useSlots();
const items = shallowRef<DataSchema[]>([]);

const realType = computed(() => {
  if (props.type) return props.type;
  return items.value.length ? "object" : "string";
});

const pageState = inject<PageState>(PAGE_STATE_KEY);
const content = getSlotContent(slots);
const defaultValue = ref<any>(getDefaultValue(realType.value, content));
const name = props.from ?? props.name;
const sourceData = pageState?.getState(name);

const data = computed({
  get() {
    if (realType.value == "object") {
      const result: Record<string, any> = {};
      for (const i of items.value) {
        if (!i.name) continue;
        result[i.name] = i.data;
      }
      return result;
    } else {
      switch (props.source) {
        case "query":
          return getDefaultValue(
            realType.value,
            new URLSearchParams(location.search).get(name)!
          );
        case "cookie":
          return getDefaultValue(realType.value, cookie.get(name));
        case "local-storage":
          return getDefaultValue(realType.value, localStorage.getItem(name)!);
        case "session-storage":
          return getDefaultValue(realType.value, sessionStorage.getItem(name)!);
        case "id":
          return pageState?.getState(name)?.value;
        default:
          return defaultValue.value;
      }
    }
  },
  set(value) {
    if (props.source == "id") {
      if (sourceData) sourceData.value = value!;
    }
    defaultValue.value = value;
  },
});

const schema = computed(() => {
  let width = props.width;

  if (width && !Number.isNaN(parseFloat(width))) {
    width = width + "px";
  }

  return {
    type: realType.value,
    name: props.name,
    data: data,
    children: items.value,
    hidden: props.hidden,
    label: props.label,
    from: props.from,
    options: props.options,
    width: width,
    disabled: props.disabled,
    source: props.source,
  };
});

if (props.id) {
  pageState?.setState(props.id, data);
}

const parent = getCurrentInstance()?.parent;

if (parent?.exposed?.items) {
  parent.exposed.items.value.push(schema.value);
}

defineExpose({ items: items });
</script>

<template>
  <div class="hidden">
    <slot />
  </div>
</template>
