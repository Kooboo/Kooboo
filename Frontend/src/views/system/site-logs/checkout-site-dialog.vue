<script lang="ts" setup>
import { ref, watch } from "vue";
import type { Rules } from "async-validator";
import type { AvailableDomain } from "@/api/console/types";
import { getAvailableDomain } from "@/api/console";
import {
  subDomainRule,
  isUniqueNameRule,
  rangeRule,
  requiredRule,
  simpleNameRule,
  domainBindingIsUniqueNameRule,
} from "@/utils/validate";
import { checkout } from "@/api/site-log";
import { isUniqueName } from "@/api/site";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

import { useI18n } from "vue-i18n";
const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

const props = defineProps<{ modelValue: boolean; id: number }>();
const { t } = useI18n();
const domains = ref<AvailableDomain[]>();

const show = ref(true);
const form = ref();
const model = ref({
  id: props.id,
  siteName: "",
  subDomain: "",
  rootDomain: "",
});
const isSubDomainEdit = ref(false);
const isSiteNameEdit = ref(false);

const rules = {
  subDomain: [
    requiredRule(t("common.subDomainRequiredTips")),
    rangeRule(1, 63),
    subDomainRule,
    domainBindingIsUniqueNameRule(model.value),
  ],
  siteName: [
    rangeRule(1, 50),
    requiredRule(t("common.nameRequiredTips")),
    simpleNameRule(),
    isUniqueNameRule(isUniqueName, t("common.siteNameExistsTips")),
  ],
  rootDomain: requiredRule(t("common.rootDomainRequiredTips")),
} as Rules;

getAvailableDomain().then((rsp) => {
  domains.value = rsp;

  if (domains.value.length) {
    model.value.rootDomain = domains.value[0].domainName;
  }
});

const onSave = async () => {
  await form.value.validate();
  await checkout(model.value);
  show.value = false;
};

watch(
  () => model.value.subDomain,
  () => {
    if (isSubDomainEdit.value) return;
    model.value.siteName = model.value.subDomain;
  }
);

watch(
  () => model.value.siteName,
  () => {
    if (isSiteNameEdit.value) return;
    model.value.subDomain = model.value.siteName;
  }
);
</script>

<template>
  <el-dialog
    :model-value="show"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.checkoutSite')"
    @closed="emit('update:modelValue', false)"
  >
    <el-form
      ref="form"
      label-position="top"
      :model="model"
      :rules="rules"
      @keydown.enter="onSave"
    >
      <el-form-item :label="t('common.siteName')" prop="siteName">
        <el-input
          v-model="model.siteName"
          class="w-255px"
          data-cy="siteName"
          @input="isSubDomainEdit = true"
        />
      </el-form-item>

      <div class="flex items-center justify-between">
        <el-form-item
          :label="t('common.domain')"
          prop="subDomain"
          class="w-255px"
        >
          <el-input
            v-model="model.subDomain"
            data-cy="subdomain"
            @input="isSiteNameEdit = true"
          />
        </el-form-item>
        <el-form-item prop="rootDomain" class="mt-30px">
          <el-select v-model="model.rootDomain" class="flex-1">
            <el-option
              v-for="item of domains"
              :key="item.domainName"
              :value="item.domainName"
              :label="'.' + item.domainName"
              data-cy="root-domain-opt"
            />
          </el-select>
        </el-form-item>
      </div>
    </el-form>

    <template #footer>
      <DialogFooterBar
        :confirm-label="t('common.start')"
        @confirm="onSave"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
<style scoped>
:deep(.el-input__wrapper) {
  width: 270px;
}
</style>
