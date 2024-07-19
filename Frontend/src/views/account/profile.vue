<script setup lang="ts">
import { ref, provide } from "vue";
import ProfileUser from "./components/profile-user.vue";
import { getUser } from "@/api/user";
import type { IUser } from "@/api/user/types";
import { useI18n } from "vue-i18n";
const { t } = useI18n();

const user = ref({
  userName: "",
  emailAddress: "",
  tel: "",
  firstName: "",
  lastName: "",
  fullName: "",
  password: "",
  currency: "",
} as IUser);

const load = async () => {
  user.value = await getUser();
};
load();
provide("user", user);
export type Load = typeof load;
provide("reloadUser", load);
</script>

<template>
  <div class="w-535px mx-auto mt-32">
    <div class="text-3l text-black dark:text-fff/86 font-medium mb-32">
      {{ t("common.profile") }}
    </div>

    <ProfileUser
      v-bind="{
        userName: user.userName,
        emailAddress: user.emailAddress,
        tel: user.tel,
        firstName: user.firstName,
        lastName: user.lastName,
        fullName: user.fullName,
        password: user.password,
        currency: user.currency,
      }"
      @reload="load"
    />
  </div>
</template>
