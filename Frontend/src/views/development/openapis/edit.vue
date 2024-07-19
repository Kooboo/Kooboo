<script lang="ts" setup>
import type { KeyValue } from "@/global/types";
import { computed, ref, watch } from "vue";
import KBottomBar from "@/components/k-bottom-bar/index.vue";
import type { OpenApi } from "@/api/openapi/types";
import { onBeforeRouteLeave, useRouter } from "vue-router";
import { getQueryString } from "@/utils/url";
import { useRouteSiteId } from "@/hooks/use-site-id";
import PageCache from "./components/cache.vue";
import TemplateSelector from "./components/template.vue";
import MonacoEditor from "@/components/monaco-editor/index.vue";
import { isUniqueName, get, post } from "@/api/openapi";
import { emptyGuid } from "@/utils/guid";
import {
  isUniqueNameRule,
  urlRule,
  letterAndDigitStartRule,
  rangeRule,
  requiredRule,
} from "@/utils/validate";

import { useI18n } from "vue-i18n";
import type { Rules } from "async-validator";
import { useSaveTip } from "@/hooks/use-save-tip";
const { t } = useI18n();
const router = useRouter();
const form = ref();
const isEdit = ref();
const saveTip = useSaveTip();

const types: KeyValue[] = [
  {
    key: "url",
    value: "URL",
  },
  {
    key: "code",
    value: t("common.code"),
  },
  {
    key: "template",
    value: t("common.template"),
  },
];

const model = ref<OpenApi>({
  name: "",
  type: "url",
  url: "",
  jsonData: "",
  caches: [],
  templateId: "",
  authUrl: "",
  customAuthorization: "",
  useCustomCode: false,
  code: `// sample code.. 
// request.headers.Authorization='bearer xxxx'
// request.querys.token='xxxx'
// request.paths.id='xxxx'
// request.cookies.id='xxxx'
// request.body.token='xxxx'`,
  useCommaArray: false,
  id: emptyGuid,
  baseUrl: "",
});

const rules = computed(
  () =>
    ({
      name: isEdit.value
        ? []
        : [
            requiredRule(t("common.fieldRequiredTips")),
            rangeRule(1, 50),
            letterAndDigitStartRule(),
            isUniqueNameRule(isUniqueName, t("common.nameExistsTips")),
          ],
      baseUrl: [urlRule(t("common.urlInvalidTips"))],
      url:
        model.value.type === "url"
          ? [
              requiredRule(t("common.fieldRequiredTips")),
              urlRule(t("common.urlInvalidTips")),
            ]
          : [],
    } as Rules)
);

const templateBaseUrl = ref("https://openapi_template.kooboo.net");
const getTemplateUrl = (id: string) => {
  return `${templateBaseUrl.value}/detail?id=${id}`;
};
const getTemplateId = (url: string) => {
  return /id=(.*)/i.exec(url)?.[1].replace(/&.*/g, "") || "";
};

const load = async () => {
  const id = getQueryString("id");
  if (id) {
    isEdit.value = true;
    model.value = await get(id);
  } else {
    isEdit.value = false;
  }
  saveTip.init(model.value);
};

onBeforeRouteLeave(async (to, from, next) => {
  if (to.name === "login") {
    next();
  } else {
    await saveTip
      .check(model.value)
      .then(() => {
        next();
      })
      .catch(() => {
        next(false);
      });
  }
});

const onSave = async () => {
  if (!model.value) return;
  await form.value.validate();
  await post(model.value);
  saveTip.init(model.value);
  onBack();
};

const onBack = () => {
  router.goBackOrTo(
    useRouteSiteId({
      name: "openapis",
    })
  );
};

watch(
  () => model.value.type,
  () => {
    // 切换后清除验证
    setTimeout(() => {
      form.value.clearValidate();
    });
  }
);
watch(
  () => model.value,
  () => {
    saveTip.changed(model.value);
  },
  {
    deep: true,
  }
);

load();
</script>

<template>
  <el-scrollbar>
    <div class="w-1120px mx-auto pt-32 pb-150px">
      <el-card>
        <template #header>OpenApi</template>
        <el-form
          ref="form"
          label-width="138px"
          class="pr-64"
          :model="model"
          :rules="rules"
        >
          <el-form-item :label="t('common.name')" prop="name">
            <el-input
              v-model="model.name"
              :readonly="isEdit"
              :disabled="isEdit ? true : false"
              data-cy="openapi-name"
            />
          </el-form-item>
          <el-form-item label="Base URL" prop="baseUrl">
            <el-input
              v-model="model.baseUrl"
              :placeholder="t('common.optionalHttp')"
              data-cy="base-url"
            />
          </el-form-item>
          <el-form-item v-if="!isEdit" :label="t('common.type')">
            <el-radio-group
              v-model="model.type"
              class="el-radio-group--rounded"
            >
              <el-radio-button
                v-for="item of types"
                :key="item.key"
                :label="item.key"
                :data-cy="`type-${item.key}`"
                >{{ item.value }}</el-radio-button
              >
            </el-radio-group>
          </el-form-item>
          <el-form-item
            v-if="model.type == 'url'"
            prop="url"
            :label="t('common.url')"
          >
            <el-input
              v-model="model.url"
              placeholder="https://generator3.swagger.io/openapi.json"
              data-cy="url"
              @input="model.url = model.url.replace(/\s+/g, '')"
            />
          </el-form-item>
          <el-form-item v-if="model.type === 'code'">
            <div
              class="rounded-normal w-full border border-solid border-blue/30 overflow-hidden"
            >
              <MonacoEditor
                v-model="model.jsonData"
                class="h-64"
                language="json"
              />
            </div>
          </el-form-item>
          <el-form-item v-if="model.type == 'template'">
            <TemplateSelector
              :id="getTemplateId(model.url)"
              @change="
                (item) => {
                  model.templateId = item._id;
                  model.baseUrl = item.baseUrl;
                  model.url = getTemplateUrl(item._id);
                  model.name || (model.name = item.name);
                  model.authUrl = item.authUrl;
                }
              "
            />
          </el-form-item>
          <el-form-item :label="t('common.cache')">
            <PageCache :list="model.caches" />
          </el-form-item>
          <el-form-item :label="t('common.useCommaArray')">
            <el-switch
              v-model="model.useCommaArray"
              data-cy="enable-comma-array"
            />
          </el-form-item>
          <el-form-item :label="t('common.customAuth')">
            <div class="w-full">
              <el-switch
                v-model="model.useCustomCode"
                data-cy="enable-custom-auth"
              />
              <div
                v-if="model.useCustomCode"
                class="mt-18px border-line border border-solid dark:border-opacity-50 w-full p-4 overflow-hidden"
              >
                <MonacoEditor
                  v-model="model.code"
                  class="h-64"
                  language="typescript"
                  k-script
                />
              </div>
            </div>
          </el-form-item>
        </el-form>
      </el-card>
    </div>
  </el-scrollbar>
  <KBottomBar
    back
    :permission="{
      feature: 'openApi',
      action: 'edit',
    }"
    @cancel="onBack"
    @save="onSave"
  />
</template>
