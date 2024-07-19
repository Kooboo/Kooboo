<script lang="ts" setup>
import { computed, ref, watch } from "vue";
import type { Template } from "@/api/template/types";
import { useTime } from "@/hooks/use-date";
import { useI18n } from "vue-i18n";
import { isUniqueName, setSitesFolder } from "@/api/site";
import { useRouter, useRoute } from "vue-router";
import { getAvailableDomain } from "@/api/console";
import type { AvailableDomain } from "@/api/console/types";
import {
  DomainRule,
  isUniqueNameRule,
  requiredRule,
  rangeRule,
  domainBindingIsUniqueNameRule,
  simpleNameRule,
} from "@/utils/validate";
import type { Rules } from "async-validator";
import { getQueryString } from "@/utils/url";
import { getTemplateDetail, use } from "@/api/template";

const { t } = useI18n();
const router = useRouter();
const route = useRoute();

const domains = ref<AvailableDomain[]>();
const form = ref();
const isSubDomainEdit = ref(false);
const isSiteNameEdit = ref(false);

const model = ref({
  rootDomain: "",
  subDomain: "",
  siteName: "",
  id: "",
});

const templateDetail = ref<Template>();
const folder = getQueryString("currentFolder");

const deviceList = ref([
  {
    name: "pc",
    title: t("common.fullScreen"),
    icon: "icon-iMac",
    width: "100%",
    height: "",
  },
  {
    name: "pad",
    title: t("common.pad"),
    icon: "icon-iPad",
    width: "820px",
    height: "1180px",
  },
  {
    name: "phone",
    title: t("inlineDesign.phone"),
    icon: "icon-iPhone",
    width: "390px",
    height: "844px",
  },
]);

const device = ref(deviceList.value[0]);

const rules = {
  subDomain: [
    requiredRule(t("common.subDomainRequiredTips")),
    rangeRule(1, 63),
    DomainRule,
    domainBindingIsUniqueNameRule(model.value),
  ],
  rootDomain: requiredRule(t("common.rootDomainRequiredTips")),
  siteName: [
    rangeRule(1, 50),
    requiredRule(t("common.nameRequiredTips")),
    simpleNameRule(),
    isUniqueNameRule(isUniqueName, t("common.siteNameExistsTips")),
  ],
} as Rules;

