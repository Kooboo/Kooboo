<script lang="ts" setup>
import { computed, ref } from "vue";
import type { Rules } from "async-validator";
import { isUniqueName } from "@/api/resource-group";
import { useScriptStore } from "@/store/script";
import SortableList from "@/components/sortable-list/index.vue";
import type { SortEvent } from "@/global/types";
import { emptyGuid } from "@/utils/guid";
import type { PostGroup } from "@/api/resource-group/types";
import { rangeRule, requiredRule, simpleNameRule } from "@/utils/validate";

import { useI18n } from "vue-i18n";
const props = defineProps<{ id?: string; hiddenName?: boolean }>();
const { t } = useI18n();
const scriptStore = useScriptStore();
const form = ref();
const model = ref<PostGroup>({
  id: props.id || emptyGuid,
  name: "",
  typeName: "Script",
  children: [],
});

const isEdit = computed(() => model.value.id !== emptyGuid);

const load = async () => {
  model.value = await scriptStore.getGroup(model.value.id);
};

const availableList = computed(() => {
  const result = [];
  for (const i of scriptStore.external) {
    if (model.value.children.some((s) => s.routeId === i.routeId)) continue;
    result.push({
      name: i.name,
      routeId: i.routeId,
    });
  }
  return result;
});

const rules = computed(() => {
  const result = {
    name: [
      rangeRule(1, 50),
      requiredRule(t("common.nameRequiredTips")),
      simpleNameRule(),
      {
        async asyncValidator(_, value, callback) {
          try {
            await isUniqueName(value, "script");
          } catch (error) {
            callback(Error(t("common.nameExist")));
          }
        },
        trigger: "blur",
      },
    ],
  } as Rules;

  if (isEdit.value) {
    delete result.name;
  }

  return result;
});

const save = async () => {
  await form.value?.validate();
  await scriptStore.updateGroup(model.value);
};

const onAdd = (value: string) => {
  const found = availableList.value.find((f) => f.routeId === value);

  if (found) {
    model.value.children.push(found);
  }
};

const onSort = (e: SortEvent) => {
  const item = model.value.children.splice(e.oldIndex, 1)[0];
  model.value.children.splice(e.newIndex, 0, item);
};

const onDelete = (id: string) => {
  const index = model.value.children.findIndex((f) => f.routeId === id);
  model.value.children.splice(index, 1);
};

if (model.value.id !== emptyGuid) {
  load();
}
const emit = defineEmits<{
  (e: "save"): void;
}>();
defineExpose({ save });
</script>

<template>
  <div class="p-24">
    <el-form
      v-if="!hiddenName"
      ref="form"
      label-position="top"
      :model="model"
      :rules="rules"
      @submit.prevent
      @keydown.enter="emit('save')"
    >
      <el-form-item :label="t('common.name')" prop="name">
        <el-input
          v-model="model.name"
          :disabled="isEdit"
          data-cy="group-name"
        />
      </el-form-item>
    </el-form>
    <el-dropdown trigger="click" @command="onAdd">
      <el-button round class="shadow-s-10 border-none" data-cy="add-script">
        <div class="flex items-center">
          <span>{{ t("common.addScript") }}</span>
          <el-icon class="iconfont icon-pull-down text-12px ml-8 !mr-0" />
        </div>
      </el-button>
      <template #dropdown>
        <div
          v-if="Object.keys(availableList).length == 0"
          class="w-100px !h-40px flex items-center justify-center"
        >
          <span>{{ t("common.noData") }}</span>
        </div>
        <el-dropdown-menu v-else>
          <el-dropdown-item
            v-for="item of availableList"
            :key="item.routeId"
            :command="item.routeId"
            data-cy="script-opt"
          >
            <span>{{ item.name }}</span>
          </el-dropdown-item>
        </el-dropdown-menu>
      </template>
    </el-dropdown>
    <SortableList
      class="!px-0"
      :list="model.children"
      display-prop="name"
      id-prop="routeId"
      @sort="onSort"
      @delete="onDelete"
    >
      <template #bottom>
        <div />
      </template>
    </SortableList>
  </div>
</template>
