<script lang="ts" setup>
import GuidInfo from "@/components/guid-info/index.vue";
import { useRoute } from "vue-router";
import { useI18n } from "vue-i18n";
const { t } = useI18n();
const tips: Record<string, string> = {
  "401": t("common.tips401"),
  "402": t("common.tips402"),
  "403": t("common.tips403"),
  "404": t("common.tips404"),
  "407": t("common.tips407"),
  "500": t("common.tips500"),
};
const route = useRoute();
</script>

<template>
  <div class="ml-32">
    <img src="@/assets/images/logo.svg" class="w-150px h-50px" />
  </div>
  <GuidInfo>
    <div v-if="tips[route.name!.toString()]" class="flex-wrap">
      <p class="text-7xl font-bold mb-16">{{ route.name }}</p>
      <p class="w-full text-666 font-normal">
        {{ tips[route.name!.toString()] }}
      </p>
    </div>
    <div v-else>
      <p class="text-7xl font-bold mb-16">{{ t("common.error") }}</p>
    </div>
    <ElButton
      round
      type="primary"
      class="mt-16"
      @click="$router.goBackOrTo({ name: 'home' })"
    >
      {{ t("common.back") }}
    </ElButton>
  </GuidInfo>
</template>
