<script lang="ts" setup>
import { updateQueryString } from "@/utils/url";
import {
  ref,
  shallowRef,
  inject,
  getCurrentInstance,
  computed,
  useSlots,
  nextTick,
  watch,
} from "vue";
import { request, typeMap } from ".";
import type { PageState } from "../k-page";
import { PAGE_AUTH_URL } from "../k-page";
import { PAGE_STATE_KEY } from "../k-page";
import type { DataSchema, DataStorage } from ".";
import {
  GetModuleUrl,
  getSlotContent,
  showConfirm,
  showLoading,
} from "../utils";

interface Props {
  id?: string;
  get?: string;
  post?: string;
  to?: string;
  initGet?: string;
  initPost?: string;
  from?: string;
  loading?: string;
  confirm?: string;
}

const pageState = inject<PageState>(PAGE_STATE_KEY);
const authUrl = inject<string>(PAGE_AUTH_URL);
const query = ref<Record<string, string>>({});
const storages = ref<DataStorage[]>([]);
const headers = ref<Record<string, string>>({});
const items = shallowRef<DataSchema[]>([]);
const props = defineProps<Props>();
const slots = useSlots();
const slotContent = getSlotContent(slots);
let rawResponse = ref(slotContent ? JSON.parse(slotContent) : undefined);

if (props.from) {
  let state = pageState?.getState(props.from, "mappedResponse");
  if (!state || !state.value) {
    state = pageState?.getState(props.from);
  }
  if (state) rawResponse = state;
}

const mappedResponse = ref(rawResponse.value);

pageState?.setState(props.id, getCurrentInstance());

function getSchemasData(schemas: DataSchema[]) {
  if (schemas.length == 1 && !schemas[0].name) {
    return schemas[0].data.value;
  } else if (schemas.length >= 1) {
    const obj: Record<string, any> = {};

    for (const i of schemas) {
      if (!i.name) continue;
      if (typeMap.get(i.type) == "object" && i.children) {
        if (i.children) {
          obj[i.name] = getSchemasData(i.children);
        }
      } else {
        obj[i.name] = i.data.value;
      }
    }
    return obj;
  }
}

const body = computed(() => getSchemasData(items.value));

function to() {
  if (!props.to) return;
  let url = props.to;
  if (Object.keys(query.value)) {
    url = updateQueryString(url, query.value);
    location.href = GetModuleUrl(url);
  }
}

async function get() {
  let url = props.initGet || props.get;
  if (url) {
    if (props.confirm) await showConfirm(props.confirm);

    await showLoading(async () => {
      try {
        rawResponse.value = await request(GetModuleUrl(url!), {
          query: query.value,
          headers: headers.value,
          authUrl: authUrl,
          storages: storages.value,
        });
      } catch (error) {
        console.error(error);
      }
    }, props.loading);
  }
}

async function post() {
  let url = props.initPost || props.post;
  if (url) {
    if (props.confirm) await showConfirm(props.confirm);

    await showLoading(async () => {
      try {
        rawResponse.value = await request(GetModuleUrl(url!), {
          method: "POST",
          query: query.value,
          body: body.value,
          authUrl: authUrl,
          headers: headers.value,
          storages: storages.value,
        });
      } catch (error) {
        console.error(error);
      }
    }, props.loading);
  }
}

function mappingObject(data: any, schemas: DataSchema[]) {
  const result: Record<string, any> = {};

  for (const item of schemas) {
    if (!item.name) continue;

    if (item.source == "code" && item.from) {
      item.data.value = (window as any)[item.from](data);
    } else {
      item.data.value = data[item.from ?? item.name];
    }

    result[item.name] = item.data.value;
  }

  return result;
}

function mappingData(data: any, schemas: DataSchema[]) {
  if (Array.isArray(data)) {
    const result: any[] = [];

    for (const item of data) {
      result.push(mappingData(item, schemas));
    }

    return result;
  } else if (typeof data == "object") {
    return mappingObject(data, schemas);
  }

  return data;
}

watch(
  () => rawResponse.value,
  () => {
    if (!items.value.length) {
      mappedResponse.value = rawResponse.value;
    } else {
      mappedResponse.value = mappingData(rawResponse.value, items.value);
    }
  },
  {
    immediate: true,
    deep: true,
  }
);

nextTick(() => {
  if (props.initGet) get();
  if (props.initPost) post();
});

defineExpose({
  headers,
  storages,
  query,
  items,
  rawResponse,
  mappedResponse,
  get: props.initGet || props.get ? get : undefined,
  post: props.initPost || props.post ? post : undefined,
  to: props.to ? to : undefined,
});
</script>

<template>
  <div class="hidden">
    <slot />
  </div>
</template>
