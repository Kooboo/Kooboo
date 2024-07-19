<template>
  <el-dialog
    v-model="visible"
    width="650px"
    :close-on-click-modal="false"
    :title="title"
    @close="handleClose"
  >
    <el-form
      v-if="types"
      ref="form"
      :rules="rules"
      :model="model"
      label-position="top"
      @submit.prevent
      @keydown.enter="handleSave"
    >
      <el-form-item v-if="isNew" prop="name" :label="t('common.name')">
        <el-input v-model="model.name" data-cy="name" />
      </el-form-item>
      <el-form-item prop="matcher" :label="t('common.matcher')">
        <el-select v-model="model.matcher" class="w-full" data-cy="matcher">
          <el-option
            v-for="item in types.matcher"
            :key="item.name"
            :value="item.name"
            data-cy="matcher-opt"
          >
            {{ item.display }}
          </el-option>
        </el-select>
      </el-form-item>
      <el-form-item v-if="model.matcher === 'Condition'" prop="options">
        <div class="w-full space-y-4">
          <div
            v-for="(item, index) in model.conditions"
            :key="index"
            class="flex items-center space-x-4"
          >
            <el-select
              v-model="item.left"
              class="w-120px"
              data-cy="condition-left"
            >
              <el-option
                v-for="i in ['url', 'method']"
                :key="i"
                :value="i"
                data-cy="condition-left-opt"
              >
                {{ i }}
              </el-option>
            </el-select>

            <el-select
              v-model="item.operator"
              class="w-150px"
              data-cy="condition-operater"
            >
              <el-option
                v-for="i in [
                  {
                    label: '=',
                    value: '=',
                  },
                  {
                    label: '!=',
                    value: '!=',
                  },
                  {
                    label: 'Contains',
                    value: 'contains',
                  },
                  {
                    label: 'Not contains',
                    value: 'notcontains',
                  },
                  {
                    label: 'Starts with',
                    value: 'startwith',
                  },
                  {
                    label: 'Not starts with',
                    value: 'notstartwith',
                  },
                ]"
                :key="i.value"
                :value="i.value"
                :label="i.label"
                data-cy="condition-operater-opt"
              />
            </el-select>

            <el-form-item
              :prop="'conditions.' + index + '.right'"
              :rules="requiredRule(t('common.valueRequiredTips'))"
            >
              <el-input v-model="item.right" data-cy="condition-right" />
            </el-form-item>
            <div>
              <el-button
                circle
                data-cy="remove-condition"
                @click="removeCondition(index)"
              >
                <el-icon class="iconfont icon-delete text-orange" />
              </el-button>
            </div>
          </div>
          <el-button circle @click="addCondition()">
            <el-icon
              class="iconfont icon-a-addto text-blue"
              data-cy="add-condition"
            />
          </el-button>
        </div>
      </el-form-item>
      <el-form-item prop="action" :label="t('common.action')">
        <el-select v-model="model.action" class="w-full" data-cy="action">
          <el-option
            v-for="item in types.action"
            :key="item.name"
            :value="item.name"
            data-cy="action-opt"
          >
            {{ item.display }}
          </el-option>
        </el-select>
      </el-form-item>

      <template v-if="model.action === 'CustomCode'">
        <el-form-item prop="customCodeName" :label="t('common.code')">
          <MonacoEditor
            v-model="model.customCode"
            class="h-200px"
            language="typescript"
            k-script
            :uri="Uri.file(model.id)"
            @keydown.enter.stop=""
          />
        </el-form-item>
      </template>
      <template v-else>
        <el-form-item prop="failedAction" :label="t('common.failedAction')">
          <el-select
            v-model="model.failedAction"
            class="w-full"
            data-cy="failed-action"
          >
            <el-option
              v-for="item in types.failedAction"
              :key="item.name"
              :value="item.name"
              data-cy="failed-action-opt"
            >
              {{ item.display }}
            </el-option>
          </el-select>
        </el-form-item>
        <el-form-item
          v-if="model.failedAction === 'ResultCode'"
          prop="httpCode"
          :label="t('common.httpCode')"
        >
          <el-input-number
            v-model="model.httpCode"
            :placeholder="t('common.httpCode')"
            data-cy="http-code"
          />
        </el-form-item>
        <el-form-item
          v-if="model.failedAction === 'Redirect'"
          prop="url"
          :label="t('common.url')"
        >
          <el-input
            v-model="model.url"
            data-cy="redirect-url"
            @input="model.url = model.url.replace(/\s+/g, '')"
          />
        </el-form-item>
      </template>
    </el-form>
    <template #footer>
      <DialogFooterBar
        :permission="{
          feature: 'authentication',
          action: 'edit',
        }"
        @confirm="handleSave"
        @cancel="handleClose"
      />
    </template>
  </el-dialog>
