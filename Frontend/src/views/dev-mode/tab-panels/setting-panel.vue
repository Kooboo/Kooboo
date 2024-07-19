<script lang="ts" setup>
import { save as saveSetting } from "@/views/system/settings/settings";
import Settings from "@/views/system/settings/settings.vue";

const save = async () => {
  saveSetting();
};

defineExpose({ save });
</script>

<template>
  <el-scrollbar>
    <div class="p-24 pb-150px">
      <Settings />
    </div>
  </el-scrollbar>
</template>
