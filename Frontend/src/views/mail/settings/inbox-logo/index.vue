<template>
  <div class="p-24">
    <div class="flex space-x-16">
      <div
        class="flex-1 bg-fff p-24 rounded-normal dark:bg-[#333] shadow-s-10 dark:text-fff/60"
      >
        <h1
          class="text-000 text-30px font-medium mb-24px h-7 leading-7 dark:text-fff/86"
        >
          {{ result?.logoName }}
        </h1>

        <div
          class="flex justify-between items-center h-28px leading-28px text-20px font-medium text-000 dark:text-fff/60"
        >
          {{ result?.logoTitle }}
        </div>
        <div class="mt-8">
          <ul
            class="space-y-8 text-000 text-opacity-65 text-m dark:text-fff/60"
          >
            <li class="ellipsis">
              <div v-html="result?.logoDescription" />
            </li>
          </ul>
        </div>
        <el-divider class="mt-28px mb-20px" />

        <span class="dark:text-fff/86">
          {{ t("common.currentUrl") }}:<span class="ml-16">{{
            result?.logo
          }}</span></span
        >
        <div class="mt-12">
          <el-form
            ref="logoForm"
            class="el-form--label-normal"
            :model="logoModel"
            :rules="rules"
          >
            <el-form-item prop="url">
              <el-input v-model="logoModel.url" class="w-400px" />
              <span
                class="text-blue cursor-pointer ml-12 h-10 leading-10"
                @click="update('logo')"
              >
                {{ t("common.update") }}
              </span>
            </el-form-item>
          </el-form>
        </div>
      </div>

      <div
        class="flex-1 bg-fff p-24 rounded-normal dark:bg-[#333] shadow-s-10 dark:text-fff/60"
      >
        <h1
          class="text-000 text-30px font-medium mb-24px h-7 leading-7 dark:text-fff/86"
        >
          {{ result?.vmcName }}
        </h1>

        <div
          class="flex justify-between items-center h-28px leading-28px text-20px font-medium text-000 dark:text-fff/60"
        >
          {{ result?.vmcTitle }}
        </div>
        <div class="mt-8">
          <ul
            class="space-y-8 text-000 text-opacity-65 text-m dark:text-fff/60"
          >
            <li class="ellipsis">
              <div v-html="result?.vmcDescription" />
            </li>
          </ul>
        </div>
        <el-divider class="mt-28px mb-20px" />

        <span class="dark:text-fff/86">
          {{ t("common.currentUrl") }}:<span class="ml-16 dark:text-fff/86">{{
            result?.vmc
          }}</span></span
        >
        <div class="mt-12">
          <el-form
            ref="vmcForm"
            class="el-form--label-normal"
            :model="vmcModel"
            :rules="rules"
          >
            <el-form-item prop="url">
              <el-input v-model="vmcModel.url" class="w-400px" />
              <span
                class="text-blue cursor-pointer ml-12 h-10 leading-10"
                @click="update('vmc')"
              >
                {{ t("common.update") }}
              </span>
            </el-form-item>
          </el-form>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { getBimi, updateVmcApi, updateLogoApi } from "@/api/mail";
import { urlRule } from "@/utils/validate";
import { ElMessage } from "element-plus";

const { t } = useI18n();
const result = ref();
const logoForm = ref();
const logoModel = ref({
  url: "",
});
const vmcForm = ref();
const vmcModel = ref({
  url: "",
});
const rules = {
  url: [urlRule(t("common.urlInvalid"))],
};

const load = async () => {
  result.value = await getBimi();
};

const update = async (type: string) => {
  if (type === "logo") {
    if (logoModel.value.url === "") {
      ElMessage.error(t("common.urlRequiredTips"));
      return;
    }
    await logoForm.value.validate();
    await updateLogoApi(logoModel.value.url);
    logoModel.value.url = "";
  } else {
    if (vmcModel.value.url === "") {
      ElMessage.error(t("common.urlRequiredTips"));
      return;
    }
    await vmcForm.value.validate();
    await updateVmcApi(vmcModel.value.url);
    vmcModel.value.url = "";
  }
  load();
};

load();
</script>
