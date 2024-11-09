<script lang="ts" setup>
import { useRoute, useRouter } from "vue-router";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useI18n } from "vue-i18n";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { deleteCarts } from "@/api/commerce/cart";
import ImageCover from "@/components/basic/image-cover.vue";
import { onMounted, ref } from "vue";
import { showDeleteConfirm } from "@/components/basic/confirm";
import type { CartListItem, CartPagingResult } from "@/api/commerce/cart";
import type { PagingParams } from "@/api/commerce/common";
import { getCarts } from "@/api/commerce/cart";
import { useTime } from "@/hooks/use-date";
import { buildOptionsDisplay } from "../products-management/product-variant";

const { t } = useI18n();
const route = useRoute();
const router = useRouter();
const routeName = route.meta.title as string;

const pagingResult = ref<CartPagingResult>();
const pagingParams = ref<PagingParams>({
  pageIndex: 1,
  pageSize: 30,
});

async function onDelete(rows: CartListItem[]) {
  await showDeleteConfirm(rows.length);
  await deleteCarts(rows.map((m) => m.id));
  load(pagingParams.value.pageIndex);
}

async function load(pageIndex = 1) {
  pagingParams.value.pageIndex = pageIndex;
  pagingResult.value = await getCarts(pagingParams.value);
}

onMounted(load);
</script>

<template>
  <div class="p-24">
    <Breadcrumb :name="routeName" />
    <div class="flex items-center py-24 space-x-16">
      <el-button
        v-hasPermission="{ feature: 'carts', action: 'edit' }"
        round
        @click="
          router.push(
            useRouteSiteId({
              name: 'cart create',
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
    <KTable
      v-if="pagingResult"
      :data="pagingResult.list"
      show-check
      :pagination="{
        currentPage: pagingResult.pageIndex,
        totalCount: pagingResult.count,
        pageSize: pagingResult.pageSize,
      }"
      @change="load"
      @delete="onDelete"
    >
      <el-table-column :label="t('common.products')">
        <template #default="{ row }">
          <div class="flex items-center space-x-4 max-w-230px overflow-x-auto">
            <el-popover
              v-for="item of row.lines"
              :key="item.variantId"
              width="auto"
              trigger="hover"
              placement="top"
              :content="item.title"
            >
              <template #reference>
                <ImageCover
                  v-model="item.image"
                  :description="`x ${item.totalQuantity}`"
                />
              </template>

              <div>
                <div>{{ item.title }}</div>
                <div class="text-s">
                  {{ buildOptionsDisplay(item.options, true) }}
                </div>
              </div>
            </el-popover>
          </div>
        </template>
      </el-table-column>

      <el-table-column :label="t('common.contact')" align="center">
        <template #default="{ row }">
          <div v-if="row.customer">
            <TruncateContent
              :tip="`${row.customer?.email} ${row.customer?.phone}`"
              >{{ row.customer?.email }}
              {{ row.customer?.phone }}</TruncateContent
            >
            <TruncateContent
              class="text-s text-666"
              :tip="`${row.customer?.firstName} ${row.customer?.lastName}`"
            >
              {{ row.customer?.firstName }} {{ row.customer?.lastName }}
            </TruncateContent>
          </div>
          <TruncateContent v-else :tip="row.contact">{{
            row.contact
          }}</TruncateContent>
        </template>
      </el-table-column>

      <el-table-column :label="t('common.country')" align="center">
        <template #default="{ row }">{{ row.country }}</template>
      </el-table-column>

      <el-table-column
        :label="t('common.lastModified')"
        width="180"
        align="center"
      >
        <template #default="{ row }">{{ useTime(row.updateAt) }}</template>
      </el-table-column>

      <el-table-column width="40">
        <template #default="{ row }">
          <div v-if="row.type == 'Automated'" class="text-s">
            <div v-for="(item, index) in row.condition.items" :key="index">
              {{ item.option }} {{ item.method }} {{ item.value }}
            </div>
          </div>
        </template>
      </el-table-column>

      <el-table-column align="right" width="120">
        <template #default="{ row }">
          <router-link
            v-if="row.lines.length"
            :to="
              useRouteSiteId({
                name: 'cart checkout',
                query: {
                  id: row.id,
                },
              })
            "
            class="mx-8"
          >
            <el-tooltip placement="top" :content="t('commerce.checkout')">
              <el-icon class="iconfont icon-yue hover:text-blue text-l" />
            </el-tooltip>
          </router-link>
          <router-link
            :to="
              useRouteSiteId({
                name: 'cart edit',
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
