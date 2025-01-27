<script lang="ts" setup>
import { ref } from "vue";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { useI18n } from "vue-i18n";
import { save } from "@/views/system/settings/settings";
import RequestAccessLimit from "@/views/system/settings/request-access-limit.vue";
import { load } from "@/views/system/settings/settings";

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

load();
const { t } = useI18n();
const show = ref(true);

const onSave = async () => {
  show.value = false;
  save();
};
</script>

<template>
  <div>
    <el-dialog
      :model-value="show"
      width="600px"
      :close-on-click-modal="false"
      :title="t('common.requestAccessLimit')"
      @closed="emit('update:modelValue', false)"
    >
      <el-form label-position="top">
        <RequestAccessLimit />
      </el-form>
      <template #footer>
        <DialogFooterBar
          :confirm-label="t('common.save')"
          @confirm="onSave"
          @cancel="show = false"
        />
      </template>
    </el-dialog>
  </div>
</template>
