<script lang="ts" setup>
import { save } from "./settings";
import KBottomBar from "@/components/k-bottom-bar/index.vue";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import Settings from "./settings.vue";
import { useI18n } from "vue-i18n";
import { useShortcut } from "@/hooks/use-shortcuts";
import { ref } from "vue";

const { t } = useI18n();
const settings = ref();

const onSave = async () => {
  await save();
  settings.value.initSite();
};

useShortcut("save", onSave);
</script>

<template>
  <div class="p-24 pb-150px">
    <Breadcrumb :name="t('common.settings')" />
    <Settings ref="settings" />
    <KBottomBar hidden-cancel @save="onSave" />
  </div>
</template>
