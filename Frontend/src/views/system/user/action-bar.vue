<script lang="ts" setup>
import { ref } from "vue";
import AddUserDialog from "./add-user-dialog.vue";

import { useI18n } from "vue-i18n";
defineEmits<{
  (e: "reload"): void;
}>();
const { t } = useI18n();

const showDialog = ref(false);
</script>

<template>
  <div class="flex items-center py-24 space-x-16">
    <el-button
      v-hasPermission="{ feature: 'siteUser', action: 'edit' }"
      round
      data-cy="add-user"
      @click="showDialog = true"
    >
      <div class="flex items-center">
        <el-icon class="mr-16 iconfont icon-a-addto" />
        {{ t("common.addUser") }}
      </div>
    </el-button>
  </div>
  <AddUserDialog
    v-if="showDialog"
    v-model="showDialog"
    @reload="$emit('reload')"
  />
</template>
