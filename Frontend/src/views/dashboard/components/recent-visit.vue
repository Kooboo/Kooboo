<script lang="ts" setup>
import { RouterLink } from "vue-router";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

defineProps<{ list: any }>();
</script>

<template>
  <el-card shadow="always">
    <div class="mb-16 text-18px">{{ t("common.recentVisits") }}</div>
    <ul v-if="list.length" class="grid grid-cols-4 gap-12">
      <li
        v-for="(item, index) of list"
        :key="index"
        class="px-12 py-8 rounded-normal bg-[#e9eaf0] dark:bg-[#555] dark:bg-[#303030]"
      >
        <RouterLink :to="item.url">
          <div class="ellipsis leading-4">
            <el-icon
              class="iconfont text-16px"
              :class="item.icon || 'icon-Datacenter'"
            />
            {{ t(item.name) }}
          </div>
        </RouterLink>
      </li>
    </ul>
    <div v-else class="flex items-center justify-center text-s text-999">
      {{ t("common.notRecentVisits") }}
    </div>
  </el-card>
</template>
