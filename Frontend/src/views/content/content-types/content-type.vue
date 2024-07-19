<template>
  <div class="p-24 pb-150px">
    <div
      class="text-2l font-bold dark:text-fff/86"
      :class="{ 'mb-12px': !isNewContentType }"
    >
      <el-form
        v-if="isNewContentType"
        ref="form"
        :model="model"
        :rules="rules"
        class="font-normal inline-flex"
        @submit.prevent
      >
        <el-form-item
          prop="name"
          class="mb-12"
          :label="t('common.contentType')"
        >
          <el-input
            v-model="model.name"
            class="w-300px"
            data-cy="content-type-name"
          />
        </el-form-item>
      </el-form>
      <template v-else
        ><span data-cy="content-type-name">
          {{ t("common.contentType") }}: {{ model.name }}
        </span></template
      >
    </div>
    <div class="flex items-center py-24 space-x-16">
      <el-button
        v-hasPermission="{
          feature: 'contentType',
          action: 'edit',
        }"
        round
        data-cy="new-field"
        @click="onAdd"
      >
        <el-icon class="iconfont icon-a-addto" />
        {{ t("common.newField") }}
      </el-button>
    </div>
    <KTable
      ref="table"
      :data="properties.slice(0, properties.length - 3)"
      :row-class-name="
        ({ row, rowIndex }) => (row.isSystemField ? '' : 'draggable')
      "
      draggable=".draggable"
      row-key="name"
      @sorted="updateTableData($event)"
    >
      <template #defaultData>
        <el-tooltip
          placement="top"
          :content="
            !defaultDataTypeDisabled
              ? t('common.clickToShowSystemField')
              : t('common.clickToHideSystemField')
          "
        >
          <div
            class="flex items-center justify-center -mt-40px h-40px cursor-pointer hover:bg-[#eff6ff] dark:hover:bg-444"
            data-cy="show-system-field"
            @click="defaultDataTypeDisabled = !defaultDataTypeDisabled"
          >
            <el-icon
              class="iconfont icon-pull-down text-s leading-none cursor-pointer transform origin-center transition duration-200 dark:text-fff/86"
              :class="defaultDataTypeDisabled ? 'rotate-180' : 'rotate-0'"
            />
          </div>
        </el-tooltip>

        <el-table
          v-if="defaultDataTypeDisabled"
          :data="properties.slice(properties.length - 3, properties.length)"
          :show-header="false"
        >
          <el-table-column :label="t('common.name')">
            <template #default="{ row }">
              <span
                :class="{ 'text-999': row.isSystemField }"
                data-cy="field-name"
                >{{ row.name }}</span
              >
            </template>
          </el-table-column>
          <el-table-column :label="t('common.displayName')"
            ><template #default="{ row }">
              <span data-cy="display-name">{{ row.displayName }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="t('common.controlType')" width="200">
            <template #default="{ row }">
              <span data-cy="control-type">{{
                getControlType(row.controlType)?.displayName
              }}</span>
            </template>
          </el-table-column>

          <el-table-column
            :label="t('common.multipleLanguage')"
            align="center"
            width="160"
          >
            <template #default="{ row }">
              <span
                :class="row.multipleLanguage ? 'text-green' : 'text-999'"
                data-cy="multiple-language"
              >
                {{ row.multipleLanguage ? t("common.yes") : t("common.no") }}
              </span>
            </template>
          </el-table-column>

          <el-table-column
            :label="t('common.summaryField')"
            width="150"
            align="center"
          >
            <template #default="{ row }">
              <span
                :class="row.isSummaryField ? 'text-green' : 'text-999'"
                data-cy="summary-field"
              >
                {{ row.isSummaryField ? t("common.yes") : t("common.no") }}
              </span>
            </template>
          </el-table-column>

          <el-table-column
            :label="t('common.userEditable')"
            align="center"
            width="150"
          >
            <template #default="{ row }">
              <span
                :class="row.editable ? 'text-green' : 'text-999'"
                data-cy="user-editable"
              >
                {{ row.editable ? t("common.yes") : t("common.no") }}
              </span>
            </template>
          </el-table-column>
          <el-table-column width="122" align="right">
            <template #default="{ row, $index }">
              <IconButton
                icon="icon-a-writein"
                :tip="t('common.edit')"
                data-cy="edit"
                @click="onEdit(row)"
              />
              <IconButton
                v-if="!row.isSystemField"
                icon="icon-delete "
                class="text-orange hover:text-orange"
                :tip="t('common.delete')"
                data-cy="remove"
                @click="removeItem($index)"
              />
            </template>
          </el-table-column>
        </el-table>
      </template>
      <el-table-column :label="t('common.name')">
        <template #default="{ row }">
          <span
            :class="{ 'text-999': row.isSystemField }"
            data-cy="field-name"
            >{{ row.name }}</span
          >
        </template>
      </el-table-column>
      <el-table-column :label="t('common.displayName')"
        ><template #default="{ row }">
          <span data-cy="display-name">{{ row.displayName }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.controlType')" width="200">
        <template #default="{ row }">
          <span data-cy="control-type">{{
            getControlType(row.controlType)?.displayName
          }}</span>
        </template>
      </el-table-column>
      <el-table-column
        :label="t('common.multipleLanguage')"
        width="160"
        align="center"
      >
        <template #default="{ row }">
          <span
            :class="row.multipleLanguage ? 'text-green' : 'text-999'"
            data-cy="multiple-language"
          >
            {{ row.multipleLanguage ? t("common.yes") : t("common.no") }}
          </span>
        </template>
      </el-table-column>
      <el-table-column
        :label="t('common.summaryField')"
        width="150"
        align="center"
      >
        <template #default="{ row }">
          <span
            :class="row.isSummaryField ? 'text-green' : 'text-999'"
            data-cy="summary-field"
          >
            {{ row.isSummaryField ? t("common.yes") : t("common.no") }}
          </span>
        </template>
      </el-table-column>
      <el-table-column
        :label="t('common.userEditable')"
        width="150"
        align="center"
      >
        <template #default="{ row }">
          <span
            :class="row.editable ? 'text-green' : 'text-999'"
            data-cy="user-editable"
          >
            {{ row.editable ? t("common.yes") : t("common.no") }}
          </span>
        </template>
      </el-table-column>
      <el-table-column width="120" align="right">
        <template #default="{ row, $index }">
          <IconButton
            icon="icon-a-writein"
            :tip="t('common.edit')"
            data-cy="edit"
            @click="onEdit(row)"
          />
          <IconButton
            v-if="!row.isSystemField"
            :permission="{
              feature: 'contentType',
              action: 'edit',
            }"
            icon="icon-delete text-orange hover:text-orange"
            :tip="t('common.delete')"
            data-cy="remove"
            @click="removeItem($index)"
          />
          <IconButton
            icon="icon-move js-sortable cursor-move"
            :tip="t('common.move')"
            data-cy="move"
          />
        </template>
      </el-table-column>
    </KTable>
    <KBottomBar
      :permission="{
        feature: 'contentType',
        action: 'edit',
      }"
      @cancel="goBack"
      @save="onSave"
    />
    <EditFieldDialog
      v-if="editFieldDialog"
      v-model="editFieldDialog"
      :fields="properties"
      :field="editingField"
      :enable-dynamic-options="true"
    />
  </div>
