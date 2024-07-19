<template>
  <el-dialog
    v-model="visible"
    :close-on-click-modal="false"
    :destroy-on-close="true"
    :title="t('common.importData')"
    @close="onClosed"
  >
    <el-form
      ref="form"
      class="el-form--label-normal"
      :model="model"
      label-position="top"
      @submit.prevent
    >
      <el-form-item prop="rowsToSkip" :label="t('common.rowsToSkip')">
        <el-input-number v-model="model.rowsToSkip" :min="0" :precision="0" />
      </el-form-item>
      <el-form-item prop="file" :label="t('common.file')">
        <KUpload
          ref="uploader"
          :permission="permission"
          :multiple="false"
          accept="text/csv"
          :action="getUploadCsvActionUrl()"
          :before-upload="handleBeforeUpload"
          :on-success="handleUploadFinish"
          :on-error="handleUploadError"
          :data="{ rowsToSkip: model.rowsToSkip }"
          data-cy="upload"
        >
          <template #default>
            <el-button
              v-hasPermission="permission"
              type="primary"
              round
              class="shadow-s-10 !py-8px"
              data-cy="upload"
            >
              <el-icon class="iconfont icon-a-Cloudupload !text-20px" />
              {{ t("common.selectFile") }}
            </el-button>
          </template>
        </KUpload>
        <span class="ml-8">
          {{ model.filename || t("common.supportFile") + ": .csv" }}
        </span>
      </el-form-item>
      <el-form-item
        v-if="model.fields.length && csvFields.length"
        :label="t('common.fieldMaps')"
      >
        <el-table :data="model.fields" :max-height="250">
          <el-table-column
            prop="dbFieldName"
            :label="t('common.databaseField')"
          />
          <el-table-column prop="csvFieldName" :label="t('common.csvField')">
            <template #default="{ row }">
              <el-select v-model="row.csvFieldName" clearable>
                <el-option
                  v-for="csvField in csvFields"
                  :key="csvField.name"
                  :value="csvField.name"
                  :label="csvField.displayName || csvField.name"
                />
              </el-select>
            </template>
          </el-table-column>
          <el-table-column
            align="center"
            prop="required"
            :label="t('common.required')"
            width="100"
          >
            <template #default="{ row }">
              <el-checkbox v-model="row.required" :label="row.name"
                >&nbsp;</el-checkbox
              >
            </template>
          </el-table-column>
          <el-table-column
            align="center"
            prop="unique"
            :label="t('common.unique')"
            width="100"
          >
            <template #default="{ row }">
              <el-checkbox v-model="row.unique" :label="row.name"
                >&nbsp;</el-checkbox
              >
            </template>
          </el-table-column>
        </el-table>
      </el-form-item>
      <el-form-item
        v-if="model.fields.some((it) => it.unique)"
        prop="overwriteExisting"
        :label="t('common.overwrite')"
      >
        <el-switch v-model="model.overwriteExisting" />
      </el-form-item>
    </el-form>
    <template #footer>
      <DialogFooterBar
        :confirm-label="t('common.start')"
        :disabled="!canSubmit"
        @confirm="handleSubmit"
        @cancel="onClosed"
      />
    </template>
  </el-dialog>
</template>

<script lang="ts" setup>
import useOperationDialog from "@/hooks/use-operation-dialog";
import type {
  DatabaseType,
  DatabaseListColumn,
  ReadCsvResponse,
  ImportRequestModel,
} from "@/api/database";
import type { UPDATE_MODEL_EVENT } from "@/constants/constants";
import { ref, computed, watch } from "vue";
import { importData } from "@/api/database";
import { isEmpty } from "lodash-es";
import { getUploadCsvActionUrl } from "@/api/content/file";

import { useI18n } from "vue-i18n";
import { ElMessage } from "element-plus";

interface PropsType {
  modelValue: boolean;
  dbType: DatabaseType;
  table: string;
  databaseColumns: DatabaseListColumn[];
}
interface EmitsType {
  (e: typeof UPDATE_MODEL_EVENT, value: boolean): void;
  (e: "done"): void;
}

const props = defineProps<PropsType>();
const emits = defineEmits<EmitsType>();
const { t } = useI18n();
const { visible, handleClose } = useOperationDialog(props, emits);
const form = ref();
const uploader = ref();

type ReadCsvModel = {
  filename: string;
  rowsToSkip: number;
};
type ViewModel = ImportRequestModel & ReadCsvModel;

const model = ref<ViewModel>({
  overwriteExisting: false,
  filename: "",
  rowsToSkip: 0,
  records: [],
  fields: [],
});
const permission = ref({
  feature: "database",
  action: "edit",
});
const databaseFields = computed(() =>
  props.databaseColumns.filter((it) => it.name !== "_id")
);
const csvFields = ref<DatabaseListColumn[]>([]);
const csvRows = ref<Record<string, string>[]>([]);
const canSubmit = computed(() => {
  return (
    !isEmpty(databaseFields.value) &&
    !isEmpty(csvRows.value) &&
    !isEmpty(model.value.fields.filter((it) => it.csvFieldName))
  );
});

async function handleBeforeUpload(file: { type: string; name: string }) {
  if (file.type !== "text/csv") {
    ElMessage.error(t("common.fileFormatIsIncorrect"));
    return false;
  }
  model.value.filename = file.name;
  return true;
}

function handleUploadFinish(db: ReadCsvResponse) {
  csvFields.value = db.columns;
  csvRows.value = db.list;
  model.value.records = db.list;
  model.value.fields = databaseFields.value.map((dbField) => {
    const csvField =
      db.columns.find(
        (it) =>
          (it.displayName || it.name).toLowerCase() ===
          dbField.name.toLowerCase()
      )?.name ?? "";
    return {
      dbFieldName: dbField.name,
      csvFieldName: csvField,
      required: false,
      unique: false,
    };
  });
}

function handleUploadError(error: Error) {
  try {
    ElMessage.error(JSON.parse(error.message).toString());
  } catch {
    ElMessage.error(error.message);
  }
}
const itemsLimit = 5000;
async function handleSubmit() {
  if (model.value.records.length > itemsLimit) {
    ElMessage.error(t("common.itemsCountExceedLimit", { count: itemsLimit }));
    return;
  }
  await importData(props.dbType, props.table, model.value);
  onClosed();
  emits("done");
}

function onClosed() {
  onReset();
  handleClose();
}

function onReset(overwrite?: Partial<ViewModel>) {
  csvFields.value = [];
  csvRows.value = [];
  model.value = {
    overwriteExisting: false,
    filename: "",
    fields: [],
    rowsToSkip: 0,
    records: [],
    ...overwrite,
  };
}

watch(
  () => model.value.rowsToSkip,
  (rowsToSkip) => {
    onReset({ rowsToSkip });
  }
);
</script>
