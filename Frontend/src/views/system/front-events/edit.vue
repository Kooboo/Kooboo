<script lang="ts" setup>
import { onUnmounted, provide, ref, nextTick, watch } from "vue";
import { getListByEvent, post } from "@/api/events";
import type { Rule } from "@/api/events/types";
import { getQueryString } from "@/utils/url";
import KBottomBar from "@/components/k-bottom-bar/index.vue";
import RuleList from "./rule-list.vue";
import { EVENT_RULE_TYPE } from "@/constants/constants";
import { newGuid } from "@/utils/guid";
import { useRouter } from "vue-router";
import { useRouteSiteId } from "@/hooks/use-site-id";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useSaveTip } from "@/hooks/use-save-tip";
import { onBeforeRouteLeave } from "vue-router";

import { useI18n } from "vue-i18n";
import { useSiteStore } from "@/store/site";
const { t } = useI18n();
const router = useRouter();
const rules = ref<Rule[]>([]);
const eventname = getQueryString("name")!;
const saveTip = useSaveTip();
const siteStore = useSiteStore();
provide("eventname", eventname);

const onAddRule = (value: string) => {
  if (value === EVENT_RULE_TYPE.do) {
    rules.value.push({ id: newGuid(), do: [] });
  } else {
    rules.value.push({ id: newGuid(), if: [] });
  }
};

const goBack = () => {
  router.goBackOrTo(
    useRouteSiteId({
      name: "frontevents",
    })
  );
};

const onSave = async () => {
  await post({
    eventName: eventname,
    rules: rules.value,
  });
  saveTip.init(rules.value);
};

const load = () => {
  getListByEvent(eventname).then((rsp) => {
    rules.value = rsp;
    if (!rules.value.length) onAddRule(EVENT_RULE_TYPE.condition);
    nextTick(() => {
      saveTip.init(rules.value);
    });
  });
};
load();
// 组件销毁时重置firstActiveMenu的值，防止影响到activeName外面的行为
onUnmounted(() => {
  siteStore.firstActiveMenu = "";
});

onBeforeRouteLeave(async (to, from, next) => {
  if (to.name === "login") {
    next();
  } else {
    siteStore.firstActiveMenu = to.meta.activeMenu ?? to.name;
    await saveTip
      .check(rules.value)
      .then(() => {
        next();
      })
      .catch(() => {
        siteStore.firstActiveMenu = "frontevents";
        next(false);
      });
  }
});
watch(
  () => rules.value,
  () => {
    saveTip.changed(rules.value);
  },
  {
    deep: true,
  }
);
</script>

<template>
  <div class="p-24 pb-150px">
    <Breadcrumb
      :crumb-path="[
        { name: t('common.frontEvent'), route: { name: 'frontevents' } },
        { name: eventname },
      ]"
    />
    <div class="flex items-center py-24 space-x-16">
      <el-dropdown trigger="click" @command="onAddRule">
        <el-button round data-cy="new-rule">
          <div class="flex items-center">
            {{ t("common.newRule") }}
            <el-icon class="iconfont icon-pull-down text-12px ml-8 !mr-0" />
          </div>
        </el-button>
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item
              v-for="(value, key) of EVENT_RULE_TYPE"
              :key="key"
              :command="value"
              :data-cy="key"
            >
              <span>{{ value }}</span>
            </el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
    </div>
    <RuleList :rules="rules" />
    <KBottomBar
      back
      :permission="{ feature: 'frontEvents', action: 'edit' }"
      @cancel="goBack"
      @save="onSave"
    />
  </div>
</template>
