<script lang="ts" setup>
import type { KeyValue } from "@/global/types";
import { requiredRule } from "@/utils/validate";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

const props = defineProps<{
  options: KeyValue[];
}>();

function removeOption(item: KeyValue) {
  props.options.splice(props.options.indexOf(item), 1);
}

function addOption() {
  props.options.push({
    key: "",
    value: "",
  });
}
</script>

<template>
  <div
    v-for="(item, index) in options"
    :key="index"
    class="w-full flex items-center space-x-4 mb-16"
  >
    <el-form-item
      :prop="'selectionOptions.' + index + '.key'"
      :rules="requiredRule(t('common.fieldRequiredTips'))"
      class="flex-1"
    >
      <el-input
        v-model="item.key"
        :placeholder="t('common.displayName')"
        data-cy="option-display-name"
      />
    </el-form-item>

    <el-form-item
      :prop="'selectionOptions.' + index + '.value'"
      :rules="requiredRule(t('common.fieldRequiredTips'))"
      class="flex-1"
    >
      <el-input
        v-model="item.value"
        :placeholder="t('common.value')"
        data-cy="option-value"
      />
    </el-form-item>
    <el-button circle data-cy="remove-option" @click="removeOption(item)">
      <el-icon class="iconfont icon-delete text-orange" />
    </el-button>
  </div>
  <div class="w-full">
    <el-button circle data-cy="add-option" @click="addOption()">
      <el-icon class="iconfont icon-a-addto text-blue" />
    </el-button>
  </div>
</template>
