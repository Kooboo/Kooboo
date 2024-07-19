<script lang="ts" setup>
import type { Menu } from "@/api/menu/types";
import EditInner from "@/views/development/menus/edit-inner.vue";
import { ref } from "vue";
import IconButton from "@/components/basic/icon-button.vue";

import { useI18n } from "vue-i18n";
defineProps<{ id: string }>();
const { t } = useI18n();
const inner = ref();
const model = ref<Menu>();
</script>

<template>
  <div class="h-full p-24">
    <div class="flex">
      <div class="flex-1" />
      <div class="pb-8">
        <IconButton
          icon="icon-code"
          class="dark:text-fff/86"
          :tip="t('common.editTemplate')"
          data-cy="edit-template"
          @click="inner.onTemplateEdit(model)"
        />
      </div>
    </div>
    <EditInner :id="id" ref="inner" @update:model="model = $event" />
  </div>
</template>
