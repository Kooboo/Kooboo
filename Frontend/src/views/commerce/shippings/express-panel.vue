<script lang="ts" setup>
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";
import { useRouteSiteId } from "@/hooks/use-site-id";
import type { ShippingListItem } from "@/api/commerce/shipping";
import {
  deleteShippings,
  getShippings,
  setDefault,
} from "@/api/commerce/shipping";
import { onMounted, ref } from "vue";
import { showDeleteConfirm } from "@/components/basic/confirm";
import CurrencyAmount from "../components/currency-amount.vue";

const { t } = useI18n();
const router = useRouter();
const list = ref<ShippingListItem[]>([]);

async function onDelete(rows: ShippingListItem[]) {
  await showDeleteConfirm(rows.length);
  await deleteShippings(rows.map((m) => m.id));
  load();
}

async function load() {
  list.value = await getShippings();
  list.value.forEach((f) => {
    if (f.isDefault) (f as any).$DisabledSelect = true;
  });
}

async function onSetDefault(id: string) {
  await setDefault(id);
  load();
}

onMounted(async () => {
  load();
});
</script>

<template>
  <div class="space-y-12">
    <div class="flex items-center space-x-16">
      <el-button
        v-hasPermission="{ feature: 'shipping', action: 'edit' }"
        round
        @click="
          router.push(
            useRouteSiteId({
              name: 'shipping create',
            })
          )
        "
      >
        <div class="flex items-center">
          <el-icon class="mr-16 iconfont icon-a-addto" />
          {{ t("common.create") }}
        </div>
      </el-button>
      <div class="flex-1" />
    </div>
    <KTable :data="list" show-check @delete="onDelete">
      <template #bar="{ selected }">
        <el-button
          v-if="selected.length == 1"
          v-hasPermission="{ feature: 'shipping', action: 'edit' }"
          round
          @click="onSetDefault(selected[0].id)"
        >
          <div class="flex items-center">
            <el-icon class="mr-16 iconfont icon-a-setup" />
            {{ t("common.setAsDefault") }}
          </div>
        </el-button>
      </template>
      <el-table-column :label="t('common.name')">
        <template #default="{ row }">
          <div class="flex items-center space-x-4">
            <span class="text-black dark:text-[#cfd3dc]">{{ row.name }}</span>
            <ElTag v-if="row.isDefault" round size="small">{{
              t("common.default")
            }}</ElTag>
          </div>
        </template>
      </el-table-column>

      <el-table-column :label="t('common.description')" align="center">
        <template #default="{ row }">
          {{ row.description }}
        </template>
      </el-table-column>

      <el-table-column :label="t('common.baseCost')" align="center">
        <template #default="{ row }">
          <CurrencyAmount :amount="row.baseCost" />
        </template>
      </el-table-column>

      <el-table-column :label="t('common.costRule')">
        <template #default="{ row }">
          <div
            v-for="(item, index) of row.additionalCosts"
            :key="index"
            class="text-s text-999 flex gap-8 items-center"
          >
            <CurrencyAmount :amount="item.cost" />
            <span>{{ item.description }}</span>
          </div>
        </template>
      </el-table-column>

      <el-table-column align="right" width="60">
        <template #default="{ row }">
          <router-link
            :to="
              useRouteSiteId({
                name: 'shipping edit',
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
  </div>
</template>
