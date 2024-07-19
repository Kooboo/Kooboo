<script lang="ts" setup>
import ConfigDialog from "@/views/content/text/edit-dialog.vue";
import { useI18n } from "vue-i18n";
import { show, config, close } from ".";

const { t } = useI18n();
</script>
<template>
  <ConfigDialog
    v-if="show"
    v-model="show"
    :current="config!"
    :alert="t('common.immediatelyChange')"
    @success="close"
  />
</template>