</template>
<script setup lang="ts">
import type { ElForm } from "element-plus";
import useOperationDialog from "@/hooks/use-operation-dialog";
import type { UPDATE_MODEL_EVENT } from "@/constants/constants";
import type {
  Authentication,
  AuthType,
} from "@/api/development/authentication";
import { save, getTypes, isUniqueName } from "@/api/development/authentication";
import { computed, ref, watch } from "vue";
import { emptyGuid } from "@/utils/guid";
import {
  letterAndDigitStartRule,
  rangeRule,
  requiredRule,
  HttpCodeRule,
} from "@/utils/validate";

import { useI18n } from "vue-i18n";
import MonacoEditor from "@/components/monaco-editor/index.vue";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { Uri } from "monaco-editor";

interface PropsType {
  modelValue: boolean;
  current?: Authentication;
}
interface EmitsType {
  (e: typeof UPDATE_MODEL_EVENT, value: boolean): void;
  (e: "save-success"): void;
}

const props = defineProps<PropsType>();
const emits = defineEmits<EmitsType>();
const { t } = useI18n();
const { visible, handleClose } = useOperationDialog(props, emits);
const defaultModel = () =>
  ({
    id: emptyGuid,
    name: "",
    matcher: "None",
    action: "None",
    failedAction: "None",
    customCodeName: "",
    customCode: `// sample code.. 
// if (k.request.url.indexOf('/api') == 0) {
//     var token = k.request.headers.get('Authorization');
//     if (token != 'abc123') k.response.unauthorized()
// }`,
    httpCode: 401,
    url: "/",
    conditions: [],
  } as Authentication);
const form = ref<InstanceType<typeof ElForm>>();
const model = ref<Authentication>({} as Authentication);
const rules = {
  name: [
    requiredRule(t("common.nameRequiredTips")),
    rangeRule(1, 50),
    letterAndDigitStartRule(),
    {
      async asyncValidator(
        _rule: unknown,
        value: string,
        callback: (error?: Error) => void
      ) {
        try {
          await isUniqueName(value);
          callback();
        } catch (error) {
          callback(Error(t("common.valueHasBeenTakenTips")));
        }
      },
      trigger: "blur",
    },
  ],
  httpCode: [requiredRule(t("common.httpCodeRequiredTips")), HttpCodeRule],
  url: [requiredRule(t("common.urlRequiredTips"))],
  options: [
    {
      validator(
        _rule: unknown,
        _value: string,
        callback: (error?: Error) => void
      ) {
        if (model.value.conditions.length === 0) {
          callback(new Error(t("common.fieldRequiredTips")));
        } else {
          callback();
        }
      },
      trigger: "blur",
    },
  ],
};
const isNew = computed(() => !props.current);
const title = computed(
  () =>
    (isNew.value ? t("common.create") : t("common.edit")) +
    (props.current?.name ? ": " + props.current?.name : "")
);
const types = ref<AuthType>();

watch(
  () => props.modelValue,
  async (val) => {
    if (val) {
      if (!types.value) {
        types.value = await getTypes();
      }
      form.value?.resetFields();
      if (props.current) {
        model.value = props.current;
      } else {
        model.value = defaultModel();
      }
    }
  }
);

function removeCondition(index: number) {
  model.value.conditions.splice(index, 1);
}
function addCondition() {
  model.value.conditions.push({ left: "url", operator: "=", right: "" });
  form.value?.clearValidate("options");
}
async function handleSave() {
  await form.value?.validate();
  await save(model.value);
  handleClose();
  emits("save-success");
}
</script>
