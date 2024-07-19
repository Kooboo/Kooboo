<script lang="ts" setup>
import type { Term } from "@/api/commerce/type";
import { useI18n } from "vue-i18n";
import EditableTags from "@/components/basic/editable-tags.vue";

const { t } = useI18n();

const termTypes = [
  { name: "Selection", display: t("common.selection") },
  { name: "Custom", display: t("common.custom") },
];

const props = defineProps<{ model: Term[]; forceSelection?: boolean }>();

function onDelete(index: number) {
  props.model.splice(index, 1);
}

function onAdd() {
  props.model.push({
    name: "",
    type: props.forceSelection ? "Selection" : "Custom",
    options: [],
  });
}
</script>

<template>
  <div class="space-y-8 w-full">
    <div v-for="(term, index) of props.model" :key="index">
      <div class="flex items-center space-x-4">
        <ElInput v-model="term.name" />
        <ElSelect v-if="!forceSelection" v-model="term.type" class="w-220px">
          <ElOption
            v-for="item in termTypes"
            :key="item.name"
            :label="item.display"
            :value="item.name"
          />
        </ElSelect>

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
      <EditableTags
        v-if="term.type == 'Selection'"
        v-model="term.options"
        class="mt-4 mb-12 mr-44px"
      />
    </div>
    <IconButton
      circle
      class="text-blue"
      icon="icon-a-addto"
      :tip="t('common.add')"
      @click="onAdd"
    />
  </div>
</template>
