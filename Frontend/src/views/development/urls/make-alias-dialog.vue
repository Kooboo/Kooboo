<script lang="ts" setup>
import { makeAlias } from "@/api/url";
import { ref } from "vue";
import { requiredRule } from "@/utils/validate";

import { useI18n } from "vue-i18n";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

const props = defineProps<{
  modelValue: boolean;
  id: string;
  value: string;
}>();
const { t } = useI18n();

const show = ref(true);
const form = ref();

function appendAlias(value: string) {
  const dotIndex = value.lastIndexOf(".");
  if (dotIndex > 0) {
    return value.slice(0, dotIndex) + "_alias" + value.slice(dotIndex);
  } else {
    return value + "_alias";
  }
}

const model = ref({
  id: props.id,
  value: appendAlias(props.value),
});

const onSave = async () => {
  await form.value?.validate();
  await makeAlias(model.value);
  show.value = false;
  emit("reload");
};
</script>

<template>
  <el-dialog
    :model-value="show"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.makeAlias')"
    @closed="emit('update:modelValue', false)"
  >
    <el-form ref="form" label-position="top" :model="model" @submit.prevent>
      <el-form-item
        label="URL"
        prop="value"
        :rules="[requiredRule(t('common.urlRequiredTips'))]"
      >
        <el-input
          v-model="model.value"
          data-cy="url-input"
          @keydown.enter="onSave"
          @input="model.value = model.value.replace(/\s+/g, '')"
        />
        <input type="hidden" />
      </el-form-item>
    </el-form>

    <template #footer>
      <DialogFooterBar
        :permission="{ feature: 'link', action: 'edit' }"
        @confirm="onSave"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
