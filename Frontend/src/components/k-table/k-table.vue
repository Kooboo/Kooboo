<script lang="ts" setup>
/* eslint-disable @typescript-eslint/no-explicit-any */
import type { ElTable } from "element-plus";
import type { ColumnCls } from "element-plus/es/components/table/src/table/defaults";
import Sortable from "sortablejs";
import type { SortableEvent } from "sortablejs";
import { computed, onMounted, onUnmounted, ref, watch } from "vue";
import IconButton from "../basic/icon-button.vue";
import type { Pagination } from "./types";

import { useI18n } from "vue-i18n";
import { useSiteStore } from "@/store/site";
const props = defineProps<{
  data: any[] | undefined;
  showCheck?: boolean;
  customCheck?: boolean; // 自定义复选框
  selectedData?: any[]; // 已选中的数据
  isRadio?: boolean;
  pagination?: Pagination;
  rowClassName?: ColumnCls<any> | undefined;
  rowKey?: string;
  draggable?: boolean | string;
  hideDelete?: boolean;
  hideHeader?: boolean;
  sort?: string;
  order?: "ascending" | "descending" | undefined;
  spanMethod?: any;
  border?: boolean;
  permission?: {
    feature: string;
    action: string;
  };
  maxHeight?: number;
  isSpanArrTable?: boolean;
  isMultiTreeTable?: boolean;
}>();

const selected = ref<any[]>([]);
const siteStore = useSiteStore();
const multiTreeDataLength = ref(0);

// 获取树状表格的数据的长度
function getCount(array: any) {
  array.forEach((f: any) => {
    if (f.children && f.children.length) {
      getCount(f.children);
    } else {
      multiTreeDataLength.value += 1;
    }
  });
}

function initMultiTreeDataLength() {
  multiTreeDataLength.value = 0;
}
const tableDataMapLength = computed(() => {
  //当table用的是合并单元行表格时,根据绑定数据的id进行去重，返回合并单元行后的数组长度（用于合并单元行的表格展示正确全选状态）
  if (props.isSpanArrTable) {
    let map = new Map();
    return props.data?.filter(
      (item: any) => !map.has(item.id) && map.set(item.id, 1)
    ).length;
  } else if (props.isMultiTreeTable) {
    // 当表格类型是树状表格时，处理一下数据长度（用于树状表格展示正确全选状态）
    //初始化树状表格数据的长度
    initMultiTreeDataLength();
    getCount(props.data);
    return multiTreeDataLength.value;
  } else {
    return props.data?.length;
  }
});

const emit = defineEmits<{
  (event: "delete", selected: any): void;
  (event: "change", value: number): void;
  (event: "update:selectedData", value: any[]): void;
  (event: "rowClick", row: any): void;
  (event: "sorted", sortedData: any[], e: SortableEvent): void;
  (event: "sort-change", value: any): void;
}>();
const { t } = useI18n();

interface Tree {
  [key: string]: any;
  children?: Tree[];
}

function flat(arr: Tree[]) {
  let res: any[] = [];
  arr.forEach((item) => {
    if (item.children) {
      res.push(...flat(item.children));
    } else {
      res.push(item);
    }
  });
  return res;
}

const onCheckAll = (value: any) => {
  if (value) {
    selected.value = flat(props.data!).filter(
      (item) => item.$DisabledSelect !== true
    );
    //当table用的是合并单元行表格时,根据绑定数据的id进行去重（用于合并单元行的表格展示正确的全选数量）
    if (props.isSpanArrTable) {
      let map = new Map();
      selected.value = selected.value.filter(
        (item: any) => !map.has(item.id) && map.set(item.id, 1)
      );
    }
  } else {
    selected.value = [];
  }
};
const shiftFlag = ref(false);
const cancelSelect = ref(false);
const lastIdx = ref(-1);

