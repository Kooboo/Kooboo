<script lang="ts" setup>
import { useI18n } from "vue-i18n";
import SearchInput from "@/components/basic/search-input.vue";
import { ref } from "vue";
import { getPurchaseDomain } from "@/api/console";
import OrderDomainDialog from "./order-domain-dialog.vue";
import type { PurchaseDomain } from "@/api/console/types";
import { useFirstInputFocus } from "@/hooks/use-first-input-focus";
import {
  rangeRule,
  domainSearchRule,
  notAllowMultilevelDomain,
} from "@/utils/validate";

const { t } = useI18n();
useFirstInputFocus();

const extensions = ref([
  {
    name: "com",
    selected: true,
  },
  {
    name: "net",
    selected: true,
  },
  {
    name: "site",
    selected: true,
  },
  {
    name: "dev",
    selected: true,
  },
  {
    name: "land",
    selected: false,
  },
  {
    name: "io",
    selected: false,
  },
  {
    name: "cc",
    selected: false,
  },
  {
    name: "co",
    selected: false,
  },
  {
    name: "eu",
    selected: false,
  },
  {
    name: "uk",
    selected: false,
  },
  {
    name: "nl",
    selected: false,
  },
  {
    name: "es",
    selected: false,
  },
]);

const searchResult = ref<PurchaseDomain[]>([]);
const model = ref({
  keyword: "",
});
const showOrderDomainDialog = ref(false);
const selected = ref<PurchaseDomain>();
const form = ref();

const rules = {
  keyword: [rangeRule(1, 63), domainSearchRule(), notAllowMultilevelDomain()],
};

async function searchEvent() {
  await form.value.validate();
  // 去掉开头和结尾的"-"
  model.value.keyword = model.value.keyword
    .replace(/^-+/, "")
    .replace(/-+$/, "");
  model.value.keyword = model.value.keyword.replace(/^\.+|\.+$/g, "");

  const tlds = extensions.value.filter((f) => f.selected).map((m) => m.name);

  // 搜索时去掉"."以及之后的文字
  const withExtension = model.value.keyword.indexOf(".") > -1;
  let keyword;

  if (withExtension) {
    tlds.push(model.value.keyword.split(".")[1]);
    keyword = model.value.keyword.split(".")[0];
  } else {
    keyword = model.value.keyword;
  }

  if (!tlds.length || !model.value.keyword.trim()) {
    searchResult.value = [];
    return;
  }

  const result = await getPurchaseDomain(tlds, keyword);
  searchResult.value = result.results;
}

function orderDomain(domain: PurchaseDomain) {
  selected.value = domain;
  showOrderDomainDialog.value = true;
}
</script>

<template>
  <div class="p-24 space-y-24">
    <div class="text-444 text-2l font-bold">
      {{ t("common.domainSearch") }}
    </div>

    <div class="bg-fff dark:bg-444 rounded-normal space-y-24 py-32">
      <el-form
        ref="form"
        :model="model"
        :rules="rules"
        class="w-full flex justify-center"
        @submit.prevent
      >
        <el-form-item prop="keyword">
          <SearchInput
            v-model="model.keyword"
            placeholder="Input domain name like: 'kooboo'"
            class="w-500px"
            @keypress.enter="searchEvent"
          />
          <el-button type="primary" round class="ml-8" @click="searchEvent">
            {{ t("common.search") }}
          </el-button>
        </el-form-item>
      </el-form>

      <div class="flex justify-center flex-wrap">
        <ElCheckbox
          v-for="item of extensions"
          :key="item.name"
          v-model="item.selected"
          class="pb-8"
          :label="`.${item.name}`"
        />
      </div>
      <div class="max-w-1000px mx-auto px-24">
        <ElTable v-if="searchResult.length" :data="searchResult">
          <ElTableColumn prop="domain" :label="t('common.name')" />
          <ElTableColumn :label="t('common.price')">
            <template #default="{ row }">
              <div v-if="row.price" class="flex items-center space-x-4">
                <span>{{ row.price.product.price.toFixed(2) }}</span>
                <div class="text-blue text-s">
                  <span>{{ row.price.product.currency }}</span>
                  <span>/</span>
                  <span>1{{ t("common.year") }}</span>
                </div>
              </div>
            </template>
          </ElTableColumn>

          <ElTableColumn :label="t('common.status')">
            <template #default="{ row }">
              <div v-if="row.status == 'free'" class="text-blue">
                {{ t("common.canRegister") }}
              </div>
              <div v-else class="text-orange">
                {{ t("common.cantRegister") }}
              </div>
            </template>
          </ElTableColumn>

          <ElTableColumn align="center">
            <template #default="{ row }">
              <ElButton
                round
                plain
                type="primary"
                :disabled="row.status != 'free'"
                @click="orderDomain(row)"
              >
                {{ t("common.immediatelyGet") }}
              </ElButton>
            </template>
          </ElTableColumn>
        </ElTable>
        <ElEmpty v-else description="Please search domain name" />
      </div>
    </div>

    <OrderDomainDialog
      v-if="showOrderDomainDialog"
      v-model="showOrderDomainDialog"
      :domain="selected!"
    />
  </div>
</template>
