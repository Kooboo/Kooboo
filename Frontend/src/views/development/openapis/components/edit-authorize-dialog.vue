<script lang="ts" setup>
import { ref, computed } from "vue";
import useOperationDialog from "@/hooks/use-operation-dialog";
import type { Rules } from "async-validator";
import {
  requiredRule,
  letterAndDigitStartRule,
  rangeRule,
  authorizeNameIsUniqueNameRule,
} from "@/utils/validate";
import { getAuthorize, postAuthorizes } from "@/api/openapi";
import { useI18n } from "vue-i18n";
import type { OpenApi, Authorize } from "@/api/openapi/types";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

const props = defineProps<{
  modelValue: boolean;
  id: string;
  openApi: OpenApi;
}>();
const emits = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "confirm", value: void): void;
}>();
const { visible, handleClose } = useOperationDialog(props, emits);
const { t } = useI18n();

const form = ref();
const model = ref<Authorize>({} as Authorize);
const data = ref<{ [key: string]: any }>({});

const rules = computed(() => {
  return {
    authorizeName: [
      requiredRule(t("common.nameRequiredTips")),
      letterAndDigitStartRule(),
      rangeRule(1, 50),
      isEdit.value
        ? ""
        : authorizeNameIsUniqueNameRule(
            model.value.openApiName + model.value.authorizeName
          ),
    ],
  } as Rules;
});

interface securityScheme {
  name: string;
  type: string;
  value: {
    scheme: string;
    type: string;
  };
}
const securities = ref<securityScheme[]>([]);
const isEdit = ref(true);
const manual = ref(false); // Oauth2 手工输入

const onLoad = async () => {
  const doc = JSON.parse(props.openApi.jsonData);
  if (doc?.components?.securitySchemes) {
    for (const key in doc.components.securitySchemes) {
      const name = standardName(key);
      securities.value.push({
        name: standardName(key),
        type: doc.components.securitySchemes[key].type,
        value: doc.components.securitySchemes[key],
      });

      data.value[name] = {};
    }
  }
  if (props.id) {
    isEdit.value = true;
    rules.value.name = [];
    model.value = await getAuthorize(props.id);
    data.value = model.value.securities;
  } else {
    isEdit.value = false;
    model.value.openApiName = props.openApi.name;
    model.value.authorizeName = "";
  }
};

const onSave = async () => {
  await form.value.validate();
  model.value.securities = data.value;
  await postAuthorizes(model.value);
  handleClose();
  emits("confirm");
};

function standardName(s: string) {
  return s.replace(/[^\w]/g, "_").toLowerCase();
}

function getScheme(item: { scheme: string; type: string }) {
  if (!item.scheme) return null;
  return item.scheme.toLowerCase();
}

// 使用该组件需要 v-if 保证每次都能重加载数据
onLoad();
</script>

<template>
  <el-dialog
    :model-value="visible"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.authorize')"
    @closed="handleClose"
  >
    <div class="h-320px overflow-hidden">
      <el-scrollbar>
        <el-form
          ref="form"
          :model="model"
          :rules="rules"
          label-width="130px"
          @submit.prevent
          @keydown.enter="onSave"
        >
          <div class="text-2l bold text-444 dark:text-fff/86">
            {{ t("common.basicInformation") }}
          </div>
          <div class="p-12">
            <el-form-item
              :label="t('common.authorizeName')"
              prop="authorizeName"
            >
              <el-input v-model="model.authorizeName" :disabled="isEdit" />
            </el-form-item>
          </div>

          <template
            v-for="securitiesItem in securities"
            :key="securitiesItem.type"
          >
            <!-- http -->
            <el-form v-if="securitiesItem.type === 'http'" label-width="130px">
              <div class="text-2l bold text-444">jwt ( http, bearer )</div>
              <div class="p-12">
                <template v-if="getScheme(securitiesItem.value) === 'basic'">
                  <el-form-item :label="t('common.username')">
                    <el-input
                      v-model="data[securitiesItem.name].username"
                      :placeholder="t('common.username')"
                    />
                  </el-form-item>
                  <el-form-item :label="t('common.password')">
                    <el-input
                      v-model="data[securitiesItem.name].password"
                      :placeholder="t('common.password')"
                    />
                  </el-form-item>
                </template>
                <template v-if="getScheme(securitiesItem.value) === 'bearer'">
                  <el-form-item :label="t('common.name')">
                    <el-input
                      v-model="data[securitiesItem.name].name"
                      :placeholder="t(`common.optionalDefaultBearer`)"
                    />
                  </el-form-item>
                  <el-form-item label="Bearer">
                    <el-input v-model="data[securitiesItem.name].accessToken" />
                  </el-form-item>
                </template>
              </div>
            </el-form>

            <!-- apiKey -->
            <el-form
              v-if="securitiesItem.type === 'apiKey'"
              label-width="130px"
            >
              <div class="text-2l bold text-444">api Key</div>
              <div class="p-12">
                <el-form-item label="accessToken">
                  <el-input
                    v-model="data[securitiesItem.name].accessToken"
                    placeholder="accessToken"
                  />
                </el-form-item>
              </div>
            </el-form>

            <!-- Oauth2 -->
            <el-form
              v-if="securitiesItem.type === 'oauth2'"
              label-width="130px"
            >
              <div class="text-2l bold text-444 dark:text-fff/86">
                {{ t("oauth2 ( oauth2, clientCredentials )") }}
              </div>
              <div class="p-24">
                <el-form-item :label="t('common.clientId')">
                  <el-input v-model="data[securitiesItem.name].clientId" />
                </el-form-item>
                <el-form-item :label="t('common.clientSecret')">
                  <el-input v-model="data[securitiesItem.name].clientSecret" />
                </el-form-item>
                <!-- 手动填写 -->
                <div v-if="!manual" class="text-center">
                  <span
                    class="text-blue cursor-pointer"
                    @click="manual = true"
                    >{{ t("common.manuallyFillOut") }}</span
                  >
                </div>
                <template v-else>
                  <el-form-item :label="t('common.name')">
                    <el-input
                      v-model="data[securitiesItem.name].name"
                      :placeholder="t('common.optionalDefaultBearer')"
                    />
                  </el-form-item>
                  <el-form-item :label="t('common.accessToken')">
                    <el-input v-model="data[securitiesItem.name].accessToken" />
                  </el-form-item>
                  <el-form-item :label="t('common.refreshToken')">
                    <el-input
                      v-model="data[securitiesItem.name].refreshToken"
                    />
                  </el-form-item>
                  <el-form-item :label="t('common.expiresIn')">
                    <el-input v-model="data[securitiesItem.name].expiresIn" />
                  </el-form-item>
                </template>
              </div>
            </el-form>
          </template>
        </el-form>
      </el-scrollbar>
    </div>
    <template #footer>
      <DialogFooterBar
        :hidden-confirm="!securities.length"
        :permission="{ feature: 'openApi', action: 'edit' }"
        @confirm="onSave"
        @cancel="handleClose"
      />
    </template>
  </el-dialog>
</template>
