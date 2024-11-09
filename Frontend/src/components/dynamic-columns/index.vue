<template>
  <el-table-column
    v-for="item in columns"
    :key="item.name"
    :prop="item.name"
    :label="item.displayName || item.name"
    v-bind="item.attrs"
    :align="item.attrs?.align || 'left'"
  >
    <template #default="{ row, $index }">
      <slot :name="item.name" :row="row" :$index="$index">
        <template
          v-if="
            isControl(item, 'MediaFile') || isControl(item, 'AdvancedMediaFile')
          "
        >
          <div class="flex space-x-4">
            <el-scrollbar v-if="item.multipleValue">
              <div class="flex">
                <ImageCover
                  v-for="it in formatData(row, item)"
                  :key="it"
                  :model-value="getSrc(it, item)"
                  class="mr-4"
                />
              </div>
            </el-scrollbar>
            <ImageCover
              v-else
              :model-value="getSrc(getValue(row, item), item)"
            />
          </div>
        </template>
        <span
          v-else-if="isControl(item, 'Switch')"
          :class="isTrueValue(row, item) ? 'text-green' : 'text-999'"
          data-cy="online"
        >
          {{ isTrueValue(row, item) ? t("common.yes") : t("common.no") }}
        </span>
        <ColorPicker
          v-else-if="isControl(item, 'ColorPicker')"
          :model-value="getValue(row, item)"
          show-alpha
          :hide-picker="true"
          disabled
        />
        <div v-else-if="isControl(item, 'KeyValues')">
          <ul class="flex flex-wrap gap-4">
            <li
              v-for="itm in formatData(row, item)"
              :key="itm.key"
              class="text-12px leading-20px flex"
            >
              <span
                :title="itm.key"
                class="h-5 px-7px bg-blue text-fff rounded-l-full"
              >
                {{ itm.key }}
              </span>
              <span
                :title="itm.value"
                class="h-5 px-7px rounded-r-full border-1 border-blue text-blue"
              >
                {{ itm.value }}
              </span>
            </li>
          </ul>
        </div>
        <div v-else class="ellipsis">
          {{ formatData(row, item).toString() }}
        </div>
      </slot>
    </template>
  </el-table-column>
</template>

<script lang="ts" setup>
import ImageCover from "@/components/basic/image-cover.vue";
import { useI18n } from "vue-i18n";
import { getValueIgnoreCase, ignoreCaseEqual } from "@/utils/string";
import { useTime, useDate } from "@/hooks/use-date";
export type SummaryColumn = {
  name: string;
  prop?: string;
  displayName?: string;
  controlType: string;
  multipleValue?: boolean;
  attrs?: Record<string, any>;
};

defineProps<{
  columns: SummaryColumn[];
}>();

const { t } = useI18n();

function isControl(column: SummaryColumn, control: string) {
  return ignoreCaseEqual(column.controlType, control);
}

function getValue(row: any, column: SummaryColumn) {
  const value = getValueIgnoreCase(row, column.prop ?? column.name) ?? "";
  if (ignoreCaseEqual(column.controlType, "DateTime")) {
    if (!value) {
      return value;
    }
    return useTime(value);
  }
  if (ignoreCaseEqual(column.controlType, "Date")) {
    if (!value) {
      return value;
    }
    return useDate(value);
  }
  return value;
}

function isTrueValue(row: any, column: SummaryColumn) {
  return ignoreCaseEqual(getValue(row, column), "true");
}

function formatData(row: any, column: SummaryColumn) {
  const value = getValue(row, column);
  if (isControl(column, "KeyValues")) {
    if (typeof value === "string" && value.trim() !== "") {
      try {
        return JSON.parse(value) || [];
      } catch (e) {
        console.warn(["parse json error", e, value]);
        return value;
      }
    }
    return value;
  }
  if (!column.multipleValue) {
    return value;
  }
  if (!value) {
    return [];
  }
  if (typeof value === "string") {
    try {
      return JSON.parse(value);
    } catch (e) {
      console.warn(["parse json error", e, value]);
      return value;
    }
  }
  return value;
}

function getSrc(value: any, column: SummaryColumn) {
  if (isControl(column, "AdvancedMediaFile")) {
    try {
      return JSON.parse(value).src;
    } catch (error) {
      return value;
    }
  }

  return value;
}
</script>
