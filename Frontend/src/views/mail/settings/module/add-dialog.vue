<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import EditForm from "./edit-form.vue";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { createMailModule } from "@/api/mail/mail-module";

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

defineProps<{ modelValue: boolean }>();

const { t } = useI18n();
const show = ref(true);
const form = ref();
const model = ref({
  name: "",
});

const onSave = async () => {
  await form.value.validate();
  await createMailModule(model.value.name);
  show.value = false;
  emit("reload");
};
</script>

<template>
  <el-dialog
    :model-value="show"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.create')"
    @closed="emit('update:modelValue', false)"
  >
    <EditForm ref="form" :model="model" @save="onSave" />
    <template #footer>
      <DialogFooterBar @confirm="onSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
