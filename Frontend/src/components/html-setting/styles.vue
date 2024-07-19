<script lang="ts" setup>
import type { SortEvent, Resource } from "@/global/types";
import StylesDialog from "./styles-dialog.vue";
import { ref } from "vue";
import SortableList from "../sortable-list/index.vue";

const props = defineProps<{ list: Resource[] }>();

const emit = defineEmits<{
  (e: "sort", value: SortEvent): void;
  (e: "delete", value: Resource): void;
  (e: "insert", value: string): void;
}>();

const onDelete = (id: string) => {
  const found = props.list.find((f) => f.id === id);
  if (!found) return;
  emit("delete", found);
};

const showDialog = ref(false);
</script>

<template>
  <SortableList
    :list="list"
    id-prop="id"
    display-prop="content"
    @sort="emit('sort', $event)"
    @delete="onDelete"
    @add="showDialog = true"
  />
  <StylesDialog
    v-if="showDialog"
    v-model="showDialog"
    :excludes="list.map((m) => m.id)"
    @selected="emit('insert', $event)"
  />
</template>
