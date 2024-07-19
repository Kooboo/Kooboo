<script lang="ts" setup>
import { useI18n } from "vue-i18n";
import { ref, watch } from "vue";
import MenuEditInner from "@/views/development/menus/edit-inner.vue";
import type { Menu } from "@/api/menu/types";
import { menuId, close } from ".";

const { t } = useI18n();
const inner = ref();
const model = ref<Menu>();
const changed = ref(false);
const show = ref(true);
let lastModelValue: string;

watch(
  () => model.value,
  () => {
    if (!lastModelValue && model.value) {
      lastModelValue = JSON.stringify(model.value);
    }

    changed.value = lastModelValue != JSON.stringify(model.value);
  },
  {
    immediate: true,
  }
);
</script>
<template>
  <el-dialog
    v-model="show"
    width="800px"
    :close-on-click-modal="false"
    :title="t('common.menu')"
    custom-class="el-dialog--zero-padding"
    draggable
    @closed="close(changed)"
  >
    <Alert :content="t('common.immediatelyChange')" />
    <div class="px-32 pb-24">
      <div class="flex w-full justify-end h-10 items-center pr-8">
        <IconButton
          icon="icon-code"
          :tip="t('common.editTemplate')"
          @click="inner.onTemplateEdit(model)"
        />
      </div>

      <MenuEditInner :id="menuId!" ref="inner" @update:model="model = $event" />
    </div>
  </el-dialog>
</template>
