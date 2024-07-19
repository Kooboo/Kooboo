<script lang="ts" setup>
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { getList } from "@/api/console";
import type { Domain } from "@/api/console/types";
import { addSite } from "@/api/search-console";

const domainList = ref<Domain[]>([]);
const props = defineProps<{ modelValue: boolean; excludes: string[] }>();
const { t } = useI18n();
const show = ref(true);
const form = ref();
const model = ref({
  url: "",
});

getList().then((rsp) => (domainList.value = rsp));

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

const onSave = async () => {
  if (!model.value.url) return;
  await addSite(model.value.url);
  emit("reload");
  show.value = false;
};

const list = computed(() => {
  var result = [];

  for (const domain of domainList.value) {
    var item = `sc-domain:${domain.domainName}`;
    if (props.excludes.find((f) => f == item)) {
      continue;
    }
    result.push(item);
  }

  return result;
});
</script>

<template>
  <el-dialog
    :model-value="show"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.addSite')"
    @closed="emit('update:modelValue', false)"
  >
    <el-form
      ref="form"
      label-position="top"
      :model="model"
      @keydown.enter="onSave"
    >
      <el-form-item :label="t('common.site')" prop="name">
        <ElSelect v-model="model.url" class="w-full">
          <ElOption
            v-for="item of list"
            :key="item"
            :label="item"
            :value="item"
          />
        </ElSelect>
      </el-form-item>
    </el-form>
    <template #footer>
      <DialogFooterBar
        :disabled="!model.url"
        @confirm="onSave"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
