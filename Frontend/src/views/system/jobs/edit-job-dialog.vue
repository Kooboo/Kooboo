<script lang="ts" setup>
import type { Job } from "@/api/site/job";
import { isUniqueName, post } from "@/api/site/job";
import MonacoEditor from "@/components/monaco-editor/index.vue";
import { emptyGuid } from "@/utils/guid";
import {
  frequenceUnitRule,
  isUniqueNameRule,
  rangeRule,
  requiredRule,
} from "@/utils/validate";
import type { Rules } from "async-validator";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { timeType } from "./job";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { Uri } from "monaco-editor";

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

const props = defineProps<{ modelValue: boolean; job?: Job }>();

const { t } = useI18n();

const rules = {
  name: props.job
    ? []
    : [
        requiredRule(t("common.nameRequiredTips")),
        rangeRule(1, 50),
        isUniqueNameRule(isUniqueName, t("common.jobExists")),
      ],
  codeId: requiredRule(t("common.codeRequiredTips")),
  startTime: requiredRule(t("common.fieldRequiredTips")),
  frequenceUnit: [
    requiredRule(t("common.fieldRequiredTips")),
    frequenceUnitRule(),
  ],
} as Rules;

const show = ref(true);
const form = ref();
const model = ref();

if (props.job) {
  model.value = JSON.parse(JSON.stringify(props.job));
  model.value.startTime += "z";
} else {
  model.value = {
    id: emptyGuid,
    name: "",
    code: `// example
//const log = "Execute at " + new Date().toString()
//k.response.write(log);
    `,
    isRepeat: false,
    frequenceUnit: 1,
    startTime: new Date(),
    frequence: "second",
    active: true,
  };
}

const onSave = async () => {
  await form.value.validate();
  const body = { ...model.value };
  await post(body);
  show.value = false;
  emit("reload");
};
</script>

<template>
  <el-dialog
    :model-value="show"
    width="800px"
    :close-on-click-modal="false"
    :title="t('common.job')"
    @closed="emit('update:modelValue', false)"
  >
    <el-scrollbar class="h-400px">
      <el-form ref="form" :model="model" label-width="120px" :rules="rules">
        <el-form-item :label="t('common.name')" prop="name">
          <el-input
            v-model="model.name"
            :disabled="!!job"
            data-cy="job-name"
            @blur="model.name = model.name.trim()"
          />
        </el-form-item>
        <el-form-item
          :label="t('common.startTime')"
          prop="startTime"
          class="mb-8"
        >
          <el-date-picker
            v-model="model.startTime"
            type="datetime"
            class="!w-full"
          />
        </el-form-item>
        <el-form-item :label="t('common.active')" class="mb-0">
          <el-switch v-model="model.active" data-cy="active" />
        </el-form-item>
        <el-form-item :label="t('common.repeat')" class="mb-8">
          <el-switch v-model="model.repeat" data-cy="repeat" />
        </el-form-item>

        <el-form-item
          v-if="model.repeat"
          :label="t('common.every')"
          prop="frequenceUnit"
        >
          <div class="flex space-x-4">
            <el-input
              v-model.number="model.frequenceUnit"
              :placeholder="t('common.inputIntervalTips')"
              data-cy="frequent"
            />
            <el-select v-model="model.frequence">
              <el-option
                v-for="item of timeType"
                :key="item.id"
                :value="item.id"
                :label="item.display"
                :data-cy="item.name"
              />
            </el-select>
          </div>
        </el-form-item>

        <el-form-item :label="t('common.executeCode')" prop="code">
          <div
            class="p-4 border border-solid border-gray dark:border-[#555] rounded-normal w-full"
          >
            <MonacoEditor
              v-model="model.code"
              class="h-300px w-full"
              language="typescript"
              :uri="Uri.file(model.id)"
              k-script
            />
          </div>
        </el-form-item>
      </el-form>
    </el-scrollbar>
    <template #footer>
      <DialogFooterBar
        :permission="{
          feature: 'job',
          action: 'edit',
        }"
        @confirm="onSave"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