</template>

<script lang="ts" setup>
import type {
  JsonStringField,
  Property,
  PropertyJsonString,
} from "@/global/control-type";
import { getContentType, save, isUniqueName } from "@/api/content/content-type";

import KTable from "@/components/k-table";
import { emptyGuid } from "@/utils/guid";
import { onMounted, onUnmounted, reactive, ref, watch } from "vue";
import KBottomBar from "@/components/k-bottom-bar/index.vue";
import { useRouteSiteId } from "@/hooks/use-site-id";
import type { ElForm } from "element-plus";
import { useControlTypes } from "@/hooks/use-control-types";
import EditFieldDialog from "@/components/fields-editor/edit-field-dialog.vue";
import { cloneDeep } from "lodash-es";
import { getQueryString } from "@/utils/url";

import {
  isUniqueNameRule,
  rangeRule,
  requiredRule,
  letterAndDigitStartRule,
} from "@/utils/validate";
import type { Rules } from "async-validator";

import { useI18n } from "vue-i18n";
import { onBeforeRouteLeave, useRouter } from "vue-router";
import { useSaveTip } from "@/hooks/use-save-tip";
import { useSiteStore } from "@/store/site";
const { t } = useI18n();
const router = useRouter();
const siteStore = useSiteStore();
const saveTip = useSaveTip();
const { getControlType } = useControlTypes();
const model = reactive({
  name: "",
});

