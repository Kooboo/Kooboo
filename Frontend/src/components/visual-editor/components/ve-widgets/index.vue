<template>
  <VueDraggable
    v-if="items?.length"
    item-key="id"
    :list="items"
    class="ml-8 text-center w-full h-full flex flex-wrap justify-start overflow-y-auto overflow-x-hidden items-start"
    :group="{
      name: 'component',
      pull: 'clone',
      put: false,
    }"
    :sort="false"
    :set-data="setData"
    @start="postInDragStart"
    @end="postInDragEnd"
  >
    <template #item="{ element: { name, icon, tooltip, id } }">
      <VeItem :data-type="id" :name="name" :icon="icon" :tooltip="tooltip" />
    </template>
  </VueDraggable>
</template>

<script lang="ts" setup>
import VueDraggable from "vuedraggable";
import VeItem from "./components/ve-item.vue";

import type { VeDraggableElement, VeWidgetType } from "../../types";
import { cloneDeep } from "lodash-es";
import { postInDragEnd, postInDragStart } from "../../utils/message";

defineProps<{
  items: VeWidgetType[];
}>();

function setData(e: DataTransfer, item: VeDraggableElement) {
  const context: VeWidgetType = cloneDeep(item.__draggable_context.element);
  context.meta!.id = undefined;
  e.setData("application/json", JSON.stringify(context.meta));
}
</script>
