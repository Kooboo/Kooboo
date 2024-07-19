<template>
  <el-dialog
    v-model="visible"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.tableRelation')"
    @close="handleClose"
  >
    <el-form
      ref="createForm"
      :model="createModel"
      :rules="createModelRules"
      label-position="top"
      @submit.prevent
      @keydown.enter="handleCreate"
    >
      <el-form-item prop="name" :label="t('common.name')">
        <el-input v-model="createModel.name" />
      </el-form-item>
      <el-form-item prop="tableA" :label="t('common.tableA')">
        <el-select v-model="createModel.tableA">
          <el-option
            v-for="table in tables"
            :key="table.name"
            :value="table.name"
          >
            {{ table.name }}
          </el-option>
        </el-select>
      </el-form-item>
      <el-form-item
        v-show="tableAObject"
        prop="fieldA"
        :label="t('common.fieldA')"
      >
        <el-select v-if="tableAObject" v-model="createModel.fieldA">
          <el-option
            v-for="field in tableAObject.fields"
            :key="field"
            :value="field"
          >
            {{ field }}
          </el-option>
        </el-select>
      </el-form-item>
      <el-form-item prop="relation" :label="t('common.relation')">
        <el-select v-model="createModel.relation">
          <el-option
            v-for="item in relationTypes"
            :key="item.type"
            :value="item.type"
            :label="item.displayName"
          >
            {{ item.displayName }}
          </el-option>
        </el-select>
      </el-form-item>
      <el-form-item prop="tableB" :label="t('common.tableB')">
        <el-select v-model="createModel.tableB">
          <el-option
            v-for="table in tables"
            :key="table.name"
            :value="table.name"
          >
            {{ table.name }}
          </el-option>
        </el-select>
      </el-form-item>
      <el-form-item
        v-show="tableBObject"
        prop="fieldB"
        :label="t('common.fieldB')"
      >
        <el-select v-if="tableBObject" v-model="createModel.fieldB">
          <el-option
            v-for="field in tableBObject.fields"
            :key="field"
            :value="field"
          >
            {{ field }}
          </el-option>
        </el-select>
      </el-form-item>
    </el-form>
    <template #footer>
      <DialogFooterBar
        :confirm-label="t('common.create')"
        @confirm="handleCreate"
        @cancel="handleClose"
      />
    </template>
  </el-dialog>
</template>
<script setup lang="ts">
import { computed, reactive, ref, watch } from "vue";
import type { ElForm } from "element-plus";
import useOperationDialog from "@/hooks/use-operation-dialog";
import type { UPDATE_MODEL_EVENT } from "@/constants/constants";
import type { RelationType, TableFields } from "@/api/database/table-relation";
import {
  getRelationTypes,
  getTablesAndFields,
  postRelation,
} from "@/api/database/table-relation";
import { isUniqueNameRule, rangeRule, requiredRule } from "@/utils/validate";
import type { Rules } from "async-validator";
import { isUniqueName } from "@/api/database/table-relation";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

import { useI18n } from "vue-i18n";
interface PropsType {
  modelValue: boolean;
}
interface EmitsType {
  (e: typeof UPDATE_MODEL_EVENT, value: boolean): void;
  (e: "create-success"): void;
}

const props = defineProps<PropsType>();
const emits = defineEmits<EmitsType>();
const { t } = useI18n();
const { visible, handleClose } = useOperationDialog(props, emits);

const createForm = ref<InstanceType<typeof ElForm>>();
const createModel = reactive({
  name: "",
  tableA: "",
  fieldA: "",
  tableB: "",
  fieldB: "",
  relation: "",
});
const createModelRules = {
  name: [
    requiredRule(t("common.fieldRequiredTips")),
    rangeRule(1, 50),
    isUniqueNameRule(isUniqueName, t("common.RelationNameExistsTips")),
  ],
  tableA: requiredRule(t("common.fieldRequiredTips")),
  fieldA: requiredRule(t("common.fieldRequiredTips")),
  tableB: requiredRule(t("common.fieldRequiredTips")),
  fieldB: requiredRule(t("common.fieldRequiredTips")),
  relation: requiredRule(t("common.fieldRequiredTips")),
} as Rules;
const tables = ref<TableFields[]>([]);
const relationTypes = ref<RelationType[]>([]);

const tableAObject = computed(() =>
  tables.value.find((f) => f.name === createModel.tableA)
);
const tableBObject = computed(() =>
  tables.value.find((f) => f.name === createModel.tableB)
);

watch(
  () => visible.value,
  (val) => {
    if (val) {
      createForm.value?.resetFields();
      getData();
    }
  }
);

async function getData() {
  const [fields, types] = await Promise.all([
    getTablesAndFields(),
    getRelationTypes(),
  ]);
  tables.value = fields;
  relationTypes.value = types;
  const defaultTable = tables.value[0];
  if (defaultTable) {
    createModel.tableA = defaultTable.name;
    createModel.tableB = defaultTable.name;
    const defaultField = defaultTable.fields[0];
    if (defaultField) {
      createModel.fieldA = defaultField;
      createModel.fieldB = defaultField;
    }
  }
  const defaultRelation = relationTypes.value[0];
  if (defaultRelation) {
    createModel.relation = defaultRelation.type;
  }
}
function handleCreate() {
  createForm.value?.validate(async (valid) => {
    if (valid) {
      await postRelation(createModel);
      handleClose();
      emits("create-success");
    }
  });
}
</script>
