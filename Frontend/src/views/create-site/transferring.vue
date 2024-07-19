<script lang="ts" setup>
import { onUnmounted, ref } from "vue";
import { getStatus } from "@/api/transfer";
import { useRoute, useRouter } from "vue-router";

import { useI18n } from "vue-i18n";
import { getQueryString } from "@/utils/url";
const { t } = useI18n();
const route = useRoute();
const router = useRouter();
const percent = ref(0);
const list = ref<{ url: string; done: boolean }[]>([]);

const clearToken = setInterval(async () => {
  list.value = await getStatus(route.query);
  const total = list.value.length;
  const done = list.value.filter((f) => f.done).length;
  let _percent = (done * 100) / total;
  if (_percent !== 100) {
    percent.value = Math.floor(_percent);
  }
  if (total > 0 && _percent === 100) {
    const folder = getQueryString("currentFolder");
    clearInterval(clearToken);
    router.replace({
      name: "home",
      query: {
        currentFolder: folder,
      },
    });
  }
}, 500);

onUnmounted(() => {
  clearInterval(clearToken);
});
</script>

<template>
  <div class="w-1120px mx-auto py-32">
    <el-card>
      <template #header>
        <div class="flex items-center space-x-12">
          <span class="font-bold">
            {{ t("common.cloneSite") }}
          </span>
          <span class="text-s text-999">
            {{ t("common.cloneWebsiteYouMayLeaveTips") }}
          </span>
        </div>
      </template>
      <el-progress
        :text-inside="true"
        :stroke-width="26"
        :percentage="percent"
      />
      <el-table class="mt-24 el-table--gray rounded-normal" :data="list">
        <el-table-column label="URL" prop="url" />
        <el-table-column
          :label="t('common.status')"
          prop="done"
          width="100px"
          align="center"
        >
          <template #default="data">
            <el-icon
              v-if="data.row.done"
              class="iconfont icon-yes2 text-green"
            />
            <div
              v-else
              v-loading="true"
              class="w-16 h-16 inline-block clone-site-loading"
            />
          </template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>
<style lang="scss">
.clone-site-loading {
  .el-loading-mask {
    @apply bg-transparent;
  }
  svg {
    @apply !w-16;
  }
}
</style>
