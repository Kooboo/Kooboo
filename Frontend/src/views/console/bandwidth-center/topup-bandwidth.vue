<template>
  <div class="p-24 tracking-[1px]">
    <div class="text-444 text-2l font-bold dark:text-999 mb-8">
      {{ t("common.domainPurchase") }}
    </div>
    <div
      class="w-full p-24 m-auto pt-16 dark:text-gray bg-fff rounded-normal text-m dark:bg-444"
    >
      <div class="space-y-16 mt-12">
        <div>
          {{ t("common.account") }}:
          <span class="ml-12 text-[#262938] dark:text-gray">ALmostblue</span>
        </div>
        <div>
          {{ t("common.remainingBandwidth") }}:
          <span class="ml-12 text-[#262938] dark:text-gray">120G</span>
        </div>
        <div class="w-full h-1px bg-[#EBEEF5] dark:bg-666" />
      </div>

      <div>
        <div class="mt-[30px] mb-[18px] text-black dark:text-999">
          {{ t("common.rechargeAmount") }}
        </div>
        <div class="flex space-x-12">
          <div
            class="cursor-pointer space-y-8 w-[220px] h-[110px] border-1 border-solid border-[#EBEEF5] rounded-normal flex items-center justify-center"
          >
            <div class="space-y-8 text-center">
              <div class="text-orange text-[18px] font-600">120G/月</div>
              <div class="text-999 text-m">连续包月$4.00/月</div>
            </div>
          </div>

          <div
            class="cursor-pointer space-y-8 w-[220px] h-[110px] border-1 border-solid border-[#EBEEF5] rounded-normal flex items-center justify-center"
          >
            <div class="space-y-8 text-center">
              <div class="text-orange text-[18px] font-600">120G/月</div>
              <div class="text-999 text-m">$8.00</div>
            </div>
          </div>

          <div
            class="space-y-8 w-[220px] h-[110px] border-1 border-solid border-[#EBEEF5] rounded-normal flex items-center justify-center"
          >
            <div>
              <div class="flex items-center mb-4">
                <el-input v-model="customBandwidth" class="w-50px" />
                <div class="w-1px h-18px bg-999 mx-4" />
                <div class="text-999 text-[18px] font-600">G</div>
              </div>
              <div class="bg-blue w-full h-1px my-4px" />
              <div class="text-999 text-m text-center">$10.00</div>
            </div>
          </div>
        </div>
        <div class="w-full h-1px bg-[#EBEEF5] my-24 dark:bg-666" />
        <div class="text-s text-[#A7B0BE] mb-8">
          温馨提示:网站设置防刷系统，请合理使用海量下载账号，如用户短时间内高频或大量下载可能会触发该系统，将暂时暂停账号下载功能，如有疑问请联系客服。下载内容不包含“全栈网站”、“平台网站库”素材。”用户自行上传”、“创意”索材因版权方要求，无法提供个人VIP下载使用，流量仅限平台使用，售出流量不退不换，请谨慎下单。
        </div>
        <div class="flex">
          <el-checkbox />
          <span class="ml-8 h-40px flex items-center"
            >{{ t("common.IhaveRead")
            }}<span class="text-blue">{{
              t("common.notesOnPlatformTopupBandwidth")
            }}</span></span
          >
        </div>
      </div>
    </div>
    <div
      v-bind="$attrs"
      class="k-bottom-bar bg-gray/10 py-15px pr-62px text-right space-x-32 fixed bottom-0 w-full left-0 z-1 backdrop-filter backdrop-blur-md shadow-m-20"
    >
      <el-button round @click="router.replace({ name: 'bandwidth' })">{{
        t("common.cancel")
      }}</el-button>
      <el-button round type="primary">{{ t("common.submitOrder") }}</el-button>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";

const router = useRouter();
const { t } = useI18n();
const customBandwidth = ref(180);

// async function load() {
//   siteList.value = await getSiteList();
//   const domain = getQueryString("domain");
//   if (domain) {
//     model.value.domain = domain;
//   }
//   if (siteList.value[0].siteId) {
//     model.value.siteId = siteList.value[0].siteId;
//   }
// }

// load();
</script>
<style scoped>
:deep(.el-input__wrapper) {
  box-shadow: 0 0 0 0px var(--el-input-border-color, var(--el-border-color))
    inset;
  padding-left: 0;
  padding-right: 0;
}
:deep(.el-input__inner) {
  height: 18px !important;
  text-align: center !important;
  font-size: 18px;
  color: #2296f3;
  font-weight: 600;
}
</style>
