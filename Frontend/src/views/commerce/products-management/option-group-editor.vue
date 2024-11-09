<script lang="ts" setup>
import { useI18n } from "vue-i18n";
import { computed } from "vue";
import type {
  OptionGroup,
  ProductVariant,
  VariantOption,
} from "@/api/commerce/product";
import OptionEditor from "./option-editor.vue";
import { errorMessage } from "@/components/basic/message";

const props = defineProps<{
  options?: string[];
  variants: ProductVariant[];
  variantOptions: VariantOption[];
}>();

const emit = defineEmits<{
  (
    e: "update-option-item",
    name: string,
    oldItem: string,
    newItem: string
  ): void;
  (e: "add-option-item", name: string, value: string): void;
  (e: "delete-option-item", name: string, value: string): void;
  (e: "delete-option", name: string): void;
  (e: "add-option", name: string): void;
  (e: "update-option-name", name: string, value: string): void;
}>();

const { t } = useI18n();

const list = computed(() => {
  const result: OptionGroup[] = [];
  if (props.options?.length) {
    for (const i of props.options) {
      const items = [];
      for (const v of props.variants) {
        items.push(
          ...v.selectedOptions.filter((f) => f.name == i).map((m) => m.value)
        );
      }
      result.push({
        name: i,
        options: Array.from(new Set(items)),
      });
    }
  } else {
    for (const v of props.variants) {
      for (const o of v.selectedOptions) {
        let item = result.find((f) => f.name == o.name);
        if (!item) {
          item = { name: o.name, options: [] };
          result.push(item);
        }
        if (!item.options.includes(o.value)) {
          item.options.push(o.value);
        }
      }
    }
  }

  return result;
});

function getVariantOption(item: OptionGroup) {
  let variantOption = props.variantOptions.find((f) => f.name == item.name);
  if (!variantOption) {
    variantOption = {
      name: item.name,
      type: "text",
      multilingual: {},
      items: [],
    };
    props.variantOptions.push(variantOption);
  }
  for (const i of item.options) {
    if (!variantOption.items.find((f) => f.name == i)) {
      variantOption.items.push({
        name: i,
        type: "text",
        multilingual: {},
      });
    }
  }

  return variantOption;
}

function deleteOptionItem(group: OptionGroup, index: number) {
  const variantOption = getVariantOption(group);
  const option = group.options[index];
  variantOption.items = variantOption.items.filter((f) => f.name != option);
  emit("delete-option-item", group.name, option);
}

function deleteOption(group: OptionGroup) {
  const index = props.variantOptions.findIndex((f) => f.name == group.name);
  props.variantOptions.splice(index, 1);
  emit("delete-option", group.name);
}

function changeOptionItem(
  group: OptionGroup,
  oldItem: string,
  newItem: string
) {
  emit("update-option-item", group.name, oldItem, newItem);
}

function addOptionItem(group: OptionGroup, value: string) {
  const variantOption = getVariantOption(group);
  variantOption.items.push({
    name: value,
    type: "text",
    multilingual: {},
  });
  emit("add-option-item", group.name, value);
}

function changeName(group: OptionGroup, value: string) {
  if (!value) return;
  if (list.value.find((f) => f.name == value)) {
    errorMessage(t("common.valueExist"));
  } else {
    getVariantOption(group).name = value;
    emit("update-option-name", group.name, value);
  }
}
</script>

<template>
  <div class="space-y-8 w-full">
    <div v-for="(item, index) of list" :key="index">
      <OptionEditor
        :model="item"
        :variant-option="getVariantOption(item)"
        :editing="!item.name"
        @change-name="changeName(item, $event)"
        @change-option="(o, n) => changeOptionItem(item, o, n)"
        @add-option="addOptionItem(item, $event)"
        @delete-option="deleteOptionItem(item, $event)"
        @delete="deleteOption(item)"
      />
    </div>

    <IconButton
      circle
      class="text-blue"
      icon="icon-a-addto"
      :tip="t('common.add')"
      @click="emit('add-option', '')"
    />
  </div>
</template>
