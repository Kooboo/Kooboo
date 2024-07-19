<script lang="ts" setup>
import { ref } from "vue";
import EditGroup from "./edit-group.vue";

import { useI18n } from "vue-i18n";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

defineProps<{ modelValue: boolean; id?: string }>();
const { t } = useI18n();
const show = ref(true);
const inner = ref();

const onSave = async () => {
  await inner.value.save();
  show.value = false;
};
</script>

<template>
  <div @click.stop>
    <el-dialog
      :model-value="show"
      custom-class="el-dialog--zero-padding"
      width="600px"
      :close-on-click-modal="false"
      :title="t('common.scriptGroup')"
      @closed="emit('update:modelValue', false)"
    >
      <EditGroup :id="id" ref="inner" class="p-32" @save="onSave" />
      <template #footer>
        <DialogFooterBar
          :permission="{ feature: 'script', action: 'edit' }"
          @confirm="onSave"
          @cancel="show = false"
        />
      </template>
    </el-dialog>
  </div>
</template>
