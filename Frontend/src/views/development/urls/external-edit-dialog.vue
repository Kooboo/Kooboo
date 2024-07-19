<script lang="ts" setup>
import { updateUrl } from "@/api/url";
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

const model = ref({
  type: "external",
  id: props.id,
  value: props.value,
});

const onSave = async () => {
  await form.value?.validate();
  await updateUrl(model.value);
  show.value = false;
  emit("reload");
};
</script>

<template>
  <el-dialog
    :model-value="show"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.editURL')"
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
