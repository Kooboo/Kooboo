<script lang="ts" setup>
import { useI18n } from "vue-i18n";
import type { Schema } from "./user-options";

const props = defineProps<{
  schema: Schema[];
}>();

const { t } = useI18n();

function onDelete(index: number) {
  props.schema.splice(index, 1);
}

function onAdd() {
  props.schema.push({
    name: "",
    display: "",
    type: "string",
    arrayType: "string",
    children: [],
  });
}
</script>

<template>
  <div class="space-y-4">
    <div
      v-for="(item, index) of schema"
      :key="index"
      class="flex flex-col space-y-4"
    >
      <div class="flex space-x-4">
        <el-input
          v-model="item.name"
          class="w-180px"
          :placeholder="t('common.name')"
        />
        <el-input
          v-model="item.display"
          class="w-180px"
          :placeholder="t('common.display')"
        />
        <el-select v-model="item.type" class="w-120px">
          <el-option value="string" :label="t('common.text')" />
          <el-option value="number" :label="t('common.number')" />
          <el-option value="boolean" :label="t('common.boolean')" />
          <el-option value="object" :label="t('common.object')" />
          <el-option value="array" :label="t('common.array')" />
        </el-select>

        <el-select
          v-if="item.type == 'array'"
          v-model="item.arrayType"
          class="w-120px"
        >
          <el-option value="string" :label="t('common.text')" />
          <el-option value="number" :label="t('common.number')" />
          <el-option value="boolean" :label="t('common.boolean')" />
          <el-option value="object" :label="t('common.object')" />
        </el-select>
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
      <div
        v-if="
          item.type == 'object' ||
          (item.type == 'array' && item.arrayType == 'object')
        "
        class="pl-32 my-8 border-l-2 border-blue/70"
      >
        <ObjectSchema :schema="item.children" />
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
</template>
