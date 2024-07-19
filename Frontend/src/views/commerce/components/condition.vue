<script lang="ts" setup>
import type { ConditionSchema, Condition } from "@/api/commerce/common";
import { useI18n } from "vue-i18n";

const props = defineProps<{
  condition: Condition;
  schemas: ConditionSchema[];
  title: string;
}>();

const { t } = useI18n();

function onDelete(index: number) {
  props.condition.items.splice(index, 1);
}

function onAdd() {
  props.condition.items.push({
    option: props.schemas[0].name,
    method: props.schemas[0].methods[0].name,
    value: "",
  });
}
</script>

<template>
  <ElForm v-if="schemas.length" label-position="top">
    <ElFormItem :label="title">
      <ElRadioGroup v-model="condition.isAny">
        <ElRadio :label="false">{{ t("commerce.allConditions") }}</ElRadio>
        <ElRadio :label="true">{{ t("commerce.anyConditions") }}</ElRadio>
      </ElRadioGroup>
    </ElFormItem>
    <ElFormItem :label="t('common.conditions')">
      <div class="space-y-4">
        <div
          v-for="(conditionItem, index) of condition.items"
          :key="index"
          class="flex items-center space-x-4"
        >
          <ElSelect
            v-model="conditionItem.option"
            @change="conditionItem.method = ''"
          >
            <ElOption
              v-for="item in schemas"
              :key="item.name"
              :label="item.display"
              :value="item.name"
            />
          </ElSelect>
          <ElSelect v-model="conditionItem.method">
            <ElOption
              v-for="item in schemas.find(
                (f) => f.name == conditionItem.option
              )!.methods"
              :key="item.name"
              :label="item.display"
              :value="item.name"
            />
          </ElSelect>
          <el-input
            v-model="conditionItem.value"
            class="w-200px"
            :placeholder="t('common.value')"
          />
          <div>
            <IconButton
              circle
              class="hover:text-orange text-orange"
              icon="icon-delete "
              :tip="t('common.delete')"
              @click="onDelete(index)"
            />
          </div>
        </div>
        <IconButton
          circle
          class="text-blue"
          icon="icon-a-addto "
          :tip="t('common.add')"
          @click="onAdd"
        />
      </div>
    </ElFormItem>
  </ElForm>
</template>
