import type { ProductVariant, Option } from "@/api/commerce/product";
import { i18n } from "@/modules/i18n";
import { newGuid } from "@/utils/guid";
import { ref } from "vue";

const t = i18n.global.t;

export const controls = [
  {
    key: "text",
    value: t("common.text"),
  },
  {
    key: "color",
    value: t("common.color"),
  },
];

export function createNewVariant(
  selectedOptions: Option[],
  defaultImage: string
): ProductVariant {
  return {
    id: newGuid(),
    active: true,
    barcode: "",
    originalInventory: 0,
    newInventory: 0,
    selectedOptions: selectedOptions,
    price: 0,
    sku: "",
    weight: 0,
    image: defaultImage,
    tags: [],
    digitalItems: [],
    autoDelivery: true,
    order: 0,
  };
}

export function getVariantOptions(variants: ProductVariant[], name: string) {
  const result: string[] = [];
  for (const i of variants) {
    for (const o of i.selectedOptions) {
      if (o.name != name) continue;
      if (result.includes(o.value)) continue;
      result.push(o.value);
    }
  }
  return result;
}

export function buildOptionsDisplay(options: Option[], hiddenName = false) {
  if (!options?.length) return "-";
  return options
    .map((m) => {
      return hiddenName ? m.value : `${m.name}: ${m.value}`;
    })
    .join(" / ");
}

export function useVariants() {
  const options = ref<string[]>([]);
  const list = ref<ProductVariant[]>([]);

  function addVariant(variant: ProductVariant) {
    list.value.push(variant);
    for (const i of variant.selectedOptions) {
      if (options.value.includes(i.name)) continue;
      options.value.push(i.name);
    }
  }

  function addOptionItem(name: string, option: string, defaultImage: string) {
    const append: Option[][] = [];

    for (const item of list.value) {
      const found = item.selectedOptions.find((f) => f.name == name);
      if (found) {
        const newItemOptions = JSON.parse(JSON.stringify(item.selectedOptions));
        newItemOptions.find((f: Option) => f.name == name).value = option;
        const newItemOptionsDisplay = buildOptionsDisplay(newItemOptions);
        if (
          !append.find((f) => buildOptionsDisplay(f) == newItemOptionsDisplay)
        ) {
          append.push(newItemOptions);
        }
      } else {
        item.selectedOptions.push({ name: name, value: option });
      }
    }

    for (const i of append) {
      list.value.push(createNewVariant(i, defaultImage));
    }
  }

  function updateOptionName(oldName: string, newName: string) {
    const index = options.value.findIndex((f) => f == oldName);
    options.value.splice(index, 1, newName);

    for (const i of list.value) {
      const optionItem = i.selectedOptions.find((f) => f.name == oldName);
      if (optionItem) optionItem.name = newName;
    }
  }

  function updateOptionItem(option: string, oldItem: string, newItem: string) {
    for (const i of list.value) {
      const optionItem = i.selectedOptions.find((f) => f.name == option);
      if (!optionItem) continue;
      if (optionItem.value == oldItem) optionItem.value = newItem;
    }
  }

  function deleteOptionItem(option: string, value: string) {
    const deleteMode = list.value.find((f) =>
      f.selectedOptions.find((o) => o.name == option && o.value != value)
    );

    for (const i of [...list.value]) {
      const optionIndex = i.selectedOptions.findIndex((f) => f.name == option);
      if (optionIndex == -1) continue;
      if (i.selectedOptions[optionIndex].value == value) {
        if (deleteMode) {
          const index = list.value.findIndex((f) => f == i);
          list.value.splice(index, 1);
        } else {
          i.selectedOptions.splice(optionIndex, 1);
        }
      }
    }
  }

  function deleteOption(option: string) {
    const index = options.value.indexOf(option);
    options.value.splice(index, 1);

    for (const i of [...list.value]) {
      const optionItem = i.selectedOptions.find((f) => f.name == option);
      if (!optionItem) continue;
      deleteOptionItem(optionItem.name, optionItem.value);
    }
  }

  function addOption(value: string) {
    options.value.push(value);
  }

  function removeVariant(id: string) {
    const index = list.value.findIndex((f) => f.id == id);
    list.value.splice(index, 1);
  }

  return {
    options,
    list,
    updateOptionName,
    updateOptionItem,
    addOptionItem,
    addVariant,
    deleteOptionItem,
    deleteOption,
    addOption,
    removeVariant,
  };
}
