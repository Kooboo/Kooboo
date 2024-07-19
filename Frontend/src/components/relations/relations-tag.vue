<script lang="ts" setup>
import { computed, ref } from "vue";
import RelationsDialog from "./relations-dialog.vue";

import { useI18n } from "vue-i18n";
const props = defineProps<{
  relations: Record<string, number>;
  id: string;
  type: string;
}>();
const { t } = useI18n();

const showDialog = ref(false);
const by = ref<string>("menu");
const displayName = ref();

const defines = [
  {
    name: "menu",
    display: t("common.menu"),
    type: "warning",
  },
  {
    name: "view",
    display: t("common.view"),
    type: "success",
  },
  {
    name: "layout",
    display: t("common.layout"),
    type: "",
  },
  {
    name: "page",
    display: t("common.page"),
    type: "success",
  },
  {
    name: "htmlBlock",
    display: t("common.htmlBlock"),
    type: "danger",
  },
  {
    name: "dataMethodSetting",
    display: t("common.dataMethodSetting"),
    type: "",
  },
  {
    name: "resourceGroup",
    display: t("common.resourceGroup"),
    type: "warning",
  },
  {
    name: "route",
    display: t("common.route"),
    type: "warning",
  },
];

const list = computed(() => {
  const result = [];
  if (!props.relations) return;
  for (const relation of Object.keys(props.relations)) {
    const value = props.relations[relation];
    const found = defines.find((f) => f.name === relation);
    if (found) {
      result.push({ ...found, value });
    } else {
      result.push({
        name: relation,
        display: relation,
        type: "",
        value,
      });
    }
  }

  return result;
});

const onShowDialog = (value: string) => {
  displayName.value = "";
  if (value === "textContent") {
    displayName.value = "content";
  } else if (value === "cmsCssRule") {
    displayName.value = "style";
  }
  by.value = value;
  showDialog.value = true;
};
</script>

<template>
  <div class="flex flex-wrap">
    <el-tag
      v-for="item of list"
      :key="item.name"
      size="small"
      class="rounded-full cursor-pointer my-4 mr-4"
      :type="(item.type as any)"
      data-cy="relation-tag"
      @click="onShowDialog(item.name)"
      >{{ item.value }} {{ item.display }}</el-tag
    >
  </div>
  <teleport to="body">
    <RelationsDialog
      v-if="showDialog"
      :id="id"
      v-model="showDialog"
      :by="by"
      :display-name="displayName"
      :type="type"
    />
  </teleport>
</template>
