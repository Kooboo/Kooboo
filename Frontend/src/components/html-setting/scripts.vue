<script lang="ts" setup>
import type { SortEvent, Resource } from "@/global/types";
import { computed, ref } from "vue";
import ScriptsDialog from "./scripts-dialog.vue";
import SortableList from "../sortable-list/index.vue";

const props = defineProps<{ list: Resource[]; hiddeBody?: boolean }>();

const emit = defineEmits<{
  (e: "sort", value: SortEvent, position: string): void;
  (e: "delete", value: Resource): void;
  (e: "insert", value: string, position: string): void;
}>();

const position = ref("head");

const groups = computed(() => {
  const result = [
    {
      name: "head",
      display: "Head",
      list: props.list.filter((f) => f.position === "head"),
    },
  ];
  if (!props.hiddeBody) {
    result.push({
      name: "body",
      display: "Body",
      list: props.list.filter((f) => f.position === "body"),
    });
  }
  return result;
});

const onDelete = (id: string) => {
  const found = props.list.find((f) => f.id === id);
  if (!found) return;
  emit("delete", found);
};

const showDialog = ref(false);
</script>

<template>
  <div class="-space-y-16">
    <template v-for="group of groups" :key="group.name">
      <SortableList
        :group="group.name"
        :list="group.list"
        id-prop="id"
        display-prop="content"
        :title="group.display"
        @sort="emit('sort', $event, group.name)"
        @delete="onDelete"
        @add="
          position = group.name;
          showDialog = true;
        "
      />
    </template>
  </div>

  <ScriptsDialog
    v-if="showDialog"
    v-model="showDialog"
    :excludes="list.map((m) => m.id)"
    @selected="emit('insert', $event, position)"
  />
</template>
