<script lang="ts" setup>
import type { ConditionSchema, Condition } from "@/api/commerce/common";
import { useI18n } from "vue-i18n";
import DropdownInput from "@/components/basic/dropdown-input.vue";

const props = defineProps<{
  condition: Condition;
  schemas: ConditionSchema[];
  title: string;
  readonly?: boolean;
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
  <div
    v-if="readonly && schemas.length"
    class="flex items-center gap-4 flex-wrap"
  >
    <template v-if="condition.items.length">
      <span class="dark:text-[#cfd3dc]">{{ t("common.when") }}</span>
      <template v-for="(conditionItem, index) of condition.items" :key="index">
        <ElTag type="success">
          <div class="flex items-center space-x-4">
            <span>{{
              schemas.find((f) => f.name == conditionItem.option)?.display
            }}</span>
            <span>{{
              schemas
                .find((f) => f.name == conditionItem.option)
                ?.methods?.find((f) => f.name == conditionItem.method)?.display
            }}</span>
            <span>{{ conditionItem.value }}</span>
          </div>
        </ElTag>
        <span
          v-if="index < condition.items.length - 1"
          class="dark:text-[#cfd3dc]"
          >{{ condition.isAny ? t("common.or") : t("common.and") }}</span
        >
      </template>
      <span class="dark:text-[#cfd3dc]">{{ t("common.then") }}</span>
    </template>
  </div>

  <ElForm v-else-if="schemas.length" label-position="top">
    <ElFormItem>
      <div class="space-y-8">
        <div class="flex gap-8">
          <div class="space-x-4">
            <span class="dark:text-[#cfd3dc]">{{ t("common.when") }}</span>
            <ElTag v-if="title" round type="success">{{ title }}</ElTag>
            <span class="dark:text-[#cfd3dc]">{{ t("common.match") }}</span>
          </div>
          <ElRadioGroup v-model="condition.isAny">
            <ElRadio :label="false">{{ t("commerce.allConditions") }}</ElRadio>
            <ElRadio :label="true">{{ t("commerce.anyConditions") }}</ElRadio>
          </ElRadioGroup>
        </div>
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
            <DropdownInput
              v-model="conditionItem.value"
              :options="
              schemas.find((f) => f.name == conditionItem.option)!.selections
            "
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
          <el-button round type="primary" @click="onAdd">
            <div class="flex items-center">
              <el-icon class="mr-16 iconfont icon-a-addto" />
              {{ t("common.addCondition") }}
            </div>
          </el-button>
        </div>
      </div>
    </ElFormItem>
  </ElForm>
</template>
