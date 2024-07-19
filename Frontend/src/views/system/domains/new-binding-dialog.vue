<script lang="ts" setup>
import { ref, watch } from "vue";
import type { KeyValue } from "@/global/types";
import type { AvailableDomain } from "@/api/console/types";
import { getAvailableDomain } from "@/api/console";
import { useAppStore } from "@/store/app";
import type { Rules } from "async-validator";
import { post } from "@/api/binding";
import {
  putIntegerNumberRule,
  portRule,
  rangeRule,
  domainBindingIsUniqueNameRule,
  requiredRule,
} from "@/utils/validate";
import { useSiteStore } from "@/store/site";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

import { useI18n } from "vue-i18n";
import { useBindingStore } from "@/store/binding";
const { t } = useI18n();

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

const types: KeyValue[] = [
  {
    key: "domain",
    value: t("common.domain"),
  },
  {
    key: "Port",
    value: t("common.port"),
  },
];

const appStore = useAppStore();
defineProps<{ modelValue: boolean }>();
const show = ref(true);
const currentType = ref(types[0].key);
const domains = ref<AvailableDomain[]>();
const form = ref();
const siteStore = useSiteStore();
const bindingStore = useBindingStore();
const showAdvanceSettings = ref(false);

getAvailableDomain().then((r) => {
  domains.value = r;
  model.value.rootDomain = r[0]?.domainName;
});

const model = ref({
  subDomain: "",
  rootDomain: "",
  port: 81,
  defaultBinding: false,
  redirect: "",
  culture: "",
});

const rules = {
  subDomain: [
    {
      pattern: /^([A-Za-z0-9\*][A-Za-z0-9\-\*]{0,})*[A-Za-z0-9\*]$/,
      message: t("common.subDomainInvalidTips"),
    },
    rangeRule(1, 63),
    domainBindingIsUniqueNameRule(model.value),
  ],
  rootDomain: requiredRule(t("common.rootDomainRequiredTips")),
  port: [portRule(), putIntegerNumberRule()],
} as Rules;

const onSave = async () => {
  await form.value.validate();
  await post(model.value);
  show.value = false;
  siteStore.loadSite();
  emit("reload");
};

watch(
  () => currentType.value,
  (value) => {
    model.value.defaultBinding = value === types[1].key;
  }
);

const onBlurAndSave = (e: any) => {
  e.target.blur();
};
</script>

<template>
  <el-dialog
    :model-value="show"
    custom-class="el-dialog--zero-padding"
    width="644px"
    :close-on-click-modal="false"
    :title="t('common.newBinding')"
    @closed="emit('update:modelValue', false)"
  >
    <div class="p-24 pl-32">
      <el-form
        v-if="appStore.header"
        ref="form"
        :model="model"
        :rules="rules"
        @keydown.enter="onSave"
        @submit.prevent
      >
        <el-form-item
          v-if="!appStore.header.isOnlineServer"
          :label="t('common.bindingTo')"
          label-width="100px"
        >
          <el-radio-group v-model="currentType" class="el-radio-group--rounded">
            <el-radio-button
              v-for="item of types"
              :key="item.key"
              :label="item.key"
              :data-cy="item.key"
              >{{ item.value }}</el-radio-button
            >
          </el-radio-group>
        </el-form-item>

        <div class="flex items-center space-x-8">
          <el-form-item
            v-if="currentType === types[0].key"
            prop="subDomain"
            :label="t('common.domain')"
            label-width="100px"
          >
            <el-input v-model="model.subDomain" data-cy="subdomain" />
          </el-form-item>
          <el-form-item v-if="currentType === types[0].key" prop="rootDomain">
            <el-select v-model="model.rootDomain" class="w-full">
              <el-option
                v-for="item of domains"
                :key="item.domainName"
                :label="'.' + item.domainName"
                :value="item.domainName"
                data-cy="root-domain-opt"
              />
            </el-select>
          </el-form-item>
        </div>

        <el-form-item
          v-if="currentType === types[1].key"
          prop="port"
          :label="t('common.port')"
          label-width="100px"
        >
          <el-input-number
            v-model="model.port"
            @keydown.enter="onBlurAndSave"
          />
        </el-form-item>

        <el-tooltip
          placement="top"
          :content="
            !showAdvanceSettings
              ? t('common.showAdvanceSettings')
              : t('common.hideAdvanceSettings')
          "
        >
          <div
            class="flex items-center justify-center h-40px cursor-pointer bg-card dark:bg-444 hover:bg-[#eff6ff] dark:hover:bg-444"
            data-cy="show-advance-settings"
            @click="showAdvanceSettings = !showAdvanceSettings"
          >
            <el-icon
              class="iconfont icon-pull-down text-s leading-none cursor-pointer transform origin-center transition duration-200 dark:text-fff/86"
              :class="showAdvanceSettings ? 'rotate-180' : 'rotate-0'"
            />
          </div>
        </el-tooltip>

        <template v-if="showAdvanceSettings">
          <el-form-item
            class="mt-16"
            :label="t('common.redirectTo')"
            label-width="100px"
          >
            <el-select v-model="model.redirect" class="w-full" clearable>
              <el-option
                v-for="item of bindingStore.bindings"
                :key="item.id"
                :value="item.fullName"
                :label="item.fullName"
              />
            </el-select>
          </el-form-item>

          <el-form-item :label="t('common.culture')" label-width="100px">
            <el-select v-model="model.culture" class="w-full" clearable>
              <el-option
                v-for="(value, key) of siteStore.site.culture"
                :key="key"
                :value="key"
                :label="value"
              />
            </el-select>
          </el-form-item>
        </template>
      </el-form>
    </div>
    <template #footer>
      <DialogFooterBar @confirm="onSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
<style scoped>
:deep(.el-input__wrapper) {
  width: 240px;
}
</style>
