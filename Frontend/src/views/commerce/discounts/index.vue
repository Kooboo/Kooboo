<script lang="ts" setup>
import { useRoute, useRouter } from "vue-router";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useI18n } from "vue-i18n";
import { onMounted, ref } from "vue";
import { useRouteSiteId } from "@/hooks/use-site-id";
import type { DiscountListItem } from "@/api/commerce/discount";
import { getDiscounts, deleteDiscounts } from "@/api/commerce/discount";
import { useTime } from "@/hooks/use-date";
import { showDeleteConfirm } from "@/components/basic/confirm";

const { t } = useI18n();
const route = useRoute();
const routeName = route.meta.title as string;
const router = useRouter();
const list = ref<DiscountListItem[]>([]);

function ActiveDateState(row: DiscountListItem) {
  const startDate = new Date(row.startDate);
  const endDate = new Date(row.endDate);
  const now = new Date();
  if (now < startDate) return "notStart";
  if (now > startDate && now < endDate) return "active";
  return "expire";
}

async function onDelete(rows: DiscountListItem[]) {
  await showDeleteConfirm(rows.length);
  await deleteDiscounts(rows.map((m) => m.id));
  load();
}

async function load() {
  list.value = await getDiscounts();
}

onMounted(async () => {
  load();
});
</script>

<template>
  <div class="p-24">
    <Breadcrumb :name="routeName" />
    <div class="flex items-center py-24 space-x-16">
      <el-button
        v-hasPermission="{ feature: 'discounts', action: 'edit' }"
        round
        @click="
          router.push(
            useRouteSiteId({
              name: 'discount create',
            })
          )
        "
      >
        <div class="flex items-center">
          <el-icon class="mr-16 iconfont icon-a-addto" />
          {{ t("common.create") }}
        </div>
      </el-button>
    </div>
    <KTable :data="list" show-check @delete="onDelete">
      <el-table-column :label="t('common.title')">
        <template #default="{ row }">
          <div class="flex items-center">
            <span class="text-black dark:text-[#cfd3dc]">{{ row.title }}</span>
          </div>
        </template>
      </el-table-column>

      <el-table-column :label="t('commerce.discountCode')" align="center">
        <template #default="{ row }">
          <ElTag v-if="row.method == 'DiscountCode'">
            {{ row.code }}
          </ElTag>
        </template>
      </el-table-column>

      <el-table-column :label="t('common.condition')" align="center">
        <template #default="{ row }">
          <div class="text-s">
            <div v-for="(item, index) in row.condition.items" :key="index">
              <span v-if="index > 0">{{
                row.condition.isAny ? t("common.or") : t("common.and")
              }}</span>
              {{ item.option }} {{ item.method }} {{ item.value }}
            </div>
          </div>
        </template>
      </el-table-column>

      <el-table-column :label="t('common.priority')" width="120" align="center">
        <template #default="{ row }">
          {{ row.priority }}
        </template>
      </el-table-column>

      <el-table-column
        :label="t('commerce.isExclusion')"
        width="120"
        align="center"
      >
        <template #default="{ row }">
          <span :class="row.isExclusion ? 'text-green' : ''">
            {{ row.isExclusion ? t("common.yes") : t("common.no") }}
          </span>
        </template>
      </el-table-column>

      <el-table-column
        :label="t('common.activeDateRange')"
        width="180"
        align="center"
      >
        <template #default="{ row }">
          <div
            class="text-s"
            :class="{
              'text-orange': ActiveDateState(row) == 'expire',
              'text-green': ActiveDateState(row) == 'active',
            }"
          >
            <div>
              {{ useTime(row.startDate) }}
            </div>
            <div>{{ useTime(row.endDate) }}</div>
          </div>
        </template>
      </el-table-column>

      <el-table-column :label="t('common.detail')" align="center">
        <template #default="{ row }">
          <div class="inline-flex items-center gap-4">
            <ElTag type="success" round>{{
              row.type == "FreeShipping"
                ? t("commerce.freeShipping")
                : row.type == "OrderAmountOff"
                ? t("commerce.order")
                : t("common.products")
            }}</ElTag>
            <ElTag v-if="row.type != 'FreeShipping'" round type="warning">{{
              row.isPercent ? `-${row.value}%` : `-$${row.value}`
            }}</ElTag>
          </div>
        </template>
      </el-table-column>

      <el-table-column align="right" width="60">
        <template #default="{ row }">
          <router-link
            :to="
              useRouteSiteId({
                name: 'discount edit',
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
