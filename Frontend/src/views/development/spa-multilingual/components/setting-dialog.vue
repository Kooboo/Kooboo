<template>
  <el-dialog
    v-model="props.modelValue"
    width="650px"
    :close-on-click-modal="false"
    :title="t('common.setting')"
    @close="handleClose"
  >
    <div class="space-y-8 space-x-8 select-none">
      <el-check-tag
        v-for="item in langList"
        :key="item"
        size="large"
        :checked="langs.includes(item)"
        @change="onCheck(item)"
        >{{ item }}</el-check-tag
      >
    </div>
    <template #footer>
      <DialogFooterBar @confirm="handleSave" @cancel="handleClose" />
    </template>
  </el-dialog>
</template>
<script setup lang="ts">
import { Langs, setLang } from "@/api/development/spa-multilingual";
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

interface PropsType {
  modelValue: boolean;
  data: Langs[];
}
interface EmitsType {
  (e: "close"): void;
  (e: "save-success"): void;
}

const props = defineProps<PropsType>();
const emits = defineEmits<EmitsType>();
const { t } = useI18n();
const langs = ref<Langs[]>([]);
const defaultLang = ref<Langs>();
const langList = Object.values(Langs);

watch(
  () => props.data,
  (data) => {
    if (data.length) {
      langs.value = [...data];
      defaultLang.value = data[0];
    }
  },
  { immediate: true, deep: true }
);

const onCheck = (item: Langs) => {
  if (item === defaultLang.value) return;
  if (langs.value.find((f) => f === item)) {
    langs.value = langs.value.filter((f) => f !== item);
  } else {
    langs.value.push(item);
  }
};

const handleClose = () => {
  emits("close");
};

async function handleSave() {
  await setLang(langs.value);
  emits("save-success");
  handleClose();
}
</script>
