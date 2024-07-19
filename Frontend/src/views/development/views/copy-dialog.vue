<script lang="ts" setup>
import { ref } from "vue";
import { copy, isUniqueName } from "@/api/view";
import type { View } from "@/api/view/types";
import { useViewStore } from "@/store/view";
import {
  isUniqueNameRule,
  rangeRule,
  requiredRule,
  simpleNameRule,
} from "@/utils/validate";
import type { Rules } from "async-validator";

import { useI18n } from "vue-i18n";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

const viewStore = useViewStore();
const props = defineProps<{ modelValue: boolean; selected: View }>();
const { t } = useI18n();
const show = ref(true);
const form = ref();

const rules = {
  name: [
    rangeRule(1, 50),
    requiredRule(t("common.nameRequiredTips")),
    simpleNameRule(),
    isUniqueNameRule(isUniqueName, t("common.viewNameExistsTips")),
  ],
} as Rules;

const model = ref({
  id: props.selected.id,
  name: props.selected.name + "_Copy",
});

const onSave = async () => {
  await form.value.validate();
  await copy(model.value.id, model.value.name);
  show.value = false;
  viewStore.load();
};
</script>

<template>
  <el-dialog
    :model-value="show"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.copyView')"
    @closed="emit('update:modelValue', false)"
  >
    <el-form
      ref="form"
      label-position="top"
      :model="model"
      :rules="rules"
      @submit.prevent
      @keydown.enter="onSave"
    >
      <el-form-item :label="t('common.name')" prop="name">
        <el-input v-model="model.name" />
      </el-form-item>
    </el-form>

    <template #footer>
      <DialogFooterBar @confirm="onSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
