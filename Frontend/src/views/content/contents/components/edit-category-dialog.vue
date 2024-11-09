<template>
  <el-dialog
    v-model="visible"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.categories')"
    custom-class="el-dialog--fixed-footer"
    @close="handleClose"
  >
    <KTable :data="list">
      <el-table-column width="50">
        <template #default="data">
          <div class="flex items-center">
            <el-checkbox
              size="large"
              class="pl-14px"
              :model-value="selectIds.includes(data.row.id)"
              @change="onCheck(data.row.id)"
            />
          </div>
        </template>
      </el-table-column>
      <el-table-column
        v-for="item in columns"
        :key="item"
        :prop="item"
        :label="item"
      />
    </KTable>
    <template #footer>
      <DialogFooterBar @confirm="handleSave" @cancel="handleClose" />
    </template>
  </el-dialog>
</template>
<script setup lang="ts">
import { ref, watch } from "vue";
import useOperationDialog from "@/hooks/use-operation-dialog";
import type { UPDATE_MODEL_EVENT } from "@/constants/constants";
import KTable from "@/components/k-table";
import type {
  ContentCategory,
  TextContentItem,
} from "@/api/content/textContent";
import { getByFolder } from "@/api/content/textContent";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

import { useI18n } from "vue-i18n";
interface PropsType {
  modelValue: boolean;
  current: ContentCategory;
}
interface EmitsType {
  (e: typeof UPDATE_MODEL_EVENT, value: boolean): void;
}

const props = defineProps<PropsType>();
const emits = defineEmits<EmitsType>();
const { t } = useI18n();
const { visible, handleClose } = useOperationDialog(props, emits);

type TableRowItem = Pick<TextContentItem, "id"> & Record<string, any>;
const list = ref<TableRowItem[]>([]);
const allContents = ref<Record<string, TextContentItem>>({});
const columns = ref<string[]>([]);
const selectIds = ref<string[]>([]);
watch(
  () => props.current,
  (val) => {
    if (val.categoryFolder) {
      fetchList();
    }
  }
);
watch(
  () => props.modelValue,
  (val) => {
    if (val) {
      selectIds.value = props.current.contents.map((item) => item.id);
    }
  }
);

async function fetchList() {
  const datas = await getByFolder({
    folderId: props.current.categoryFolder.id,
    pageNr: 1,
    pageSize: 99999,
  });
  setTableList(datas.list, datas.columns);
}

function setTableList(contents: TextContentItem[], columnList: any) {
  const contentMaps: Record<string, TextContentItem> = {};
  contents.forEach((it) => {
    contentMaps[it.id] = it;
  });
  allContents.value = contentMaps;
  const defaultColumn = Object.keys(contents[0]?.textValues ?? {})[0];
  if (defaultColumn) {
    columns.value = [defaultColumn];
    const column = columnList.find((f: any) => f.name == defaultColumn);
    list.value = contents.map((item) => {
      let value = item.textValues[defaultColumn];
      if (column?.selectionOptions) {
        try {
          const options = JSON.parse(column.selectionOptions);
          let values = [value];
          if (column.controlType == "CheckBox") {
            values = JSON.parse(value);
          }
          const displayValues = [];
          for (const i of values) {
            const option = options.find((f: any) => f.value == i);
            if (option) displayValues.push(option.key);
            else displayValues.push(i);
          }
          value = displayValues.join(",");
        } catch {
          //
        }
      }
      return {
        id: item.id,
        [defaultColumn]: value,
      };
    });
  }
}
function handleSave() {
  props.current.contents = selectIds.value
    .map((id) => {
      const content = allContents.value[id];
      if (!content) {
        return null;
      }
      return content;
    })
    .filter((it) => it)
    .map((it: any, ix: number) => {
      it.order = ix;
      return it as TextContentItem;
    });
  handleClose();
}

function onCheck(id: string) {
  const index = selectIds.value.indexOf(id);
  if (index > -1) {
    selectIds.value.splice(index, 1);
  } else {
    if (props.current.multipleChoice) {
      selectIds.value.push(id);
    } else {
      selectIds.value = [id];
    }
  }
}
</script>
