<script lang="ts" setup>
import { computed, getCurrentInstance, ref, watch } from "vue";
import { inject, useSlots } from "vue";
import type { PageState } from "../k-page";
import { PAGE_STATE_KEY } from "../k-page";
import { getSlotContent } from "../utils";
import type { DataSource } from ".";
import Cookie from "universal-cookie";

interface Props {
  name: string;
  id?: string;
  from?: string;
  source?: DataSource;
  getOnChange?: boolean;
  postOnChange?: boolean;
}

const cookie = new Cookie();
const slots = useSlots();
const props = defineProps<Props>();
const pageState = inject<PageState>(PAGE_STATE_KEY);
let defaultValue = ref(getSlotContent(slots) ?? "");
const headers = getCurrentInstance()?.parent?.exposed?.headers;
const get = getCurrentInstance()?.parent?.exposed?.get;
const post = getCurrentInstance()?.parent?.exposed?.post;
const name = props.from ?? props.name;

if (headers && props.name) {
  const data = computed({
    get() {
      switch (props.source) {
        case "query":
          return new URLSearchParams(location.search).get(name);
        case "cookie":
          return cookie.get(name);
        case "local-storage":
          return localStorage.getItem(name) ?? "";
        case "session-storage":
          return sessionStorage.getItem(name) ?? "";
        case "id":
          return (pageState?.getState(name)?.value as any)?.toString();
        default:
          return defaultValue.value;
      }
    },
    set(value) {
      if (props.source == "id") {
        const state = pageState?.getState(name);
        if (state) state.value = value!;
      }
      defaultValue.value = value as any;
    },
  });

  headers.value[props.name] = data;
  pageState?.setState(props.id, data);

  if (props.getOnChange) {
    watch(
      () => data.value,
      () => {
        if (get) get();
      }
    );
  }

  if (props.postOnChange) {
    watch(
      () => data.value,
      () => {
        if (post) post();
      }
    );
  }
}
</script>

<template>
  <div class="hidden">
    <slot />
  </div>
</template>
