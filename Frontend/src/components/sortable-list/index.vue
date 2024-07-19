<script lang="ts" setup>
import type { SortEvent, SortChangeEvent } from "@/global/types";
import { newGuid } from "@/utils/guid";
import VueDraggable from "vuedraggable";
import { withDefaults, toRaw } from "vue";
import { cloneDeep } from "lodash-es";

interface Props {
  list: unknown[];
  idProp?: string;
  displayProp?: string;
  group?: string;
  title?: string;
  wrapperClass?: string;
  editable?: boolean;
  deleteTip?: (row: any) => string;
  disableActions?: {
    edit?: (row: any) => boolean;
    delete?: (row: any) => boolean;
  };
}

const props = withDefaults(defineProps<Props>(), {
  group: newGuid(),
  displayProp: undefined,
  title: undefined,
  wrapperClass: undefined,
  idProp: undefined,
  disableActions: undefined,
});

const emit = defineEmits<{
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  (
    e: "delete",
    id: string,
    value: any,
    context: { index: number; list: unknown[] }
  ): void;
  (e: "sort", value: SortEvent): void;
  (e: "add"): void;
  (
    e: "edit",
    id: string,
    value: any,
    context: { index: number; list: unknown[] }
  ): void;
  (e: "change", list: unknown[]): void;
}>();

function onChange(e: SortChangeEvent, list: unknown[]) {
  const clonedList = cloneDeep(toRaw(list));
  if (e.moved) {
    clonedList.splice(e.moved.oldIndex, 1);
    clonedList.splice(e.moved.newIndex, 0, e.moved.element);
  } else if (e.added) {
    clonedList.splice(e.added.newIndex, 0, e.added.element);
  } else if (e.removed) {
    clonedList.splice(e.removed.oldIndex, 1);
  }

  emit("change", clonedList);
}

function disableEdit(row: any) {
  const action = props.disableActions?.edit;
  return disableAction(row, action);
}
function disableDelete(row: any) {
  const action = props.disableActions?.delete;
  return disableAction(row, action);
}
function disableAction(row: any, action?: (row: any) => boolean) {
  if (!action) {
    return undefined;
  }
  if (typeof action === "function") {
    return action(row);
  }
  return action || undefined;
}

function onEdit(element: any, index: number) {
  if (disableEdit(element)) {
    return;
  }
  emit("edit", props.idProp ? element[props.idProp] : element, element, {
    index,
    list: props.list,
  });
}
function onDelete(element: any, index: number) {
  if (disableDelete(element)) {
    return;
  }
  emit("delete", props.idProp ? element[props.idProp] : element, element, {
    index,
    list: props.list,
  });
}
</script>

<template>
  <div :class="wrapperClass ?? 'space-y-4 px-24 py-16'">
    <h3 v-if="title" class="font-bold">{{ title }}</h3>
    <VueDraggable
      :model-value="list"
      :group="group"
      :disabled="list.length < 2"
      :item-key="idProp ?? ''"
      class="space-y-4"
      @sort="emit('sort', $event)"
      @change="onChange($event, list)"
    >
      <template #item="{ element, index }">
        <div
          class="text-666 dark:text-fff/67 flex items-center pl-16 pr-8 w-full border-line dark:border-666 rounded-normal border border-solid ellipsis bg-fff dark:bg-[#333]"
          data-cy="added-item"
          :class="{ 'cursor-move': list.length > 1 }"
        >
          <template v-if="list.length > 1">
            <div class="pr-8 w-32"># {{ index + 1 }}</div>
          </template>
          <div
            class="overflow-hidden leading-40px flex-1 mr-8 ellipsis"
            data-cy="text"
          >
            <slot :item="(element as any)" :index="index"
              ><span :title="element[displayProp || '']?.trim()" data-cy="name">
                {{ element[displayProp || ""]?.trim() }}</span
              >
            </slot>
          </div>
          <div>
            <slot name="right" :item="(element as any)" :index="index" />
          </div>
          <div v-if="editable" class="p-4">
            <el-icon
              class="iconfont icon-a-setup"
              :class="[
                disableEdit(element)
                  ? 'cursor-not-allowed text-opacity-50'
                  : 'cursor-pointer',
              ]"
              data-cy="edit"
              @click.stop="onEdit(element, index)"
            />
          </div>
          <div class="p-4">
            <el-tooltip
              placement="top-start"
              :content="deleteTip?.(element)"
              :disabled="!deleteTip?.(element)"
            >
              <el-icon
                class="text-orange iconfont icon-delete"
                :class="[
                  disableDelete(element)
                    ? 'cursor-not-allowed text-opacity-50'
                    : 'cursor-pointer',
                ]"
                color="#fab6b6"
                data-cy="remove"
                @click.stop="onDelete(element, index)"
              />
            </el-tooltip>
          </div>
          <template v-if="list.length > 1">
            <div class="p-4">
              <el-icon class="iconfont icon-move" data-cy="move" />
            </div>
          </template>
        </div>
      </template>
    </VueDraggable>
    <slot name="bottom">
      <el-button circle data-cy="add" @click="$emit('add')">
        <el-icon class="text-blue iconfont icon-a-addto" />
      </el-button>
    </slot>
  </div>
</template>
