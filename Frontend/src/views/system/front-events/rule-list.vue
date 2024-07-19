<script lang="ts" setup>
import type { Rule, Condition } from "@/api/events/types";
import VueDraggable from "vuedraggable";
import RulePanel from "./rule-panel.vue";
import { newGuid } from "@/utils/guid";
import { EVENT_RULE_TYPE } from "@/constants/constants";
import type { SortEvent } from "@/global/types";
import { ref } from "vue";
import ConditionDialog from "./condition-dialog.vue";
import { useI18n } from "vue-i18n";
const { t } = useI18n();

const props = defineProps<{
  rules: Rule[];
  showAdd?: boolean;
}>();

const conditions = ref<Condition[]>();
const showEditCondition = ref(false);

const onEditCondition = (value: Condition[]) => {
  conditions.value = value;
  showEditCondition.value = true;
};

const onAddRule = (value: string) => {
  if (value === EVENT_RULE_TYPE.do) {
    props.rules.push({ id: newGuid(), do: [] });
  } else {
    props.rules.push({ id: newGuid(), if: [] });
  }
};

const onSort = (e: SortEvent) => {
  const item = props.rules.splice(e.oldIndex, 1)[0];
  props.rules.splice(e.newIndex, 0, item);
};

const onDelete = (rule: Rule) => {
  const index = props.rules.findIndex((f) => f === rule);
  props.rules.splice(index, 1);
};
</script>

<template>
  <div class="space-y-8">
    <VueDraggable
      :model-value="rules"
      :group="newGuid()"
      item-key="id"
      class="space-y-8"
      handle=".el-card__header"
      @sort="onSort"
    >
      <template #item="{ element }">
        <RulePanel
          :data="element"
          @delete="onDelete(element)"
          @edit-condition="onEditCondition"
        />
      </template>
    </VueDraggable>
    <el-dropdown v-if="showAdd" trigger="click" @command="onAddRule">
      <el-button circle @command="onAddRule">
        <el-icon class="iconfont icon-a-addto text-blue" />
      </el-button>
      <template #dropdown>
        <el-dropdown-menu>
          <el-dropdown-item
            v-for="(value, key) of EVENT_RULE_TYPE"
            :key="key"
            :command="value"
          >
            <span>{{ value }}</span>
          </el-dropdown-item>
        </el-dropdown-menu>
      </template>
    </el-dropdown>
    <ConditionDialog
      v-if="showEditCondition"
      v-model="showEditCondition"
      :conditions="conditions!"
    />
  </div>
</template>
