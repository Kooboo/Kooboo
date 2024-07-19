<script setup lang="ts">
import { getMemberOptions, purchaseMemberShip } from "@/api/member";
import type { MemberOption } from "@/api/member/types";
import { getUser } from "@/api/user";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";
const { t } = useI18n();
const router = useRouter();
const memberOptionList = ref<MemberOption[]>();

const load = async () => {
  await getUser();
  memberOptionList.value = await getMemberOptions();
};

const purchase = async (servicelevel: number) => {
  const orderId = await purchaseMemberShip(servicelevel);
  router.push({
    name: "checkOrder",
    query: {
      orderId: orderId as string,
    },
  });
};
load();
</script>

<template>
  <div class="p-24">
    <div class="flex space-x-32 px-64">
      <div
        v-for="item in memberOptionList"
        :key="item.name"
        class="border-[#cccdcd] flex-1 rounded-normal hover:shadow-l-10 dark:hover:shadow-m-20 group box-border dark:text-gray"
      >
        <div class="h-16 w-full rounded-t-normal bg-fff dark:bg-[#252526]" />
        <div class="bg-[#f9faff] relative">
          <div class="dark:bg-[#252526]">
            <div
              class="p-16 rounded-b-normal flex justify-center min-h-260px shadow-s-10 group-hover:shadow-s-4 text-m bg-fff dark:bg-[#252526]"
            >
              <div class="flex flex-col text-center">
                <div class="py-8 text-18px font-bold">{{ item.title }}</div>
                <div class="my-8 text-m text-[#666F80] dark:text-gray">
                  {{ item.headLine }}
                </div>

                <div class="flex-1 flex items-center justify-center">
                  <span class="text-32px mx-8 font-bold"
                    >{{ item.currencySymbol }} {{ item.price }}</span
                  >
                  <span class="text-s text-[#666F80] dark:text-gray">
                    /{{
                      item.isMonth ? t("common.month") : t("common.year")
                    }}</span
                  >
                </div>
                <div class="flex items-center justify-center py-24">
                  <el-button
                    round
                    type="primary"
                    :disabled="item.isActive"
                    @click="purchase(item.serviceLevel)"
                    ><span class="text-fff">{{
                      !item.isActive
                        ? t("common.purchaseNow")
                        : t("common.inUse")
                    }}</span></el-button
                  >
                </div>
              </div>
            </div>
          </div>
        </div>

        <div
          class="p-24 bg-[#f9faff] rounded-b-normal dark:bg-[#252526] dark:bg-opacity-60"
        >
          <div class="space-y-16 my-12 font-600">
            <div class="text-[#32373F] dark:text-gray mt-8">
              {{ t("common.FreeToHostYourself") }}
            </div>
            <div class="text-[#32373F] dark:text-gray">
              {{ item.functionTitle }}
            </div>
          </div>

          <div class="mt-32 text-m">
            <div class="text-[#666F80] mb-8 dark:text-gray">
              {{ t("common.OnlineFeatures") }}
            </div>
            <ul class="list-disc list-inside min-h-150px">
              <li
                v-for="(itm, index) in item.functions"
                :key="index"
                class="py-4 px-8 text-[#2D61FC]"
              >
                <span class="text-[#2F353F] dark:text-gray">
                  {{ itm }}
                </span>
              </li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
