<script lang="ts" setup>
import { useRoute, useRouter } from "vue-router";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useI18n } from "vue-i18n";
import { computed, onMounted, ref } from "vue";
import { useRouteSiteId } from "@/hooks/use-site-id";
import type { TaxItem } from "@/api/commerce/tax";
import { getTaxes, deleteTaxes } from "@/api/commerce/tax";
import { showDeleteConfirm } from "@/components/basic/confirm";
import SelectCountryDialog from "../components/select-country-dialog.vue";
import type { Country } from "@/api/commerce/address";
import { useCommerceStore } from "@/store/commerce";
import { systemDisplay } from "@/utils/commerce";

const { t } = useI18n();
const route = useRoute();
const routeName = route.meta.title as string;
const router = useRouter();
const list = ref<TaxItem[]>([]);
const showSelectCountryDialog = ref(false);
const commerceStore = useCommerceStore();

function countrySelected(country: Country) {
  router.push(
    useRouteSiteId({
      name: "taxes create",
      query: {
        country: country.name,
      },
    })
  );
}

async function onDelete(rows: TaxItem[]) {
  await showDeleteConfirm(rows.length);
  await deleteTaxes(rows.map((m) => m.id));
  load();
}

async function load() {
  list.value = await getTaxes();
}

onMounted(async () => {
  load();
});

function getCountry(name: string) {
  return commerceStore?.countries.find((f) => f.name == name);
}

const excludeCountries = computed(() => list.value.map((m) => m.country));
</script>

<template>
  <div class="p-24">
    <Breadcrumb :name="routeName" />
    <div class="flex items-center py-24 space-x-16">
      <el-button
        v-hasPermission="{ feature: 'taxes', action: 'edit' }"
        round
        @click="showSelectCountryDialog = true"
      >
        <div class="flex items-center">
          <el-icon class="mr-16 iconfont icon-a-addto" />
          {{ t("common.create") }}
        </div>
      </el-button>
    </div>
    <KTable :data="list" show-check @delete="onDelete">
      <el-table-column :label="t('common.country')">
        <template #default="{ row }">
          <div v-if="getCountry(row.country)" class="flex items-center gap-4">
            <Country
              :only-flag="true"
              :name-or-code="getCountry(row.country)?.code"
            />
            <span class="text-black dark:text-[#cfd3dc]">{{
              systemDisplay(
                getCountry(row.country)!.nameTranslations,
                row.country
              )
            }}</span>
          </div>
        </template>
      </el-table-column>

      <el-table-column :label="t('common.baseTax')" align="center">
        <template #default="{ row }"> {{ row.baseTax }}% </template>
      </el-table-column>

      <el-table-column align="right" width="60">
        <template #default="{ row }">
          <router-link
            :to="
              useRouteSiteId({
                name: 'taxes edit',
                query: {
                  id: row.id,
                },
              })
            "
            class="mx-8"
          >
            <el-tooltip placement="top" :content="t('common.edit')">
              <el-icon class="iconfont icon-a-writein hover:text-blue text-l" />
            </el-tooltip>
          </router-link>
        </template>
      </el-table-column>
    </KTable>

    <SelectCountryDialog
      v-if="showSelectCountryDialog"
      v-model="showSelectCountryDialog"
      :excludes="excludeCountries"
      @selected="countrySelected"
    />
  </div>
</template>
