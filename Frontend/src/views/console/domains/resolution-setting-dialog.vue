<template>
  <el-dialog
    v-model="visible"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.addRecord')"
    @closed="emits('update:modelValue', false)"
  >
    <el-form
      ref="form"
      class="el-form--label-normal"
      :model="model"
      :rules="rules"
      label-position="top"
      @submit.prevent
    >
      <el-form-item :label="t('common.hostRecord')" prop="host">
        <el-input v-model="model.host">
          <template #append>
            {{ model.domain }}
          </template>
        </el-input>
      </el-form-item>
      <el-form-item :label="t('common.recordType')">
        <el-select v-model="model.type" class="w-full" placeholder=" ">
          <el-option
            v-for="item of recordTypeList"
            :key="item"
            :label="item"
            :value="item"
          />
        </el-select>
      </el-form-item>
      <el-form-item prop="value" :label="t('common.recordValue')">
        <el-input v-model="model.value" />
      </el-form-item>
      <el-form-item
        v-if="model.type === 'MX'"
        prop="priority"
        :label="t('common.priority')"
      >
        <el-input v-model="model.priority" />
      </el-form-item>
      <el-form-item prop="TTL" label="TTL">
        <el-select v-model="model.TTL" class="w-full">
          <el-option
            v-for="item of TTLList"
            :key="item.value"
            :label="item.name"
            :value="item.value"
          />
        </el-select>
      </el-form-item>
    </el-form>

    <template #footer>
      <DialogFooterBar
        :confirm-label="t('common.create')"
        @confirm="create"
        @cancel="visible = false"
      />
    </template>
  </el-dialog>
</template>
<script setup lang="ts">
import { ref } from "vue";
import type { ElForm } from "element-plus";
import { hostRecordRule, requiredRule } from "@/utils/validate";
import type { Rules } from "async-validator";

import { useI18n } from "vue-i18n";
import { newGuid } from "@/utils/guid";
import { addDns, getDnsType, getTTL } from "@/api/console";
import type { DNS } from "@/api/console/types";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { useRoute } from "vue-router";

const { t } = useI18n();
const route = useRoute();

interface PropsType {
  modelValue: boolean;
}
interface EmitsType {
  (e: "update:modelValue", value: boolean): void;
  (e: "createSuccess"): void;
}
defineProps<PropsType>();
const emits = defineEmits<EmitsType>();
const visible = ref(true);
const recordDomainName = ref();

const rules = {
  TTL: [requiredRule(t("common.valueRequiredTips"))],
  value: [requiredRule(t("common.valueRequiredTips"))],
  host: [hostRecordRule],
} as Rules;
const form = ref<InstanceType<typeof ElForm>>();

const model = ref<DNS>({
  id: newGuid(),
  domain: recordDomainName.value,
  type: "",
  host: "",
  value: "",
  priority: 0,
  TTL: 10,
});
const TTLList = ref<{ name: string; value: number }[]>();
const recordTypeList = ref<string[]>();
const load = async () => {
  model.value.domain = route.query.domainName as string;
  recordTypeList.value = await getDnsType();
  TTLList.value = await getTTL();
};

const create = async () => {
  await form.value?.validate();
  await addDns(model.value);
  emits("createSuccess");
  visible.value = false;
};
load();
</script>

<style scoped>
:deep(.el-input-group__append) {
  max-width: 200px;
  padding: 0 15px;
  background-color: transparent;
}
</style>
