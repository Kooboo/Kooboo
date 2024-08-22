<script lang="ts" setup>
import { getAvailableDomain } from "@/api/console";
import type { AvailableDomain } from "@/api/console/types";
import type { ByLevelBody } from "@/api/transfer/types";
import { byLevel, getSubUrl, byPage } from "@/api/transfer";
import { ref, watch } from "vue";
import { doubleSuffix } from "./create";
import {
  DomainRule,
  urlRule,
  isUniqueNameRule,
  requiredRule,
  rangeRule,
  domainBindingIsUniqueNameRule,
  putIntegerNumberRule,
  simpleNameRule,
} from "@/utils/validate";
import type { Rules } from "async-validator";
import { useRouter } from "vue-router";
import { isUniqueName, setSitesFolder } from "@/api/site";
import { useI18n } from "vue-i18n";
import { getQueryString } from "@/utils/url";
import { useAppStore } from "@/store/app";
import Schema from "async-validator";

const { t } = useI18n();
const domains = ref<AvailableDomain[]>();
const router = useRouter();
const appStore = useAppStore();

const model = ref<ByLevelBody>({
  rootDomain: "",
  subDomain: "",
  siteName: "",
  url: "",
  TotalPages: 20,
  Depth: 2,
  headless: false,
  convertToRoot: false,
});

const cloneSetting = {
  auto: t("common.auto"),
  semiAuto: t("common.semiAuto"),
  manual: t("common.manual"),
};

const selectedSetting = ref(Object.keys(cloneSetting)[0]);

const scanList = ref<string[]>([]);
const selectedList = ref<string[]>([]);
const isSubDomainEdit = ref(false);
const isSiteNameEdit = ref(false);

const scan = async () => {
  scanList.value = await getSubUrl(model.value.url, model.value.TotalPages!);
};

const rules = {
  url: [
    urlRule(t("common.urlInvalid")),
    requiredRule(t("common.urlRequiredTips")),
  ],
  subDomain: [
    requiredRule(t("common.subDomainRequiredTips")),
    rangeRule(1, 63),
    DomainRule,
    domainBindingIsUniqueNameRule(model.value),
  ],

  TotalPages: [
    putIntegerNumberRule(),
    requiredRule(t("common.valueRequiredTips")),
  ],
  Depth: [putIntegerNumberRule(), requiredRule(t("common.valueRequiredTips"))],
  rootDomain: requiredRule(t("common.rootDomainRequiredTips")),
  siteName: [
    rangeRule(1, 50),
    requiredRule(t("common.nameRequiredTips")),
    isUniqueNameRule(isUniqueName, t("common.siteNameExistsTips")),
    simpleNameRule(),
  ],
} as Rules;

const scanEnable = ref(false);
const form = ref();

getAvailableDomain().then((rsp) => {
  domains.value = rsp;

  if (domains.value.length) {
    model.value.rootDomain = domains.value[0].domainName;
  }
});

watch(
  () => model.value.url,
  async () => {
    scanList.value = [];
    try {
      await new Schema({
        url: rules.url,
      }).validate(model.value);
      scanEnable.value = true;
    } catch (error) {
      scanEnable.value = false;
    }

    var tempName = "",
      isDouble = false;
    try {
      var url = new URL(model.value.url);
      tempName = url.host;
    } catch {
      //
    } finally {
      doubleSuffix.forEach(function (suffix) {
        if (tempName.indexOf(suffix, tempName.length - suffix.length) !== -1) {
          isDouble = true;
          tempName = tempName.substr(0, tempName.lastIndexOf(suffix));
        }
      });

      var pureNameArray = tempName.split("."),
        pureName = pureNameArray[pureNameArray.length - 2 + (isDouble ? 1 : 0)];
      model.value.subDomain = pureName || "";
    }
  }
);

