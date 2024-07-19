<template>
  <el-form label-position="top" @submit.prevent>
    <template v-for="(item, index) in columns" :key="index">
      <el-form-item v-if="!item.isSystem" :label="item.name">
        <p v-if="item.isIncremental">
          {{ item.value || "Empty" }}
          <span class="text-999 text-s">{{
            "(" + t("common.unableChangeSelfIncrementalField") + ")"
          }}</span>
        </p>
        <template v-else>
          <el-input
            v-if="item.controlType === 'TextBox'"
            v-model="item.value"
            style="width: 100%"
          />
          <el-input
            v-if="item.controlType === 'TextArea'"
            v-model="item.value"
            type="textarea"
            :autosize="{ minRows: 8, maxRows: 15 }"
            style="width: 100%"
          />
          <KEditor v-if="item.controlType === 'Tinymce'" v-model="item.value" />
          <el-select
            v-if="item.controlType === 'Selection'"
            v-model="item.value"
            clearable
          >
            <el-option
              v-for="opt in item.options"
              :key="opt.value"
              :value="opt.value"
              :label="opt.key"
            />
          </el-select>
          <el-checkbox-group
            v-if="item.controlType === 'CheckBox'"
            v-model="item.value"
          >
            <el-checkbox
              v-for="opt in item.options"
              :key="opt.value"
              size="large"
              :label="opt.value"
              >{{ opt.key }}</el-checkbox
            >
          </el-checkbox-group>

          <el-radio-group
            v-if="item.controlType === 'RadioBox'"
            v-model="item.value"
          >
            <el-radio
              v-for="opt in item.options"
              :key="opt.value"
              size="large"
              :label="opt.value"
              >{{ opt.key }}</el-radio
            >
          </el-radio-group>
          <el-switch
            v-if="item.controlType === 'Boolean'"
            v-model="item.value"
          />
          <el-date-picker
            v-if="item.controlType === 'DateTime'"
            v-model="item.value"
            type="datetime"
            value-format="YYYY-MM-DD HH:mm:ss"
          />
          <el-input-number
            v-if="item.controlType === 'Number'"
            v-model="item.value"
            controls-position="right"
          />
        </template>
      </el-form-item>
    </template>
  </el-form>
</template>

<script lang="ts" setup>
import type {
  ColumnSetting,
  DatabaseColumn as DatabaseColumnAPI,
  DatabaseType,
} from "@/api/database";
import { getDatabaseEdit, updateTableData } from "@/api/database";
import type { KeyValue } from "@/global/types";
import { onUnmounted, ref, watch } from "vue";
import KEditor from "@/components/k-editor/index.vue";
import { ElMessage } from "element-plus";

import { useI18n } from "vue-i18n";
import { onBeforeRouteLeave } from "vue-router";
import { useSiteStore } from "@/store/site";
import { useSaveTip } from "@/hooks/use-save-tip";

const siteStore = useSiteStore();
const saveTip = useSaveTip();
const props = defineProps<{
  dbType: DatabaseType;
  table: string;
  id: string;
}>();
const { t } = useI18n();

type DatabaseColumn = DatabaseColumnAPI & {
  options?: KeyValue[];
};

const columns = ref<DatabaseColumn[]>([]);

const load = async () => {
  const response = await getDatabaseEdit(props.dbType, {
    tableName: props.table,
    id: props.id,
  });

  columns.value = response.map((data) => {
    if (data.controlType?.toLowerCase() === "checkbox") {
      let value = null;
      if (Array.isArray(data.value)) {
        value = data.value;
      } else {
        try {
          if (data.value) {
            value = JSON.parse(data.value);
          }
        } catch (ex) {
          value = [];
        }
      }
      data.value = value || [];
    } else if (data.controlType?.toLowerCase() === "number") {
      if (data.value === null) {
        data.value = undefined;
      }
    } else if (data.controlType?.toLowerCase() === "datetime") {
      if (data.value && data.value.includes(".")) {
        data.value = data.value.split(".")[0];
      }
    } else if (data.controlType?.toLowerCase() === "boolean") {
      if (data.value === null) {
        data.value = false;
      }
    }
    if (data.setting) {
      const setting = JSON.parse(data.setting as string) as ColumnSetting;
      if (setting && setting.options && setting.options.length) {
        data.options = setting.options;
        // if (
        //   data.options &&
        //   data.options.length &&
        //   data.controlType?.toLowerCase() === "selection" &&
        //   !data.value
        // ) {
        //   data.value = data.options[0].value;
        // }
      }
    }
    return data;
  });
  saveTip.init(columns.value);
};

load();

// 组件销毁时重置firstActiveMenu的值，防止影响到activeName外面的行为
onUnmounted(() => {
  siteStore.firstActiveMenu = "";
});

onBeforeRouteLeave(async (to, from, next) => {
  if (to.name === "login") {
    next();
  } else {
    siteStore.firstActiveMenu = to.meta.activeMenu ?? to.name;
    await saveTip
      .check(columns.value)
      .then(() => {
        next();
      })
      .catch(() => {
        // 当前路由的query的dbType不存在时，firstActiveMenu默认是table
        siteStore.firstActiveMenu = from.query.dbType
          ? (from.query.dbType as string).toLocaleLowerCase() + ".table"
          : "table";
        next(false);
      });
  }
});

function validate() {
  const primaryKey = columns.value.find(
    (item) =>
      item.isPrimaryKey &&
      !item.isSystem &&
      (item.value === null || item.value === "")
  );
  if (primaryKey) {
    ElMessage.error(t("common.fieldRequiredTips2", { field: primaryKey.name }));
    return false;
  }
  return true;
}

async function save() {
  if (!validate()) return;

  const values = columns.value.map((col) => {
    let value = col.value;
    if (Array.isArray(col.value)) {
      value = JSON.stringify(col.value);
    }
    if (col.value?.value) {
      value = col.value.value;
    }
    return {
      name: col.name,
      value: value === undefined ? null : value,
      rawValue: JSON.stringify(value ?? null),
    };
  });

  await updateTableData(props.dbType, {
    tableName: props.table,
    id: props.id,
    values: values,
  });
  saveTip.init(columns.value);
}

defineExpose({ save });
watch(
  () => columns.value,
  () => {
    saveTip.changed(columns.value);
  },
  {
    deep: true,
  }
);
</script>

<style scoped lang="scss">
:deep(.el-input),
:deep(.el-textarea),
:deep(.el-select) {
  width: 504px;
}
</style>
