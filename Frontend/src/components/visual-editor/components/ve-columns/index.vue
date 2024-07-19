<template>
  <VueDraggable
    class="mx-8 text-center w-full h-full flex flex-wrap justify-start overflow-y-auto overflow-x-hidden items-start"
    :list="widgets"
    item-key="id"
    :group="{
      name: 'rows',
      pull: 'clone',
      put: false,
    }"
    :sort="false"
    :set-data="setData"
    @start="postInDragStart"
    @end="postInDragEnd"
  >
    <template
      #item="{
        element: {
          meta: { name, children },
        },
      }"
    >
      <div
        class="w-full mx-12 h-[80px] mt-3 dark:bg-444 bg-fff border border-999 flex flex-col justify-between items-center text-[#6b7280] text-[12px] font-bold hover:shadow-444 cursor-move"
        :title="name"
      >
        <div class="w-full h-full flex justify-between my-8">
          <div
            v-for="(column, i) in children"
            :key="i"
            class="mx-8 ve-column h-full text-center"
            :style="{ width: `${column.props['widthPercent']}%` }"
          >
            &nbsp;
          </div>
        </div>
        <div class="w-full text-center text-bold text-xs mb-12">
          {{ name }}
        </div>
      </div>
    </template>
  </VueDraggable>
</template>

<script lang="ts" setup>
import VueDraggable from "vuedraggable";
import { cloneDeep } from "lodash-es";
import type { VeWidgetType, VeDraggableElement } from "../../types";
import { useColumns } from "./effects";
import { postInDragEnd, postInDragStart } from "../../utils/message";

const { widgets } = useColumns();

function setData(e: DataTransfer, item: VeDraggableElement) {
  const context: VeWidgetType = cloneDeep(item.__draggable_context.element);
  context.meta!.id = undefined; // 用作区分来源于toolbox还是placeholder之间的拖动
  e.setData("application/json", JSON.stringify(context.meta));
}
</script>

<style lang="scss" scoped>
.ve-column {
  border: 2px dashed #b1b1b1;
  pointer-events: none;
}
</style>
