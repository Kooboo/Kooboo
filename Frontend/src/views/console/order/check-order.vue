<template>
  <div class="p-24 dark:text-[#fffa] overflow-x-hidden">
    <el-icon
      class="iconfont icon-fanhui1 cursor-pointer hover:text-blue"
      :title="t('common.back')"
      @click="$router.back()"
    />
    <h1
      class="text-000 dark:text-[#fffa] text-52px font-medium mt-4 mb-24px h-73px leading-73px"
    >
      {{ t("order.checkout") }}
    </h1>

    <div class="jumbotron flex">
      <!-- left -->
      <div
        class="px-6 pt-5 pb-32 bg-fff dark:bg-[#333] w-620px mr-40px rounded-normal shadow-s-10 flex-shrink-0"
      >
        <div class="flex justify-between items-center h-28px leading-28px">
          <h1 class="text-20px font-medium text-000 dark:text-[#fffa]">
            {{ info?.title }}
          </h1>
        </div>
        <ul class="space-y-8 text-000 dark:text-[#fffa] text-opacity-65 text-m">
          <li class="ellipsis">
            <div v-html="info?.summary" />
          </li>
        </ul>
        <el-divider class="mt-28px mb-20px" />
        <h1 class="text-20px font-medium text-000 dark:text-[#fffa] mb-4">
          {{ t("common.paymentMethod") }}
        </h1>
        <el-tabs
          v-model="activeTab"
          class="el-tabs--hide-content partnerTab mb-28px"
        >
          <el-tab-pane :label="t('common.creditCard')" name="CreditCard" />
          <el-tab-pane :label="t('common.wechatPay')" name="WechatPay" />
        </el-tabs>

        <div v-if="activeTab === 'CreditCard'">
          <div class="space-x-24 flex mb-28px">
            <div
              v-for="item in cardList"
              :key="item.display"
              class="w-94px h-36px rounded-full flex items-center justify-center cursor-pointer"
              :class="
                item.id === currentCard?.id && !showAddCardForm
                  ? 'bg-blue text-fff'
                  : 'bg-[#192845] bg-opacity-5 dark:bg-666'
              "
              @click="chooseCard(item)"
            >
              *{{ item.display }}
            </div>
            <div
              v-if="!showAddCardForm"
              class="w-36px h-36px rounded-full bg-blue flex items-center justify-center cursor-pointer"
              @click="showAddCardForm = true"
            >
              <el-icon class="iconfont icon-a-addto text-fff font-medium" />
            </div>
          </div>
          <el-form
            v-if="showAddCardForm"
            ref="form"
            :model="model"
            :rules="rules"
            label-position="top"
            @submit.prevent
          >
            <div class="flex items-center">
              <el-form-item
                prop="cardNumber"
                :label="t('common.cardNumber')"
                class="!mr-32"
              >
                <el-input
                  v-model="model.cardNumber"
                  class="w-260px"
                  maxlength="19"
                  placeholder="0000-0000-0000-0000"
                  @input="
                    model.cardNumber = model.cardNumber
                      .replace(/\s/g, '')
                      .replace(/[^\d]/g, '')
                      .replace(/(\d{4})(?=\d)/g, '$1-')
                  "
                />
              </el-form-item>
              <div class="flex items-center justify-between space-x-16">
                <el-form-item
                  prop="month"
                  :label="t('common.expirationDate')"
                  class="flex-1"
                >
                  <el-select
                    v-model="model.month"
                    class=" "
                    :placeholder="t('common.month')"
                    size="large"
                  >
                    <el-option
                      v-for="item in months"
                      :key="item"
                      :label="item"
                      :value="item"
                    />
                  </el-select>
                </el-form-item>
                <el-form-item prop="year" class="flex-1 yearFormItem" label=" ">
                  <el-select
                    v-model="model.year"
                    class=" "
                    :placeholder="t('common.year')"
                    size="large"
                  >
                    <el-option
                      v-for="item in years"
                      :key="item"
                      :label="item"
                      :value="item"
                    />
                  </el-select>
                </el-form-item>
              </div>
            </div>
            <div class="flex items-center justify-between mt-6px">
              <el-form-item
                prop="name"
                :label="t('common.nameOnCard')"
                class="mr-32"
              >
                <el-input
                  v-model="model.name"
                  class="w-260px"
                  :placeholder="t('common.enterYourNameHere')"
                />
              </el-form-item>
              <el-form-item prop="cvc" :label="t('common.cvc')" class="flex-1">
                <el-input
                  v-model="model.cvc"
                  class="w-full"
                  maxlength="3"
                  :placeholder="t('common.enterCVCHere')"
                  @input="
                    model.cvc = model.cvc
                      .replace(/\s/g, '')
                      .replace(/[^\d]/g, '')
                  "
                />
              </el-form-item>
            </div>
          </el-form>
        </div>

        <div v-if="activeTab === 'WechatPay'" class="w-180px h-220px mx-auto">
          <div v-if="payInfo" class="flex flex-col">
            <VueQr
              :text="payInfo.nextAction.responseData"
              :size="2000"
              class="transform scale-75"
            />
            <div class="flex items-center justify-center h-auto">
              <img
                class="transform scale-80 -m-12"
                src="https://td.cdn-go.cn/enterprise_payment/v0.0.9/logo.png"
              />
            </div>
          </div>
        </div>
      </div>
      <!-- right -->
      <div
        class="px-6 pt-5 pb-44px bg-fff dark:bg-[#333] text-m w-500px rounded-normal shadow-s-10 text-000 dark:text-[#fffa]"
        style="height: fit-content"
      >
        <h1 class="text-20px font-medium">{{ t("common.orderSummary") }}</h1>
        <div
          v-for="item in info?.items"
          :key="item.name"
          class="flex justify-between items-center h-6 leading-6 text-m my-8"
        >
          <span>{{ item.name }}</span>
          <span class="text-000 dark:text-[#fffa]"> {{ item.price }}</span>
        </div>
        <el-divider class="mb-28px mt-5" />
        <ul class="space-y-8 mb-8">
          <li class="flex justify-between items-center h-5 leading-5">
            <span>{{ t("common.subtotal") }}</span
            ><span>{{ info?.subTotal }}</span>
          </li>
          <li class="flex justify-between items-center h-5 leading-5">
            <span>{{ t("common.vat") }}</span
            ><span>{{ info?.tax }}</span>
          </li>
        </ul>
        <div
          class="flex justify-between items-center font-medium h-60px text-20px mt-28px mb-37px border-t-1 border-b-1 border-[#dcdfe6] dark:border-[#4c4d4f]"
        >
          <span>{{ t("common.total") }} ({{ info?.currency }})</span>
          <span>{{ info?.total }}</span>
        </div>
        <span class="flex items-center justify-center">
          <el-button
            type="primary"
            round
            class="w-300px"
            :disabled="
              activeTab === 'WechatPay' ||
              (!showAddCardForm && currentCard?.id === '')
            "
            @click="submit"
            >{{
              activeTab === "CreditCard"
                ? t("common.submitPayment")
                : t("common.scanToPay")
            }}</el-button
          >
        </span>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { onUnmounted, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";
import { cvcRangeRule, rangeRule, requiredRule } from "@/utils/validate";
import request from "@/utils/request";
import {
  getCards,
  getInfo,
  IPaymentResponse,
  submitPaymentMethod,
  submitCreditCard,
} from "@/api/console";
import VueQr from "vue-qr/src/packages/vue-qr.vue";
import type { Card, OrderInfo } from "@/api/console/types";
import { getQueryString } from "@/utils/url";

const { t } = useI18n();
const router = useRouter();
const activeTab = ref("CreditCard");
const showAddCardForm = ref(false);
let intervalId: any;

const cardList = ref<Card[]>();
const info = ref<OrderInfo>();
const currentCard = ref<Card>({
  id: "",
  type: "",
  isDefault: false,
  display: "",
});
const chooseCard = (card: Card) => {
  currentCard.value = JSON.parse(JSON.stringify(card));
  showAddCardForm.value = false;
};
const model = ref({
  cardNumber: "",
  year: "",
  month: "",
  name: "",
  cvc: "",
});
const rules = {
  name: [requiredRule(t("common.fieldRequiredTips")), rangeRule(1, 50)],
  cardNumber: [requiredRule(t("common.fieldRequiredTips"))],
  year: [requiredRule(t("common.fieldRequiredTips"))],
  month: [requiredRule(t("common.fieldRequiredTips"))],
  cvc: [requiredRule(t("common.fieldRequiredTips")), cvcRangeRule()],
};

const form = ref();
const months = ref([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12]);
const years = ref([
  2023, 2024, 2025, 2026, 2027, 2028, 2029, 2030, 2031, 2032, 2033, 2034, 2035,
  2036, 2037, 2038, 2039, 2040, 2041, 2042, 2043, 2044, 2045, 2046, 2047, 2048,
  2049, 2050, 2051, 2052, 2053, 2054, 2055, 2056, 2057, 2058, 2059, 2060, 2061,
  2062, 2063, 2064, 2065, 2066, 2067, 2068, 2069, 2070, 2071, 2072, 2073,
]);
const payInfo = ref();
const orderId = getQueryString("orderId");

const submit = async () => {
  let result: any;
  if (showAddCardForm.value) {
    await form.value.validate();
    result = await submitCreditCard({
      name: model.value.name,
      cardNumber: model.value.cardNumber.replaceAll("-", ""),
      year: model.value.year,
      month: model.value.month,
      cvc: model.value.cvc,
      orderId: orderId,
    });
  } else {
    result = await submitPaymentMethod(currentCard.value.id, orderId!);
  }
  if (!result.success) {
    window.location.href = result.redirectUrl;
  } else {
    router.push({
      name: "consoleOrder",
    });
  }
};

watch(
  () => showAddCardForm.value,
  () => {
    if (!showAddCardForm.value) {
      model.value.cardNumber = "";
      model.value.year = "";
      model.value.month = "";
      model.value.name = "";
      model.value.cvc = "";
    }
  }
);

const load = async () => {
  if (orderId) {
    info.value = await getInfo(orderId);
  }
  cardList.value = await getCards();
  if (cardList.value.length) {
    currentCard.value = cardList.value.filter((f) => f.isDefault)[0];
  }
};
load();
async function checkStatus() {
  var url = `marketOrder/CheckStatus?id=${payInfo.value!.requestId}`;
  var rsp = await request.get<{ paid: string }>(url, undefined, {
    hiddenLoading: true,
  });
  if (rsp.paid) {
    clearInterval(intervalId);
    router.push({
      name: "consoleOrder",
    });
  }
}

watch(
  () => activeTab.value,
  async () => {
    if (activeTab.value === "WechatPay" && orderId) {
      payInfo.value = await IPaymentResponse(orderId);
      intervalId = setInterval(checkStatus, 1000);
    } else {
      clearInterval(intervalId);
    }
  }
);

onUnmounted(() => {
  clearInterval(intervalId);
});
</script>
<style>
.partnerTab .el-tabs__header.is-top {
  padding-left: 0;
  padding-right: 0;
  background: transparent;
}
.yearFormItem label {
  opacity: 0;
}
</style>