const defaultDataTypeDisabled = ref(false);
const form = ref<InstanceType<typeof ElForm>>();
const rules = {
  name: [
    requiredRule(t("common.fieldRequiredTips")),
    rangeRule(1, 50),
    letterAndDigitStartRule(),
    isUniqueNameRule(isUniqueName, t("common.datatypeExistsTips")),
  ],
} as Rules;
const contentTypeId = (getQueryString("id") as string) || emptyGuid;
const isNewContentType = contentTypeId === emptyGuid;
const properties = ref<Property[]>([]);
const editFieldDialog = ref(false);
const editingField = ref<Property | undefined>(undefined);

onMounted(() => {
  getContentTypeData();
});

onBeforeRouteLeave(async (to, from, next) => {
  if (to.name === "login") {
    next();
  } else {
    siteStore.firstActiveMenu = to.meta.activeMenu ?? to.name;
    await saveTip
      .check([properties.value, model])
      .then(() => {
        next();
      })
      .catch(() => {
        siteStore.firstActiveMenu = "contenttypes";
        next(false);
      });
  }
});

// 组件销毁时重置firstActiveMenu的值，防止影响到activeName外面的行为
onUnmounted(() => {
  siteStore.firstActiveMenu = "";
});

const updateTableData = (value: Property[]) => {
  const sortedMaps: Record<string, Property> = {};
  value.forEach((it: Property, index: number) => {
    it.order = index;
    sortedMaps[it.name] = it;
  });
  properties.value = sortProperties(properties.value).map((it: Property) => {
    return sortedMaps[it.name] || it;
  });
};
async function getContentTypeData() {
  const response = await getContentType({ id: contentTypeId });
  model.name = response.name;
  if (model.name === null) {
    model.name = "";
  }
  properties.value = sortProperties(response.properties);

  saveTip.init([properties.value, model]);
}

function sortProperties(props: Property[]) {
  return props.sort((a, b) => {
    if (a.isSystemField !== b.isSystemField) {
      if (a.isSystemField) {
        return 1;
      }
      if (b.isSystemField) {
        return -1;
      }
    }
    return a.order > b.order ? 1 : -1;
  });
}

async function removeItem(index: number) {
  if (siteStore.hasAccess("content", "edit")) {
    properties.value.splice(index, 1);
  }
}

async function onSave() {
  await form.value?.validate();
  const fields: PropertyJsonString[] = properties.value.map((item) => {
    return {
      ...item,
      selectionOptions: parseJsonFieldToString(item, "selectionOptions"),
      validations: parseJsonFieldToString(item, "validations"),
      settings: parseJsonFieldToString(item, "settings"),
    };
  });

  await save({
    id: contentTypeId,
    name: model.name,
    properties: fields,
  });
  saveTip.init([properties.value, model]);
  back();
}
function back() {
  const fromRouter = getQueryString("fromRouter") || "contenttypes";
  const fromFolder = getQueryString("fromFolder");
  router.push(
    useRouteSiteId({ name: fromRouter, query: { folder: fromFolder } })
  );
}

async function goBack() {
  back();
}
function onAdd() {
  editingField.value = undefined;
  editFieldDialog.value = true;
}
function parseJsonFieldToJson(row: Property, field: JsonStringField) {
  if (row[field]) {
    row[field] =
      typeof row[field] === "string"
        ? JSON.parse(row[field] as unknown as string)
        : row[field];
  } else {
    row[field] = [];
  }
}
function parseJsonFieldToString(row: Property, field: JsonStringField): string {
  return (typeof row[field] === "string"
    ? row[field]
    : JSON.stringify(row[field])) as unknown as string;
}
function onEdit(row: Property) {
  editingField.value = cloneDeep(row);
  parseJsonFieldToJson(editingField.value, "selectionOptions");
  parseJsonFieldToJson(editingField.value, "validations");
  parseJsonFieldToJson(editingField.value, "settings");
  editFieldDialog.value = true;
}

watch(
  () => [properties.value, model],
  () => {
    saveTip.changed([properties.value, model]);
  },
  {
    deep: true,
  }
);
</script>

<style lang="scss" scoped>
:deep(.el-table__row) {
  &:not(.draggable) {
    .js-sortable {
      display: none;
    }
  }
}
</style>
