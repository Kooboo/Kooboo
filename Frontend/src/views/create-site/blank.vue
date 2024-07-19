<script lang="ts" setup>
import { getAvailableDomain } from "@/api/console";
import { newSite, isUniqueName } from "@/api/site";
import type { AvailableDomain } from "@/api/console/types";
import { ref, watch } from "vue";
import {
  subDomainRule,
  isUniqueNameRule,
  requiredRule,
  rangeRule,
  domainBindingIsUniqueNameRule,
  simpleNameRule,
} from "@/utils/validate";
import type { Rules } from "async-validator";
import { useRouter } from "vue-router";
import { setSitesFolder } from "@/api/site";

import { useI18n } from "vue-i18n";
import { getQueryString } from "@/utils/url";
const { t } = useI18n();
const domains = ref<AvailableDomain[]>();
const router = useRouter();
const isSubDomainEdit = ref(false);
const isSiteNameEdit = ref(false);

const model = ref({
  subDomain: "",
  rootDomain: "",
  siteName: "",
});
const rules = {
  subDomain: [
    requiredRule(t("common.subDomainRequiredTips")),
    rangeRule(1, 63),
    subDomainRule,
    domainBindingIsUniqueNameRule(model.value),
  ],
  rootDomain: requiredRule(t("common.rootDomainRequiredTips")),
  siteName: [
    rangeRule(1, 50),
    requiredRule(t("common.nameRequiredTips")),
    isUniqueNameRule(isUniqueName, t("common.siteNameExistsTips")),
    simpleNameRule(),
  ],
} as Rules;

const form = ref();

getAvailableDomain().then((rsp) => {
  domains.value = rsp;

  if (domains.value.length) {
    model.value.rootDomain = domains.value[0].domainName;
  }
});

const create = async () => {
  await form.value.validate();
  const site = await newSite(model.value);
  const folder = getQueryString("currentFolder");

  if (folder) {
    await setSitesFolder({
      siteIds: [site.id],
      folderName: folder,
    });
  }

  router.push({
    name: "home",
    query: {
      currentFolder: folder,
    },
  });
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
  <div class="text-444 dark:text-fff/86">
    <p class="text-3l mb-8">{{ t("common.createSite") }}</p>
    <p class="text-s">
      {{ t("common.createTips") }}
    </p>
    <div
      class="border-t border-b border-line dark:border-[#555] border-solid my-24 py-24"
    >
      <el-form
        ref="form"
        label-position="top"
        :model="model"
        :rules="rules"
        @keydown.enter="create"
      >
        <el-form-item :label="t('common.siteName')" prop="siteName">
          <el-input
            v-model.trim="model.siteName"
            class="w-394px"
            data-cy="siteName"
            @input="isSubDomainEdit = true"
          />
        </el-form-item>
        <div class="flex items-center space-x-8">
          <el-form-item :label="t('common.domain')" prop="subDomain">
            <el-input
              v-model="model.subDomain"
              class="w-394px"
              data-cy="subdomain"
              @input="isSiteNameEdit = true"
            />
          </el-form-item>
          <el-form-item prop="rootDomain" class="mt-30px">
            <el-select v-model="model.rootDomain" data-cy="root-domain">
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
    </div>
    <el-button round type="primary" data-cy="start-create" @click="create">
      {{ t("common.startCreate") }}
    </el-button>
  </div>
</template>
