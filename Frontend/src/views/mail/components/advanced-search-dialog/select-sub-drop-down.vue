<template>
  <div v-if="optionItem.items?.length">
    <el-option
      :key="optionItem.name"
      :style="{
              paddingLeft: 20 + optionItem.floor! * 16 + 'px',
            }"
      :value="optionItem.name"
      @click="emits('selectFolder', optionItem.name)"
    >
      <span class="ellipsis" :title="optionItem.displayName"
        >{{ optionItem.displayName }}
      </span>
    </el-option>
    <!-- 多级嵌套菜单渲染 -->
    <SelectSubDropDown
      v-for="item in optionItem?.items"
      :key="item.name"
      :option-item="item"
      @select-folder="emits('selectFolder', $event)"
    />
  </div>
  <el-option
    v-else
    :key="optionItem.name"
    :style="{
              paddingLeft: 20 + optionItem.floor! * 16 + 'px',
            }"
    :value="optionItem.name"
    @click="emits('selectFolder', optionItem.name)"
  >
    <span class="ellipsis" :title="optionItem.displayName"
      >{{ optionItem.displayName }}
    </span>
  </el-option>
</template>

<script lang="ts" setup>
import type { EmailFolder } from "@/api/mail/types";

defineProps<{
  optionItem: EmailFolder;
}>();

const emits = defineEmits<{
  (e: "selectFolder", value: any): void;
}>();
</script>
