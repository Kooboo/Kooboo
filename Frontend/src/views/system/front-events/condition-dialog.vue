<script lang="ts" setup>
import { ref } from "vue";
import { getConditionOptions } from "@/api/events";
import type { Option, Condition } from "@/api/events/types";
import { getQueryString } from "@/utils/url";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

import { useI18n } from "vue-i18n";
const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

const props = defineProps<{ modelValue: boolean; conditions: Condition[] }>();
const { t } = useI18n();
const model = ref<Condition[]>(JSON.parse(JSON.stringify(props.conditions)));

const show = ref(true);
const options = ref<Option[]>([]);
getConditionOptions(getQueryString("name")!).then((r) => {
  options.value = r;
  if (!model.value.length) {
    onAdd();
  }
});

const onAdd = async () => {
  model.value.push({
    left: options.value[0].name,
    operator: options.value[0].operator[0],
    right: "",
  });
};

const onSave = async () => {
  props.conditions.splice(0, props.conditions.length);
  props.conditions.push(...model.value);
  show.value = false;
};
</script>

<template>
  <el-dialog
    :model-value="show"
    custom-class="el-dialog--zero-padding"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.editCondition')"
    @closed="emit('update:modelValue', false)"
  >
    <div>
      <div class="p-24">
        <div class="space-y-4">
          <div
            v-for="(item, index) of model"
            :key="index"
            class="flex items-center space-x-4"
          >
            <el-select
              v-model="item.left"
              class="w-280px"
              data-cy="condition-left"
            >
              <el-option
                v-for="option of options"
                :key="option.name"
                :value="option.name"
                :label="option.name"
                :data-cy="option.name"
              />
            </el-select>
            <el-select
              v-model="item.operator"
              class="w-300px"
              data-cy="condition-operator"
            >
              <el-option
                v-for="option of options.find((f) => f.name === item.left)
                  ?.operator"
                :key="option"
                :value="option"
                :label="option"
                :data-cy="option"
              />
            </el-select>

            <el-input
              v-model="item.right"
              :placeholder="t('common.value')"
              data-cy="condition-right"
              @keydown.enter="onSave"
            />
            <div>
              <el-button
                circle
                data-cy="remove-condition"
                @click="model.splice(index, 1)"
              >
                <el-icon class="iconfont icon-delete text-orange" />
              </el-button>
            </div>
          </div>
          <el-button circle data-cy="add-condition" @click="onAdd">
            <el-icon class="iconfont icon-a-addto text-blue" />
          </el-button>
        </div>
      </div>
    </div>
    <template #footer>
      <DialogFooterBar @confirm="onSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
