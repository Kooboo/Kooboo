<script lang="ts" setup>
import { onBeforeMount, ref } from "vue";
import type { Rules } from "async-validator";
import { createSub, updateInfo } from "@/api/menu";
import { rangeRule, requiredRule } from "@/utils/validate";
import MultilingualSelector from "@/components/multilingual-selector/index.vue";
import type { Menu } from "@/api/menu/types";
import { useMultilingualStore } from "@/store/multilingual";
import { useI18n } from "vue-i18n";
import { computed } from "@vue/reactivity";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import type { ResolvedRoute } from "@/api/route/types";
import { routesByType } from "@/api/route";

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

const props = defineProps<{ modelValue: boolean; menu: Menu }>();
const { t } = useI18n();
const routes = ref<ResolvedRoute[]>([]);

const rules = {
  name: [requiredRule(t("common.nameRequiredTips")), rangeRule(1, 50)],
  url: [],
} as Rules;

routesByType("Page").then((rsp) => (routes.value = rsp));
const multilingualStore = useMultilingualStore();
const show = ref(true);
const form = ref();

const values = ref<Record<string, string>>(props.menu.values);

onBeforeMount(() => {
  if (!(multilingualStore.default in values.value)) {
    values.value[multilingualStore.default] = props.menu.name;
  }
});

const model = ref({
  rootId: props.menu.rootId,
  parentId: props.menu.parentId,
  name: props.menu.name,
  url: props.menu.url,
  id: props.menu.id,
});

const validModel = computed(() => {
  if (!model.value) return;
  return { ...model.value, ...values.value };
});

const onSave = async () => {
  await form.value.validate();
  var body = { ...model.value, Values: JSON.stringify(values.value) };
  if (!body.url) body.url = "#";
  body.name = values.value[multilingualStore.default];
  if (model.value.id) {
    await updateInfo(body);
  } else {
    await createSub(body);
  }
  show.value = false;
  emit("reload");
};

const getNameLabel = (value: string) => {
  if (value === multilingualStore.default) {
    return multilingualStore.selected.length > 1
      ? t("common.name") + " - " + value + " (" + t("common.default") + ")"
      : t("common.name");
  } else return t("common.name") + " - " + value;
};
</script>

<template>
  <el-dialog
    :model-value="show"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.menuItem')"
    @closed="emit('update:modelValue', false)"
  >
    <div class="flex justify-end">
      <MultilingualSelector />
    </div>
    <el-form
      ref="form"
      label-position="top"
      :model="validModel"
      :rules="rules"
      @keydown.enter="onSave"
    >
      <el-form-item
        v-for="item of multilingualStore.selected"
        :key="item"
        :label="getNameLabel(item)"
        :rules="rules.name"
        :prop="item"
      >
        <el-input v-model="values[item]" data-cy="name" />
      </el-form-item>
      <el-form-item label="URL" prop="url">
        <el-select
          v-model="model.url"
          filterable
          allow-create
          default-first-option
          class="w-full"
          data-cy="url-dropdown"
        >
          <el-option
            v-for="item of routes"
            :key="item.id"
            :value="item.name"
            :label="item.name"
            data-cy="url-opt"
          />
        </el-select>
      </el-form-item>
    </el-form>

    <template #footer>
      <DialogFooterBar @confirm="onSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
