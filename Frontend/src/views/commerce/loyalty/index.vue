<script lang="ts" setup>
import { useRoute, useRouter } from "vue-router";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useI18n } from "vue-i18n";
import { ref } from "vue";
import MemberPanel from "./member-panel.vue";
import MembershipPanel from "./membership-panel.vue";
import EarnPointsConfigPanel from "./earn-points-config-panel.vue";
import RedeemPointsConfigPanel from "./redeem-points-config-panel.vue";
import { getQueryString } from "@/utils/url";

const { t } = useI18n();
const route = useRoute();
const routeName = route.meta.title as string;
const router = useRouter();

const tabs = [
  {
    displayName: t("common.memberships"),
    value: "membership",
    component: MembershipPanel,
  },
  {
    displayName: t("common.members"),
    value: "member",
    component: MemberPanel,
  },
  {
    displayName: t("commerce.earnPointsConfig"),
    value: "earnPointsConfig",
    component: EarnPointsConfigPanel,
  },
  {
    displayName: t("commerce.redeemPointsConfig"),
    value: "redeemPointsConfig",
    component: RedeemPointsConfigPanel,
  },
];

const activeTab = ref(getQueryString("name") || tabs[0].value);

const pushActiveName = () => {
  router.push({
    name: route.name?.toString(),
    query: {
      ...route.query,
      name: activeTab.value,
    },
  });
};
</script>

<template>
  <div class="p-24">
    <Breadcrumb :name="routeName" />
  </div>
  <el-tabs v-model="activeTab" @tab-change="pushActiveName">
    <el-tab-pane
      v-for="tab in tabs"
      :key="tab.value"
      :label="tab.displayName"
      :name="tab.value"
    >
      <component :is="tab.component" v-if="activeTab === tab.value" />
    </el-tab-pane>
  </el-tabs>
</template>
