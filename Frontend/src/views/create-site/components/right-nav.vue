<script lang="ts" setup>
import { createTypes } from "../create";
import { useI18n } from "vue-i18n";
import { getQueryString } from "@/utils/url";

const { t } = useI18n();
</script>

<template>
  <div>
    <p class="p-24 font-bold dark:text-fff/86">
      {{ t("common.changeSiteSetupModeHere") }}
    </p>
    <div
      v-for="item of createTypes"
      :key="item.name"
      class="group px-24 py-16 border-line dark:border-[#555] border-solid border-t flex items-center transition-all hover:(bg-fff dark:bg-[#555] shadow-s-10)"
      :data-cy="'type-' + `${item.name}`"
    >
      <template v-if="$route.name !== item.route">
        <span class="flex-1 cursor-default dark:text-fff/60">
          <el-icon
            class="iconfont icon-next mr-8 text-blue !dark:bg-transparent group-hover:(bg-[#dceff8] text-[#2296f3] rounded-full)"
          />
          {{ item.description }}
        </span>
        <el-button
          class="w-100px w-transparent"
          round
          type="primary"
          plain
          data-cy="type-button"
          @click="
            $router.push({
              name: item.route,
              query: { currentFolder: getQueryString('currentFolder') },
            })
          "
        >
          {{ item.btn }}
        </el-button>
      </template>
      <template v-else>
        <span class="flex-1 cursor-default dark:text-fff/86">
          <el-icon class="iconfont icon-next mr-8 text-999" />
          {{ t("common.currentPosition") }}
          -
          {{ item.description }}
        </span>
        <div class="h-40px" />
      </template>
    </div>
  </div>
</template>
