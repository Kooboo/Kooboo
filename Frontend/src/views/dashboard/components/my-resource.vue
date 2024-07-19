<script lang="ts" setup>
import { useI18n } from "vue-i18n";

const { t } = useI18n();

defineProps<{ list: any }>();
</script>

<template>
  <el-card shadow="always">
    <div class="mb-16 text-18px">
      <span>{{ t("common.myResource") }}</span>
      <Tooltip :tip="t('common.myResourceTips')" custom-class="ml-4" />
    </div>
    <el-scrollbar class="overflow-x-auto">
      <table id="myResource" class="w-full border-collapse">
        <template v-for="(item, index) in list" :key="item.name">
          <tr v-if="index % 2 == 0 || index == 0">
            <td
              class="border-1 border-[#e1e2e8] dark:border-666 px-12 py-8 leading-24px"
            >
              <div class="flex justify-between">
                <span class="leading-24px w-100px">
                  <el-tag class="rounded-full" size="small">
                    {{ list[index].name }}
                  </el-tag>
                </span>
                <div class="flex space-x-16">
                  <div class="flex items-center space-x-8 w-70px">
                    <span class="text-999">{{ t("common.count") }}</span
                    ><span class="font-bold">{{ list[index].count }}</span>
                  </div>
                  <div class="flex items-center space-x-8 w-110px">
                    <span class="text-999">{{ t("common.size") }}</span
                    ><span class="font-bold">{{ list[index].sizeString }}</span>
                  </div>
                </div>
              </div>
            </td>

            <td
              class="border-1 border-[#e1e2e8] dark:border-666 px-12 py-8 leading-24px"
            >
              <div class="flex justify-between">
                <span class="leading-24px w-100px">
                  <el-tag class="rounded-full" size="small">
                    {{ list[index + 1].name }}
                  </el-tag>
                </span>
                <div class="flex space-x-16">
                  <div class="flex items-center space-x-8 w-70px">
                    <span class="text-999">{{ t("common.count") }}</span
                    ><span class="font-bold">{{ list[index + 1].count }}</span>
                  </div>
                  <div class="flex items-center space-x-8 w-110px">
                    <span class="text-999">{{ t("common.size") }}</span
                    ><span class="font-bold">{{
                      list[index + 1].sizeString
                    }}</span>
                  </div>
                </div>
              </div>
            </td>
          </tr>
        </template>
      </table>
    </el-scrollbar>
  </el-card>
</template>
