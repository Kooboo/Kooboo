<script lang="ts" setup>
import { ref } from "vue";
import { emptyGuid } from "@/utils/guid";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { updateServer, getServerHosts } from "@/api/publish";
import { useI18n } from "vue-i18n";
import type { OrganizationList } from "@/api/organization/types";
import {
  rangeRule,
  requiredRule,
  simpleNameRule,
  urlAndIpRule,
} from "@/utils/validate";
import type { Rules } from "async-validator";
import type { KeyValue } from "@/global/types";
import type { ServerHost } from "@/api/publish/types";

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

const props = defineProps<{
  modelValue: boolean;
  organizations: OrganizationList[];
}>();

const { t } = useI18n();
const show = ref(true);
const form = ref();
const serverHosts = ref<ServerHost[]>([]);

const model = ref({
  id: emptyGuid,
  name: "",
  serverUrl: "",
  orgId: props.organizations.find((f) => f.isAdmin)?.id,
});

const rules = {
  name: [
    requiredRule(t("common.nameRequiredTips")),
    simpleNameRule,
    rangeRule(1, 50),
  ],
  serverUrl: [
    urlAndIpRule(t("common.urlInvalid")),
    requiredRule(t("common.serverUrlRequiredTips")),
  ],
} as Rules;

const fromList = ref<KeyValue[]>([
  {
    key: "kooboo",
    value: "Kooboo" + t("common.online"),
  },
  {
    key: "custom",
    value: t("common.custom"),
  },
]);

const from = ref(fromList.value[0].key);

const onSave = async () => {
  await form.value.validate();
  await updateServer(model.value);
  emit("reload");
  show.value = false;
};

const loadServerHost = async () => {
  if (!model.value.orgId) return;
  const org = props.organizations.find((f) => f.id == model.value.orgId)!;
  serverHosts.value = (await getServerHosts(model.value.orgId)).map((m) => {
    m.host = `http://${org.name}.${m.host}`;
    return m;
  });
};

const onChangeType = () => {
  model.value.serverUrl = "";
};

loadServerHost();
</script>

<template>
  <el-dialog
    :model-value="show"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.addServer')"
    @closed="emit('update:modelValue', false)"
  >
    <el-form ref="form" label-position="top" :model="model" :rules="rules">
      <el-form-item prop="name" :label="t('common.name')">
        <el-input v-model="model.name" placeholder="kooboocn" />
      </el-form-item>
      <el-form-item :label="t('common.organization')" prop="orgId" required>
        <div class="flex items-center space-x-8 w-full">
          <el-select
            v-model="model.orgId"
            class="w-full"
            @change="loadServerHost"
          >
            <el-option
              v-for="item of organizations"
              :key="item.id"
              :label="item.displayName"
              :value="item.id"
            />
          </el-select>
        </div>
      </el-form-item>
      <el-form-item :label="t('common.from')">
        <el-radio-group
          v-model="from"
          class="el-radio-group--rounded h-38px"
          @change="onChangeType"
        >
          <el-radio-button
            v-for="item of fromList"
            :key="item.key"
            :label="item.key"
            >{{ item.value }}</el-radio-button
          >
        </el-radio-group>
      </el-form-item>

      <el-form-item v-if="from == 'kooboo'" prop="serverUrl">
        <div class="flex items-center space-x-8 w-full">
          <el-select v-model="model.serverUrl" class="w-full">
            <el-option
              v-for="item of serverHosts"
              :key="item.host"
              :label="`${item.dataCenter} (${item.host})`"
              :value="item.host"
            />
          </el-select>
        </div>
      </el-form-item>
      <el-form-item v-if="from == 'custom'" prop="serverUrl">
        <el-input
          v-model="model.serverUrl"
          placeholder="https://www.kooboo.cn"
        />
      </el-form-item>
    </el-form>
    <template #footer>
      <DialogFooterBar
        :disabled="!model.name || !model.serverUrl"
        @confirm="onSave"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
