<script lang="ts" setup>
import { computed, inject, ref, withDefaults } from "vue";
import { Edit, Delete, Plus, View } from "@element-plus/icons-vue";
import KForm from "../k-form.vue";
import { useI18n } from "vue-i18n";
import type { PageState } from "../k-page";
import { PAGE_STATE_KEY } from "../k-page";
import type { DataSchema } from "../data";
import type { Action } from ".";
import { showConfirm } from "@/components/basic/confirm";

export interface Props {
  edit?: string;
  data?: string;
  remove?: string;
  create?: string;
}

const props = withDefaults(defineProps<Props>(), {
  edit: undefined,
  remove: undefined,
  data: undefined,
  create: undefined,
});

const pageState = inject<PageState>(PAGE_STATE_KEY);
const { t } = useI18n();
const showCreateDialog = ref(false);
const showEditDialog = ref(false);
const actions = ref<Action[]>([]);

const data = computed(() => {
  let state = pageState?.getState(props.data, "mappedResponse");
  if (!state || !state.value) {
    state = pageState?.getState(props.data);
  }
  if (!state || !Array.isArray(state.value)) return [];
  return state.value ?? [];
});

function createRow() {
  const postAction = pageState?.getExposedAction(props.edit, "post");
  if (postAction) {
    showCreateDialog.value = true;
  } else {
    const toAction = pageState?.getExposedAction(props.edit, "to");
    if (toAction) toAction();
  }
}

function editRow(row: any) {
  const postAction = pageState?.getExposedAction(props.edit, "post");

  if (postAction) {
    const items = pageState?.getState<DataSchema[]>(props.edit, "items");

    if (items) {
      for (const item of items.value) {
        if (item.name) item.data.value = row[item.name];
      }
    }

    showEditDialog.value = true;
  } else {
    const toAction = pageState?.getExposedAction(props.edit, "to");
    if (toAction) {
      fillQuery(props.edit, row);
      toAction();
    }
  }
}

async function removeRow(row: any) {
  fillQuery(props.remove, row);
  const action = pageState?.getExposedAction(props.remove, "post");
  if (action) await action();
  location.reload();
}

async function invokeAction(action: Action, row: any) {
  if (action.confirm) await showConfirm(action.confirm);

  if (action.get) {
    fillQuery(action.get, row);
    const getAction = pageState?.getExposedAction(action.get, "get");
    if (getAction) await getAction();
  }

  if (action.post) {
    fillQuery(action.post, row);
    const postAction = pageState?.getExposedAction(action.post, "post");
    if (postAction) await postAction();
  }

  if (action.to) {
    fillQuery(action.to, row);
    const toAction = pageState?.getExposedAction(action.to, "to");
    if (toAction) await toAction();
  }
}

async function fillQuery(id: string, row: any) {
  const query = pageState?.getState<Record<string, string>>(id, "query")?.value;

  if (!query) return;

  if (Object.keys(query).length) {
    for (const key in query) {
      query[key] = row[key];
    }
  }
}

const columns = computed(() => {
  const list: DataSchema[] = [];
  const items = pageState?.getState<DataSchema[]>(props.data, "items")?.value;

  if (items?.length) {
    list.push(...items);
  } else {
    for (const item of data.value) {
      const keys = Object.keys(item);
      for (const key of keys) {
        if (list.some((s) => s.name == key)) continue;
        list.push({
          name: key,
          label: key,
          type: "string",
          data: ref(),
        });
      }
    }
  }

  return list;
});

defineExpose({ actions });
</script>

<template>
  <el-table :data="data" v-bind="$attrs">
    <template v-for="column of columns" :key="column.name">
      <el-table-column
        v-if="!column.hidden"
        :prop="column.name"
        :label="column.label"
        :width="column.width"
      >
        <template #default="{ row }">
          <template v-if="column.type == 'image'">
            <ElImage :src="row[column.name!]" class="w-full h-full" />
          </template>
          <template v-else-if="column.type == 'switch'">
            <ElSwitch :model-value="row[column.name!]" />
          </template>
          <template v-else>
            {{ row[column.name!] }}
          </template>
        </template>
      </el-table-column>
    </template>

    <el-table-column
      v-if="create || edit || remove || actions.length"
      fixed="right"
      align="right"
    >
      <template #header>
        <div class="flex items-center justify-end">
          <el-button v-if="create" type="primary" @click="createRow">
            <el-icon :size="12">
              <Plus />
            </el-icon>
            <span>{{ t("common.create") }}</span>
          </el-button>
        </div>
      </template>

      <template #default="{ row }">
        <div class="whitespace-nowrap">
          <ElButton
            v-for="(item, index) of actions"
            :key="index"
            @click="invokeAction(item, row)"
            >{{ item.label }}</ElButton
          >
          <el-button v-if="edit" type="success" @click="editRow(row)">
            <el-icon :size="12">
              <Edit />
            </el-icon>
            <span>{{ t("common.edit") }}</span>
          </el-button>
          <el-button v-if="remove" type="danger" @click="removeRow(row)">
            <el-icon :size="12">
              <Delete />
            </el-icon>
            <span>{{ t("common.delete") }}</span>
          </el-button>
        </div>
      </template>
    </el-table-column>
  </el-table>
  <div class="hidden">
    <slot />
  </div>

  <teleport to="body">
    <el-dialog
      v-if="showEditDialog && edit"
      :model-value="showEditDialog"
      @closed="showEditDialog = false"
    >
      <KForm :data="edit" />
    </el-dialog>

    <el-dialog
      v-if="showCreateDialog && create"
      :model-value="showCreateDialog"
      @closed="showCreateDialog = false"
    >
      <KForm :data="create" />
    </el-dialog>
  </teleport>
</template>
