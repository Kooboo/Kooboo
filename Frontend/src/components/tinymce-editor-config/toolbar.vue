<template>
  <div class="px-16 pb-8">
    <span class="text-12px text-999 pb-8">
      {{ t("common.richEditorToolbarConfigurationTips") }}
    </span>
    <el-row>
      <el-col v-for="item of checkList" :key="item" :span="6">
        <el-checkbox-group v-model="configs">
          <el-checkbox
            :key="item"
            :label="item"
            :disabled="
              configs.length === 1 && configs[0] === item ? true : false
            "
          >
            <span
              class="ellipsis max-w-120px inline-block"
              :title="t(`tinymce.${item}`)"
              >{{ t(`tinymce.${item}`) }}</span
            >
          </el-checkbox>
        </el-checkbox-group>
      </el-col>
    </el-row>
  </div>
</template>

<script lang="ts" setup>
import { onMounted, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { isEqual } from "lodash-es";

const { t } = useI18n();
interface PropsType {
  modelValue?: string;
}

const props = defineProps<PropsType>();

interface EmitType {
  (e: "update:modelValue", data: string): void;
}

const emit = defineEmits<EmitType>();

type ToolbarOptionItem = {
  name: string;
  items: string[];
};

const options = ref<ToolbarOptionItem[]>([
  { name: "history", items: ["undo", "redo"] },
  {
    name: "styles",
    items: [
      "fontselect",
      "formatselect",
      "fontsizeselect",
      "bold",
      "italic",
      "forecolor",
      "backcolor",
      "removeformat",
    ],
  },
  { name: "indentation", items: ["indent", "outdent"] },
  {
    name: "alignment",
    items: ["alignleft", "aligncenter", "alignright", "alignjustify"],
  },
  {
    name: "list",
    items: ["bullist", "numlist"],
  },
  {
    name: "insert",
    items: ["image", "link"],
  },
  {
    name: "code",
    items: ["codesample", "code"],
  },
]);

const checkList = options.value.map((m) => m.items).flat();
const configs = ref<string[]>([]);

function generateString(
  options: ToolbarOptionItem[],
  checkItems: string[]
): string {
  const selectedItems: string[] = [];
  options.forEach(({ items }) => {
    const groupItems = [];
    for (const item of items) {
      if (checkItems.includes(item)) {
        groupItems.push(item);
      }
    }
    if (groupItems.length) {
      selectedItems.push(groupItems.join(" "));
    }
  });

  return selectedItems.join(" | ");
}

function initSelection() {
  if (props.modelValue) {
    configs.value = props.modelValue.split(" ").filter((it) => it !== "|");
  } else {
    configs.value = checkList;
  }
}

onMounted(initSelection);

watch(() => props.modelValue, initSelection);

watch(
  () => configs.value,
  () => {
    if (isEqual(configs.value, checkList)) {
      emit("update:modelValue", "");
      return;
    }
    const value = generateString(options.value, configs.value);
    emit("update:modelValue", value);
  }
);
</script>
