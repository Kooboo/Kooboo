<script lang="ts" setup>
import type { VariantOption } from "@/api/commerce/product";
import { useMultilingualStore } from "@/store/multilingual";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

const { t } = useI18n();
const show = ref(true);
const multilingualStore = useMultilingualStore();

const props = defineProps<{
  variantOption: VariantOption;
  modelValue: boolean;
}>();

const copedModel = ref<VariantOption>(
  JSON.parse(JSON.stringify(props.variantOption))
);

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "update:variantOption", value: VariantOption): void;
}>();

function getValue(multilingual: Record<string, string>, lang: string) {
  if (!Object.keys(multilingual).includes(lang)) {
    multilingual[lang] = "";
  }

  return multilingual[lang];
}

function setValue(
  multilingual: Record<string, string>,
  lang: string,
  value: string
) {
  multilingual[lang] = value;
}

function onSave() {
  emit("update:variantOption", copedModel.value);
  show.value = false;
}
</script>

<template>
  <el-dialog
    :model-value="show"
    width="600px"
    :title="t('common.multilingual')"
    :close-on-click-modal="false"
    @closed="emit('update:modelValue', false)"
  >
    <div>
      <div class="flex justify-end">
        <MultilingualSelector />
      </div>
      <el-scrollbar max-height="400px">
        <el-form label-position="top">
          <div class="space-y-8">
            <el-form-item :label="variantOption.name">
              <div class="space-y-4 w-full">
                <ElInput
                  v-for="i of multilingualStore.selected"
                  :key="i"
                  :model-value="getValue(copedModel.multilingual, i)"
                  @update:model-value="
                    setValue(copedModel.multilingual, i, $event)
                  "
                >
                  <template #suffix>{{ i }}</template>
                </ElInput>
              </div>
            </el-form-item>
            <template v-for="(item, index) of copedModel.items" :key="index">
              <el-form-item class="space-y-4" :label="item.name">
                <div class="space-y-4 w-full">
                  <ElInput
                    v-for="i of multilingualStore.selected"
                    :key="i"
                    :model-value="getValue(item.multilingual, i)"
                    @update:model-value="setValue(item.multilingual, i, $event)"
                  >
                    <template #suffix>{{ i }}</template>
                  </ElInput>
                </div>
              </el-form-item>
            </template>
          </div>
        </el-form>
      </el-scrollbar>
    </div>
    <template #footer>
      <DialogFooterBar @confirm="onSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
