<script lang="ts" setup>
import type { Rule, CodeItem, Condition } from "@/api/events/types";
import SortableList from "@/components/sortable-list/index.vue";
import { newGuid } from "@/utils/guid";
import * as RuleList from "./rule-list.vue";
import { ref } from "vue";
import type { SortEvent } from "@/global/types";
import CodeDialog from "./code-dialog.vue";

import { useI18n } from "vue-i18n";
// eslint-disable-next-line @typescript-eslint/no-explicit-any
const RuleListDefault = (RuleList as any).default;
const showCodeDialog = ref(false);

const sample = `
//common event varilables: k.event.url, k.event.userAgent, k.event.culture; 
//variables per event. k.event.page, k.event.view, k.event.route; 
// Finding=before object found. Found = object founded. 
//example, url redirect. only valid on RouteFinding event. 
//if (k.event.url.indexOf(""pagetwo"")>-1)
//{
//     k.event.url = ""/pageone"";
//};
`;

defineEmits<{
  (e: "delete"): void;
  (e: "editCondition", value: Condition[]): void;
}>();

const props = defineProps<{ data: Rule }>();
const { t } = useI18n();

if (!props.data.then) props.data.then = [];
if (!props.data.else) props.data.else = [];

const position = {
  then: "THEN",
  else: "ELSE",
};

const selectedCode = ref<CodeItem>();

const onEditCode = (value: CodeItem) => {
  selectedCode.value = value;
  showCodeDialog.value = true;
};

const onAddCode = () => {
  selectedCode.value = {
    code: sample,
    setting: {},
  };

  showCodeDialog.value = true;
};

const getCondition = (list: Condition[]) => {
  return list.map((m) => `${m.left} ${m.operator} ${m.right}`).join(" && ");
};

const onSort = (e: SortEvent) => {
  const item = props.data.do!.splice(e.oldIndex, 1)[0];
  props.data.do!.splice(e.newIndex, 0, item);
};

const onCodeSaved = () => {
  if (!props.data.do || !selectedCode.value) return;
  if (!props.data.do.find((f) => f == selectedCode.value)) {
    props.data.do.push(selectedCode.value);
  }
};
</script>

<template>
  <div>
    <el-card shadow="hover" class="w-full">
      <template #header>
        <div class="flex items-center cursor-move">
          <el-icon class="iconfont icon-move" />
          <div class="flex-1" />
          <IconButton
            class="text-orange hover:text-orange"
            icon="icon-delete"
            :tip="t('common.delete')"
            data-cy="remove"
            @click="$emit('delete')"
          />
        </div>
      </template>
      <div
        v-if="
          data.if?.length || data.then?.length || data.else?.length || !data.do
        "
        class="space-y-8"
      >
        <div class="flex items-center">
          <span class="w-64 text-right p-8">IF</span>
          <div class="flex-1">
            <el-tag
              size="small"
              class="rounded-full cursor-pointer"
              data-cy="edit-condition"
              @click="$emit('editCondition', data.if!)"
            >
              {{
                data.if?.length
                  ? getCondition(data.if)
                  : t("common.editCondition")
              }}
            </el-tag>
          </div>
        </div>
        <div v-for="(value, key) of position" :key="key" class="flex">
          <span class="w-64 text-right p-8 leading-40px">{{ value }}</span>
          <div class="flex-1 overflow-auto pb-4">
            <RuleListDefault :rules="(data[key] as Rule[])" show-add />
          </div>
        </div>
      </div>
      <div v-else>
        <SortableList
          :list="data.do"
          id-prop="codeId"
          :group="newGuid()"
          class="!p-0"
          @sort="onSort"
          @delete="(id, obj) => (data.do = data.do?.filter((f) => f !== obj))"
        >
          <template #default="{ item }">
            <div class="ellipsis">{{ item.code }}</div>
          </template>
          <template #right="{ item }">
            <div class="p-8 cursor-pointer" @click="onEditCode(item)">
              <el-icon class="iconfont icon-a-writein" />
            </div>
          </template>
          <template #bottom>
            <el-button circle data-cy="add-code" @click="onAddCode">
              <el-icon class="text-blue iconfont icon-a-addto" />
            </el-button>
          </template>
        </SortableList>
      </div>
    </el-card>
    <CodeDialog
      v-if="showCodeDialog && selectedCode"
      v-model="showCodeDialog"
      v-model:code="selectedCode.code"
      @update:code="onCodeSaved"
    />
  </div>
</template>
