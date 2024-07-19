<template>
  <template v-if="hoverItem && hoverStyle && hoverItem.id !== activeItem?.id">
    <div class="ve-select-holder hover" :style="hoverStyle.border">&nbsp;</div>
    <div class="ve-widget-controls hover" :style="hoverStyle.control">
      <div class="info">
        <ul>
          <li>
            <a href="javascript:;">
              {{ displayType(hoverItem) }}
            </a>
          </li>
        </ul>
      </div>
      <div class="actions" />
    </div>
  </template>
  <template v-if="activeItem && activeStyle">
    <div class="ve-select-holder active" :style="activeStyle.border">
      &nbsp;
    </div>
    <div class="ve-widget-controls active" :style="activeStyle.control">
      <div class="info">
        <ul>
          <li>
            <a href="javascript:;" class="current" @click.stop>
              {{ displayType(activeItem) }}
            </a>
          </li>
          <li v-for="it in activeItemTree" :key="it.id">
            <a href="javascript:;" @click.stop="onWidgetClick(it)">
              {{ t(it.type) }}
            </a>
          </li>
        </ul>
      </div>
      <div class="actions">
        <template v-if="activeItem.type !== 'column'">
          <button @click.stop="onCopy">
            {{ t("common.copy") }}
          </button>
          <button class="delete" @click.stop="onDelete">
            {{ t("common.delete") }}
          </button>
        </template>
      </div>
    </div>
  </template>
  <template v-if="dragToItem && dragToStyle">
    <div class="ve-select-holder drag" :style="dragToStyle">
      <div class="line">&nbsp;</div>
      <div class="tip">
        {{ t("ve.dragItHere") }}
      </div>
    </div>
  </template>
</template>

<script lang="ts" setup>
import type { Meta, InSelectWidgetContextType } from "../../types";
import { useInjectGlobalStore } from "./inject-global-store";
import { computed } from "vue";
import { postParentMessage } from "./message";
import { isCustomWidget, isRow, isColumn } from "../../utils/widget";

const {
  hoverItem,
  activeItem,
  activeRect,
  hoverRect,
  activeGroup,
  activeContext,
  activeItemTree,
  dragToItem,
  selectWidget,
  t,
} = useInjectGlobalStore();

const activeStyle = computed(() => {
  const rect = activeRect.value;
  if (!rect) {
    return;
  }
  const docRect = document.documentElement.getBoundingClientRect();
  const top = rect.top - docRect.top;
  const left = rect.left - docRect.left;
  const width = `${rect.width}px`; // 扣除border width
  const height = `${rect.height}px`;
  return {
    border: {
      height,
      width,
      top: `${top}px`,
      left: `${left}px`,
    },
    control: {
      width,
      top: `${top + rect.height}px`,
      left: `${left}px`,
    },
  };
});

const hoverStyle = computed(() => {
  const rect = hoverRect.value;
  if (!rect) {
    return;
  }
  const docRect = document.documentElement.getBoundingClientRect();
  const top = rect.top - docRect.top;
  const left = rect.left - docRect.left;
  const width = `${rect.width}px`; // 扣除border width
  const height = `${rect.height}px`;
  return {
    border: {
      height,
      width,
      top: `${top}px`,
      left: `${left}px`,
    },
    control: {
      width,
      top: `${top + rect.height}px`,
      left: `${left}px`,
    },
  };
});

const dragToStyle = computed(() => {
  if (!dragToItem.value) {
    return null;
  }

  const rect = document
    .querySelector(`[data-id="${dragToItem.value.id}"]`)
    ?.getBoundingClientRect();
  if (!rect) {
    return null;
  }

  const docRect = document.documentElement.getBoundingClientRect();
  const top = rect.top - docRect.top;
  const left = rect.left - docRect.left;
  return {
    width: `${rect.width}px`,
    top: `${top + rect.height}px`,
    left: `${left}px`,
  };
});

function onCopy() {
  const data: InSelectWidgetContextType = {
    meta: activeItem.value,
    context: activeContext.value,
    group: activeGroup.value,
  };
  postParentMessage(data, "copy", activeGroup.value);
}

function onDelete() {
  const data: InSelectWidgetContextType = {
    meta: activeItem.value,
    context: activeContext.value,
    group: activeGroup.value,
  };
  postParentMessage(data, "delete", activeGroup.value);
}

function onWidgetClick(item: Meta) {
  const el: HTMLElement | null = document.querySelector(
    `[data-id="${item.id}"]`
  );
  if (!el) {
    return;
  }
  if (isRow(item) || isColumn(item)) {
    selectWidget(item, el, activeGroup.value ?? "");
  } else {
    selectWidget(item, el, activeGroup.value ?? "", {
      id: item.id,
      rowId: activeContext.value?.rowId,
      columnId: activeContext.value?.columnId,
    });
  }
}

function displayType(item: Meta) {
  if (isCustomWidget(item)) {
    return `${t(item.type)}: ${item.name}`;
  }

  return t(item.type);
}
</script>
<style lang="scss" scoped>
.ve-select-holder {
  position: absolute;
  pointer-events: none;
  box-sizing: border-box;
  border-spacing: 0;
  font-size: 1px;
  &.active {
    border: 2px solid #4cb9ea;
  }
  &.hover {
    border: 2px solid #a5d4ef;
  }
  &.drag {
    display: flex;
    justify-content: center;
    flex-direction: column;
    align-items: center;
    .line {
      height: 5px;
      width: 100%;
      background-color: #4cb9ea;
    }
    .tip {
      position: absolute;
      left: 50%;
      top: 50%;
      transform: translate(-50%, -50%);
      color: #ffffff;
      border-radius: 20px;
      background-color: #4cb9ea;
      padding: 5px 20px;
      font-size: 12px;
    }
  }
}

.ve-widget-controls {
  display: flex;
  justify-content: space-between;
  width: auto;
  height: 30px;
  position: absolute;
  right: 0;
  font-size: 15px;
  line-height: 30px;
  text-align: right;
  margin-right: -2px;
  pointer-events: none;
  box-sizing: border-box;
  border-spacing: 0;

  &.active .info {
    background-color: #4cb9ea;
  }
  &.hover .info {
    background-color: #a5d4ef;
  }

  .info {
    color: #fff;
    padding-left: 4px;
    padding-right: 4px;
    border-bottom-left-radius: 4px;
    border-bottom-right-radius: 4px;
    ul {
      display: flex;
      flex-direction: row;
      list-style-type: none;
      padding: 0;
      margin: 0 10px 0 0;
      height: 20px;
      pointer-events: all;
      li {
        margin-left: 10px;
        &:not(:last-child) {
          &:after {
            content: "<";
            margin-left: 10px;
          }
        }
        a,
        a:hover {
          text-decoration: none;
          color: #ffffff;
          background-color: transparent;
          cursor: pointer;
          &.current {
            cursor: not-allowed;
            filter: opacity(0.7);
          }
        }
      }
    }
  }
  .actions {
    pointer-events: all;
  }
  button {
    color: #192845;
    font-weight: 400;
    box-shadow: 0px 2px 4px 0px rgba(0, 0, 0, 0.1);
    padding: 5px 11px;
    font-size: 12px;
    display: inline-flex;
    cursor: pointer;
    align-items: center;
    justify-content: center;
    text-align: center;
    outline: 0;
    border-radius: 7px;
    border: 1px solid rgba(0, 0, 0, 0);
    background-color: #fff;
    height: 24px;
    line-height: 12px;
  }
  .delete {
    margin-left: 10px;
  }
}
</style>
