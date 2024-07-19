<script lang="ts" setup>
import { ref } from "vue";
import type { Rules } from "async-validator";
import {
  isUniqueNameRule,
  simpleNameRule,
  rangeRule,
  requiredRule,
} from "@/utils/validate";
import { isUniqueName } from "@/api/menu";
import { useMenuStore } from "@/store/menu";
import { useI18n } from "vue-i18n";
import type { Menu } from "@/api/menu/types";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "created", value: Menu): void;
}>();

defineProps<{ modelValue: boolean }>();
const { t } = useI18n();

const rules = {
  name: [
    requiredRule(t("common.nameRequiredTips")),
    simpleNameRule(),
    rangeRule(1, 50),
    isUniqueNameRule(isUniqueName, t("common.menuNameExistsTips")),
  ],
} as Rules;

const show = ref(true);
const form = ref();
const menuStore = useMenuStore();
const model = ref({
  name: "",
});

const onSave = async () => {
  await form.value.validate();
  const menu = await menuStore.createMenu(model.value.name);
  emit("created", menu);
  show.value = false;
};
</script>

<template>
  <div @click.stop>
    <el-dialog
      :model-value="show"
      width="600px"
      :close-on-click-modal="false"
      :title="t('common.addMenu')"
      @closed="emit('update:modelValue', false)"
    >
      <div @keydown.enter="onSave">
        <el-form
          ref="form"
          label-position="top"
          :model="model"
          :rules="rules"
          @submit.prevent
        >
          <el-form-item :label="t('common.name')" prop="name">
            <el-input v-model="model.name" data-cy="menu-name" />
          </el-form-item>
        </el-form>
      </div>

      <template #footer>
        <DialogFooterBar @confirm="onSave" @cancel="show = false" />
      </template>
    </el-dialog>
  </div>
</template>
