<template>
  <el-dialog
    :model-value="show"
    width="670px"
    :title="
      addType === 'new' ? t('common.addSubAccount') : t('common.addExisting')
    "
    @closed="emit('update:modelValue', false)"
  >
    <el-form
      ref="form"
      class="el-form--label-normal"
      :model="model"
      :rules="(rules as any)"
      label-position="left"
      label-width="150px"
      @submit.prevent
    >
      <el-form-item prop="username" :label="t('common.username')">
        <el-input v-model="model.username" class="w-400px" />
      </el-form-item>
      <el-form-item prop="password" :label="t('common.password')">
        <el-input v-model="model.password" class="w-400px" type="password" />
      </el-form-item>
      <el-form-item prop="server" :label="t('common.server')">
        <el-select v-model="model.serverId" class="w-400px" placeholder=" ">
          <el-option
            v-for="item of servers"
            :key="item.id"
            :label="item.displayName"
            :value="item.id"
          />
        </el-select>
      </el-form-item>
      <div v-if="addType === 'new'">
        <el-form-item prop="remark" :label="t('common.remark')">
          <el-input v-model="model.remark" class="w-400px" />
        </el-form-item>
        <el-form-item prop="email" :label="t('common.email')">
          <el-input v-model="model.email" class="w-400px" />
        </el-form-item>
        <el-form-item prop="phone" :label="t('common.phone')">
          <el-input v-model="model.phone" class="w-400px" />
        </el-form-item>
      </div>
    </el-form>
    <template #footer>
      <DialogFooterBar
        :confirm-label="t('common.add')"
        @confirm="onAdd"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
<script lang="ts" setup>
import {
  commerceValueRangeRule,
  passwordRule,
  requiredRule,
  emailRule,
  phoneRule,
  usernameRules,
  passwordLengthRule,
} from "@/utils/validate";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import {
  addNewAccount,
  addExistingAccount,
  getPartnerServer,
} from "@/api/partner";
import type { AddUser, partnerServer } from "@/api/partner/type";

const props = defineProps<{
  modelValue: boolean;
  addType: string;
}>();
const { t } = useI18n();
const show = ref(true);

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "add-success"): void;
}>();

const model = ref<AddUser>({
  username: "",
  password: "",
  serverId: "",
  remark: "",
  email: "",
  phone: "",
});
const servers = ref<partnerServer[]>([]);

const form = ref();
const rules = {
  username:
    props.addType === "new"
      ? usernameRules
      : requiredRule(t("common.inputUsernameTips")),
  password:
    props.addType === "new"
      ? [
          passwordLengthRule,
          passwordRule,
          requiredRule(t("common.inputPasswordTips")),
        ]
      : [requiredRule(t("common.inputPasswordTips"))],
  email: emailRule,
  phone: phoneRule,
};

const onAdd = async () => {
  await form.value.validate();
  if (props.addType === "new") {
    await addNewAccount(model.value);
  } else {
    await addExistingAccount(model.value);
  }
  show.value = false;
  emit("add-success");
};
const load = async () => {
  servers.value = await getPartnerServer();
  if (servers.value.length) {
    model.value.serverId = servers.value[0].id;
  }
};
load();
</script>
