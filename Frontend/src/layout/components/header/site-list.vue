<script lang="ts" setup>
import { useRoute } from "vue-router";
import { useSiteStore } from "@/store/site";
import { useI18n } from "vue-i18n";
import { ref } from "vue";
const { t } = useI18n();
const siteStore = useSiteStore();
const route = useRoute();
const emit = defineEmits<{
  (e: "change", value: boolean): void;
}>();

const dropdown = ref();
const closeDropdown = () => {
  dropdown.value.handleClose();
  emit("change", true);
};

siteStore.loadSites();
</script>

<template>
  <div
    class="flex items-center justify-center h-full cursor-pointer border-l border-solid border-line dark:border-opacity-50"
    :title="t('common.switchSite')"
  >
    <el-dropdown
      ref="dropdown"
      trigger="click"
      max-height="calc(100vh - 100px)"
      class="h-full"
    >
      <span
        class="flex items-center text-black dark:text-fff/86 px-16 h-full hover:bg-[#EFF6FF] dark:hover:bg-444"
      >
        <span
          class="text-m ellipsis max-w-120px mr-8"
          :title="siteStore.site?.displayName"
        >
          {{ siteStore.site?.displayName ?? t("common.switchSite") }}
        </span>

        <el-icon class="iconfont icon-pull-down text-s leading-none" />
      </span>

      <template #dropdown>
        <el-dropdown-menu>
          <div v-for="item of siteStore.sites" :key="item.key">
            <router-link
              :to="{
                name: route.name === 'dev-mode' ? 'dev-mode' : 'dashboard',
                query: { SiteId: item.key },
              }"
              @click="closeDropdown()"
            >
              <el-dropdown-item
                :command="item.key"
                style="max-width: 300px"
                :class="{ selected: item.key === siteStore.site?.id }"
              >
                <span class="ellipsis" :title="item.value">{{
                  item.value
                }}</span>
              </el-dropdown-item>
            </router-link>
          </div>
        </el-dropdown-menu>
      </template>
    </el-dropdown>
  </div>
</template>
