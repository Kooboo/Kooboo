<template>
  <el-form-item
    v-if="model[actionTypeName] === 'openPage'"
    :label="t('common.page')"
    prop="pageLink"
    :rules="valueRules"
  >
    <el-select v-model="model['pageLink']">
      <el-option
        v-for="page in pages"
        :key="page.value"
        :label="page.key"
        :value="page.value"
      />
    </el-select>
  </el-form-item>
  <template v-else-if="model[actionTypeName] === 'sendEmail'">
    <el-form-item :label="t('mail.to')" prop="emailTo" :rules="emailRules">
      <el-input v-model="model['emailTo']" />
    </el-form-item>
    <el-form-item :label="t('common.subject')" prop="emailSubject">
      <el-input v-model="model['emailSubject']" />
    </el-form-item>
    <el-form-item prop="emailBody">
      <el-input
        v-model="model['emailBody']"
        :placeholder="t('common.emailBody')"
        type="textarea"
      />
    </el-form-item>
  </template>
  <el-form-item
    v-else-if="model[actionTypeName] === 'callPhone'"
    prop="phoneNumber"
    :label="t('common.phoneNumber')"
    :rules="phoneRules"
  >
    <el-input v-model="model['phoneNumber']" />
  </el-form-item>
  <el-form-item
    v-else-if="model[actionTypeName] !== 'unsubscribe'"
    :label="field.displayName"
    :prop="field.name"
  >
    <el-input v-model="model[field.name]" />
  </el-form-item>
  <el-form-item
    v-if="!['sendEmail', 'callPhone'].includes(actionType) && !isClassic"
    :label="t('ve.linkTarget')"
    prop="linkTarget"
  >
    <el-select v-model="model['linkTarget']">
      <el-option :label="t('ve.currentTab')" value="_self" />
      <el-option :label="t('ve.newTab')" value="_blank" />
    </el-select>
  </el-form-item>
</template>

<script lang="ts" setup>
import type { Field } from "@/components/field-control/types";
import { computed, inject, onMounted, ref } from "vue";
import { get } from "lodash-es";
import type { KeyValue } from "@/global/types";
import { useI18n } from "vue-i18n";
import { emailRule, phoneRule, requiredRule } from "@/utils/validate";
import type { Rule } from "async-validator";
const isClassic = inject<boolean>("is-classic");

const props = defineProps<{
  model: Record<string, any>;
  field: Field;
  cssClass?: any;
}>();

const { t } = useI18n();

const emailRules = [
  emailRule,
  requiredRule(t("common.inputEmailTips")),
] as Rule;

const phoneRules = [
  phoneRule,
  requiredRule(t("common.inputPhoneTips")),
] as Rule;

const valueRules = [requiredRule(t("common.pageUrlRequiredTips"))] as Rule;

const getPages = inject<() => KeyValue[]>("ve-get-pages");
const pages = ref<KeyValue[]>([]);
onMounted(() => {
  if (typeof getPages === "function") {
    pages.value = getPages();
  }
});

const actionType = computed(() => props.model[actionTypeName.value]);

const actionTypeName = computed(() =>
  get(props.field, 'settings["data-type"]', "actionType")
);
</script>