const load = async () => {
  if (!route.query.templateId) return;
  templateDetail.value = await getTemplateDetail(
    route.query.templateId as string
  );
  model.value.id = templateDetail.value.id;
  getAvailableDomain().then((rsp) => {
    domains.value = rsp;
    if (domains.value.length) {
      model.value.rootDomain = domains.value[0].domainName;
    }
  });
};
load();
const create = async () => {
  await form.value.validate();
  const site = await use(model.value);

  if (folder) {
    await setSitesFolder({
      siteIds: [site],
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

const contents = computed(() => [
  {
    property: templateDetail.value?.layoutCount,
    display: t("common.layout"),
  },
  {
    property: templateDetail.value?.menuCount,
    display: t("common.menu"),
  },
  {
    property: templateDetail.value?.pageCount,
    display: t("common.page"),
  },
  {
    property: templateDetail.value?.viewCount,
    display: t("common.view"),
  },
  {
    property: templateDetail.value?.imageCount,
    display: t("common.image"),
  },
  {
    property: templateDetail.value?.contentCount,
    display: t("common.content"),
  },
]);

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
  <div
    class="max-w-1120px mx-auto mt-16 mb-24 bg-fff rounded-normal pt-21px px-24 pb-12 relative dark:bg-[#333333]"
  >
    <div class="flex">
      <div class="flex items-center justify-center text-2l text-blue mr-12">
        <el-icon
          class="iconfont icon-fanhui1 cursor-pointer"
          :title="t('common.back')"
          data-cy="back"
          @click="
            router.replace({
              name: 'templates',
              query: {
                currentFolder: folder,
              },
            })
          "
        />
      </div>

      <div class="flex justify-between h-38px flex-1">
        <div
          class="text-3l h-38px leading-38px dark:text-fff ellipsis max-w-260px"
          :title="templateDetail?.name"
          data-cy="template-name"
        >
          {{ templateDetail?.name }}
        </div>
        <el-form
          ref="form"
          :model="model"
          :rules="rules"
          @keydown.enter="create"
        >
          <div class="flex items-center space-x-8">
            <el-form-item :label="t('common.siteName')" prop="siteName">
              <el-input
                v-model="model.siteName"
                class="w-185px"
                data-cy="siteName"
                @input="isSubDomainEdit = true"
              />
            </el-form-item>
            <el-form-item prop="subDomain" :label="t('common.domain')">
              <el-input
                v-model="model.subDomain"
                class="w-185px"
                data-cy="subdomain"
                @input="isSiteNameEdit = true"
              />
            </el-form-item>
            <el-form-item prop="rootDomain">
              <div class="flex items-center space-x-12">
                <el-select
                  v-model="model.rootDomain"
                  data-cy="root-domain"
                  class="w-185px"
                >
                  <el-option
                    v-for="item of domains"
                    :key="item.domainName"
                    :value="item.domainName"
                    :label="item.domainName"
                    data-cy="root-domain-opt"
                  />
                </el-select>
                <div>
                  <el-button
                    type="primary"
                    round
                    data-cy="start-create"
                    @click="create"
                    >{{ t("common.startCreate") }}</el-button
                  >
                </div>
              </div>
            </el-form-item>
          </div>
        </el-form>
      </div>
    </div>

    <div class="w-full bg-[#E9EAF0] h-1px my-16 dark:bg-666" />

    <div
      v-if="
        !(templateDetail?.screenShotUrl && templateDetail?.type?.value !== 1)
      "
      class="w-full h-[60px] bg-[#F3F5F5] dark:bg-[#545454] flex items-center justify-center gap-6 mb-[16px]"
    >
      <el-icon
        v-for="item in deviceList"
        :key="item.name"
        class="iconfont text-[32px] text-[#8BA5B0] hover:text-[#4EABF5] cursor-pointer"
        :class="{
          [item.icon]: true,
          'text-[#2296F3]': device.name === item.name,
        }"
        :title="item.title"
        @click="device = item"
      />
    </div>

    <img
      v-if="templateDetail?.screenShotUrl && templateDetail?.type?.value !== 1"
      class="w-full"
      :src="templateDetail?.screenShotUrl"
    />

    <iframe
      v-else
      ref="iframe"
      :src="templateDetail?.previewUrl"
      class="origin-top-left h-500px w-full mx-auto border-gray border-1 border-solid shadow-blue-md iframe dark:border-666"
      style="transform: scale(1)"
      :style="{ width: device.width, height: device.height }"
    />

    <div
      class="space-y-12 my-24 px-24 py-16 rounded-normal text-m bg-[#f3f5f5] dark:bg-[#545454] bg-opacity-50"
    >
      <div class="flex space-x-32">
        <div>
          <span class="text-999 mr-8 dark:text-gray"
            >{{ t("common.author") }}:</span
          >
          <span class="dark:text-fff" data-cy="author">{{
            templateDetail?.userName
          }}</span>
        </div>
        <div>
          <span class="text-999 mr-8 dark:text-gray"
            >{{ t("common.category") }}:</span
          >
          <span class="dark:text-fff" data-cy="category">{{
            templateDetail?.type.name ?? t("common.noCategory")
          }}</span>
        </div>
        <div>
          <span class="text-999 mr-8 dark:text-gray"
            >{{ t("common.size") }}:</span
          >
          <span class="dark:text-fff" data-cy="size">{{
            templateDetail?.sizeString
          }}</span>
        </div>
        <div>
          <span class="text-999 mr-8 dark:text-gray"
            >{{ t("common.downloads") }}:</span
          >
          <span class="dark:text-fff" data-cy="downloads">{{
            templateDetail?.downloadCount
          }}</span>
        </div>
        <div>
          <span class="text-999 mr-8 dark:text-gray"
            >{{ t("common.lastModified") }}:</span
          >
          <span
            v-if="templateDetail?.lastModified"
            class="dark:text-fff"
            data-cy="last-modified"
            >{{ useTime(templateDetail!.lastModified) }}</span
          >
        </div>
      </div>
      <div class="flex">
        <span class="text-999 mr-8 leading-5 dark:text-gray"
          >{{ t("pickTemplate.contents") }}:</span
        >

        <div
          v-if="
            contents.map((content) => content.property).every((e) => e === 0)
          "
        >
          {{ t("common.noContent") }}
        </div>
        <div v-else class="space-x-4 -ml-4">
          <template v-for="(item, index) of contents">
            <el-tag
              v-if="item.property"
              :key="index"
              size="small"
              class="rounded-full cursor-pointer ml-4"
              data-cy="content"
              >{{ item.property }} {{ item.display }}</el-tag
            >
          </template>
        </div>
      </div>
    </div>
  </div>
</template>
<style scoped>
.iframe {
  box-shadow: 7px 11px 9px 0px rgba(0, 0, 0, 0.1);
}
</style>
