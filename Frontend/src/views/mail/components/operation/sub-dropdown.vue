<template>
  <div v-if="dropdownItem.items?.length">
    <el-dropdown-item
      :style="{
            paddingLeft: 16 + dropdownItem.floor! * 10 + 'px',
          }"
      :disabled="dropdownItem.name === route.query.folderName"
      @click="emits('moveMessage', dropdownItem.name)"
    >
      <span class="ellipsis" :title="dropdownItem.displayName"
        >{{ dropdownItem.displayName }}
      </span>
    </el-dropdown-item>
    <!-- 多级嵌套菜单渲染 -->
    <SubDropdown
      v-for="menuItem in dropdownItem?.items"
      :key="menuItem.name"
      :dropdown-item="menuItem"
      @move-message="emits('moveMessage', $event)"
    />
  </div>
  <el-dropdown-item
    v-else
    :style="{
            paddingLeft: 16 + dropdownItem.floor! * 10 + 'px',
          }"
    :disabled="dropdownItem.name === route.query.folderName"
    @click="emits('moveMessage', dropdownItem.name)"
  >
    <span class="ellipsis" :title="dropdownItem.displayName"
      >{{ dropdownItem.displayName }}
    </span>
  </el-dropdown-item>
</template>

<script lang="ts" setup>
import type { EmailFolder } from "@/api/mail/types";
import { useRoute } from "vue-router";

const route = useRoute();

defineProps<{
  dropdownItem: EmailFolder;
}>();

const emits = defineEmits<{
  (e: "moveMessage", value: any): void;
}>();
</script>
