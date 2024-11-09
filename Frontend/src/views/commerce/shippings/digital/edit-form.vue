<script lang="ts" setup>
import type { DigitalShipping } from "@/api/commerce/digital-shipping";
import { preview as previewApi } from "@/api/commerce/digital-shipping";
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useEmailStore } from "@/store/email";
import { portRule, requiredRule, urlAndIpRule } from "@/utils/validate";

const props = defineProps<{ model: DigitalShipping }>();
const { t } = useI18n();

const mailServers = [
  {
    key: "kooboo",
    value: "Kooboo",
  },
  {
    key: "custom",
    value: t("common.custom"),
  },
];

const types = [
  {
    key: "preview",
    value: t("common.preview"),
  },
  {
    key: "code",
    value: t("common.code"),
  },
];

const type = ref("preview");
const preview = ref({
  subject: "",
  body: "",
});

const emailStore = useEmailStore();

watch(
  () => type.value,
  async () => {
    if (type.value != "preview") return;
    preview.value = await previewApi(props.model.mailTemplate);
  },
  {
    immediate: true,
  }
);

watch(
  () => props.model.mailServerType,
  () => {
    if (props.model.mailServerType == "kooboo") {
      emailStore.loadAddress("addresses");
    }
  },
  {
    immediate: true,
  }
);
</script>

<template>
  <div class="px-24 pt-0 pb-84px space-y-12">
    <div class="bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal">
      <ElForm label-position="top" :model="model">
        <ElFormItem
          :label="t('common.name')"
          prop="name"
          :rules="[requiredRule(t('common.inputNameTips'))]"
        >
          <ElInput v-model="model.name" />
        </ElFormItem>
        <ElFormItem :label="t('common.description')">
          <ElInput v-model="model.description" type="textarea" />
        </ElFormItem>
        <ElFormItem :label="t('common.mailServer')">
          <el-radio-group
            v-model="model.mailServerType"
            class="el-radio-group--rounded"
          >
            <el-radio-button
              v-for="item of mailServers"
              :key="item.key"
              :label="item.key"
              >{{ item.value }}</el-radio-button
            >
          </el-radio-group>
        </ElFormItem>
        <ElFormItem
          v-if="model.mailServerType == 'kooboo'"
          :label="t('mail.from')"
        >
          <template #label>
            <div class="inline-flex items-center space-x-4">
              <div>{{ t("common.fromAddress") }}</div>
              <Tooltip
                :tip="t('common.koobooEmailAddressTip')"
                custom-class="ml-4"
              />
            </div>
          </template>
          <el-select
            v-model="model.koobooEmailAddress"
            :placeholder="t('common.pleaseSelect')"
            class="w-250px"
          >
            <el-option
              v-for="opt in emailStore.addressList.filter(
                (f) => f.addressType !== 'Wildcard'
              )"
              :key="opt.id"
              :value="opt.address"
              :label="opt.address"
            />
          </el-select>
        </ElFormItem>
        <template v-else>
          <el-form-item
            prop="customMailServer.server"
            :rules="[
              urlAndIpRule(t('common.serverUrlInvalid')),
              requiredRule(t('common.serverUrlRequiredTips')),
            ]"
          >
            <ElInput
              v-model="model.customMailServer.server"
              placeholder="smtp.qq.com"
            />
          </el-form-item>
          <el-form-item
            :label="t('common.port')"
            prop="customMailServer.port"
            :rules="[portRule()]"
          >
            <ElInputNumber
              v-model.number="model.customMailServer.port"
              placeholder="587"
            />
          </el-form-item>
          <el-form-item label="SSL">
            <el-switch v-model="model.customMailServer.ssl" />
          </el-form-item>
          <el-form-item :label="t('common.fromAddress')">
            <ElInput
              v-model="model.customMailServer.from"
              placeholder="name@example.com"
            />
          </el-form-item>
          <el-form-item
            :label="t('common.account')"
            prop="customMailServer.userName"
            :rules="[requiredRule(t('common.inputAccountTips'))]"
          >
            <ElInput
              v-model="model.customMailServer.userName"
              placeholder="xxx@qq.com"
            />
          </el-form-item>
          <el-form-item
            :label="t('common.password')"
            prop="customMailServer.password"
            :rules="[requiredRule(t('common.inputPasswordTips'))]"
          >
            <ElInput
              v-model="model.customMailServer.password"
              type="password"
            />
          </el-form-item>
        </template>
      </ElForm>
    </div>
    <div class="bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal space-y-8">
      <el-radio-group v-model="type" class="el-radio-group--rounded">
        <el-radio-button
          v-for="item of types"
          :key="item.key"
          :label="item.key"
          >{{ item.value }}</el-radio-button
        >
      </el-radio-group>
      <div
        v-if="type == 'preview'"
        class="border border-line rounded-normal p-8 el-dialog__body"
      >
        <h1>Subject: {{ preview?.subject }}</h1>
        <el-divider class="my-12" />
        <KFrame class="h-300px" :content="preview?.body ?? ''" />
      </div>
      <template v-else>
        <el-form-item :label="t('common.subject')" prop="subjectTemplate">
          <el-input v-model="model.mailTemplate.subject" />
        </el-form-item>
        <el-form-item :label="t('common.content')" prop="bodyTemplate">
          <MonacoEditor
            v-model="model.mailTemplate.body"
            class="border border-999"
            language="html"
            k-script
          />
        </el-form-item>
      </template>
    </div>
  </div>
</template>