const handleKeyDown = ({ key }: any) => {
  if (shiftFlag.value || key !== "Shift") return;
  shiftFlag.value = true;
};
const handleKeyUp = ({ key }: any) => {
  if (!shiftFlag.value || key !== "Shift") return;
  shiftFlag.value = false;
};

const onCheck = (row: any) => {
  if (!shiftFlag.value) {
    lastIdx.value = table.value?.store?.states?.data.value.findIndex(
      (f: any) => f === row
    );
  }
  if (lastIdx.value === -1) {
    lastIdx.value = table.value?.store?.states?.data.value.findIndex(
      (f: any) => f === row
    );
  }

  let [start, end] = [
    lastIdx.value,
    table.value?.store?.states?.data.value.findIndex((f: any) => f === row),
  ];
  if (start > end) [start, end] = [end, start];

  //按住shift时
  if (shiftFlag.value) {
    // cancelSelect为false的时候，shift做连续选中操作
    if (!cancelSelect.value) {
      selected.value.forEach((f) => {
        let index = table.value?.store?.states?.data.value.findIndex(
          (item: any) => item === f
        );
        if (index > end) {
          selected.value = selected.value.filter((item) => item !== f);
        }
      });
      for (let i = start; i <= end; i++) {
        const temp = table.value?.store?.states?.data.value[i];
        if (!selected.value.find((f) => f === temp) && !temp.$DisabledSelect) {
          selected.value.push(temp);
          cancelSelect.value = false;
        }
      }
    } else {
      // cancelSelect为true的时候，shift做连续取消选中操作
      for (let i = start; i <= end; i++) {
        const temp = table.value?.store?.states?.data.value[i];
        if (selected.value.find((f) => f === temp)) {
          selected.value = selected.value.filter((f) => f !== temp);
          cancelSelect.value = true;
        }
      }
    }
  } else {
    //未按住shift时
    if (selected.value.find((f) => f === row)) {
      selected.value = selected.value.filter((f) => f !== row);
      cancelSelect.value = true;
    } else {
      selected.value.push(row);
      cancelSelect.value = false;
    }
  }
};

const handelRowClick = (row: any) => {
  emit("rowClick", row);
};

watch(
  () => props.data,
  () => (selected.value = [])
);

watch(
  () => props.selectedData,
  (n) => {
    if (n) {
      selected.value = n;
    }
  },
  { deep: true }
);

watch(selected, (n) => {
  emit("update:selectedData", n);
});

const table = ref<InstanceType<typeof ElTable>>();
onMounted(() => {
  if (props.draggable) {
    setSortable();
  }

  document.addEventListener("keydown", handleKeyDown);
  document.addEventListener("keyup", handleKeyUp);
});

let sortable: Sortable | null = null;
onUnmounted(() => {
  if (sortable) {
    sortable.destroy();
    sortable = null;
  }
  document.removeEventListener("keydown", handleKeyDown);
  document.removeEventListener("keyup", handleKeyUp);
});

function setSortable() {
  if (!props.rowKey) {
    console.error("draggable table should set row-key");
  }

  const el = table.value?.$el.querySelector(".el-table__body > tbody");
  const options: Sortable.Options = {
    handle: ".js-sortable",
    setData(dataTransfer) {
      // to avoid Firefox bug
      // Detail see : https://github.com/RubaXa/Sortable/issues/1012
      dataTransfer.setData("Text", "");
    },
    onEnd(evt) {
      const targetRow = props.data?.splice(evt.oldIndex as number, 1)[0];
      props.data?.splice(evt.newIndex as number, 0, targetRow);
      emit("sorted", props.data ?? [], evt);
    },
  };
  if (typeof props.draggable === "string") {
    options.draggable = props.draggable;
  }
  if (sortable) {
    sortable.destroy();
    sortable = null;
  }
  sortable = Sortable.create(el, options);
}

const onDelete = (selected: unknown[]) => {
  if (
    props.permission &&
    !siteStore.hasAccess(props.permission.feature, props.permission.action)
  )
    return;
  emit("delete", selected);
};

