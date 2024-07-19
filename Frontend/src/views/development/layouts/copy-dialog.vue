<script lang="ts" setup>
import { ref } from "vue";
import { copy } from "@/api/layout";
import type { ListItem } from "@/api/layout/types";
import { useLayoutStore } from "@/store/layout";

import { useI18n } from "vue-i18n";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

const layoutStore = useLayoutStore();
const props = defineProps<{ modelValue: boolean; selected: ListItem }>();
const { t } = useI18n();
const rules = layoutStore.getRules(false);

const show = ref(true);
const form = ref();
const model = ref({
  id: props.selected.id,
  name: props.selected.name + "_Copy",
});

const onSave = async () => {
  await form.value.validate();
  await copy(model.value.id, model.value.name);
  show.value = false;
  layoutStore.load();
};
</script>

<template>
  <el-dialog
    :model-value="show"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.copyLayout')"
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
        <el-input v-model="model.name" data-cy="layout-name" />
      </el-form-item>
    </el-form>

    <template #footer>
      <DialogFooterBar @confirm="onSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
