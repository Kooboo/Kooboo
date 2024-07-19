<script lang="ts" setup>
import type { Menu } from "@/api/menu/types";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { getQueryString } from "@/utils/url";
import { ref } from "vue";
import EditInner from "./edit-inner.vue";
import { useI18n } from "vue-i18n";

const { t } = useI18n();
const inner = ref();
const model = ref<Menu>();
</script>

<template>
  <div class="p-24">
    <Breadcrumb
      :crumb-path="[
        { name: t('common.menus'), route: { name: 'menus' } },
        { name: model?.name || '' },
      ]"
    />

    <el-card class="mt-24" shadow="never">
      <template #header>
        <div class="flex items-center">
          <span class="font-bold text-444 dark:text-fff/60" data-cy="menu-name">
            {{ model?.name }}
          </span>
          <div class="flex-1" />
          <IconButton
            icon="icon-code"
            :tip="t('common.editTemplate')"
            data-cy="edit-template"
            @click="inner.onTemplateEdit(model)"
          />
        </div>
      </template>
      <EditInner
        :id="getQueryString('id')!"
        ref="inner"
        @update:model="model = $event"
      />
    </el-card>
  </div>
</template>