watch(() => props.draggable, setSortable);

defineExpose({
  table,
  selected,
});
</script>

<template>
  <div
    class="shadow-s-10 rounded-normal overflow-hidden bg-fff dark:bg-[#303030]"
  >
    <div
      v-if="showCheck && !hideHeader"
      class="h-72px px-15px flex items-center bg-fff dark:bg-[#303030]"
      data-cy="table-check-all"
    >
      <el-checkbox
        v-if="!isRadio"
        :label="t('common.countSelected', { count: selected.length })"
        :model-value="
          !!selected.length && selected.length === tableDataMapLength
        "
        :indeterminate="
          !!selected.length && selected.length !== tableDataMapLength
        "
        size="large"
        class="w-30"
        @change="onCheckAll"
      />
      <slot name="leftBar" :selected="(selected as any)" />

      <div class="flex-1" />
      <slot name="bar" :selected="(selected as any)" />
      <IconButton
        v-if="selected.length && !Boolean(hideDelete)"
        :permission="permission"
        class="!text-orange"
        icon="icon-delete"
        :tip="t('common.delete')"
        circle
        data-cy="delete"
        @click="onDelete(selected)"
      />
      <slot name="rightBar" :selected="(selected as any)" />
    </div>
    <slot name="tableTopContent" />

    <el-table
      ref="table"
      :data="data"
      :class="
        pagination?.pageCount! > 1 ||
        pagination?.totalCount! > pagination?.pageSize!
          ? ''
          : 'mb-40px'
      "
      :row-class-name="rowClassName"
      :row-key="rowKey"
      :default-sort="
        sort && order
          ? {
              prop: sort,
              order: order,
            }
          : undefined
      "
      :span-method="spanMethod"
      :border="border ? true : false"
      :max-height="maxHeight"
      @row-click="handelRowClick"
      @sort-change="$emit('sort-change', $event)"
    >
      <!-- 
        如果props传入customCheck则不显示，让组件外层(selectedData)来控制selected数据
       -->
      <el-table-column
        v-if="showCheck && !customCheck"
        type="selection"
        :width="isRadio ? 40 : 50"
        :class-name="isRadio ? 'kradio' : ''"
      >
        <template #default="{ row }">
          <div
            v-if="(!row.children && rowKey) || !rowKey"
            class="flex items-center justify-center !w-full"
          >
            <el-checkbox
              v-if="!isRadio"
              size="large"
              class="el-table-checkbox"
              :disabled="row?.$DisabledSelect"
              :model-value="selected.some((f) => f === row)"
              data-cy="checkbox-label"
              @change="onCheck(row)"
            />
            <el-radio
              v-else
              :disabled="row?.$DisabledSelect"
              :model-value="selected[0]?.id"
              :label="row?.id"
              data-cy="radio-label"
              @change="() => (selected[0] = row)"
            />
          </div>
          <div v-else />
        </template>
      </el-table-column>
      <slot />
    </el-table>
    <slot name="defaultData" />
    <div v-if="pagination" class="text-center">
      <el-pagination
        class="py-8"
        layout="prev, pager, next"
        hide-on-single-page
        :page-size="pagination.pageSize"
        :page-count="pagination.pageCount"
        :current-page="pagination.currentPage"
        :total="pagination.totalCount"
        @current-change="emit('change', $event)"
      />
    </div>
  </div>
</template>
<style>
.kradio .el-radio__input.is-checked .el-radio__inner {
  border-color: #409eff;
  background: #fff;
}

.kradio .el-radio__inner::after {
  width: 8px;
  height: 8px;
  background-color: #409eff;
}

.kradio span.el-radio__inner {
  width: 16px;
  height: 16px;
}

.el-table__row > .kradio .cell {
  display: flex;
  align-items: center;
}

.kradio span.el-radio__label {
  display: none;
}
</style>
