<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";

const props = defineProps<{ value: string }>();
const emit = defineEmits<{
  (e: "finish", changed: boolean, value: string): void;
}>();

const { t } = useI18n();
const show = ref(true);
const content = ref(props.value);
let changed = ref(false);

const save = () => {
  changed.value = props.value != content.value;
  show.value = false;
};
</script>

<template>
  <div>
    <el-dialog
      :model-value="show"
      width="600px"
      :close-on-click-modal="false"
      :title="t('common.text')"
      draggable
      @closed="emit('finish', changed, content)"
    >
      <el-input v-model="content" :rows="4" type="textarea" />
      <template #footer>
        <el-button round @click="show = false">{{
          t("common.cancel")
        }}</el-button>
        <el-button type="primary" round @click="save">{{
          t("common.save")
        }}</el-button>
      </template>
    </el-dialog>
  </div>
</template>
