<script setup lang="ts">
import KAvatar from "@/components/basic/avatar.vue";
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import ChangePhoneDialog from "./change-phone-dialog.vue";
import ChangeEmailDialog from "./change-email-dialog.vue";
import ChangePasswordDialog from "./change-password-dialog.vue";
import ChangeCurrencyDialog from "./change-currency-dialog.vue";
import EditNameDialog from "./edit-name-dialog.vue";
import { getPayload } from "@/utils/jwt";
import { useAppStore } from "@/store/app";
import Cookies from "universal-cookie";
const cookies = new Cookies();

const { t } = useI18n();
const token = cookies.get("jwt_token");
const payload = getPayload(token!);
const appStore = useAppStore();

interface Props {
  userName: string;
  emailAddress: string;
  tel: string;
  firstName: string;
  lastName: string;
  password: string;
  currency: string;
  fullName: string;
}

const emit = defineEmits<{
  (e: "reload"): void;
}>();

const currentPhone = ref<{ tel: string }>({
  tel: "",
});

type ShowDialogType = {
  phone: boolean;
  email: boolean;
  password: boolean;
  name: boolean;
  currency: boolean;
};

const showDialog = ref<ShowDialogType>({
  phone: false,
  email: false,
  password: false,
  name: false,
  currency: false,
});

const currentName = ref<{ firstName: string; lastName: string }>({
  firstName: "",
  lastName: "",
});
const currentUsername = ref<string>("");
const currentEmail = ref<string>("");
const props = defineProps<Props>();
const changePhone = () => {
  showDialog.value.phone = true;
  currentPhone.value.tel = props.tel;
};
const editName = () => {
  showDialog.value.name = true;
  currentName.value.firstName = props.firstName;
  currentName.value.lastName = props.lastName;
};

const changePassword = () => {
  showDialog.value.password = true;
  currentUsername.value = props.userName;
};

const changeEmail = () => {
  showDialog.value.email = true;
  currentEmail.value = props.emailAddress;
  currentUsername.value = props.userName;
};

const changeCurrency = () => {
  showDialog.value.currency = true;
};

watch(
  () => showDialog.value,
  (dialog) => {
    if (!dialog.phone) {
      currentPhone.value.tel = "";
    }
    if (!dialog.name) {
      currentName.value.firstName = "";
      currentName.value.lastName = "";
    }
  }
);
</script>

<template>
  <div
    class="p-24 bg-fff dark:bg-[#333] rounded-t-normal shadow-s-10 mb-1px"
    data-cy="username-abbr"
  >
    <div class="flex items-center">
      <KAvatar :username="props.userName" :size="96" class="mr-16" />
      <div
        class="text-l text-black dark:text-fff/86 flex items-center"
        data-cy="username"
      >
        <div class="mr-8">{{ t("common.username") }}:</div>
        {{ props.userName }}
      </div>
    </div>
  </div>
  <div class="p-24 bg-fff dark:bg-[#333] rounded-b-normal shadow-s-10 mb-24">
    <table class="dark:text-fff/80 text-m">
      <tr class="h-36px leading-9">
        <td class="pr-32">{{ t("common.email") }}:</td>
        <td class="flex items-center">
          <span
            class="ellipsis w-200px"
            :title="props.emailAddress || t('common.notBound')"
            >{{ props.emailAddress || t("common.notBound") }}</span
          >
        </td>
        <td>
          <div
            class="text-blue cursor-pointer ml-8"
            data-cy="change-email"
            @click="changeEmail"
          >
            {{ props.emailAddress ? t("common.change") : t("common.bind") }}
          </div>
        </td>
      </tr>
      <tr v-if="payload" class="h-36px leading-9">
        <td class="pr-32">{{ t("common.password") }}:</td>
        <td class="w-200px relative">
          <span class="absolute top-4">********</span>
        </td>
        <td>
          <div
            class="text-blue cursor-pointer ml-8"
            data-cy="change-password"
            @click="changePassword"
          >
            {{ t("profile.change") }}
          </div>
        </td>
      </tr>
      <tr class="h-36px leading-9">
        <td class="pr-32">{{ t("profile.name") }}:</td>
        <td class="flex items-center">
          <span
            v-if="props.firstName?.length + props.lastName?.length"
            class="w-200px ellipsis"
            :title="props.fullName"
          >
            {{ props.fullName }}
          </span>
          <span v-else>{{ t("common.none") }}</span>
        </td>

        <td>
          <div class="text-blue cursor-pointer ml-8" @click="editName">
            {{ t("common.edit") }}
          </div>
        </td>
      </tr>
      <tr class="h-36px leading-9">
        <td class="pr-32">{{ t("profile.phone") }}:</td>
        <td class="w-200px">
          <div class="">{{ props.tel || t("common.notBound") }}</div>
        </td>
        <td>
          <div class="text-blue cursor-pointer ml-8" @click="changePhone">
            {{ props.tel ? t("common.change") : t("common.bind") }}
          </div>
        </td>
      </tr>
      <tr v-if="appStore.currentOrg?.isAdmin" class="h-36px leading-9">
        <td class="pr-32">{{ t("common.currency") }}:</td>
        <td class="w-200px">
          <div class="">{{ props.currency }}</div>
        </td>
        <td>
          <div class="text-blue cursor-pointer ml-8" @click="changeCurrency">
            {{ t("common.changeCurrency") }}
          </div>
        </td>
      </tr>
    </table>
  </div>
  <ChangePhoneDialog
    v-if="showDialog.phone"
    v-model="showDialog.phone"
    :phone="currentPhone"
  />
  <ChangeEmailDialog
    v-if="showDialog.email"
    v-model="showDialog.email"
    :email="currentEmail"
  />
  <EditNameDialog
    v-if="showDialog.name"
    v-model="showDialog.name"
    :name="currentName"
  />
  <ChangePasswordDialog
    v-if="showDialog.password"
    v-model="showDialog.password"
    :username="currentUsername"
  />
  <ChangeCurrencyDialog
    v-if="showDialog.currency"
    v-model="showDialog.currency"
    :currency="props.currency"
  />
</template>
