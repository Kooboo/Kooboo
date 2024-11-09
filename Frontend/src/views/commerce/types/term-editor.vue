<script lang="ts" setup>
import type { Term } from "@/api/commerce/type";
import { useI18n } from "vue-i18n";
import TermItem from "./term-item.vue";

const { t } = useI18n();

const props = defineProps<{
  model: Term[];
  forceSelection?: boolean;
  nameLabel?: string;
  valueLabel?: string;
  namePlaceholder?: string;
}>();

function onAdd() {
  props.model.push({
    name: "",
    type: props.forceSelection ? "Selection" : "Custom",
    valueType: "text",
    options: [],
  });
}

function changeName(term: Term, value: string) {
  if (props.model.find((f) => f.name == value)) return;
  term.name = value;
}

function changeOption(term: Term, oldValue: string, newValue: string) {
  const index = term.options.indexOf(oldValue);
  if (index > -1) term.options.splice(index, 1, newValue);
}
</script>

<template>
  <div class="space-y-8 w-full">
    <TermItem
      v-for="(term, index) of props.model"
      :key="index"
      :model="term"
      :force-selection="forceSelection"
      :editing="!term.name"
      :name-label="nameLabel"
      :value-label="valueLabel"
      :name-placeholder="namePlaceholder"
      @change-name="changeName(term, $event)"
      @change-option="(o, n) => changeOption(term, o, n)"
      @add-option="term.options.push($event)"
      @delete-option="term.options.splice($event, 1)"
      @delete="props.model.splice(props.model.indexOf(term), 1)"
    />
    <IconButton
      circle
      class="text-blue"
      icon="icon-a-addto"
      :tip="t('common.add')"
      @click="onAdd"
    />
  </div>
</template>