const clone = async () => {
  await form.value.validate();
  const body = { ...model.value };
  let rsp: any;

  if (selectedSetting.value === "manual") {
    rsp = await byPage({
      ...body,
      Urls: selectedList.value,
    });
  } else {
    if (selectedSetting.value === "auto") {
      delete body.TotalPages;
      delete body.Depth;
    }
    rsp = await byLevel(body);
  }

  const folder = getQueryString("currentFolder");

  if (folder) {
    await setSitesFolder({
      siteIds: [rsp.siteId],
      folderName: folder,
    });
  }

  router.replace({
    name: "transferring",
    query: {
      siteId: rsp.siteId,
      id: rsp.taskId,
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
    <p class="text-3l mb-8">{{ t("common.cloneSite") }}</p>
    <p class="text-s">
      {{ t("common.cloneTips") }}
    </p>
    <div
      class="border-t border-b border-line dark:border-[#555] border-solid my-24 py-24"
    >
      <el-form
        ref="form"
        label-position="top"
        :model="model"
        :rules="rules"
        @keydown.enter="clone"
      >
        <el-form-item label="URL" prop="url">
          <el-input
            v-model="model.url"
            autofocus
            tabindex="1"
            class="w-394px"
            :placeholder="t('common.inputURLTips')"
            data-cy="url"
          />
        </el-form-item>
        <el-form-item :label="t('common.siteName')" prop="siteName">
          <el-input
            v-model="model.siteName"
            class="w-394px"
            data-cy="siteName"
            @input="isSubDomainEdit = true"
          />
        </el-form-item>
        <div class="flex items-center space-x-8">
          <el-form-item :label="t('common.domain')" prop="subDomain">
            <el-input
              v-model="model.subDomain"
              tabindex="2"
              class="w-394px"
              data-cy="subdomain"
              @input="isSiteNameEdit = true"
            />
          </el-form-item>
          <el-form-item prop="rootDomain" class="mt-30px">
            <el-select v-model="model.rootDomain">
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
        <el-form-item>
          <el-checkbox
            v-if="!appStore.header?.isOnlineServer"
            v-model="model.headless"
          >
            <span>{{ t("common.useHeadlessMode") }}</span>
            <Tooltip
              :tip="t('common.useHeadlessModeTip')"
              custom-class="ml-4"
            />
          </el-checkbox>
          <el-checkbox v-model="model.convertToRoot">
            <span>{{ t("common.trimSubPath") }}</span>
            <Tooltip :tip="t('common.trimSubPathTip')" custom-class="ml-4" />
          </el-checkbox>
        </el-form-item>
        <template v-if="appStore.header?.isOnlineServer">
          <p class="text-444 text-s mt-24">
            {{ t("common.cloneVersionTips") }}
          </p>
        </template>
        <template v-else>
          <el-form-item>
            <el-radio-group
              v-model="selectedSetting"
              class="el-radio-group--rounded"
            >
              <el-radio-button
                v-for="(value, key) of cloneSetting"
                :key="key"
                :label="key"
                :data-cy="'clone-type' + `${value}`"
                >{{ value }}
              </el-radio-button>
            </el-radio-group>
          </el-form-item>
          <template v-if="selectedSetting == 'semiAuto'">
            <div class="flex space-x-16">
              <el-form-item :label="t('common.pages')" prop="TotalPages">
                <div class="flex flex-col">
                  <el-input-number
                    v-model="model.TotalPages"
                    :min="1"
                    data-cy="semiauto-pages"
                  />
                  <p class="text-s mt-4">
                    {{ t("common.maxAmountOfPagesToClone") }}
                  </p>
                </div>
              </el-form-item>
              <el-form-item :label="t('common.deeps')" prop="Depth">
                <div class="flex flex-col">
                  <el-input-number
                    v-model="model.Depth"
                    data-cy="deeps"
                    :min="1"
                  />
                  <p class="text-s mt-4">
                    {{ t("common.theNumberOfClicksAwayFromHomePage") }}
                  </p>
                </div>
              </el-form-item>
            </div>
          </template>
          <template v-if="selectedSetting == 'manual'">
            <KTable
              :data="scanList"
              show-check
              hide-delete
              @update:selected-data="selectedList = $event"
            >
              <template #bar>
                <div class="flex items-center space-x-12">
                  <div>
                    <span>{{ t("common.pages") }}</span>
                    <Tooltip
                      :tip="t('common.maxAmountOfPagesToClone')"
                      custom-class="ml-4"
                    />
                  </div>
                  <el-form-item prop="TotalPages" class="mb-0">
                    <el-input-number
                      v-model="model.TotalPages"
                      :min="1"
                      data-cy="manual-pages"
                    />
                  </el-form-item>
                  <el-button
                    round
                    type="primary"
                    :disabled="!scanEnable"
                    data-cy="scan"
                    @click="scan"
                    >{{ t("common.scan") }}</el-button
                  >
                </div>
              </template>
              <el-table-column width="50px">
                <template #default="data">
                  {{ data.$index + 1 }}
                </template>
              </el-table-column>
              <el-table-column label="URL">
                <template #default="data">
                  {{ data.row }}
                </template>
              </el-table-column>
            </KTable>
          </template>
        </template>
      </el-form>
    </div>
    <el-button
      round
      type="primary"
      data-cy="start-clone"
      :disabled="!selectedList.length && selectedSetting === 'manual'"
      @click="clone"
    >
      {{ t("common.startClone") }}
    </el-button>
  </div>
</template>
